
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sjop.Models;
using static Sjop.Models.Order;

namespace Sjop.ViewModels
{
    public class OrderRegistrationVM
    {
        public OrderRegistrationVM()
        {
            Customer = new CustomerInfo();
            PaymentProvider = new PaymentProviderType();
            Cart = new CartVM();
        }

		public CustomerInfo Customer { get; set;}
        public PaymentProviderType PaymentProvider { get; set;}
        public CartVM Cart { get; set; }
    }
}
