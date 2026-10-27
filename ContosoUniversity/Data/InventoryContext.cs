using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BugBeading.Models;

namespace BugBeading.Data
{
    public class InventoryContext : DbContext
    {
        public InventoryContext(
            DbContextOptions<InventoryContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = default!;

        public DbSet<Customer> Customers { get; set; } = default!;

        public DbSet<Order> Orders { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .ToTable(nameof(Product));

        }
    }
}
