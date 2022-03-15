using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trucks.Models;

namespace Trucks.Data
{
    public class TrucksDbContext : DbContext
    {
        public TrucksDbContext(DbContextOptions<TrucksDbContext> options)
            : base(options)
        {
        }

        public DbSet<Trucks.Models.Truck> Truck { get; set; }
    }
}
