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
    [Route("api/v1/cart")]
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
        public async Task<CartVM> GetCart()
        {
            _logger.LogInformation("*** Cart request ***");

            CartVM cart =
                HttpContext.Session.Get<CartVM>("_Cart") ??
                SaveNewCartToSession();
            
            return cart;
        }

        // POST: api/cart/items
        [HttpPost]
        [Route("items")]
        public async Task<ActionResult<CartVM>> AddProductToCart([FromBody]AddItemToCartVM product)
        {
            _logger.LogInformation("*** Add item to cart requested ***");
            if (product == null)
            {
                return BadRequest();
            }

            CartVM cart =
                HttpContext.Session.Get<CartVM>("_Cart") ??
                new CartVM();
            
            if (cart.CartId == null) {
                cart.CartId = Guid.NewGuid().ToString();
            }

            var newItem = new CartItem() { 
                ProductId = product.ProductId, 
                Quantity = 1, 
                CartId = Guid.Parse(cart.CartId)
                };

            cart.CartItems.Add(newItem);
            HttpContext.Session.Set<CartVM>("_Cart", cart);

            return cart;
        }

        // POST: api/cart/register-order
        [HttpPost]
        [Route("register-order")]
        public async Task<ActionResult<CartVM>> CreateOrder([FromBody]OrderRegistrationVM orderRegistration)
        {
            _logger.LogInformation("*** Submit order from cart ***");
            
            throw new NotImplementedException();
        }


        private CartVM SaveNewCartToSession()
        {
            var cart = new CartVM();
            cart.CartId = Guid.NewGuid().ToString();
            HttpContext.Session.Set<CartVM>("_Cart", cart);
            return cart;
        }
    }
}
