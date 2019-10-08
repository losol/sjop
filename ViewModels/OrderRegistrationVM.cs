
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shoppur.Models;
using static Shoppur.Models.Order;

namespace Shoppur.ViewModels
{
    public class OrderRegistrationVM
    {
        public OrderRegistrationVM()
        {
            // Initialize here
        }

        public string CartId { get; set; }

		public CustomerInfo Customer { get; set;}
        public CartVM Cart { get; set; }
    }
}
