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
using static Shoppur.Models.Order;
using static Shoppur.ViewModels.CartVM;

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

		// DELETE: api/cart
		[HttpDelete]
		public async Task<CartVM> DeleteCart()
		{
			_logger.LogInformation("*** Delete cart request ***");

			CartVM cart = new CartVM();

			// Save Cart to session
			HttpContext.Session.Set<CartVM>("_Cart", cart);

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

			if (cart.CartId == null)
			{
				cart.CartId = Guid.NewGuid().ToString();
			}

			var dbproduct = await _context.Products.Where(o => o.Id == product.ProductId).FirstOrDefaultAsync();

			var existingCartItem = cart.CartItems.Where(
				p => p.ProductId == product.ProductId)
				.FirstOrDefault() ?? null;

			if (existingCartItem != null)
			{
				var cartitem = cart.CartItems.Where(
					p => p.ProductId == product.ProductId)
					.FirstOrDefault()
					.Quantity += 1;
			}
			else
			{
				var newItem = new CartItem()
				{
					ProductId = product.ProductId,
					Quantity = 1,
					CartId = Guid.Parse(cart.CartId),
					Product = dbproduct
				};

				cart.CartItems.Add(newItem);
			}

			// Update shipping cost
			cart.ShippingCost = GetShippingCost(cart);

			// Save Cart to session
			HttpContext.Session.Set<CartVM>("_Cart", cart);

			return cart;
		}

		// POST: api/cart/removeitem
		[HttpPost]
		[Route("removeitem")]
		public async Task<ActionResult<CartVM>> RemoveProductFromCart([FromBody]AddItemToCartVM product)
		{
			_logger.LogInformation("*** Remove item from cart requested ***");
			if (product == null)
			{
				return BadRequest();
			}

			CartVM cart =
				HttpContext.Session.Get<CartVM>("_Cart") ??
				new CartVM();

			if (cart.CartId == null)
			{
				cart.CartId = Guid.NewGuid().ToString();
			}

			var dbproduct = await _context.Products.Where(o => o.Id == product.ProductId).FirstOrDefaultAsync();

			var existingCartItem = cart.CartItems.Where(
				p => p.ProductId == product.ProductId)
				.FirstOrDefault() ?? null;

			if (existingCartItem != null)
			{
				cart.CartItems.Where(
					p => p.ProductId == product.ProductId)
					.FirstOrDefault()
					.Quantity -= 1;
			}

			cart.CartItems.RemoveAll(p => p.Quantity == 0);

			// TODO do this in a proper way
			if (cart.ShippingCost.TotalShippingCost == 0)
			{
				{
					cart.ShippingCost.ShippingCost = 31.2M;
					cart.ShippingCost.VatPercent = 25;
					cart.ShippingCost.TotalShippingCost = 39;
				}
			}

			// Save Cart to session
			HttpContext.Session.Set<CartVM>("_Cart", cart);

			return cart;
		}

		// POST: api/cart/customer
		[HttpPost]
		[Route("customer")]
		public async Task<ActionResult<CartVM>> UpdateCustomer([FromBody]CartCustomerInfo customer)
		{
			_logger.LogInformation("*** Add customer to cart ***");
			if (customer == null)
			{
				return BadRequest();
			}

			CartVM cart =
				HttpContext.Session.Get<CartVM>("_Cart") ??
				new CartVM();

			if (cart.CartId == null)
			{
				cart.CartId = Guid.NewGuid().ToString();
			}

			var cartCustomer = new CartCustomerInfo()
			{
				Name = customer.Name,
				Email = customer.Email,
				Address = customer.Address,
				Zip = customer.Zip,
				City = customer.City,
				Country = customer.Country
			};

			cart.Customer = cartCustomer;

			// Save Cart to session
			HttpContext.Session.Set<CartVM>("_Cart", cart);

			return cart;
		}

		// POST: api/cart/shippingprovider
		[HttpPost]
		[Route("shippingprovider")]
		public async Task<ActionResult<CartVM>> UpdateShippingProvider([FromBody]ShippingProviderVM shippingProvider)
		{
			_logger.LogInformation($"*** Update paymentprovider in cart to {shippingProvider.ShippingMethod} ***");

			CartVM cart =
				HttpContext.Session.Get<CartVM>("_Cart") ??
				new CartVM();

			if (cart.CartId == null)
			{
				cart.CartId = Guid.NewGuid().ToString();
			}

			if (shippingProvider.ShippingMethod != null)
			{
				cart.ShippingMethod = (ShippingType)Enum.Parse(typeof(ShippingType), shippingProvider.ShippingMethod);
			}

			// Update shipping cost
			cart.ShippingCost = GetShippingCost(cart);

			// Save Cart to session
			HttpContext.Session.Set<CartVM>("_Cart", cart);

			return cart;
		}

		// POST: api/cart/customer
		[HttpPost]
		[Route("paymentprovider")]
		public async Task<ActionResult<CartVM>> UpdatePaymentProvider([FromBody]PaymentProviderVM paymentprovider)
		{
			_logger.LogInformation($"*** Update paymentprovider in cart to {paymentprovider.PaymentProviderType} ***");

			CartVM cart =
				HttpContext.Session.Get<CartVM>("_Cart") ??
				new CartVM();

			if (cart.CartId == null)
			{
				cart.CartId = Guid.NewGuid().ToString();
			}

			if (paymentprovider.PaymentProviderType != null)
			{
				cart.PaymentProvider = (PaymentProviderType)Enum.Parse(typeof(PaymentProviderType), paymentprovider.PaymentProviderType);
			}

			// Save Cart to session
			HttpContext.Session.Set<CartVM>("_Cart", cart);

			return cart;
		}

		// POST: api/cart/submit-order
		[HttpPost]
		[Route("submit-order")]
		public async Task<ActionResult<Order>> SubmitOrder()
		{
			_logger.LogInformation("*** Submitting cart as order ***");
			var cart = HttpContext.Session.Get<CartVM>("_Cart");

			var newCustomer = new CustomerInfo()
			{
				Name = cart.Customer.Name,
				Email = cart.Customer.Email,
				ShippingAddress = new StreetAddress()
				{
					Address = cart.Customer.Address,
					Zip = cart.Customer.Zip,
					City = cart.Customer.City,
					Country = cart.Customer.Country
				}
			};

			var newOrder = new Order()
			{
				Customer = newCustomer,
				PaymentProvider = cart.PaymentProvider,
				ShippingMethod = cart.ShippingMethod
			};
			await _context.Orders.AddAsync(newOrder);
			await _context.SaveChangesAsync();

			foreach (var cartitem in cart.CartItems)
			{
				var product = await _context.Products.Where(p => p.Id == cartitem.ProductId).FirstOrDefaultAsync();
				var line = new OrderLine()
				{
					ProductId = cartitem.ProductId,
					ProductName = cartitem.Product.Name,
					OrderId = newOrder.Id,
					Quantity = cartitem.Quantity,
					Price = product.Price,
					VatPercent = product.VatPercent
				};
				newOrder.OrderLines.Add(line);
			}

			// Add shipping
			var shippingline = new OrderLine()
			{
				ProductName = "Frakt",
				OrderId = newOrder.Id,
				Price = cart.ShippingCost.ShippingCost
			};
			newOrder.OrderLines.Add(shippingline);

			_context.Orders.Update(newOrder);
			await _context.SaveChangesAsync();

			_logger.LogInformation($"*** Added order with id: {newOrder.Id} ***");

			return newOrder;
		}

		// GET: api/cart/item/5
		[HttpGet("{id}")]
		public async Task<ActionResult<CartItem>> GetCartItem(Guid id)
		{
			var cartItem = await _context.CartItems.FindAsync(id);

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
			_context.CartItems.Add(cartItem);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetCartItem", new { id = cartItem.CartItemId }, cartItem);
		}

		// DELETE: api/cart/item/5
		[HttpDelete("{id}")]
		public async Task<ActionResult<CartItem>> DeleteCartItem(Guid id)
		{
			var cartItem = await _context.CartItems.FindAsync(id);
			if (cartItem == null)
			{
				return NotFound();
			}

			_context.CartItems.Remove(cartItem);
			await _context.SaveChangesAsync();

			return cartItem;
		}

		private bool CartItemExists(Guid id)
		{
			return _context.CartItems.Any(e => e.CartItemId == id);
		}

		private CartVM SaveNewCartToSession()
		{
			var cart = new CartVM();
			cart.CartId = Guid.NewGuid().ToString();
			HttpContext.Session.Set<CartVM>("_Cart", cart);
			return cart;
		}

		private ShippingDetails GetShippingCost(CartVM c)
		{
			var shippingDetails = new ShippingDetails();

			if (c.ShippingMethod == ShippingType.Mail)
			{
				shippingDetails.ShippingCost = 31.2M;
				shippingDetails.VatPercent = 25;
				shippingDetails.TotalShippingCost = 39;
			}
			else
			{
				shippingDetails.ShippingCost = 0;
				shippingDetails.VatPercent = 25;
				shippingDetails.TotalShippingCost = 0;
			}

			return shippingDetails;
		}
	}
}
