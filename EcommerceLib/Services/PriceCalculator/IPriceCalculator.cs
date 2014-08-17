using EcommerceLib.Domain;
using EcommerceLib.Domain.PricingStrategies;

namespace EcommerceLib.Services.PriceCalculator
{
    public interface IPriceCalculator
    {
        IPricingStrategy Default { get; }
        decimal CalculatePrice(OrderItem item);
    }
}
