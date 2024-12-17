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
        public DbSet<CartObject> cartObjects { get; set; } = null!;
        public DbSet<ProductCartObject> cartproductObjects { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Database=Bot;Port=5432;User Id=root;Password=password;");
        }
    }
}
