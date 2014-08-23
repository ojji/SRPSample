using System;

namespace EcommerceLib.Domain.PricingStrategies
{
    public class FlatXPercentOff : IOrderItemDiscount
    {
        public string ItemId { get; private set; }
        public decimal Discount { get; private set; }
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a flat-x-percent off discount object.
        /// </summary>
        /// <param name="itemId">The itemid this discount applies to.</param>
        /// <param name="discount">The discount percentage.</param>
        /// <exception cref="ArgumentNullException">The itemId is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The discount percentage should be between ]0.0m,1.0m[.</exception>
        public FlatXPercentOff(string itemId, decimal discount)
        {
            if (string.IsNullOrWhiteSpace(itemId)) { throw new ArgumentNullException("itemId"); }
            if (discount <= 0.0m || discount >= 1.0m) { throw new ArgumentOutOfRangeException("discount"); }
            ItemId = itemId;
            Discount = discount;
            Name = string.Format("DISCOUNT: {1} - {0:p2} off.", Discount, ItemId);
        }
        
        public bool MatchesItem(OrderItem order)
        {
            return (order != null) && (order.Identifier == ItemId);
        }

        public decimal CalculateItemPrice(OrderItem order)
        {
            if (!MatchesItem(order)) { throw new ArgumentException("Discount cannot be applied to the orderitem."); }
            return order.Quantity*order.ItemCost*(1-Discount);
        }
    }
}