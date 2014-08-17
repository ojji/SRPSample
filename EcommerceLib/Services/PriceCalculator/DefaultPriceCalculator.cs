using System.Linq;
using EcommerceLib.Domain;
using EcommerceLib.Domain.PricingStrategies;
using EcommerceLib.Repositories;

namespace EcommerceLib.Services.PriceCalculator
{
    /// <summary>
    /// A price calculator class.
    /// </summary>
    public class DefaultPriceCalculator : IPriceCalculator
    {
        private readonly IDiscountRepository _repository;

        /// <summary>
        /// Initializes a price calculator class.
        /// </summary>
        public DefaultPriceCalculator() : this(null) { }

        /// <summary>
        /// Initializes a price calculator class.
        /// </summary>
        /// <param name="repository">The repository containing our discounts.</param>
        public DefaultPriceCalculator(IDiscountRepository repository)
        {
            Default = new DefaultPricing();
            _repository = repository;
        }

        /// <summary>
        /// The default pricing strategy to use when no discounts are found.
        /// </summary>
        public IPricingStrategy Default { get; private set; }

        /// <summary>
        /// Calculates an <see cref="OrderItem"/>'s lowest price according to
        /// the currently stored discounts.
        /// </summary>
        /// <param name="item">The <see cref="OrderItem"/> for which the price should be calculated.</param>
        /// <returns>The item's lowest price.</returns>
        public decimal CalculatePrice(OrderItem item)
        {
            if (_repository == null)
            {
                return Default.CalculateItemPrice(item);
            }

            var biggestDiscount = _repository.GetDiscountsFor(item.Identifier)
                .OrderBy(e => e.CalculateItemPrice(item))
                .FirstOrDefault();

            if (biggestDiscount == null)
            {
                return Default.CalculateItemPrice(item);
            }

            return biggestDiscount.CalculateItemPrice(item);
        }
    }
}