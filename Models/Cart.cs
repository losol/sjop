using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Sjop.Models.Order;

namespace Sjop.Models
{
	public class Cart
	{
		public Guid CartId { get; set; }

		public CustomerInfo Customer { get; set; }

		public PaymentProviderType PaymentProvider { get; set; } = PaymentProviderType.StripeElements;


		[DataType(DataType.DateTime)]
		public DateTime DateCreated { get; set; } = DateTime.Now;

		public ICollection<CartItem> CartItems { get; set; }
	}
}
