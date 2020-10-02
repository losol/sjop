using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sjop.Config;
using Sjop.Data;
using Sjop.ViewModels;
using Stripe;
using Stripe.Checkout;
using static Sjop.Models.Order;

namespace Sjop.Controllers
{

    [Route("api/v1/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly Config.StripeSettings _stripeSettings;
        private readonly Site _site;

        public PaymentController(ApplicationDbContext context, ILogger<CartController> logger, Config.StripeSettings stripeSettings, Site siteSetttings)
        {
            _context = context;
            _logger = logger;
            _stripeSettings = stripeSettings;
            _site = siteSetttings;
        }

        [HttpPost]
        public async Task<ActionResult> Pay([FromBody] PayOrder payOrder)
        {
            _logger.LogInformation($"*** Paying order: {payOrder.OrderId}.");

            var order = await _context.Orders
                .Where(p => p.Id == payOrder.OrderId)
                .Include(order => order.OrderLines)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return BadRequest("No order found");
            }

            switch (order.PaymentProvider)
            {
                case PaymentProviderType.StripeCheckout:
                    _logger.LogInformation($"* Pay with StripeCheckout");
                    return await PayWithStripeCheckout(order);

                case PaymentProviderType.StripeElements:
                    _logger.LogInformation($"* Pay with StripeElements");
                    return await PayWithStripeElements(order);

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

        private async Task<ActionResult> PayWithStripeCheckout(Sjop.Models.Order order)
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
                SuccessUrl = _site.BaseUrl + "/PaymentSuccess?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = _site.BaseUrl + "/PaymentFailed",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            order.PaymentProviderSessionId = session.Id;
            _context.Update(order);
            await _context.SaveChangesAsync();

            return Ok(session);
        }

        private async Task<ActionResult> PayWithStripeElements(Sjop.Models.Order order)
        {
            // Read Stripe API key from config
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            var paymentIntentCreateOptions = new PaymentIntentCreateOptions
            {
                Customer = StripeCustomer(order).Id,
                Amount = Convert.ToInt32(order.OrderTotalprice * 100),
                Currency = "nok",
                PaymentMethodTypes = new List<string> { "card" },
                Description = "Bestilling fra Losvik kommune",
                ReceiptEmail = order.Customer.Email,
                StatementDescriptor = "Losvik kommune",
                Metadata = new Dictionary<String, String>()
                {
                    { "OrderId", order.Id.ToString()}
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(paymentIntentCreateOptions);

            return Ok(intent);
        }

        private Task<ActionResult> PayWithStripeBilling(Sjop.Models.Order order)
        {
            throw new NotImplementedException();
        }

        private Task<ActionResult> PayWithVipps(Sjop.Models.Order order)
        {
            throw new NotImplementedException();

            // return BadRequest();
        }

        private Customer StripeCustomer(Sjop.Models.Order order)
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
    }
}