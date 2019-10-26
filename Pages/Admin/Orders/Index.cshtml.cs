using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Sjop.Data;
using Sjop.Models;

namespace Sjop.Pages.Admin.Orders
{
	public class IndexModel : PageModel
	{
		private readonly Sjop.Data.ApplicationDbContext _context;

		public IndexModel(Sjop.Data.ApplicationDbContext context)
		{
			_context = context;
		}

		public IList<Order> PaidOrders { get; set; }
        public IList<Order> OtherOrders { get; set; }

		public async Task OnGetAsync()
		{
			PaidOrders = await _context.Orders
                .Where(p =>  p.Status == OrderStatus.Paid)
				.Include(i => i.OrderLines)
				.OrderByDescending(i => i.Id)
				.ToListAsync();

            OtherOrders = await _context.Orders
                .Where(p => p.Status != OrderStatus.Paid)
				.Include(i => i.OrderLines)
				.OrderByDescending(i => i.Id)
				.ToListAsync();   
		}
	}
}
