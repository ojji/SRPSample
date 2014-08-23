using System;
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
        /// Initializes a price calculator class with full price default pricing strategy.
        /// </summary>
        /// <param name="repository">The repository containing our discounts.</param>
        public DefaultPriceCalculator(IDiscountRepository repository) : this(repository, new FullPrice())
        {
        }

        /// <summary>
        /// Initializes a price calculator class with supplied default pricing strategy.
        /// </summary>
        /// <param name="repository">The repository containing our discounts.</param>
        /// <param name="defaultPriceStrategy">The default pricing strategy to use when no discounts can be applied.</param>
        public DefaultPriceCalculator(IDiscountRepository repository, IOrderItemDiscount defaultPriceStrategy)
        {
            if (repository == null) { throw new ArgumentNullException("repository"); }
            if (defaultPriceStrategy == null) { throw new ArgumentNullException("defaultPriceStrategy"); }
            _repository = repository;
            Default = defaultPriceStrategy;
        }

        /// <summary>
        /// The default pricing strategy to use when no discounts are found.
        /// </summary>
        public IOrderItemDiscount Default { get; private set; }

        /// <summary>
        /// Calculates an <see cref="OrderItem"/>'s price for the <see cref="Customer"/>,
        /// applying the available discounts.
        /// </summary>
        /// <param name="customer">The <see cref="Customer"/> for which the price should be calculated.</param>
        /// <param name="item">The <see cref="OrderItem"/> for which the price should be calculated.</param>
        /// <returns>The item's lowest price.</returns>
        public decimal CalculatePrice(Customer customer, OrderItem item)
        {
            var biggestDiscount = _repository.GetDiscountsFor(item.Identifier)
                .Where(d => d.MatchesTo(customer, item))
                .OrderBy(e => e.Discount.CalculateItemPrice(item))
                .FirstOrDefault();

            if (biggestDiscount == null)
            {
                return Default.CalculateItemPrice(item);
            }

            return biggestDiscount.Discount.CalculateItemPrice(item);
        }
    }
}