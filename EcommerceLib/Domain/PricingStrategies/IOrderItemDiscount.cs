namespace EcommerceLib.Domain.PricingStrategies
{
    public interface IOrderItemDiscount
    {
        string Name { get; }
        bool MatchesItem(OrderItem order);
        decimal CalculateItemPrice(OrderItem order);
    }
}