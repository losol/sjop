using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // POST: api/cart/additem
        [HttpPost]
        [Route("additem")]
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

            // TODO do this in a proper way
            if (cart.ShippingCost.TotalShippingCost == 0) {{
                cart.ShippingCost.ShippingCost = 39.2M;
                cart.ShippingCost.VatPercent = 25;
                cart.ShippingCost.TotalShippingCost = 49;
            }}

            // Save Cart to session
            HttpContext.Session.Set<CartVM>("_Cart", cart);

            return cart;
        }

        // GET: api/cart/item/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(Guid id)
        {
            var cartItem = await _context.CartItem.FindAsync(id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return cartItem;
        }

        // PUT: api/cart/item/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(Guid id, CartItem cartItem)
        {
            if (id != cartItem.CartItemId)
            {
                return BadRequest();
            }

            _context.Entry(cartItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/cart/item
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem(CartItem cartItem)
        {
            _context.CartItem.Add(cartItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCartItem", new { id = cartItem.CartItemId }, cartItem);
        }

        // DELETE: api/cart/item/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CartItem>> DeleteCartItem(Guid id)
        {
            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItem.Remove(cartItem);
            await _context.SaveChangesAsync();

            return cartItem;
        }

        private bool CartItemExists(Guid id)
        {
            return _context.CartItem.Any(e => e.CartItemId == id);
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
