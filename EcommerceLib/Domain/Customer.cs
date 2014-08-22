namespace EcommerceLib.Domain
{
    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public MembershipLevel Level { get; set; }
    }
}