using System.Collections.Generic;
using EcommerceLib.Domain;
using EcommerceLib.Domain.PricingStrategies;

namespace EcommerceLib.Repositories
{
    public interface IDiscountRepository
    {
        IEnumerable<IMembershipDiscount> GetDiscountsFor(string itemId);
        IOrderItemDiscount GetDiscountForCode(string code);
    }
}