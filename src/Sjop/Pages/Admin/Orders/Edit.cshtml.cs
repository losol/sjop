using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sjop.Data;
using Sjop.Models;

namespace Sjop.Pages.Admin.Orders
{
	public class EditModel : PageModel
	{
		private readonly Sjop.Data.ApplicationDbContext _context;

		public EditModel(Sjop.Data.ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public Order Order { get; set; }

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			Order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == id);

			if (Order == null)
			{
				return NotFound();
			}
			return Page();
		}

		// To protect from overposting attacks, please enable the specific properties you want to bind to, for
		// more details see https://aka.ms/RazorPagesCRUD.
		public async Task<IActionResult> OnPostAsync()
		{
			var dbOrder = await _context.Orders.Where(i => i.Id == Order.Id).FirstOrDefaultAsync();

            if (dbOrder.Status != Order.Status) {
                dbOrder.Status = Order.Status;
                dbOrder.AddLog();
            }
			dbOrder.Status = Order.Status;
			dbOrder.Log = Order.Log;
			_context.Update(dbOrder);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}

	}
}
