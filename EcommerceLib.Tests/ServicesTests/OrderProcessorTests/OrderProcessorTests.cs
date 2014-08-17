using System;
using System.Collections.Generic;
using EcommerceLib.Domain;
using EcommerceLib.Services;
using EcommerceLib.Services.OrderProcessor;
using EcommerceLib.Services.PriceCalculator;
using Moq;
using NUnit.Framework;

namespace EcommerceLib.Tests.ServicesTests.OrderProcessorTests
{
    public class OrderProcessorTests
    {
        public class Checking_out : CheckoutTestBase
        {
            [Test]
            public void An_empty_shopping_cart_should_return_immediately()
            {
                var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);
                var emptyShoppingCart = new ShoppingCart(new Mock<IPriceCalculator>().Object) { CustomerEmail = "empty@user.com" };
                var emptyOrder = new Order(emptyShoppingCart, new PaymentDetails());

                subject.Checkout(emptyOrder);
                InventoryService.Verify(s => s.ReserveItems(It.IsAny<IEnumerable<OrderItem>>()), Times.Never);
            }


            [Test]
            public void Should_reserve_items_in_the_inventory()
            {
                var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);
                subject.Checkout(Order);
                InventoryService.Verify(s => s.ReserveItems(Order.Cart.Items), Times.Once);
            }

            [TestFixture]
            public class When_inventory_reservation_fails : CheckoutTestBase
            {
                [SetUp]
                public new void SetUp()
                {
                    InventoryService = OrderTestUtils.GetFailingInventoryService();
                }

                [Test]
                public void Should_throw_OrderFailedException()
                {
                    var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);

                    Assert.That(() => subject.Checkout(Order), Throws.Exception.TypeOf<OrderFailedException>());
                    Assert.That(() => subject.Checkout(Order), Throws.InnerException.TypeOf<ItemReservationFailedException>());
                }

                [Test]
                public void The_item_reservation_exception_should_contain_the_items_that_could_not_be_reserved()
                {
                    var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);
                    Assert.That(() => subject.Checkout(Order), Throws.InnerException.Property("Items").EqualTo(Order.Cart.Items));
                }

                [Test]
                public void The_user_should_not_be_charged()
                {
                    try
                    {
                        var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);
                        subject.Checkout(Order);
                    }
                    catch { }
                    finally
                    {
                        PaymentService.Verify(s => s.ProcessPayment(It.IsAny<PaymentDetails>(), It.IsAny<decimal>()),
                        Times.Never);
                    }
                }
            }

            [TestFixture]
            public class When_inventory_reservation_succeeds : CheckoutTestBase
            {
                [Test]
                public void Should_call_charge_with_the_carts_total_value()
                {
                    try
                    {
                        var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);
                        subject.Checkout(Order);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        PaymentService.Verify(s => s.ProcessPayment(Order.PaymentDetails, Order.Cart.GetTotalCost()), Times.Once);
                    }
                }

                [TestFixture]
                public class When_charging_attempt_fails : When_inventory_reservation_succeeds
                {
                    [SetUp]
                    public new void SetUp()
                    {
                        PaymentService = OrderTestUtils.GetFailingPaymentService();
                    }

                    [Test]
                    public void Should_put_back_reserved_items_to_the_inventory()
                    {
                        try
                        {
                            var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);
                            subject.Checkout(Order);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            InventoryService.Verify(s => s.CancelReservation(Order.Cart.Items), Times.Once);
                        }
                    }

                    [Test]
                    public void Should_throw_OrderFailedException()
                    {
                        var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);

                        Assert.That(() => subject.Checkout(Order), Throws.Exception.TypeOf<OrderFailedException>());
                        Assert.That(() => subject.Checkout(Order), Throws.InnerException.TypeOf<PaymentFailedException>().And.InnerException.Property("PaymentDetails").EqualTo(Order.PaymentDetails));
                    }
                }

                [TestFixture]
                public class When_charge_succeeds : CheckoutTestBase
                {
                    [Test]
                    public void Checking_out_should_not_throw()
                    {
                        var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);
                        Assert.DoesNotThrow(() => subject.Checkout(Order));
                    }

                    [Test]
                    public void Checking_out_successfully_should_create_a_confirmation_to_the_user()
                    {
                        var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);
                        subject.Checkout(Order);
                        NotificationService.Verify(s => s.NotifyCustomerOrderCreated(Order.Cart), Times.Once);
                    }

                    [Test]
                    public void Reserved_items_should_be_shipped_to_user()
                    {
                        var subject = new OrderProcessor(InventoryService.Object, PaymentService.Object, NotificationService.Object);
                        subject.Checkout(Order);
                        InventoryService.Verify(s => s.HandoutItemsToDelivery(Order.Cart.Items), Times.Once);
                    }
                }
            }
        }

        public abstract class CheckoutTestBase
        {
            protected Order Order;
            protected Mock<IInventoryService> InventoryService;
            protected Mock<IPaymentService> PaymentService;
            protected Mock<INofiticationService> NotificationService;

            [SetUp]
            public void SetUp()
            {
                Order = new Order(OrderTestUtils.GetSimpleShoppingCart(), OrderTestUtils.GetSimplePaymentDetails());
                InventoryService = OrderTestUtils.GetSucceedingInventoryService();
                PaymentService = OrderTestUtils.GetSucceedingPaymentService();
                NotificationService = OrderTestUtils.GetSimpleNotificationService();
            }
        }

        internal static class OrderTestUtils
        {
            internal static ShoppingCart GetSimpleShoppingCart()
            {
                var cart = new ShoppingCart(new Mock<IPriceCalculator>().Object)
                {
                    CustomerEmail = "sample@user.com"
                };
                cart.Add(new OrderItem());
                return cart;
            }

            internal static Mock<IInventoryService> GetSucceedingInventoryService()
            {
                var inventoryService = new Mock<IInventoryService>();
                inventoryService.Setup(s => s.ReserveItems(It.IsAny<IEnumerable<OrderItem>>()))
                    .Returns(true);

                return inventoryService;
            }

            internal static Mock<IInventoryService> GetFailingInventoryService()
            {
                var inventoryService = new Mock<IInventoryService>();
                inventoryService.Setup(s => s.ReserveItems(It.IsAny<IEnumerable<OrderItem>>()))
                    .Returns(false);

                return inventoryService;
            }

            internal static Mock<IPaymentService> GetSucceedingPaymentService()
            {
                var paymentService = new Mock<IPaymentService>();
                paymentService.Setup(s => s.ProcessPayment(It.IsAny<PaymentDetails>(), It.IsAny<decimal>()))
                    .Returns(true);

                return paymentService;
            }

            internal static Mock<IPaymentService> GetFailingPaymentService()
            {
                var paymentService = new Mock<IPaymentService>();
                paymentService.Setup(s => s.ProcessPayment(It.IsAny<PaymentDetails>(), It.IsAny<decimal>()))
                    .Returns(false);

                return paymentService;
            }

            internal static PaymentDetails GetSimplePaymentDetails()
            {
                return new PaymentDetails()
                {
                    CardHolderName = "Sample User",
                    CreditCardNumber = "1234-1234-1234-1234",
                    ExpiryDate = new DateTime(2015, 09, 01),
                    PaymentMethod = PaymentMethod.CreditCard
                };
            }

            internal static Mock<INofiticationService> GetSimpleNotificationService()
            {
                return new Mock<INofiticationService>();
            }
        }
    }
}