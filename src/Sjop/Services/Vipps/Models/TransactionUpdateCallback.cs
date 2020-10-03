using Microsoft.EntityFrameworkCore;

namespace Sjop.Services.Vipps.Models
{
    public class TransactionUpdateCallback
    {
        public string MerchantSerialNumber { get; set; }
        public string OrderId { get; set; }
        public CallbackTransactionInfo TransactionInfo { get; set; }

        public ErrorInfo ErrorInfo { get; set; }

        [Owned]
        public class CallbackTransactionInfo
        {
            public decimal Amount { get; set; }
            public string Status { get; set; }
            public string TimeStamp { get; set; }
            public string TransactionId { get; set; }

        }
    }
}