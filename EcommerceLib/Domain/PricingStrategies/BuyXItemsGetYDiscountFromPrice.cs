using System;

namespace EcommerceLib.Domain.PricingStrategies
{
    public class BuyXItemsGetYDiscountFromPrice : IPricingStrategy
    {
        public string ItemId { get; private set; }
        public int DiscountedQuantity { get; private set; }
        public decimal Discount { get; private set; }

        public BuyXItemsGetYDiscountFromPrice(string itemId, int discountedQuantity, decimal discount)
        {
            if (string.IsNullOrEmpty(itemId)) { throw new ArgumentNullException("itemId"); }
            if (discountedQuantity < 1) { throw new ArgumentOutOfRangeException("discountedQuantity"); }
            if (discount <= 0m || discount >= 1.0m) { throw new ArgumentOutOfRangeException("discount"); }
            ItemId = itemId;
            DiscountedQuantity = discountedQuantity;
            Discount = discount;
        }

        public bool MatchesItem(OrderItem order)
        {
            return (order.Identifier == ItemId && order.Quantity >= DiscountedQuantity);
        }

        public decimal CalculateItemPrice(OrderItem order)
        {
            if (!MatchesItem(order)) { throw new ArgumentException("The price strategy can't be applied for this order: {0}", order.ToString());}
            int fullCostItems = order.Quantity % DiscountedQuantity;
            int discountedItems = order.Quantity - fullCostItems;
            return (discountedItems * ((1 - Discount) * order.ItemCost) + fullCostItems * order.ItemCost);
        }
    }
}