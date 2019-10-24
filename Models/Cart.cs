using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Shoppur.Models.Order;

namespace Shoppur.Models
{
    public class Cart
    {
        public Guid CartId { get; set; }

        public CustomerInfo Customer {get;set;}

        public PaymentProviderType PaymentProvider {get;set;} = PaymentProviderType.StripeCheckout;
        

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public ICollection<CartItem> CartItems { get; set; }
    }
}
