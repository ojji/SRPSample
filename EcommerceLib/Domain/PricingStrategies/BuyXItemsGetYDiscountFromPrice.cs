using System;

namespace EcommerceLib.Domain.PricingStrategies
{
    /// <summary>
    /// Pricing strategy for Buy-X-Items-Get-Y-Percent-Off discounts.
    /// </summary>
    public class BuyXItemsGetYDiscountFromPrice : IPricingStrategy
    {
        /// <summary>
        /// The item id that the discount applies to.
        /// </summary>
        public string ItemId { get; private set; }

        /// <summary>
        /// The minimum items needed to apply the discount.
        /// </summary>
        public int DiscountedQuantity { get; private set; }

        /// <summary>
        /// The discount amount between 0.0m and 1.0m.
        /// </summary>
        public decimal Discount { get; private set; }

        /// <summary>
        /// The discount name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a Buy-X-Item-Get-Y-Percent-Off pricing strategy object.
        /// </summary>
        /// <param name="itemId">The item id that the discount applies to.</param>
        /// <param name="discountedQuantity">The minimum items needed to apply the discount.</param>
        /// <param name="discount">The discount amount between 0.0m and 1.0m.</param>
        public BuyXItemsGetYDiscountFromPrice(string itemId, int discountedQuantity, decimal discount)
        {
            if (string.IsNullOrEmpty(itemId)) { throw new ArgumentNullException("itemId"); }
            if (discountedQuantity < 1) { throw new ArgumentOutOfRangeException("discountedQuantity"); }
            if (discount <= 0m || discount >= 1.0m) { throw new ArgumentOutOfRangeException("discount"); }
            ItemId = itemId;
            DiscountedQuantity = discountedQuantity;
            Discount = discount;
            Name = string.Format("DISCOUNT: Buy {0}, get {1:P} off.", DiscountedQuantity, Discount);
        }
        
        /// <summary>
        /// Tells whether the discount applies to the supplied <see cref="OrderItem"/> or not.
        /// </summary>
        /// <param name="order">The <see cref="OrderItem"/> to decide the discount is appliable or not to.</param>
        /// <returns>True if the discount applies, false otherwise.</returns>
        public bool MatchesItem(OrderItem order)
        {
            return (order.Identifier == ItemId && order.Quantity >= DiscountedQuantity);
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
            int fullCostItems = order.Quantity % DiscountedQuantity;
            int discountedItems = order.Quantity - fullCostItems;
            return (discountedItems * ((1 - Discount) * order.ItemCost) + fullCostItems * order.ItemCost);
        }
    }
}