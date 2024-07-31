﻿using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using FlowerSellerTgBot.Model;
using FlowerSellerTgBot.DataBase;

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
        }
        

        [HttpPost]
        public async void Post([FromBody] Update update)
        {
            long chatId = update.Message.Chat.Id;
            
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
            //    await _bot.SendTextMessageAsync(chatId, "Echo: " + update.Message.Text);
            //    _logger.LogInformation(update.Message.Chat.FirstName + " " + update.Message.Chat.LastName + " " + update.Message.Text);
                await _modulBot.handleMessage(_bot, update.Message);
            }
        }

        [HttpGet]
        public string Get()
        {
            return "Telegram bot was started";
        }

    }
}
