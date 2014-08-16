using System.Collections.Generic;

namespace EcommerceLib.Domain
{
    public class ShoppingCart
    {
        public IEnumerable<OrderItem> Items { get; set; }
        public decimal TotalCost { get; set; }
        public string CustomerEmail { get; set; }
    }
}