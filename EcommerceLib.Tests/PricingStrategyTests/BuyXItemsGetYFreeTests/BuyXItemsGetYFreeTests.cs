using System;
using EcommerceLib.Domain;
using EcommerceLib.Domain.PricingStrategies;
using NUnit.Framework;

namespace EcommerceLib.Tests.PricingStrategyTests.BuyXItemsGetYFreeTests
{
    public class BuyXItemsGetYFreeTests
    {
        [Test]
        public void Null_or_empty_itemid_should_throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BuyXItemsGetYFree(null, 2, 1));
            Assert.Throws<ArgumentNullException>(() => new BuyXItemsGetYFree("", 2, 1));
        }
      

        [Test]
        public void Quantity_to_buy_should_be_positive()
        {
            Assert.DoesNotThrow(() => new BuyXItemsGetYFree("item-1", 2, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BuyXItemsGetYFree("item-1", 0, 1));
        }

        [Test]
        public void Quantity_to_get_free_should_be_positive()
        {
            Assert.DoesNotThrow(() => new BuyXItemsGetYFree("item-1", 2, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BuyXItemsGetYFree("item-1", 2, 0));
        }
      
        public class Matches_items
        {
            [Test]
            public void Should_return_false_for_non_matched_items()
            {
                var subject = new BuyXItemsGetYFree("item-1", 2, 1);
                var incorrectOrderItem = new OrderItem {Identifier = "item-2", ItemCost = 10m, Quantity = 3};

                Assert.IsFalse(subject.MatchesItem(incorrectOrderItem));
            }

            [Test]
            public void Should_return_false_for_less_than_x_items()
            {
                var subject = new BuyXItemsGetYFree("item-1", 2, 1);
                var orderItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 1 };

                Assert.IsFalse(subject.MatchesItem(orderItem));
            }

            [Test]
            public void Should_return_true_for_at_least_x_items()
            {
                var subject = new BuyXItemsGetYFree("item-1", 2, 1);
                var firstItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 2 };
                var secondItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 3 };
                var thirdItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 4 };

                Assert.IsTrue(subject.MatchesItem(firstItem));
                Assert.IsTrue(subject.MatchesItem(secondItem));
                Assert.IsTrue(subject.MatchesItem(thirdItem));
            }
        }

        public class Item_price_calculation
        {
            [Test]
            public void Should_throw_ArgumentException_for_invalid_orderitem()
            {
                var subject = new BuyXItemsGetYFree("item-1", 2, 1);
                var incorrectOrderItem = new OrderItem { Identifier = "item-2", ItemCost = 10m, Quantity = 3 };

                Assert.Throws<ArgumentException>(() => subject.CalculateItemPrice(incorrectOrderItem));
            }

            [Test]
            public void Should_calculate_set_price_for_exact_sets()
            {
                var subject = new BuyXItemsGetYFree("item-1", 3, 2);
                var firstOrderItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 5 };
                var secondOrderItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 10 };

                Assert.That(subject.CalculateItemPrice(firstOrderItem), Is.EqualTo(30m));
                Assert.That(subject.CalculateItemPrice(secondOrderItem), Is.EqualTo(60m));
            }

            [Test]
            public void Should_calculate_set_price_when_ordered_quantity_is_more_than_X_but_free_items_are_less_than_Y()
            {
                var subject = new BuyXItemsGetYFree("item-1", 3, 2);
                var firstOrderItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 3 };
                var secondOrderItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 4 };
                var thirdOrderItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 5 };

                Assert.That(subject.CalculateItemPrice(firstOrderItem), Is.EqualTo(30m));
                Assert.That(subject.CalculateItemPrice(secondOrderItem), Is.EqualTo(30m));
                Assert.That(subject.CalculateItemPrice(thirdOrderItem), Is.EqualTo(30m));
            }

            [Test]
            public void Should_calculate_full_price_for_items_not_in_set()
            {
                var subject = new BuyXItemsGetYFree("item-1", 3, 2);
                var firstOrderItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 5 };
                var secondOrderItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 6 };
                var thirdOrderItem = new OrderItem { Identifier = subject.ItemId, ItemCost = 10m, Quantity = 7 };

                Assert.That(subject.CalculateItemPrice(firstOrderItem), Is.EqualTo(30m));
                Assert.That(subject.CalculateItemPrice(secondOrderItem), Is.EqualTo(40m));
                Assert.That(subject.CalculateItemPrice(thirdOrderItem), Is.EqualTo(50m));
            }
        }
    }
}