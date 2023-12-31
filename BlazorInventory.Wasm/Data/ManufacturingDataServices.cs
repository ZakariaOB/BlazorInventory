﻿using BlazorInventory.Data;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.EntityFrameworkCore;


namespace BlazorInventory.Wasm.Data
{
    public static class ManufacturingDataServices
    {
        public static void AddManufacturingDataClient(
            this IServiceCollection serviceCollection, 
            Action<IServiceProvider, ManufacturingDataClientOptions> configure)
        {
            serviceCollection.AddScoped(services =>
            {
                var options = new ManufacturingDataClientOptions();
                configure(services, options);
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, options.MessageHandler!));
                var channel = GrpcChannel.ForAddress(options.BaseUri!, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null });
                return new ManufacturingData.ManufacturingDataClient(channel);
            });
        }
    }
}
