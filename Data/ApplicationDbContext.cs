using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SounDesign_Web_02.Models;

namespace SounDesign_Web_02.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Product> Inventory { get; set; }
        public DbSet<Staging> Staging { get; set; }
        public DbSet<Truck> Truck { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}