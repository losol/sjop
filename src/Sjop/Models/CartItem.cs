using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sjop.Models
{
    public class CartItem
    {
        public Guid CartItemId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CartId { get; set; }
        [Required]
        public int ProductId { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal LineTotal => Product.TotalPrice * Quantity;

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public virtual Product Product { get; set; }
    }
}
