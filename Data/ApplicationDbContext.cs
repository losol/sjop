using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sjop.Models;

namespace Sjop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Sjop.Models.Product> Products { get; set; }
        public DbSet<Sjop.Models.Order> Orders { get; set; }
        public DbSet<Sjop.Models.OrderLine> OrderLines { get; set; }
        public DbSet<Sjop.Models.CartItem> CartItems { get; set; }
    }
}
