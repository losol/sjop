using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sjop.Config;
using Sjop.Data;
using Sjop.Models;
using Sjop.Services.Vipps;
using Sjop.ViewModels;

namespace Sjop.Controllers
{
    [ApiController]
    [Route("api/payments/vipps")]
    public class VippsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly VippsSettings _vippsSettings;
        private readonly IVippsApiClient _vippsApiClient;

        public VippsController(ApplicationDbContext context, ILogger<VippsController> logger, VippsSettings vippsSettings, IVippsApiClient vippsApiClient)
        {
            _context = context;
            _logger = logger;
            _vippsSettings = vippsSettings;
            _vippsApiClient = vippsApiClient;
        }


        [HttpPost]
        public async Task<ActionResult> PayWithVipps()
        {
            _logger.LogInformation($"*** Pay with Vipps, client id: {_vippsSettings.ClientId} ***");

            Order order = new Order()
            {
                Customer = new Order.CustomerInfo
                {
                    Email = "skoddlosen@gmail.com",
                    Name = "Dis og tåke-mannen"
                },
                PaymentProvider = Order.PaymentProviderType.Vipps
            };

            order.OrderLines.Add(new OrderLine()
            {
                Id = order.Id,
                ProductName = "Losvik kommune kalender",
                Price = 299
            });


            var pay = _vippsApiClient.InitiatePayment(order, _vippsSettings);

            return StatusCode(418, "Lag litt kaffe likevel?");

        }

    }
}
