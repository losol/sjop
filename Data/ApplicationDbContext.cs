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
        public DbSet<Shoppur.Models.Product> Products { get; set; }
        public DbSet<Shoppur.Models.Order> Orders { get; set; }
        public DbSet<Shoppur.Models.OrderLine> OrderLines { get; set; }
        public DbSet<Shoppur.Models.CartItem> CartItems { get; set; }
    }
}
