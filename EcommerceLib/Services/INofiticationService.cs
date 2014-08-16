using EcommerceLib.Domain;

namespace EcommerceLib.Services
{
    public interface INofiticationService
    {   
        void NotifyCustomerOrderCreated(ShoppingCart cart);
    }
}