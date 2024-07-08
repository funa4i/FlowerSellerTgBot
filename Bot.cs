using Telegram.Bot;
namespace FlowerSellerTgBot
{
    public class Bot
    {
        private static TelegramBotClient _client;
        
        public static TelegramBotClient GetTelegramBotClient()
        {
            if (_client != null)
            {
                return _client;
            }

            string tocken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

            if (string.IsNullOrEmpty(tocken)) throw new ArgumentNullException(); 

            _client = new TelegramBotClient(tocken);
            return _client;
        }
    }
}
