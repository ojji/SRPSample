namespace EcommerceLib.Domain.PricingStrategies
{
    /// <summary>
    /// The default, full-price pricing strategy.
    /// </summary>
    public class DefaultPricing : IPricingStrategy
    {
        /// <summary>
        /// The pricing strategy name.
        /// </summary>
        public string Name { get { return "Full price."; } }

        /// <summary>
        /// Tells whether the pricing strategy applies to the given <see cref="OrderItem"/>.
        /// It returns always true.
        /// </summary>
        /// <param name="order">The <see cref="OrderItem"/> to decide the discount is appliable or not to.</param>
        /// <returns>Always true.</returns>
        public bool MatchesItem(OrderItem order)
        {
            return true;
        }

        /// <summary>
        /// Calculates the full price for the given <see cref="OrderItem"/>.
        /// </summary>
        /// <param name="order">The <see cref="OrderItem"/> the price is calculated for.</param>
        /// <returns>The full price.</returns>
        public decimal CalculateItemPrice(OrderItem order)
        {
            return order.ItemCost*order.Quantity;
        }
    }
}