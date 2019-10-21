using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shoppur.Config;
using Shoppur.Data;
using Stripe;

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
        public async Task<ActionResult> Test()
        {
            StripeConfiguration.ApiKey = _stripeSettings.ApiKey;

			var options = new ChargeCreateOptions {
				Amount = 1000,
				Currency = "usd",
				Source = "tok_visa",
				ReceiptEmail = "jenny.rosen@example.com",
			};
			var service = new ChargeService();
			Charge charge = service.Create(options);
            
            return Ok(charge);
        }
	}
}