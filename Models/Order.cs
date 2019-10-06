using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppur.Models
{
    public class Order
    {
        [Required]
        public int Id { get; set; }
        public CustomerInfo Customer { get; set; }
        public PaymentProviderType PaymentProvider { get; set; }
        public OrderStatus Status { get; set; }
        public ShippingStatus Shipping { get; set; }
        public string Log { get; set; }

        // Navigational properties
        public ICollection<OrderLine> OrderLines { get; set; }

        public class CustomerInfo
        {
            public string Email { get; set; }
            [Required]
            public string Name { get; set; }
            public string Address { get; set; }
            public string Address2 { get; set; }
            [Required]
            public string Zip { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            public string Country { get; set; } = "Norge";
            public string Phone { get; set; }
        }

        public enum OrderStatus
        {
            Draft,
            Verified,
            Invoiced,
            Paid,
            Cancelled,
            Refunded
        }

        public enum ShippingStatus
        {
            Draft,
            Planned,
            Shipped
        }

        public enum PaymentProviderType
        {
            Stripe,
            Vipps
        }

        public void AddLog(string text = null)
        {
            var logText = $"{DateTime.UtcNow.ToString("u")}: ";
            if (!string.IsNullOrWhiteSpace(text))
            {
                logText += $"{text}";
            }
            else
            {
                logText += $"{Status}";
            }
            Log += logText + "\n";
        }

    }
}
