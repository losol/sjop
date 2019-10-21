using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shoppur.Config;
using Shoppur.Data;
using Stripe;
using Stripe.Checkout;

namespace Shoppur.Controllers {

    [Route("api/v1/payment")]
    [ApiController]
    public class PaymentController : ControllerBase {
		private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
		private readonly StripeSettings _stripeSettings;

		public PaymentController(ApplicationDbContext context, ILogger<CartController> logger, StripeSettings stripeSettings)
        {
            _context = context;
            _logger = logger;
			_stripeSettings = stripeSettings;
        }

		[HttpGet]
        public async Task<ActionResult> Pay()
        {
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

			
		var options = new SessionCreateOptions {
			PaymentMethodTypes = new List<string> {
				"card",
			},
			LineItems = new List<SessionLineItemOptions> {
				new SessionLineItemOptions {
					Name = "T-shirt",
					Description = "Comfortable cotton t-shirt",
					Amount = 500,
					Currency = "nok",
					Quantity = 1,
				},
			},
			SuccessUrl = "https://example.com/success?session_id={CHECKOUT_SESSION_ID}",
			CancelUrl = "https://example.com/cancel",
		};

		var service = new SessionService();
		Session session = service.Create(options);
            
            return Ok(session);
        }
	}
}