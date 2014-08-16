using System.Collections.Generic;
using System.Linq;

namespace EcommerceLib.Domain
{
    public class ShoppingCart
    {
        private readonly List<OrderItem> _orderItems = new List<OrderItem>();

        public IEnumerable<OrderItem> Items
        {
            get { return _orderItems; }
        }

        public decimal TotalCost { get; set; }
        public string CustomerEmail { get; set; }

        public void Add(OrderItem item)
        {
            var existingItem = _orderItems.FirstOrDefault(i => i.Identifier == item.Identifier);
            if (existingItem == null)
            {
                _orderItems.Add(item);
            }
            else
            {
                existingItem.Quantity += item.Quantity;
            }
        }

        public void Remove(string itemId)
        {
            OrderItem itemToDelete = _orderItems.FirstOrDefault(i => i.Identifier == itemId);
            if (itemToDelete != null)
            {
                _orderItems.Remove(itemToDelete);
            }
        }
    }
}