using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppur.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal VatPercent { get; set; } = 25;
        public decimal TotalPrice => (Price + Price * (VatPercent * 0.01m));
    }
}
