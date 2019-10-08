using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shoppur.Models;

namespace Shoppur.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Shoppur.Models.Product> Product { get; set; }
        public DbSet<Shoppur.Models.Order> Order { get; set; }
        public DbSet<Shoppur.Models.OrderLine> OrderLine { get; set; }
        public DbSet<Shoppur.Models.CartItem> CartItem { get; set; }
    }
}
