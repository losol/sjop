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
					_logger.LogInformation($"* Pay with StripeCheckout");
					return await PayWithStripeCheckout(order);

				case PaymentProviderType.StripeElements:
					_logger.LogInformation($"* Pay with StripeElements");
					return await PayWithStripeElements(order, payOrder.PaymentToken);

				case PaymentProviderType.StripeBilling:
					_logger.LogInformation($"* Pay with StripeBilling");
					return await PayWithStripeCheckout(order);

				case PaymentProviderType.Vipps:
					_logger.LogInformation($"* Pay with Vipps ");
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
				_logger.LogInformation($"linjepris: {ol.TotalPrice}");
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

		private async Task<ActionResult> PayWithStripeElements(Shoppur.Models.Order order, string cardToken)
		{
			// Read Stripe API key from config
			StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

			var service = new PaymentIntentService();
			var paymentIntentCreateOptions = new PaymentIntentCreateOptions
			{
				Customer = StripeCustomer(order).Id,
				Amount = Convert.ToInt32(order.OrderTotalprice * 100),
				Currency = "nok",
				Description = "Bestilling fra Losvik kommune",
				ReceiptEmail = order.Customer.Email,
				StatementDescriptor = "Losvik kommune"
			};

			var intent = service.Create(paymentIntentCreateOptions);

			return Ok(intent);
		}

		private async Task<ActionResult> PayWithStripeBilling(Shoppur.Models.Order order)
		{
			// Read Stripe API key from config
			StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
			throw new NotImplementedException();
		}

		private async Task<ActionResult> PayWithVipps(Shoppur.Models.Order order)
		{
			throw new NotImplementedException();

			// return BadRequest();
		}

		private Customer StripeCustomer(Shoppur.Models.Order order)
		{
			var options = new CustomerCreateOptions
			{
				Name = order.Customer.Name,
				Email = order.Customer.Email,
				Phone = order.Customer.Phone,
				PreferredLocales = new List<string> { "nb", "en" }
			};

			var service = new CustomerService();
			var customer = service.Create(options);

			return customer;
		}

		private Customer StripeCustomer(Shoppur.Models.Order order, string cardToken)
		{
			var options = new CustomerCreateOptions
			{
				Name = order.Customer.Name,
				Email = order.Customer.Email,
				Phone = order.Customer.Phone,
				PreferredLocales = new List<string> { "nb", "en" },
				Source = cardToken
			};

			var service = new CustomerService();
			var customer = service.Create(options);

			return customer;
		}
	}
}