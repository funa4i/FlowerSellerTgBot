using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using FlowerSellerTgBot.Model;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.Controllers
{
    [ApiController]
    [Route("/")]
    public class BotController : ControllerBase
    {
        private readonly ILogger<BotController> _logger;
        private readonly ITelegramBotClient _bot;
        private readonly IModulBot _modulBot;
        public BotController(ILogger<BotController> logger, ITelegramBotClient bot, IModulBot modul) 
        {
            _logger = logger;
            _bot = bot;
            _modulBot = modul;
            bot.GetUpdates();
            bot.SetWebhook("https://9bc3-176-116-141-50.ngrok-free.app");
        }
        

        [HttpPost]
        public async void Post([FromBody] Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await _modulBot.handleMessage(_bot, update.Message);
                    break;
                case UpdateType.CallbackQuery:
                    await _modulBot.handleCallbackQuery(_bot, update.CallbackQuery);
                    break;
            }
        }

        [HttpGet]
        public string Get()
        {
            return "Telegram bot was started";
        }

    }
}
