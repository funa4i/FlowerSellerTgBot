using Telegram.Bot.Types;
using Telegram.Bot;

namespace FlowerSellerTgBot.MachineStates
{
    public abstract class MachineState
    {
        protected long Id { get; }

        protected event Action<MachineState>_stateDone;
        
        protected Timer _timer;

        protected MachineState(long id)
        {
            Id = id;
            _timer = new Timer();
           
        }

        public void AddActionStateDoneListener (Action<MachineState> listener)
        {
            _stateDone += listener;
        }

        public abstract void MachineStateDo(ITelegramBotClient bot, Message message);

    }
}
