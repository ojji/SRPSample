using System;
using EcommerceLib.Domain;
using EcommerceLib.Domain.PricingStrategies;
using NUnit.Framework;

namespace EcommerceLib.Tests.PricingStrategyTests.BuyXItemsGetYDiscountFromPriceTests
{
    public class BuyXItemsGetYDiscountFromPriceTests
    {
        [Test]
        public void Empty_or_null_itemid_should_throw_ArgumentNullException()
        {
            Assert.DoesNotThrow(() => new BuyXItemsGetYDiscountFromPrice("item-1", 1, 0.1m));
            Assert.Throws<ArgumentNullException>(() => new BuyXItemsGetYDiscountFromPrice("", 1, 0.1m));
            Assert.Throws<ArgumentNullException>(() => new BuyXItemsGetYDiscountFromPrice(null, 1, 0.1m));
            
        }
      
        [Test]
        public void Quantity_should_be_positive()
        {
            Assert.DoesNotThrow(() => new BuyXItemsGetYDiscountFromPrice("item-1", 1, 0.1m));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BuyXItemsGetYDiscountFromPrice("item-1", 0, 0.1m));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BuyXItemsGetYDiscountFromPrice("item-1", -1, 0.1m));
        }

        [Test]
        public void Discount_should_be_between_0_and_100_percent()
        {
            Assert.DoesNotThrow(() => new BuyXItemsGetYDiscountFromPrice("item-1", 4, 0.2m));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BuyXItemsGetYDiscountFromPrice("item-1", 4, 1.00m));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BuyXItemsGetYDiscountFromPrice("item-1", 4, 0.00m));
        }

        public class Matches_item
        {
            [Test]
            public void Should_return_false_for_non_matched_item()
            {
                var subject = new BuyXItemsGetYDiscountFromPrice("item-1", 2, 0.25m);
                var orderitem = new OrderItem {Identifier = "item-2", ItemCost = 20m, Quantity = 2};

                Assert.IsFalse(subject.MatchesItem(orderitem));
            }

            [Test]
            public void Should_return_false_for_less_than_x_items()
            {
                var subject = new BuyXItemsGetYDiscountFromPrice("item-1", 2, 0.25m);
                var orderitem = new OrderItem { Identifier = subject.ItemId, ItemCost = 20m, Quantity = 1 };

                Assert.IsFalse(subject.MatchesItem(orderitem));
            }

            [Test]
            public void Should_return_true_for_at_least_x_items()
            {
                var subject = new BuyXItemsGetYDiscountFromPrice("item-1", 2, 0.25m);
                var orderitem = new OrderItem { Identifier = subject.ItemId, ItemCost = 20m, Quantity = 2 };
                var secondOrderitem = new OrderItem { Identifier = subject.ItemId, ItemCost = 20m, Quantity = 2 };

                Assert.IsTrue(subject.MatchesItem(orderitem));
                Assert.IsTrue(subject.MatchesItem(secondOrderitem));
            }
        }

        public class Price_calculation
        {
            [Test]
            public void Should_throw_ArgumentException_for_non_matching_orderitem()
            {
                var subject = new BuyXItemsGetYDiscountFromPrice("item-1", 3, 0.25m);
                var incorrectItem = new OrderItem() {Identifier = "item-2", ItemCost = 10m, Quantity = 1};

                Assert.Throws<ArgumentException>(() => subject.CalculateItemPrice(incorrectItem));
            }

            [Test]
            public void When_having_exactly_x_items_the_discount_should_apply_to_all_of_them()
            {
                var subject = new BuyXItemsGetYDiscountFromPrice("item-1", 3, 0.25m);
                var item = new OrderItem {Identifier = "item-1", ItemCost = 10m, Quantity = 3};

                var actual = subject.CalculateItemPrice(item);
                Assert.That(actual, Is.EqualTo(3 * 7.5m));
            }

            [Test]
            public void When_having_multiple_sets_of_x_items_the_discount_should_apply_to_all_of_them()
            {
                var subject = new BuyXItemsGetYDiscountFromPrice("item-1", 3, 0.25m);
                var item = new OrderItem { Identifier = "item-1", ItemCost = 10m, Quantity = 6 };

                var actual = subject.CalculateItemPrice(item);
                Assert.That(actual, Is.EqualTo(6 * 7.5m));
            }
      

            [Test]
            public void When_having_not_exact_sets_of_x_items_the_remaining_items_should_cost_full_price()
            {
                var subject = new BuyXItemsGetYDiscountFromPrice("item-1", 3, 0.25m);
                var item = new OrderItem { Identifier = "item-1", ItemCost = 10m, Quantity = 5 };

                var actual = subject.CalculateItemPrice(item);
                Assert.That(actual, Is.EqualTo(3 * 7.5m + 2 * 10m));
            }
        }
    }
}