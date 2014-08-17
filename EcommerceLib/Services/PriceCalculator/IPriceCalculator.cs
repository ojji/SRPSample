using EcommerceLib.Domain;

namespace EcommerceLib.Services.PriceCalculator
{
    public interface IPriceCalculator
    {
        decimal CalculatePrice(OrderItem item);
    }
}