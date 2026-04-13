using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GreenFieldLocalHub.Models;

namespace GreenFieldLocalHub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<GreenFieldLocalHub.Models.Basket> Basket { get; set; } = default!;
        public DbSet<GreenFieldLocalHub.Models.BasketProducts> BasketProducts { get; set; } = default!;
        public DbSet<GreenFieldLocalHub.Models.Farmers> Farmers { get; set; } = default!;
        public DbSet<GreenFieldLocalHub.Models.Favourites> Favourites { get; set; } = default!;
        public DbSet<GreenFieldLocalHub.Models.LoyaltyAccount> LoyaltyAccount { get; set; } = default!;
        public DbSet<GreenFieldLocalHub.Models.LoyaltyTransactions> LoyaltyTransactions { get; set; } = default!;
        public DbSet<GreenFieldLocalHub.Models.OrderProducts> OrderProducts { get; set; } = default!;
        public DbSet<GreenFieldLocalHub.Models.Orders> Orders { get; set; } = default!;
        public DbSet<GreenFieldLocalHub.Models.Products> Products { get; set; } = default!;
    }
}
