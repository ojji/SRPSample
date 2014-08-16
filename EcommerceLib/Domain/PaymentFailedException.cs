using System;

namespace EcommerceLib.Domain
{
    public class PaymentFailedException : Exception
    {
        private readonly PaymentDetails _paymentDetails;

        public PaymentFailedException(PaymentDetails paymentDetails, string message = null) : base(message)
        {
            _paymentDetails = paymentDetails;
        }

        public PaymentDetails PaymentDetails
        {
            get { return _paymentDetails; }
        }

    }
}