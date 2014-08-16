using System;
using System.Collections.Generic;

namespace EcommerceLib.Domain
{
    public class ItemReservationFailedException : Exception
    {
        private readonly IEnumerable<OrderItem> _failedItems;

        public ItemReservationFailedException(IEnumerable<OrderItem> failedItems, string message = null)
            : base(message)
        {
            _failedItems = failedItems;
        }

        public IEnumerable<OrderItem> Items
        {
            get { return _failedItems; }
        }
    }
}