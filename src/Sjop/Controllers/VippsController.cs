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
using Sjop.ViewModels;

namespace Sjop.Controllers
{
    [ApiController]
    [Route("api/payments/vipps")]
    public class VippsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly Config.VippsSettings _vippsSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public VippsController(ApplicationDbContext context, ILogger<VippsController> logger, Config.VippsSettings vippsSettings, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _vippsSettings = vippsSettings;
            _httpClientFactory = httpClientFactory;
        }


        [HttpPost]
        public async Task<ActionResult> PayWithVipps()
        {
            _logger.LogInformation($"*** Pay with Vipps, client id: {_vippsSettings.ClientId} ***");

            return StatusCode(418, "Lag litt kaffe likevel?");

        }

        private async Task<string> GetVippsAccessToken()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_vippsSettings.BaseUrl);

            HttpResponseMessage response = new HttpResponseMessage();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/accesstoken/get");

            request.Headers.Add("client_id", _vippsSettings.ClientId);
            request.Headers.Add("client_secret", _vippsSettings.ClientSecret);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _vippsSettings.SubscriptionKey);

            response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<VippsAccessTokenResponse>(responseContent);
            return json.AccessToken;

        }

        private class VippsAccessTokenResponse
        {
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
            [JsonPropertyName("expires_in")]
            public string ExpiresIn { get; set; }
            [JsonPropertyName("ext_expires_in")]
            public string ExtExpiresIn { get; set; }

            [JsonPropertyName("expires_on")]
            public string ExpiresOn { get; set; }

            [JsonPropertyName("not_before")]
            public string NotBefore { get; set; }

            [JsonPropertyName("resource")]
            public string Resource { get; set; }
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }


        }
    }
}