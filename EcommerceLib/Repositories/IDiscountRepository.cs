using System.Collections.Generic;
using EcommerceLib.Domain.PricingStrategies;

namespace EcommerceLib.Repositories
{
    public interface IDiscountRepository
    {
        IEnumerable<IPricingStrategy> GetDiscountsFor(string itemId);
        IPricingStrategy GetDiscountForCode(string code);
    }
}