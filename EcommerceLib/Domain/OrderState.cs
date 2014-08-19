namespace EcommerceLib.Domain
{
    public enum OrderState
    {
        AwaitingProcess,
        InventoryReservationFailed,
        PaymentFailed,
        Processed
    }
}