
using BlazorInventory.Data.Request;
using BlazorInventory.Data.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Data.Common;
using System.Net.Http.Json;
using System.Net.Http;
using System.Runtime.InteropServices;
using Google.Protobuf.WellKnownTypes;
using BlazorInventory.Data.Models;
using System.IO;

namespace BlazorInventory.Data
{

    // This service synchronizes the Sqlite DB with both the backend server and the browser's IndexedDb storage
    class DataSynchronizer
    {
        public const string SqliteDbFilename = "items.db";
        private readonly Task firstTimeSetupTask;
        private readonly IDbContextFactory<ClientSideDbContext> dbContextFactory;
        private readonly ManufacturingData.ManufacturingDataClient manufacturingData;
        private bool isSynchronizing;
        private HttpClient _httpClient;
        private IConfiguration _configuration;

        public DataSynchronizer(IJSRuntime js,
            IDbContextFactory<ClientSideDbContext> dbContextFactory,
            ManufacturingData.ManufacturingDataClient manufacturingData,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            this.dbContextFactory = dbContextFactory;
            this.manufacturingData = manufacturingData;
            _httpClient = httpClient;
            _configuration = configuration;
            firstTimeSetupTask = FirstTimeSetupAsync(js, synchronizeWithIndexedDb: true);
        }

        public int SyncCompleted { get; private set; }
        public int SyncTotal { get; private set; }

        public async Task<ClientSideDbContext> GetPreparedDbContextAsync()
        {
            await firstTimeSetupTask;
            return await dbContextFactory.CreateDbContextAsync();
        }

        public void SynchronizeInBackground()
        {
            _ = EnsureSynchronizingAsync();
        }

        public event Action? OnUpdate;
        public event Action<Exception>? OnError;

        private async Task FirstTimeSetupAsync(IJSRuntime js, bool synchronizeWithIndexedDb = true)
        {
            if (synchronizeWithIndexedDb)
            {
                var module = await js.InvokeAsync<IJSObjectReference>("import", "./dbstorage.js");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser")))
                {
                    await module.InvokeVoidAsync("synchronizeFileWithIndexedDb", SqliteDbFilename);
                }
            }
            using var db = await dbContextFactory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();
        }

