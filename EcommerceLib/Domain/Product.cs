using System;
using EcommerceLib.Domain.PricingStrategies;

namespace EcommerceLib.Domain
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
    }
}