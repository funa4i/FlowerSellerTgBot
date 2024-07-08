using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlowerSellerTgBot.Controllers
{
    [ApiController]
    [Route("/")]
    public class BotController : ControllerBase
    {
        private readonly ILogger<BotController> _logger;
        private readonly ITelegramBotClient _bot;
        public BotController(ILogger<BotController> logger, ITelegramBotClient bot) 
        {
            _logger = logger;
            _bot = bot;
        }
        

        [HttpPost]
        public async void Post([FromBody] Update update)
        {
            long chatId = update.Message.Chat.Id;
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message  && update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                await _bot.SendTextMessageAsync(chatId, "Echo: " + update.Message.Text);
                _logger.LogInformation(update.Message.Chat.FirstName + " " + update.Message.Chat.LastName + " " + update.Message.Text);
            }
        }

        [HttpGet]
        public string Get()
        {
            return "Telegram bot was started";
        }

    }
}
