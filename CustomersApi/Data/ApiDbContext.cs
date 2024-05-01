using CustomersApi.Models;

using Microsoft.EntityFrameworkCore;

namespace CustomersApi.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=CustomerApiDb;User=sa;Password=password");
        }
    }
}