﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shoppur.Models;
using static Shoppur.Models.Order;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shoppur.ViewModels
{
    public class CartVM
    {
        public CartVM()
        {
            CartItems = new List<CartItem>();
            ShippingCost = new ShippingDetails();
        }

        public string CartId { get; set; }
        public CustomerInfo Customer { get;set; }
        public PaymentProviderType PaymentProvider {get;set;} = PaymentProviderType.Stripe;
        public ShippingDetails ShippingCost {get;set;}
        public decimal CartTotal => CartItems.Sum(o=> o.Product.TotalPrice) + ShippingCost.TotalShippingCost;
        
        public List<CartItem> CartItems { get; set; }

        public class ShippingDetails {
            public decimal ShippingCost {get;set;}
            public decimal VatPercent {get;set;}
            public decimal TotalShippingCost {get;set;}
        }
        
    }
}
