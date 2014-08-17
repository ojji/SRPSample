namespace EcommerceLib.Domain.PricingStrategies
{
    public class DefaultPricing : IPricingStrategy
    {
        public string Name { get { return "Full price."; } }

        public bool MatchesItem(OrderItem order)
        {
            return true;
        }

        public decimal CalculateItemPrice(OrderItem order)
        {
            return order.ItemCost*order.Quantity;
        }
    }
}