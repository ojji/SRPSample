using System;
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

        [Test]
        public void Calculator_with_null_discount_repository_should_throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultPriceCalculator(null));
        }

        [Test]
        public void Calculator_with_no_custom_default_strategy_supplied_should_use_the_fullprice_strategy()
        {
            var subject = new DefaultPriceCalculator(new Mock<IDiscountRepository>().Object);
            Assert.IsInstanceOf<FullPrice>(subject.Default);
        }

        [Test]
        public void Calculator_should_throw_argumentnullexception_when_null_custom_default_pricingstrategy_is_supplied()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultPriceCalculator(new Mock<IDiscountRepository>().Object, null));
        }

        public class Calculate_price
        {
            [Test]
            public void Calculating_price_for_null_customer_should_calculate_the_full_price()
            {
                var repository = new Mock<IDiscountRepository>();
                var orderItem = new OrderItem {Identifier = "item-1", ItemCost = 10m, Quantity = 2};

                var subject = new DefaultPriceCalculator(repository.Object);
                Assert.That(subject.CalculatePrice(null, orderItem),
                    Is.EqualTo(orderItem.ItemCost*orderItem.Quantity));
            }
      
            [Test]
            public void Should_check_customers_orderitem_for_discounts()
            {
                var repository = new Mock<IDiscountRepository>();
                var basicCustomer = new Customer() {Level = MembershipLevel.Basic};
                var subject = new DefaultPriceCalculator(repository.Object);
                var orderItem = new OrderItem {Identifier = "item-1", ItemCost = 10m, Quantity = 2};

                subject.CalculatePrice(basicCustomer, orderItem);
                repository.Verify(r => r.GetDiscountsFor(orderItem.Identifier), Times.Once);
            }

            [Test]
            public void Should_verify_whether_discount_is_appliable_to_customer()
            {
                var repository = new Mock<IDiscountRepository>();
                var basicCustomer = new Customer() { Level = MembershipLevel.Basic };
                var orderItem = new OrderItem { Identifier = "item-1", ItemCost = 10m, Quantity = 2 };

                var notAppliableDiscount = new Mock<IMembershipDiscount>();
                notAppliableDiscount.Setup(d => d.MatchesTo(basicCustomer, It.IsAny<OrderItem>())).Returns(false);
                notAppliableDiscount.SetupGet(d => d.Discount).Returns(new Mock<IOrderItemDiscount>().Object);
                
                repository.Setup(r => r.GetDiscountsFor(It.IsAny<string>())).Returns(new List<IMembershipDiscount>() {notAppliableDiscount.Object});
                var subject = new DefaultPriceCalculator(repository.Object);

                Assert.That(subject.CalculatePrice(basicCustomer, orderItem), Is.EqualTo(20m));
            }
      

            [Test]
            public void Items_not_discounted_should_have_full_price()
            {
                var repository = new Mock<IDiscountRepository>();
                var basicCustomer = new Customer() { Level = MembershipLevel.Basic };
                var orderItem = new OrderItem {Identifier = "item-1", ItemCost = 10m, Quantity = 2};
                var subject = new DefaultPriceCalculator(repository.Object);

                Assert.That(subject.CalculatePrice(basicCustomer, orderItem), Is.EqualTo(orderItem.Quantity*orderItem.ItemCost));
            }

            [Test]
            public void Items_having_multiple_discounts_should_return_with_the_lowest_price()
            {
                var order = new OrderItem { Identifier = "item-1", ItemCost = 50m, Quantity = 2 };
                var basicCustomer = new Customer() { Level = MembershipLevel.Basic };

                var higherPriceStrategy = new Mock<IOrderItemDiscount>();
                higherPriceStrategy.Setup(s => s.CalculateItemPrice(order)).Returns(1000m);
                var higherDiscount = new Mock<IMembershipDiscount>() {Name = "higher discount"};
                higherDiscount.SetupGet(d => d.Discount).Returns(higherPriceStrategy.Object);
                higherDiscount.Setup(d => d.MatchesTo(basicCustomer, order)).Returns(true);

                var lowestPriceStrategy = new Mock<IOrderItemDiscount>();
                lowestPriceStrategy.Setup(s => s.CalculateItemPrice(order)).Returns(1m);
                var lowestDiscount = new Mock<IMembershipDiscount>();
                lowestDiscount.SetupGet(d => d.Discount).Returns(lowestPriceStrategy.Object);
                lowestDiscount.Setup(d => d.MatchesTo(basicCustomer, order)).Returns(true);

                var highestPriceStrategy = new Mock<IOrderItemDiscount>();
                highestPriceStrategy.Setup(s => s.CalculateItemPrice(order)).Returns(2000m);
                var highestDiscount = new Mock<IMembershipDiscount>();
                highestDiscount.SetupGet(d => d.Discount).Returns(highestPriceStrategy.Object);
                highestDiscount.Setup(d => d.MatchesTo(basicCustomer, order)).Returns(true);

                var repository = new Mock<IDiscountRepository>();
                repository
                    .Setup(r => r.GetDiscountsFor(It.IsAny<string>()))
                    .Returns(new List<IMembershipDiscount>
                    {
                        higherDiscount.Object,
                        lowestDiscount.Object,
                        highestDiscount.Object
                    });

                var subject = new DefaultPriceCalculator(repository.Object);
                Assert.That(subject.CalculatePrice(basicCustomer, order), Is.EqualTo(1m));
            }
        }
    }
}