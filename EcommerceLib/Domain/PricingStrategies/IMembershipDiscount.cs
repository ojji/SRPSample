namespace EcommerceLib.Domain.PricingStrategies
{
    public interface IMembershipDiscount
    {
        MembershipLevel MinimumLevel { get; }
        MembershipLevel MaximumLevel { get; }
        IOrderItemDiscount Discount { get; }
        bool MatchesTo(Customer customer, OrderItem orderItem);
    }
}