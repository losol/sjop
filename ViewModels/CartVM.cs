using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shoppur.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shoppur.ViewModels
{
    public class CartVM
    {
        public CartVM()
        {
            CartItems = new List<CartItem>();
        }

        public string CartId { get; set; }
        public List<CartItem> CartItems { get; set; }
        
    }
}
