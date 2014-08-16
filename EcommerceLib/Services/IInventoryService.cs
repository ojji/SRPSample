using System.Collections.Generic;
using EcommerceLib.Domain;

namespace EcommerceLib.Services
{
    public interface IInventoryService
    {
        bool ReserveItems(IEnumerable<OrderItem> items);
        void CancelReservation(IEnumerable<OrderItem> items);
        void HandoutItemsToDelivery(IEnumerable<OrderItem> items);
    }
}