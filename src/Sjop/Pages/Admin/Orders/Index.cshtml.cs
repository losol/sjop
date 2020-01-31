using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Sjop.Data;
using Sjop.Models;
using static Sjop.Models.Order;

namespace Sjop.Pages.Admin.Orders
{
	public class IndexModel : PageModel
	{
		private readonly Sjop.Data.ApplicationDbContext _context;

		public IndexModel(Sjop.Data.ApplicationDbContext context)
		{
			_context = context;
		}

		public IEnumerable<Order> Orders { get; set; }
		public IEnumerable<Order> PaidOrders { get; set; }

		public async Task OnGetAsync()
		{
			Orders = await _context.Orders
				.Include(i => i.OrderLines)
				.OrderByDescending(i => i.Id)
				.ToListAsync();

			PaidOrders = await _context.Orders
				.Where(i => i.Status == OrderStatus.Paid)
				.Include(i => i.OrderLines)
				.OrderByDescending(i => i.Id)
				.ToListAsync();
		}
	}
}
