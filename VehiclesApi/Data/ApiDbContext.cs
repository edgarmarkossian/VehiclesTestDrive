﻿using Microsoft.EntityFrameworkCore;

using VehiclesApi.Models;

namespace VehiclesApi.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=VehicleApiDb;User=sa;Password=password");
        }
    }
} 