using EcommerceLib.Domain;

namespace EcommerceLib.Services.OrderProcessor
{
    public interface IOrderProcessor
    {
        void Checkout(Order order);
    }
}