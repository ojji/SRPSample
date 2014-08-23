using EcommerceLib.Domain;
using EcommerceLib.Domain.PricingStrategies;

namespace EcommerceLib.Services.PriceCalculator
{
    public interface IPriceCalculator
    {
        IOrderItemDiscount Default { get; }
        decimal CalculatePrice(Customer customer, OrderItem item);
    }
}
