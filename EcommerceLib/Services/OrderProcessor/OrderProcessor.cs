﻿using System.Linq;
using EcommerceLib.Domain;

namespace EcommerceLib.Services.OrderProcessor
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPaymentService _paymentService;
        private readonly INofiticationService _notificationService;

        public OrderProcessor(IInventoryService inventoryService, IPaymentService paymentService, INofiticationService notificationService)
        {
            _inventoryService = inventoryService;
            _paymentService = paymentService;
            _notificationService = notificationService;
        }

        public void Checkout(Order order)
        {
            if (!order.Cart.Items.Any()) return;

            if (!_inventoryService.ReserveItems(order.Cart.Items))
            {
                throw new OrderFailedException("order failed", new ItemReservationFailedException(order.Cart.Items));
            }

            if (!_paymentService.ProcessPayment(order.PaymentDetails, order.Cart.GetTotalCost()))
            {
                _inventoryService.CancelReservation(order.Cart.Items);
                throw new OrderFailedException("order failed", new PaymentFailedException(order.PaymentDetails));
            }

            _notificationService.NotifyCustomerOrderCreated(order.Cart);
            _inventoryService.HandoutItemsToDelivery(order.Cart.Items);
        }
    }
}