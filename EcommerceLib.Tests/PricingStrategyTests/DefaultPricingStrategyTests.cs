using EcommerceLib.Domain;
using EcommerceLib.Domain.PricingStrategies;
using NUnit.Framework;

namespace EcommerceLib.Tests.PricingStrategyTests
{
    public class DefaultPricingStrategyTests
    {
        [Test]
        public void Should_match_any_orderitem()
        {
            var subject = new DefaultPricingStrategy();
            var firstItem = new OrderItem { Identifier = "Item-1", ItemCost = 10m, Quantity = 1 };
            var secondItem = new OrderItem { Identifier = "Item-2", ItemCost = 10m, Quantity = 4 };

            Assert.IsTrue(subject.MatchesItem(firstItem));
            Assert.IsTrue(subject.MatchesItem(secondItem));
        }

        [Test]
        public void The_price_should_equal_to_the_product_of_quantity_and_itemprice()
        {
            var subject = new DefaultPricingStrategy();
            var firstItem = new OrderItem { Identifier = "Item-1", ItemCost = 10m, Quantity = 1};
            var secondItem = new OrderItem { Identifier = "Item-2", ItemCost = 10m, Quantity = 4 };
            
            Assert.That(subject.CalculateItemPrice(firstItem), Is.EqualTo(10m));
            Assert.That(subject.CalculateItemPrice(secondItem), Is.EqualTo(40m));
        }
    }
}