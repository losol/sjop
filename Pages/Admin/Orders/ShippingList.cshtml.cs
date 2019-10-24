using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shoppur.Data;
using Shoppur.Models;
using static Shoppur.Models.Order;

namespace Shoppur.Pages.Admin.Orders
{
    public class ShippingListModel : PageModel
    {
        private readonly Shoppur.Data.ApplicationDbContext _context;

        public ShippingListModel(Shoppur.Data.ApplicationDbContext context)
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
