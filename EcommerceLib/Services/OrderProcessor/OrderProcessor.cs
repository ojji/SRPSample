using System;
using System.Linq;
using EcommerceLib.Domain;

namespace EcommerceLib.Services.OrderProcessor
{
    /// <summary>
    /// This class provides the different needs for processing orders.
    /// </summary>
    public class OrderProcessor : IOrderProcessor
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPaymentService _paymentService;
        private readonly INofiticationService _notificationService;

        /// <summary>
        /// Initializes a new order processing object.
        /// </summary>
        /// <param name="inventoryService">The inventory managing service object.</param>
        /// <param name="paymentService">The payment service to use with the orderprocessor.</param>
        /// <param name="notificationService">The notification service to inform customers about their processed order.</param>
        public OrderProcessor(IInventoryService inventoryService, IPaymentService paymentService, INofiticationService notificationService)
        {
            _inventoryService = inventoryService;
            _paymentService = paymentService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Performs an order checkout.
        /// </summary>
        /// <param name="order">The order to be processed.</param>
        /// <exception cref="OrderFailedException">Processing this order was unsuccessful. The inner exception type represents the reason of failing.</exception>
        public void Checkout(Order order)
        {
            if (!order.Cart.Items.Any()) return;
            if (order.State != OrderState.AwaitingProcess) { throw new InvalidOperationException("This order is already processed."); }

            if (!_inventoryService.ReserveItems(order.Cart.Items))
            {
                order.State = OrderState.InventoryReservationFailed;
                throw new OrderFailedException("order failed", new ItemReservationFailedException(order.Cart.Items));
            }

            if (!_paymentService.ProcessPayment(order.PaymentDetails, order.Cart.GetTotalCost()))
            {
                order.State = OrderState.PaymentFailed;
                _inventoryService.CancelReservation(order.Cart.Items);
                throw new OrderFailedException("order failed", new PaymentFailedException(order.PaymentDetails));
            }

            _notificationService.NotifyCustomerOrderCreated(order.Cart);
            _inventoryService.HandoutItemsToDelivery(order.Cart.Items);
            order.State = OrderState.Processed;
        }
    }
}