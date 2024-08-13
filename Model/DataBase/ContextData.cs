using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Microsoft.EntityFrameworkCore;

namespace FlowerSellerTgBot.Model.DataBase
{
    public class DataContext : DbContext
    {
        public DataContext() => Database.EnsureCreated();
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<ProductObject> productObjects { get; set; } = null!;
        public DbSet<CategoryObject> categoryObjects { get; set; } = null!;
        public DbSet<PhotoObject> photoObjects { get; set; } = null!;
        public DbSet<VideoObject> videoObjects { get; set; } = null!;
        public DbSet<SellerObject> sellerObjects { get; set; } = null!;
        public DbSet<CartObject> cartObjects { get; set; }
        public DbSet<ProductCartObject> cartproductObjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql("Server=95.163.221.233;Database=simbir_practice_db;Port=5432;User Id=simbir_practice_user;Password=simbirpass;");

        }
    }
}
