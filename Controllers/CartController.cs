using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shoppur.Data;
using Shoppur.Models;
using Shoppur.Utilities;
using Shoppur.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shoppur.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/cart
        [HttpGet]
        public async Task<ShoppingCartViewModel> GetCart()
        {
            if (HttpContext.Session.GetString("_CartId") == null)
            {
                HttpContext.Session.SetString("_CartId", Guid.NewGuid().ToString());
            }

            ShoppingCartViewModel cart =
                HttpContext.Session.Get<ShoppingCartViewModel>("_CartItems") ??
                new ShoppingCartViewModel();
            return cart;
        }

        // POST: api/cart/add/3
        [HttpPost]
        public async Task<ActionResult<ShoppingCartViewModel>> AddProductToCart(int productId)
        {

            ShoppingCartViewModel cart = 
                HttpContext.Session.Get<ShoppingCartViewModel>("_CartItems") ?? 
                new ShoppingCartViewModel();

            var testitem = new CartItem() { ProductId = 1, Quantity = 7, ShoppingCartId = 1 };
            cart.CartItems.Add(testitem);
            if (cart.CartItems.Any())
            {
                HttpContext.Session.Set<ShoppingCartViewModel>("_CartItems", cart);
            }

            return cart;
        }
    }
}
