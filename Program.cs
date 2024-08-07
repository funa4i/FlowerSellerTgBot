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
                options.UseNpgsql(builder.Configuration.GetConnectionString("ServerConn"));
            });

            builder.Services.AddControllers();

            builder.Services.ConfigureTelegramBotMvc();
            
            builder.Services.ConfigureTelegramBot<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt => opt.SerializerOptions);

  


            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            
            app.Run();

      
        }
    }
}
