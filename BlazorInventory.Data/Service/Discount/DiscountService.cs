
using BlazorInventory.Data;

namespace BlazorInventory.Server.Service
{
    public class DiscountService
    {
        private List<DiscountRule> discountRules;

        public DiscountService()
        {
            // Initialize the discount rules from a data source or configuration
            discountRules = new List<DiscountRule>
            {
                new DiscountRule { MinPrice = 0, MaxPrice = 10, MinQuantity = 1, MaxQuantity = 5, MinSalesCount = 0, MaxSalesCount = 10, DiscountPercentage = 0.1m },
                new DiscountRule { MinPrice = 0, MaxPrice = 10, MinQuantity = 1, MaxQuantity = 5, MinSalesCount = 11, MaxSalesCount = int.MaxValue, DiscountPercentage = 0.15m },
                // Add more discount rules as needed
            };
        }

        public decimal ApplyDiscount(Item item)
        {
            foreach (var rule in discountRules)
            {
                if (item.Price >= rule.MinPrice && item.Price <= rule.MaxPrice
                    && item.Quantity >= rule.MinQuantity && item.Quantity <= rule.MaxQuantity
                    && item.SalesCount >= rule.MinSalesCount && item.SalesCount <= rule.MaxSalesCount)
                {
                    decimal discountAmount = item.Price * rule.DiscountPercentage;
                    item.Price -= discountAmount;
                    return discountAmount;
                }
            }

            return 0; // No applicable discount rule found
        }
    }
}
