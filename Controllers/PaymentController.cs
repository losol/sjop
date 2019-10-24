using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shoppur.Config;
using Shoppur.Data;
using Shoppur.ViewModels;
using Stripe;
using Stripe.Checkout;
using static Shoppur.Models.Order;

namespace Shoppur.Controllers
{

	[Route("api/v1/payment")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger _logger;
		private readonly StripeSettings _stripeSettings;
		private readonly SiteSettings _siteSettings;

		public PaymentController(ApplicationDbContext context, ILogger<CartController> logger, StripeSettings stripeSettings, SiteSettings siteSetttings)
		{
			_context = context;
			_logger = logger;
			_stripeSettings = stripeSettings;
			_siteSettings = siteSetttings;
		}

		[HttpPost]
		public async Task<ActionResult> Pay([FromBody]PayOrder payOrder)
		{
			_logger.LogInformation($"*** Paying order: {payOrder.OrderId}.");

			var order = _context.Orders
				.Where(p => p.Id == payOrder.OrderId)
				.Include(order => order.OrderLines)
				.FirstOrDefault();

			switch (order.PaymentProvider)
			{
				case PaymentProviderType.StripeCheckout:
					_logger.LogInformation($"* With StripeCheckout");
					return await PayWithStripeCheckout(order);
				default:
					return BadRequest();
			}
		}

		private async Task<ActionResult> PayWithStripeCheckout(Shoppur.Models.Order order)
		{
			// Read Stripe API key from config
			StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

			// Add orderlines to Checkout session
			var lines = new List<SessionLineItemOptions>();
			foreach (var ol in order.OrderLines)
			{
				var newline = new SessionLineItemOptions
				{
					Name = ol.ProductName,
					Description = ol.ProductDescription,
					Amount = Convert.ToInt64(ol.TotalPrice * 100),
					Currency = "nok",
					Quantity = ol.Quantity
				};
				lines.Add(newline);
			}
			var options = new SessionCreateOptions
			{
				ClientReferenceId = order.Id.ToString(),
				CustomerEmail = order.Customer.Email,
				Locale = "nb",
				PaymentMethodTypes = new List<string> {
				"card",
			},
				LineItems = lines,
				SuccessUrl = _siteSettings.BaseUrl + "/PaymentSuccess?session_id={CHECKOUT_SESSION_ID}",
				CancelUrl = _siteSettings.BaseUrl + "/PaymentFailed",
			};

			var service = new SessionService();
			Session session = service.Create(options);

			order.PaymentProviderSessionId = session.Id;
			_context.Update(order);
			await _context.SaveChangesAsync();

			return Ok(session);
		}


		private async Task<ActionResult> PayWithStripeBilling(Shoppur.Models.Order order)
		{
			throw new NotImplementedException();
			// Read Stripe API key from config
			// StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

			// return BadRequest();
		}
	}
}