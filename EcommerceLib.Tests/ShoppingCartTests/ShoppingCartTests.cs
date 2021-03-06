﻿using System.Linq;
using EcommerceLib.Domain;
using EcommerceLib.Services.PriceCalculator;
using Moq;
using NUnit.Framework;

namespace EcommerceLib.Tests.ShoppingCartTests
{
    public class ShoppingCartTests
    {
        protected Mock<IPriceCalculator> PriceCalculator;

        [SetUp]
        public void SetUp()
        {
            PriceCalculator = new Mock<IPriceCalculator>();
        }

        [Test]
        public void New_shopping_cart_should_be_empty()
        {
            var subject = new ShoppingCart(PriceCalculator.Object);
            Assert.That(subject.Items, Is.Empty);
        }

        [Test]
        public void A_newly_added_item_should_be_in_the_carts_items()
        {
            var subject = new ShoppingCart(PriceCalculator.Object);
            var orderItem = new OrderItem {Identifier = "Item-1", ItemCost = 50m, Quantity = 1};

            subject.Add(orderItem);
            Assert.That(subject.Items.Count(), Is.EqualTo(1));
            Assert.That(subject.Items.Contains(orderItem));
        }

        [Test]
        public void Removing_an_item_should_be_removed_from_the_items()
        {
            var subject = new ShoppingCart(PriceCalculator.Object);
            var orderItem = new OrderItem { Identifier = "Item-1", ItemCost = 50m, Quantity = 1 };
            subject.Add(orderItem);

            subject.Remove(orderItem.Identifier);
            Assert.That(subject.Items, Is.Empty);
        }

        [Test]
        public void Trying_to_removing_a_nonexistent_item_should_not_change_the_carts_items()
        {
            var subject = new ShoppingCart(PriceCalculator.Object);
            var orderItem = new OrderItem { Identifier = "Item-1", ItemCost = 50m, Quantity = 1 };
            subject.Add(orderItem);


            subject.Remove("Invalid-1");
            Assert.That(subject.Items.Count(), Is.EqualTo(1));
            Assert.That(subject.Items.Contains(orderItem));
        }

        [Test]
        public void Adding_an_existing_item_should_change_the_quantity_in_the_carts_items()
        {
            var subject = new ShoppingCart(PriceCalculator.Object);
            var orderItem = new OrderItem { Identifier = "Item-1", ItemCost = 50m, Quantity = 1 };
            subject.Add(orderItem);

            var newOrderItem = new OrderItem {Identifier = "Item-1", ItemCost = 50m, Quantity = 3 };
            subject.Add(newOrderItem);
            Assert.That(subject.Items.Count(), Is.EqualTo(1));
            Assert.That(subject.Items.Single(i => i.Identifier == orderItem.Identifier).Quantity, Is.EqualTo(4));
        }

        [Test]
        public void Totalcost_calculation_should_use_the_pricecalculators_CalculatePrice_for_every_item()
        {
            var subject = new ShoppingCart(PriceCalculator.Object);
            var firstItem = new OrderItem {Identifier = "Item-1", ItemCost = 50m, Quantity = 1};
            var secondItem = new OrderItem {Identifier = "Item-2", ItemCost = 150m, Quantity = 3};
            subject.Add(firstItem);
            subject.Add(secondItem);

            subject.GetTotalCost();
            PriceCalculator.Verify(pc => pc.CalculatePrice(It.IsAny<Customer>(), firstItem), Times.Once);
            PriceCalculator.Verify(pc => pc.CalculatePrice(It.IsAny<Customer>(), secondItem), Times.Once);
        }
    }
}