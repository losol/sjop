using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sjop.Config;
using Sjop.Data;
using Stripe;
using Stripe.Checkout;

namespace Sjop.Controllers
{
	[ApiController]
	[Route("webhooks/stripe")]
	public class StripeWebHookController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger _logger;
		private readonly StripeSettings _stripeSettings;

		public StripeWebHookController(ApplicationDbContext context, ILogger<StripeWebHookController> logger, StripeSettings stripeSettings)
		{
			_context = context;
			_logger = logger;
			_stripeSettings = stripeSettings;
		}

		[HttpPost]
		public async Task<ActionResult> Index()
		{
			_logger.LogDebug("*** Webhook received ***");
			var webhookSecret = _stripeSettings.WebhookSecret;
			StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

			try
			{
				var stripeEvent = EventUtility.ConstructEvent(json,
					HttpContext.Request.Headers["Stripe-Signature"], webhookSecret);

				// Handle the hooks
				switch (stripeEvent.Type)
				{
					case Events.CheckoutSessionCompleted:
						var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
						_logger.LogCritical("** YEAH MONEY COMING **: " + session.ToString());

						var order = await _context.Orders.Where(m => m.Id == Convert.ToInt32(session.ClientReferenceId)).FirstOrDefaultAsync();
						order.Status = Models.Order.OrderStatus.Paid;
						order.AddLog(session.ToJson());
						_context.Update(order);
						await _context.SaveChangesAsync();

						break;

					case Events.PaymentIntentSucceeded:
						var intent = (PaymentIntent)stripeEvent.Data.Object;
						_logger.LogInformation($"* Payment intent succeded for order {intent.Metadata["OrderId"]}");
						var orderId = Convert.ToInt32(intent.Metadata["OrderId"]);

						var payorder = await _context.Orders.Where(m => m.Id == orderId).FirstOrDefaultAsync();
						payorder.Status = Models.Order.OrderStatus.Paid;
						payorder.AddLog(intent.ToJson());
						_context.Update(payorder);
						await _context.SaveChangesAsync();

						break;
					default:
						// Handle other event types

						break;
				}

			}
			catch (StripeException e)
			{
				_logger.LogError("EXCEPTION: " + e.ToString());
				return BadRequest();
			}
			return Ok();

		}
	}
}