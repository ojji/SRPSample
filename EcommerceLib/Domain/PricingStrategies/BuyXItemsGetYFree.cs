using System;

namespace EcommerceLib.Domain.PricingStrategies
{
    public class BuyXItemsGetYFree : IPricingStrategy
    {
        public string ItemId { get; private set; }
        public int QuantityToBuy { get; private set; }
        public int QuantityToGetFree { get; private set; }
        public string Name { get; private set; }

        public BuyXItemsGetYFree(string itemId, int quantityToBuy, int quantityToGetFree)
        {
            if (string.IsNullOrEmpty(itemId)) { throw new ArgumentNullException("itemId"); }
            if (quantityToBuy < 1) { throw new ArgumentOutOfRangeException("quantityToBuy"); }
            if (quantityToGetFree < 1) { throw new ArgumentOutOfRangeException("quantityToGetFree"); }
            ItemId = itemId;
            QuantityToBuy = quantityToBuy;
            QuantityToGetFree = quantityToGetFree;
            Name = string.Format("DISCOUNT: Buy {0} get {1} free.", QuantityToBuy, QuantityToGetFree);
        }
        
        public bool MatchesItem(OrderItem order)
        {
            return (order.Identifier == ItemId && order.Quantity >= QuantityToBuy);
        }

        public decimal CalculateItemPrice(OrderItem order)
        {
            if (!MatchesItem(order)) { throw new ArgumentException("The price strategy can't be applied for this order: {0}", order.ToString());}

            decimal totalCost = 0m;

            int remainingItems = order.Quantity;
            while (remainingItems > 0)
            {
                if (remainingItems < QuantityToBuy)
                {
                    totalCost += remainingItems * order.ItemCost;
                }
                else
                {
                    totalCost += QuantityToBuy * order.ItemCost;
                }
                remainingItems -= (QuantityToBuy + QuantityToGetFree);
            }

            return totalCost;
        }
    }
}