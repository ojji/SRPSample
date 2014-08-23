using System;
using EcommerceLib.Domain;
using EcommerceLib.Domain.PricingStrategies;
using NUnit.Framework;

namespace EcommerceLib.Tests.PricingStrategyTests
{
    public class FlatXPercentOffTests
    {
        [Test]
        public void Null_or_empty_itemid_should_throw_ArgumentNullException()
        {
            Assert.DoesNotThrow(() => new FlatXPercentOff("item-1", 0.15m));
            Assert.Throws<ArgumentNullException>(() => new FlatXPercentOff("", 0.15m));
            Assert.Throws<ArgumentNullException>(() => new FlatXPercentOff(null, 0.15m));
        }

        [Test]
        public void Discount_Percentage_should_be_between_0_and_100_percent()
        {
            Assert.DoesNotThrow(() => new FlatXPercentOff("item-1", 0.15m));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FlatXPercentOff("item-1", 0.0m));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FlatXPercentOff("item-1", 1.00m));
        }

        public class Matches_item
        {
            [Test]
            public void Should_return_false_for_non_matched_item()
            {
                var subject = new FlatXPercentOff("item-1", 0.15m);
                var orderItem = new OrderItem {Identifier = "item-2", ItemCost = 50m, Quantity = 10};

                Assert.IsFalse(subject.MatchesItem(orderItem));
            }

            [Test]
            public void Should_return_true_for_matching_item()
            {
                var subject = new FlatXPercentOff("item-1", 0.15m);
                var orderItem = new OrderItem { Identifier = "item-1", ItemCost = 50m, Quantity = 10 };

                Assert.IsTrue(subject.MatchesItem(orderItem));
            }

            [Test]
            public void ItemId_and_Discount_properties_should_comply_with_construction_parameters()
            {
                var subject = new FlatXPercentOff("item-1", 0.30m);

                Assert.That(subject.ItemId, Is.EqualTo("item-1"));
                Assert.That(subject.Discount, Is.EqualTo(0.3m));
            }

            [Test]
            public void Name_should_contain_the_discount_percentage_and_off()
            {
                var subject = new FlatXPercentOff("item-1", 0.30m);

                Assert.IsTrue(subject.Name.Contains(string.Format("{0:p2} off", 0.3m)));
            }

            [Test]
            public void Should_not_match_when_order_is_null()
            {
                var subject = new FlatXPercentOff("item-1", 0.3m);
                Assert.IsFalse(subject.MatchesItem(null));
            }
        }

        public class Calculate_price
        {
            [Test]
            public void Should_throw_ArgumentException_for_non_matching_orderitem()
            {
                var subject = new FlatXPercentOff("item-1", 0.15m);
                var orderItem = new OrderItem { Identifier = "item-2", ItemCost = 50m, Quantity = 10 };
                Assert.Throws<ArgumentException>(() => subject.CalculateItemPrice(orderItem));
            }

            [Test]
            public void Should_return_discounted_price_for_matching_orderitem()
            {
                var subject = new FlatXPercentOff("item-1", 0.10m);
                var orderItem = new OrderItem { Identifier = "item-1", ItemCost = 50m, Quantity = 10 };
                Assert.That(subject.CalculateItemPrice(orderItem), Is.EqualTo(500m*0.9m));
            }
        }
    }
}