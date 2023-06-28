namespace BlazorInventory.Data.Request
{
    public class ItemsRequest
    {
        public long ModifiedSinceTicks { get; set; }

        public int MaxCount { get; set; }
    }
}
