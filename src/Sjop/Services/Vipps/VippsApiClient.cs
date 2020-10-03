using System;
using System.ComponentModel.DataAnnotations;
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

namespace Sjop.Services.Vipps
{
    public class VippsApiClient : IVippsApiClient
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private VippsAccessTokenResponse _accessToken;

        private JsonSerializerOptions vippsJsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true
        };

        public VippsApiClient(ILogger<VippsApiClient> logger, IHttpClientFactory httpClientFactory)
        {

            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }


        private async Task<VippsAccessTokenResponse> GetAccessToken(VippsSettings vippsSettings)
        {
            _logger.LogInformation("*** Request access token from Vipps");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(vippsSettings.ApiBaseUrl);

            HttpResponseMessage response = new HttpResponseMessage();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/accesstoken/get");

            request.Headers.Add("client_id", vippsSettings.ClientId);
            request.Headers.Add("client_secret", vippsSettings.ClientSecret);
            request.Headers.Add("Ocp-Apim-Subscription-Key", vippsSettings.SubscriptionKey);

            response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            VippsAccessTokenResponse token = JsonSerializer.Deserialize<VippsAccessTokenResponse>(responseContent);

            _accessToken = token;

            return token;

        }

        public async Task<InitiatePaymentResponseOk> InitiatePayment(Order order, VippsSettings vippsSettings)
        {
            _logger.LogInformation($"*** Creating Vipps payemnt for order {order.Id}");
            var PaymentUrl = "/ecomm/v2/payments";

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(vippsSettings.ApiBaseUrl);
            HttpResponseMessage response = new HttpResponseMessage();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, PaymentUrl);

            var token = await GetAccessToken(vippsSettings);
            request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");
            request.Headers.Add("Merchant-Serial-Number", "210066");
            request.Headers.Add("Ocp-Apim-Subscription-Key", vippsSettings.SubscriptionKey);


            var init = new InitateVippsPayment
            {
                CustomerInfo = new CustomerInfo
                {
                    MobileNumber = "99433012"
                },
                MerchantInfo = new MerchantInfo()
                {
                    CallbackPrefix = vippsSettings.CallbackBaseUrl,
                    FallBack = vippsSettings.RedirectUrl,
                    MerchantSerialNumber = vippsSettings.MerchantSerialNumber,
                    AuthToken = order.PaymentProviderToken
                },
                Transaction = new Transaction()
                {
                    Amount = (int)(order.OrderTotalprice * 100),
                    OrderId = order.Id.ToString(),
                    TransactionText = "Test betaling"
                }

            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(init, vippsJsonOptions),
                Encoding.UTF8,
                "application/json");

            var payRequest = await client.SendAsync(request);

            if (!payRequest.IsSuccessStatusCode)
            {
                var error = await payRequest.Content.ReadAsStringAsync();
                throw new HttpRequestException(error);
            }
            var payResponse = await payRequest.Content.ReadAsStringAsync();


            var result = JsonSerializer.Deserialize<InitiatePaymentResponseOk>(payResponse, vippsJsonOptions);

            return result;
        }

        public async Task<CapturePaymentResponseOk> CapturePayment(Order order, VippsSettings vippsSettings)
        {
            _logger.LogInformation($"*** Vipps Api Client: Capturing payemnt for order {order.Id}");

            var CaptureUrl = $"/ecomm/v2/payments/{order.Id}/capture";

            // TODO Save this request id?
            var requestId = Guid.NewGuid().ToString();

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(vippsSettings.ApiBaseUrl);
            HttpResponseMessage response = new HttpResponseMessage();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, CaptureUrl);

            var token = await GetAccessToken(vippsSettings);
            request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");
            request.Headers.Add("Merchant-Serial-Number", "210066");
            request.Headers.Add("Ocp-Apim-Subscription-Key", vippsSettings.SubscriptionKey);
            request.Headers.Add("X-Request-Id", requestId);

            var capture = new CapturePaymentRequest
            {
                MerchantInfo = new CapturePaymentMerchantInfo
                {
                    MerchantSerialNumber = vippsSettings.MerchantSerialNumber
                },
                Transaction = new CapturePaymentTransaction
                {
                    Amount = (int)(order.OrderTotalprice * 100),
                    TransactionText = "asdf"
                }
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(capture, vippsJsonOptions),
                Encoding.UTF8,
                "application/json");

            var captureRequest = await client.SendAsync(request);
            var captureResponse = await captureRequest.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CapturePaymentResponseOk>(captureResponse, vippsJsonOptions);

            return result;
        }


        public class VippsAccessTokenResponse
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

        public class InitateVippsPayment
        {
            public CustomerInfo CustomerInfo { get; set; }
            public MerchantInfo MerchantInfo { get; set; }
            public Transaction Transaction { get; set; }

        }

        public class CapturePaymentRequest
        {
            public CapturePaymentMerchantInfo MerchantInfo { get; set; }
            public CapturePaymentTransaction Transaction { get; set; }

        }

        public class CapturePaymentResponseOk
        {
            public string PaymentInstrument { get; set; }
            public string OrderId { get; set; }
            public CapturePaymentResponseTransactionInfo TransactionInfo { get; set; }
            public CapturePaymentResponseTransactionSummary TransactionSummary { get; set; }
        }

        public class CapturePaymentResponseTransactionInfo
        {
            public int Amount { get; set; }
            public string Status { get; set; }
            public string TimeStamp { get; set; }
            public string TransactionId { get; set; }
            public string TransactionText { get; set; }
        }

        public class CapturePaymentResponseTransactionSummary
        {
            public int CapturedAmount { get; set; }
            public int RefundedAmount { get; set; }
            public int RemainingAmountToCapture { get; set; }
            public int RemainingAmountToRefund { get; set; }
            public string BankIdentificationNumber { get; set; }

        }

        public class CustomerInfo
        {
            [Required]
            public string MobileNumber { get; set; }
        }

        public class MerchantInfo
        {
            public string AuthToken { get; set; }
            [Required]
            public string CallbackPrefix { get; set; }
            public string ConsentRemovalPrefix { get; set; }
            [Required]
            public string FallBack { get; set; }
            public bool IsApp { get; set; } = false;
            [Required]
            public string MerchantSerialNumber { get; set; }
            public string PaymentType { get; set; }
            public string ShippingDetailsPrefix { get; set; }

        }

        public class CapturePaymentMerchantInfo
        {
            public string MerchantSerialNumber { get; set; }
        }

        public class CapturePaymentTransaction
        {
            public int Amount { get; set; }
            public string TransactionText { get; set; }
        }

        public class Transaction
        {
            [Required]
            public int Amount { get; set; }
            [Required]
            public string OrderId { get; set; }
            public string TimeStamp { get; set; }
            [Required]
            public string TransactionText { get; set; }
            public bool SkipLandingPage { get; set; }

        }
        public class InitiatePaymentResponseOk
        {
            public string OrderId { get; set; }
            public string Url { get; set; }
        }
    }
}