namespace BlazorInventory.Data.Response
{
    public class ItemsResponse
    {
        public int ModifiedCount { get; set; }
        public List<Item> Items { get; set; } = new();
    }
}
