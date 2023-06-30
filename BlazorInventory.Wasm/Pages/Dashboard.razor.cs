using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using BlazorInventory.Wasm;
using BlazorInventory.Wasm.Shared;
using QuickGrid;
using BlazorInventory.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BlazorInventory.Data.Repository;

namespace BlazorInventory.Wasm.Pages
{
    public partial class Dashboard : ComponentBase
    {
        private Exception? loadingException;
        private int partsDataCount;
        ClientSideDbContext? clientSideDbContext;
        private static readonly string empty = string.Empty;
        private string? mostExpensivePart = empty;

        private string? mostUsedCategory = empty;
        private string? lessUsedCategory = empty;

        private string? partWithMaxStock = empty;
        private string? partWithMinStock = empty;

        private string? locationWithMostParts = empty;
        private string? locationWithLowestParts = empty;
        bool isLoading = false;


        [Inject]
        DataSynchronizer? DataSynchronizer { get; set; }

        [Inject]
        IItemRepository? ItemRepository { get; set; }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            try
            {
                clientSideDbContext = await DataSynchronizer!.GetPreparedDbContextAsync();
                await this.InitDashboardData();
            }
            catch (Exception ex)
            {
                loadingException = ex;
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task InitDashboardData()
        {
            /*
            var items = clientSideDbContext?.Items.ToList();
            var item = new Item
            {
                Id = 1,
                Description = "Hello",
                Quantity = 50
            };
            ItemRepository!.Add(item);
            var items2 = clientSideDbContext?.Items.ToList();
            
            IQueryable<Part>? parts = clientSideDbContext?.Parts;
            if (parts == null || !parts.Any())
            {
                return;
            }

            partsDataCount = await clientSideDbContext!.Parts.CountAsync();

            var maxPrice = await parts.MaxAsync(part => part.PriceCents);
            mostExpensivePart = await GetPartNameByProperty(parts, p => p.PriceCents == maxPrice);

            mostUsedCategory = GetPartByPropertyCount(
                parts, 
                part => part.Category,
                orderByDesc: true);

            var maxStock = await parts.MaxAsync(part => part.Stock);
            partWithMaxStock = await GetPartNameByProperty(parts, p => p.Stock == maxStock);

            locationWithMostParts = GetPartByPropertyCount(
                parts, 
                part => part.Location,
                orderByDesc: true);*/
        }

        private static string? GetPartByPropertyCount(
            IQueryable<Part>? parts, 
            Expression<Func<Part, string>> selector,
            bool orderByDesc)
        {
            var groupedParts = parts!
                .GroupBy(selector)
                .Select(grp =>
                        new {
                            Name = grp.Key,
                            Count = grp.Count()
                        }).ToList();

            var orderedParts = orderByDesc 
                ? groupedParts!.OrderByDescending(g => g.Count)
                : groupedParts!.OrderBy(g => g.Count);

            return orderedParts.FirstOrDefault()?.Name;
        }

        private async static Task<string?> GetPartNameByProperty(
            IQueryable<Part>? parts,
            Expression<Func<Part, bool>> predicate)
        {
            var part = await parts!.FirstOrDefaultAsync(predicate);
            string? partName = part?.Name;
            if (string.IsNullOrEmpty(partName))
            {
                return empty;
            }
            // Avoid long parts names
            return partName[..Math.Min(partName.Length, 15)];
        }
    }
}