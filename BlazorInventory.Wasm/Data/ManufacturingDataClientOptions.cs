namespace BlazorInventory.Wasm.Data
{
    public class ManufacturingDataClientOptions
    {
        public string? BaseUri { get; set; }
        public HttpMessageHandler? MessageHandler { get; set; }
    }
}
