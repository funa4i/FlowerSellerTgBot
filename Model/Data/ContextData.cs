using FlowerSellerTgBot.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FlowerSellerTgBot.Model.Data
{
    public class DataContext : DbContext
    {
        public DataContext() => Database.EnsureCreated();
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<ProductObject> productObjects { get; set; } = null!;
        public DbSet<CategoryObject> categoryObjects { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql("Server=95.163.221.233;Database=simbir_practice_db;Port=5432;User Id=simbir_practice_user;Password=simbirpass;");
            
        }

    }
}
