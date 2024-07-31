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
        /// <param name="bot"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task handleMessage(ITelegramBotClient bot, Message message);
    }
}
