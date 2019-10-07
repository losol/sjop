using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shoppur.Data;
using Shoppur.Models;
using Shoppur.Utilities;
using Shoppur.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shoppur.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        public CartController(ApplicationDbContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: api/cart
        [HttpGet]
        public async Task<ShoppingCartViewModel> GetCart()
        {
            _logger.LogInformation("*** Cart request ***");
            if (HttpContext.Session.GetString("_CartId") == null)
            {
                HttpContext.Session.SetString("_CartId", Guid.NewGuid().ToString());
            }

            ShoppingCartViewModel cart =
                HttpContext.Session.Get<ShoppingCartViewModel>("_CartItems") ??
                new ShoppingCartViewModel();
            return cart;
        }

        // POST: api/cart/items
        [HttpPost]
        [Route("items")]
        public async Task<ActionResult<ShoppingCartViewModel>> AddProductToCart([FromBody]AddProductToCartVM product)
        {
            _logger.LogInformation("*** Add to cart requested ***");
            if (product == null) {
                return BadRequest();
            }

            
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
