using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shoppur.Data;
using Shoppur.Models;

namespace Shoppur.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly Shoppur.Data.ApplicationDbContext _context;

        public IndexModel(Shoppur.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; }

        public async Task OnGetAsync()
        {
            Product = await _context.Product.ToListAsync();
        }
    }
}
