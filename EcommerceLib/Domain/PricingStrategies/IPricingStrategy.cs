namespace EcommerceLib.Domain.PricingStrategies
{
    public interface IPricingStrategy
    {
        string Name { get; }
        bool MatchesItem(OrderItem order);
        decimal CalculateItemPrice(OrderItem order);
    }
}