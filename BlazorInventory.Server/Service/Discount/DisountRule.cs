namespace BlazorInventory.Server.Service.Discount
{
    public class DiscountRule
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public int MinSalesCount { get; set; }
        public int MaxSalesCount { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}
