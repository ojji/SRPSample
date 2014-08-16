namespace EcommerceLib.Domain
{
    public class OrderItem
    {
        public string Identifier { get; set; }
        public int Quantity { get; set; }
        public decimal ItemCost { get; set; }
    }
}