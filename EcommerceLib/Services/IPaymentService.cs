using EcommerceLib.Domain;

namespace EcommerceLib.Services
{
    public interface IPaymentService
    {
        bool ProcessPayment(PaymentDetails paymentDetails, decimal amountToCharge);
    }
}