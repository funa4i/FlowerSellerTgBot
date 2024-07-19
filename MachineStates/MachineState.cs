using Telegram.Bot.Types;
using Telegram.Bot;

namespace FlowerSellerTgBot.MachineStates
{
    public abstract class MachineState
    {
        protected long Id { get; }

        

        protected MachineState(long id)
        {
            Id = id;
        }

        public abstract void MachineStateDo(ITelegramBotClient bot, Message message);





    }
}
