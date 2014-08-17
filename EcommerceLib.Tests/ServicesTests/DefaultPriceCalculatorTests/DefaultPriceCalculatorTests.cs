using System.Collections.Generic;
using EcommerceLib.Domain;
using EcommerceLib.Domain.PricingStrategies;
using EcommerceLib.Repositories;
using EcommerceLib.Services.PriceCalculator;
using Moq;
using NUnit.Framework;

namespace EcommerceLib.Tests.ServicesTests.DefaultPriceCalculatorTests
{
    public class DefaultPriceCalculatorTests
    {
        protected Mock<IDiscountRepository> MockedRepository;

        public class Calculate_price
        {
            [Test]
            public void Price_calculator_without_a_discount_repo_should_calculate_full_price()
            {
                var subject = new DefaultPriceCalculator();
                var orderItem = new OrderItem { Identifier = "item-1", ItemCost = 10m, Quantity = 2 };

                subject.CalculatePrice(orderItem);
                Assert.That(subject.CalculatePrice(orderItem), Is.EqualTo(20m));
            }
      

            [Test]
            public void Should_check_orderitem_for_discounts()
            {
                var repository = new Mock<IDiscountRepository>();
                var subject = new DefaultPriceCalculator(repository.Object);
                var orderItem = new OrderItem {Identifier = "item-1", ItemCost = 10m, Quantity = 2};

                subject.CalculatePrice(orderItem);
                repository.Verify(r => r.GetDiscountsFor(orderItem.Identifier), Times.Once);
            }

            [Test]
            public void Items_not_discounted_should_have_full_price()
            {
                var repository = new Mock<IDiscountRepository>();
                var orderItem = new OrderItem {Identifier = "item-1", ItemCost = 10m, Quantity = 2};
                var subject = new DefaultPriceCalculator(repository.Object);

                Assert.That(subject.CalculatePrice(orderItem), Is.EqualTo(orderItem.Quantity*orderItem.ItemCost));
            }

            [Test]
            public void Items_having_multiple_discounts_should_return_with_the_lowest_price()
            {
                var higherPriceStrategy = new Mock<IPricingStrategy>();
                higherPriceStrategy.Setup(s => s.CalculateItemPrice(It.IsAny<OrderItem>())).Returns(1000m);
                var lowestPriceStrategy = new Mock<IPricingStrategy>();
                lowestPriceStrategy.Setup(s => s.CalculateItemPrice(It.IsAny<OrderItem>())).Returns(1m);
                var highestPriceStrategy = new Mock<IPricingStrategy>();
                highestPriceStrategy.Setup(s => s.CalculateItemPrice(It.IsAny<OrderItem>())).Returns(2000m);
                var repository = new Mock<IDiscountRepository>();
                repository
                    .Setup(r => r.GetDiscountsFor(It.IsAny<string>()))
                    .Returns(new List<IPricingStrategy>
                    {
                        higherPriceStrategy.Object,
                        lowestPriceStrategy.Object,
                        higherPriceStrategy.Object
                    });

                var order = new OrderItem {Identifier = "item-1", ItemCost = 50m, Quantity = 2};
                var subject = new DefaultPriceCalculator(repository.Object);
                Assert.That(subject.CalculatePrice(order), Is.EqualTo(1m));
            }
        }
    }
}