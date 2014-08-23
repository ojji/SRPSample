using System.Collections.Generic;
using System.Linq;
using EcommerceLib.Services.PriceCalculator;

namespace EcommerceLib.Domain
{
    public class ShoppingCart
    {
        private readonly List<OrderItem> _orderItems;
        private readonly IPriceCalculator _priceCalculator;

        public Customer CurrentCustomer { get; set; }

        public ShoppingCart(IPriceCalculator priceCalculator)
        {
            _priceCalculator = priceCalculator;
            _orderItems = new List<OrderItem>();
        }

        public IEnumerable<OrderItem> Items
        {
            get { return _orderItems; }
        }

        public decimal GetTotalCost()
        {
            return _orderItems.Sum(orderItem => _priceCalculator.CalculatePrice(CurrentCustomer, orderItem));
        }

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