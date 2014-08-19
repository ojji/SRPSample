namespace EcommerceLib.Domain
{
    public class Order
    {
        private readonly ShoppingCart _cart;
        private readonly PaymentDetails _paymentDetails;
        public OrderState State { get; set; }

        public Order(ShoppingCart cart, PaymentDetails paymentDetails)
        {
            _cart = cart;
            _paymentDetails = paymentDetails;
            State = OrderState.AwaitingProcess;
        }

        public ShoppingCart Cart
        {
            get { return _cart; }
        }

        public PaymentDetails PaymentDetails
        {
            get { return _paymentDetails; }
        }
    }
}