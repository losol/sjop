using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Sjop.Data;
using Sjop.Models;

namespace Sjop.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly Sjop.Data.ApplicationDbContext _context;

        public IndexModel(Sjop.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; }

        public async Task OnGetAsync()
        {
            Product = await _context.Products.ToListAsync();
        }
    }
}
