using Telegram.Bot;
namespace FlowerSellerTgBot
{
    public class Bot
    {
        public static TelegramBotClient GetTelegramBotClient()
        {
            string tocken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

            if (string.IsNullOrEmpty(tocken)) throw new ArgumentNullException(); 

            var _client = new TelegramBotClient(tocken);
            return _client;
        }
    }
}
