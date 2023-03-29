using Microsoft.EntityFrameworkCore;
using TakeawayOrder.Models;

namespace TakeawayOrder.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductOrder> ProductOrder { get; set; }
        public DbSet<Promo> Promos { get; set; }
    }
}
