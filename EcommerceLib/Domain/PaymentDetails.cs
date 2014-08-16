using System;

namespace EcommerceLib.Domain
{
    public class PaymentDetails
    {
        public PaymentMethod PaymentMethod { get; set; }
        public string CreditCardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string CardHolderName { get; set; }
    }
}