        private async Task EnsureSynchronizingAsync()
        {
            // Don't run multiple syncs in parallel. This simple logic is adequate because of single-threadedness.
            if (isSynchronizing)
            {
                return;
            }

            try
            {
                isSynchronizing = true;
                SyncCompleted = 0;
                SyncTotal = 0;

                // Get a DB context
                using var db = await GetPreparedDbContextAsync();
                db.ChangeTracker.AutoDetectChangesEnabled = false;
                db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                var connection = db.Database.GetDbConnection();
                connection.Open();

                IEnumerable<Group>? groups = await GetGroups();
                if (!db.Groups.Any())
                {
                    BulkInsertGroups(connection, groups!);
                }

                IEnumerable<SubGroup>? subGroups = await GetSubGroups();
                if (!db.SubGroups.Any())
                {
                    BulkInsertSubGroups(connection, subGroups!);
                }

                // Begin fetching any updates to the dataset from the backend server
                var mostRecentUpdate = db.Items.OrderByDescending(p => p.Id).FirstOrDefault()?.Id;
                while (true)
                {
                    var request = new ItemsRequest { MaxCount = 5000, ModifiedSinceTicks = mostRecentUpdate ?? -1 };
                    var response = await GetItems(request);
                    var syncRemaining = response!.ModifiedCount - response.Items.Count;
                    SyncCompleted += response.Items.Count;
                    SyncTotal = SyncCompleted + syncRemaining;

                    if (response.Items.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        mostRecentUpdate = response.Items.Last().Id;
                        BulkInsertItems(connection, response.Items);
                        OnUpdate?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: use logger also
                OnError?.Invoke(ex);
            }
            finally
            {
                isSynchronizing = false;
            }
        }

        private void BulkInsert(DbConnection connection, IEnumerable<Part> parts)
        {
            // Since we're inserting so much data, we can save a huge amount of time by dropping down below EF Core and
            // using the fastest bulk insertion technique for Sqlite.
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                var partId = AddNamedParameter(command, "$PartId");
                var category = AddNamedParameter(command, "$Category");
                var subcategory = AddNamedParameter(command, "$Subcategory");
                var name = AddNamedParameter(command, "$Name");
                var location = AddNamedParameter(command, "$Location");
                var stock = AddNamedParameter(command, "$Stock");
                var priceCents = AddNamedParameter(command, "$PriceCents");
                var modifiedTicks = AddNamedParameter(command, "$ModifiedTicks");

                command.CommandText =
                    $"INSERT OR REPLACE INTO Parts (PartId, Category, Subcategory, Name, Location, Stock, PriceCents, ModifiedTicks) " +
                    $"VALUES ({partId.ParameterName}, {category.ParameterName}, {subcategory.ParameterName}, {name.ParameterName}, {location.ParameterName}, {stock.ParameterName}, {priceCents.ParameterName}, {modifiedTicks.ParameterName})";

                foreach (var part in parts)
                {
                    partId.Value = part.PartId;
                    category.Value = part.Category;
                    subcategory.Value = part.Subcategory;
                    name.Value = part.Name;
                    location.Value = part.Location;
                    stock.Value = part.Stock;
                    priceCents.Value = part.PriceCents;
                    modifiedTicks.Value = part.ModifiedTicks;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }

        }

        private void BulkInsertItems(DbConnection connection, IEnumerable<Item> items)
        {
            // Since we're inserting so much data, we can save a huge amount of time by dropping down below EF Core and
            // using the fastest bulk insertion technique for Sqlite.
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                var Id = AddNamedParameter(command, "$Id");
                var name = AddNamedParameter(command, "$Name");
                var description = AddNamedParameter(command, "$Description");
                var quantity = AddNamedParameter(command, "$Quantity");
                var price = AddNamedParameter(command, "$Price");
                var salesCount = AddNamedParameter(command, "$SalesCount");
                var responsibleIdentifier = AddNamedParameter(command, "$ResponsibleIdentifier");
                var subGroupId = AddNamedParameter(command, "$SubGroupId");
                var stock = AddNamedParameter(command, "$Stock");

                command.CommandText =
                    $"INSERT INTO Item (Id, Name, Description, Quantity, Price, Stock, SalesCount, ResponsibleIdentifier, SubGroupId) " +
                    $"VALUES (" +
                    $"{Id.ParameterName}, " +
                    $"{name.ParameterName}, " +
                    $"{description.ParameterName}, " +
                    $"{quantity.ParameterName}, " +
                    $"{price.ParameterName}, " +
                    $"{stock.ParameterName}, " +
                    $"{salesCount.ParameterName}, " +
                    $"{responsibleIdentifier.ParameterName}, " +
                    $"{subGroupId.ParameterName})";

                foreach (var item in items)
                {
                    Id.Value = item.Id;
                    name.Value = item.Name;
                    description.Value = item.Description;
                    quantity.Value = item.Quantity;
                    price.Value = item.Price;
                    salesCount.Value = item.SalesCount;
                    responsibleIdentifier.Value = item.ResponsibleIdentifier;
                    subGroupId.Value = item.SubGroupId;
                    stock.Value = item.Stock;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        private void BulkInsertGroups(DbConnection connection, IEnumerable<Group> groups)
        {
            // Since we're inserting so much data, we can save a huge amount of time by dropping down below EF Core and
            // using the fastest bulk insertion technique for Sqlite.
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                var Id = AddNamedParameter(command, "$Id");
                var name = AddNamedParameter(command, "$Name");
                var description = AddNamedParameter(command, "$Description");

                command.CommandText =
                    $"INSERT INTO Groups (Id, Name, Description) " +
                    $"VALUES (" +
                    $"{Id.ParameterName}, " +
                    $"{name.ParameterName}, " +
                    $"{description.ParameterName})";

                foreach (var item in groups)
                {
                    Id.Value = item.Id;
                    name.Value = item.Name;
                    description.Value = item.Description;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        private void BulkInsertSubGroups(DbConnection connection, IEnumerable<SubGroup> subGroups)
        {
            // Since we're inserting so much data, we can save a huge amount of time by dropping down below EF Core and
            // using the fastest bulk insertion technique for Sqlite.
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                var Id = AddNamedParameter(command, "$Id");
                var name = AddNamedParameter(command, "$Name");
                var description = AddNamedParameter(command, "$Description");
                var groupId = AddNamedParameter(command, "$GroupId");

                command.CommandText =
                    $"INSERT INTO SubGroup (Id, Name, Description, GroupId) " +
                    $"VALUES (" +
                    $"{Id.ParameterName}, " +
                    $"{name.ParameterName}, " +
                    $"{description.ParameterName}, " +
                    $"{groupId.ParameterName})";

                foreach (var item in subGroups)
                {
                    Id.Value = item.Id;
                    name.Value = item.Name;
                    description.Value = item.Description;
                    groupId.Value = item.GroupId;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        private static DbParameter AddNamedParameter(DbCommand command, string name)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
            return parameter;
        }

        public async Task<ItemsResponse?> GetItems(ItemsRequest request)
        {
            if (request == null)
            {
                return new ItemsResponse();
            }
            var queryParams = new Dictionary<string, string>
            {
                { nameof(request.ModifiedSinceTicks), request.ModifiedSinceTicks.ToString() },
                { nameof(request.MaxCount), request.MaxCount.ToString() }
            };

            string queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            var apiUrl = $"{_configuration["BackendOrigin"]}/api/items/request";

            HttpResponseMessage response = await _httpClient.GetAsync($"{apiUrl}?{queryString}");
            response.EnsureSuccessStatusCode();
            ItemsResponse? itemsResponse = await response.Content.ReadFromJsonAsync<ItemsResponse>();
            
            return itemsResponse;
        }

        public async Task<IEnumerable<Group>?> GetGroups()
        {
            string apiUrl = $"{_configuration["BackendOrigin"]}/api/groups";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            IEnumerable<Group>? groups = await response.Content.ReadFromJsonAsync<IEnumerable<Group>>();
            return groups;
        }

        public async Task<IEnumerable<SubGroup>?> GetSubGroups()
        {
            string apiUrl = $"{_configuration["BackendOrigin"]}/api/subgroups";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            IEnumerable<SubGroup>? subGroups = await response.Content.ReadFromJsonAsync<IEnumerable<SubGroup>>();
            return subGroups;
        }
    }
}

