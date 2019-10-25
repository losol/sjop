using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shoppur.Data;
using Shoppur.Models;
using Shoppur.ViewModels;
using static Shoppur.Models.Order;

namespace Shoppur.Controllers
{
	[Authorize(Policy = "RequireAdministratorRole")]
	[Route("api/v1/orders")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger _logger;

		public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
		{
			_context = context;
			_logger = logger;
		}

		// GET: api/orders
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
		{
			return await _context.Orders.ToListAsync();
		}

		// GET: api/Orders/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Order>> GetOrder(int id)
		{
			var order = await _context.Orders.FindAsync(id);

			if (order == null)
			{
				return NotFound();
			}

			return order;
		}

		// PUT: api/orders/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for
		// more details see https://aka.ms/RazorPagesCRUD.
		[HttpPut("{id}")]
		public async Task<IActionResult> PutOrder(int id, Order order)
		{
			if (id != order.Id)
			{
				return BadRequest();
			}

			_context.Entry(order).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!OrderExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/orders
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for
		// more details see https://aka.ms/RazorPagesCRUD.
		[HttpPost]
		public async Task<ActionResult<Order>> PostOrder(Order order)
		{
			_context.Orders.Add(order);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetOrder", new { id = order.Id }, order);
		}

		// DELETE: api/Orders/5
		[HttpDelete("{id}")]
		public async Task<ActionResult<Order>> DeleteOrder(int id)
		{
			var order = await _context.Orders.FindAsync(id);
			if (order == null)
			{
				return NotFound();
			}

			_context.Orders.Remove(order);
			await _context.SaveChangesAsync();

			return order;
		}

		private bool OrderExists(int id)
		{
			return _context.Orders.Any(e => e.Id == id);
		}
	}
}
