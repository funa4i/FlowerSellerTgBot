using Telegram.Bot.Types;
using Telegram.Bot;

namespace FlowerSellerTgBot.Model
{
    public interface IModulBot
    {
        public void StartMachineState(ITelegramBotClient bot, Message message);

        public void DoCommand(ITelegramBotClient bot, Message message);
    }
}
