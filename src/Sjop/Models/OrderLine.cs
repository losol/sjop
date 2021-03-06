﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sjop.Models
{
	public class OrderLine
	{
		[Required]
		public int Id { get; set; }
		[Required, ForeignKey("Order")]
		public int OrderId { get; set; }

		[ForeignKey("Product")]
		public int? ProductId { get; set; }
		public OrderLineType LineType { get; set; } = OrderLineType.Product;
		public int Quantity { get; set; } = 1;

		public string ProductName { get; set; }
		public string ProductDescription { get; set; }

		[DataType(DataType.Currency)]
		public decimal Price { get; set; }
		public decimal VatPercent { get; set; } = 25;
		public decimal TotalPrice => (Price + Price * (VatPercent * 0.01m));
		public decimal LineTotalPrice => (Price + Price * (VatPercent * 0.01m)) * Quantity;

		public string Comments { get; set; }

		// Navigational properties
		[InverseProperty("OrderLines")]
		public Order Order { get; set; }
		public Product Product { get; set; }

		public enum OrderLineType
		{
			Product,
			Shipping
		}

	}
}