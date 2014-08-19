using EcommerceLib.Domain;
using EcommerceLib.Services.PriceCalculator;
using Moq;
using NUnit.Framework;

namespace EcommerceLib.Tests.DomainTests.OrderTests
{
    public class OrderTests
    {
        [Test]
        public void New_orders_should_have_a_waiting_for_processing_state()
        {
            var subject = new Order(new ShoppingCart(new Mock<IPriceCalculator>().Object), new PaymentDetails());
            Assert.That(subject.State, Is.EqualTo(OrderState.AwaitingProcess));
        }
       
    }
}