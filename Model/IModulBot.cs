using Telegram.Bot.Types;
using Telegram.Bot;

namespace FlowerSellerTgBot.Model
{
    public interface IModulBot
    {
        public void startMachineStateProduct(ITelegramBotClient bot, Message message);

        /// <summary>
        /// Обработка сообщений типа Message
        /// </summary>
        public Task handleMessage(ITelegramBotClient bot, Message message);
        /// <summary>
        /// Обработка сообщений типа CallbackQuerry
        /// </summary>
        public Task handleCallbackQuery(ITelegramBotClient bot, CallbackQuery query);
    }
}
