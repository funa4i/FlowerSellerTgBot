using Telegram.Bot;
using FlowerSellerTgBot.Model;
using Microsoft.EntityFrameworkCore;
using FlowerSellerTgBot.Model.DataBase;

namespace FlowerSellerTgBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
          
            var builder = WebApplication.CreateBuilder(args);
            
            
            var token = builder.Configuration["Telegram:Token"] ?? throw new InvalidOperationException("Token is null");


            builder.Services.AddHttpClient<ITelegramBotClient, TelegramBotClient>((client, _) => new TelegramBotClient(token, client));

            //реализация db
            builder.Services.AddSingleton<IDataBase, DatabaseSDK>();

            builder.Services.AddSingleton<IModulBot, FlowerBotModul>();

            builder.Services.AddDbContext<DataContext>(options =>
            {
                    options.UseNpgsql(builder.Configuration.GetConnectionString("Server=127.0.0.1;Database=Bot;Port=5432;User Id=root;Password=password;"));   
            });
            
            builder.Services.AddControllers();

            builder.Services.ConfigureTelegramBotMvc();

            builder.Services.ConfigureTelegramBot<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt => opt.SerializerOptions);




            var app = builder.Build();

            //using (var scope = app.Services.CreateScope())
            //{
            //    var serv = scope.ServiceProvider;
            //    var cont = serv.GetRequiredService<DataContext>();
            //    Console.WriteLine(cont.Database.CanConnect());
            //    cont.Database.Migrate();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            
            app.Run();

      
        }
    }
}
