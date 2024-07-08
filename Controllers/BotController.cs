using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlowerSellerTgBot.Controllers
{
    [ApiController]
    [Route("/")]
    public class BotController : ControllerBase
    {
        private Telegram.Bot.TelegramBotClient bot = Bot.GetTelegramBotClient();

        [HttpPost]
        public async void Post([FromBody] Update update)
        {
            long chatId = update.Message.Chat.Id;
            await bot.SendTextMessageAsync(chatId, "Echo: " + update.Message.Text);
            Console.WriteLine(update.Message.Chat.FirstName + " " + update.Message.Chat.LastName + " " + update.Message.Text);
        }

        [HttpGet]
        public string Get()
        {
            return "Telegram bot was started";
        }

    }
}
