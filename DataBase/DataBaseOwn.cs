using FlowerSellerTgBot.Controllers;
using Telegram.Bot;

namespace FlowerSellerTgBot.DataBase
{
    public class DataBaseOwn : IDataBase
    {
        private readonly ILogger<BotController> _logger;
        protected string urlHost { get; set;}

        public DataBaseOwn(ILogger<BotController> logger)
        {
            _logger = logger;
        }

        
        public void connectBase()
        {
            _logger.LogInformation("Подключение");
        }
    }
}
