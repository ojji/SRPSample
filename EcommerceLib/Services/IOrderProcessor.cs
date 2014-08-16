using EcommerceLib.Domain;

namespace EcommerceLib.Services
{
    public interface IOrderProcessor
    {
        void Checkout(Order order);
    }
}