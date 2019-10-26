using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sjop.Models
{
	public class Order
	{
		public Order()
		{
			Customer = new CustomerInfo();
			PaymentProvider = new PaymentProviderType();
			OrderLines = new List<OrderLine>();
		}
		[Required]
		public int Id { get; set; }
		public CustomerInfo Customer { get; set; }
		public PaymentProviderType PaymentProvider { get; set; } = PaymentProviderType.StripeElements;
		public string PaymentProviderSessionId { get; set; }
		public OrderStatus Status { get; set; } = OrderStatus.Draft;
		public ShippingStatus Shipping { get; set; } = ShippingStatus.Draft;
		public ShippingType ShippingMethod { get; set; } = ShippingType.Mail;

		public decimal OrderTotalprice => OrderLines.Sum(w => w.TotalPrice);

		public string Log { get; set; }

		// Navigational properties
		public ICollection<OrderLine> OrderLines { get; set; }

		[Owned]
		public class CustomerInfo
		{
			[DataType(DataType.EmailAddress)]
			public string Email { get; set; }
			[Required]
			public string Name { get; set; }

			[DataType(DataType.PhoneNumber)]
			public string Phone { get; set; }
			public StreetAddress ShippingAddress { get; set; }
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

		[Owned]
		public class StreetAddress
		{
			public string Address { get; set; }
			public string Address2 { get; set; }
			public string Zip { get; set; }
			public string City { get; set; }
			public string Country { get; set; }
		}

		public enum ShippingStatus
		{
			Draft,
			Planned,
			Shipped
		}

		public enum ShippingType
		{
			None,
			Mail,
			Digital
		}

		public enum PaymentProviderType
		{
			None,
			StripeCheckout,
			StripeElements,
			StripeBilling,
			Vipps
		}

		public void AddLog(string text = null)
		{
			var logText = $"[{DateTime.UtcNow.ToString("u")}]: ";
			if (!string.IsNullOrWhiteSpace(text))
			{
				logText += $"{text}";
			}
			else
			{
				logText += $"[Status: {Status}]";
			}
			Log += logText + "\n";
		}

	}
}
