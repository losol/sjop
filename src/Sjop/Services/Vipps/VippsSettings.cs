namespace Sjop.Services.Vipps
{

    public class VippsSettings
    {
        public string ApiBaseUrl { get; set; }
        public string MerchantSerialNumber { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SubscriptionKey { get; set; }
        public string CallbackBaseUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string TransactionText { get; set; }
    }
}
