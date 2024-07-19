using Telegram.Bot;
using FlowerSellerTgBot.DataBase;
using FlowerSellerTgBot.Model;

namespace FlowerSellerTgBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
          
            var builder = WebApplication.CreateBuilder(args);
            
            
            var token = builder.Configuration["Telegram:Token"] ?? throw new InvalidOperationException("Token is null");
            
            
            builder.Services.AddHttpClient<ITelegramBotClient, TelegramBotClient>((client, _) => new TelegramBotClient(token, client));

            builder.Services.AddSingleton<IDataBase, DataBaseOwn>();
            builder.Services.AddSingleton<IModulBot, FlowerBotModul>();
            
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
