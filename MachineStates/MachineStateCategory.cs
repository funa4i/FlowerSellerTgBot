using FlowerSellerTgBot.MachineStates.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlowerSellerTgBot.MachineStates
{
    public class MachineStateCategory : MachineState
    {
        //TODO: Заменить на объект Category после его доработки
        public string? Name { get; private set; }
        //
        private States state;

        public MachineStateCategory(long chatId) : base(chatId) 
        {
             state = States.None;   
        }

        public override void MachineStateDo(ITelegramBotClient bot, Message message)
        {
            throw new NotImplementedException();
        }
    }
}
