namespace EcommerceLib.Domain.PricingStrategies
{
    public interface IPricingStrategy
    {
        bool MatchesItem(OrderItem order);
        decimal CalculateItemPrice(OrderItem order);
    }
}