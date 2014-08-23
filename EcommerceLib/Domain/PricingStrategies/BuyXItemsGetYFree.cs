using System;

namespace EcommerceLib.Domain.PricingStrategies
{
    /// <summary>
    /// Pricing strategy for Buy-X-Items-Get-Y-Items-Free discounts.
    /// </summary>
    public class BuyXItemsGetYFree : IOrderItemDiscount
    {
        /// <summary>
        /// The item id that the discount applies to.
        /// </summary>
        public string ItemId { get; private set; }

        /// <summary>
        /// The minimum items needed to apply the discount.
        /// </summary>
        public int QuantityToBuy { get; private set; }

        /// <summary>
        /// The number of items to get free when the discount applies.
        /// </summary>
        public int QuantityToGetFree { get; private set; }

        /// <summary>
        /// The discount name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a Buy-X-Items-Get-Y-Items-Free pricing strategy object.
        /// </summary>
        /// <param name="itemId">The item id that the discount applies to.</param>
        /// <param name="quantityToBuy">The minimum items needed to apply the discount.</param>
        /// <param name="quantityToGetFree">The number of items to get free when the discount applies.</param>
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

        /// <summary>
        /// Tells whether the discount applies to the supplied <see cref="OrderItem"/> or not.
        /// </summary>
        /// <param name="order">The <see cref="OrderItem"/> to decide the discount is appliable or not to.</param>
        /// <returns>True if the discount applies, false otherwise.</returns>
        public bool MatchesItem(OrderItem order)
        {
            return (order.Identifier == ItemId && order.Quantity >= QuantityToBuy);
        }

        /// <summary>
        /// Calculates the discounted price for the given <see cref="OrderItem"/>.
        /// </summary>
        /// <param name="order">The <see cref="OrderItem"/> the price is calculated for.</param>
        /// <returns>The discounted price.</returns>
        /// <exception cref="ArgumentException">The discount cannot be applied for the <see cref="OrderItem"/>.</exception>
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