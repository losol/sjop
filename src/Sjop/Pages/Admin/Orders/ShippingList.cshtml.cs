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
    public class ShippingListModel : PageModel
    {
        private readonly Sjop.Data.ApplicationDbContext _context;

        public ShippingListModel(Sjop.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Order> Orders { get;set; }

        public async Task OnGetAsync()
        {
            Orders = await _context.Orders
                .Include(i => i.OrderLines)
                .Where(i => i.Status == OrderStatus.Paid)
                .ToListAsync();
        }
    }
}
