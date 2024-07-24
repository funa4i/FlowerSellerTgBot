using Telegram.Bot.Types;
using Telegram.Bot;

namespace FlowerSellerTgBot.MachineStates
{
    public abstract class MachineState
    {
        public long _chatId { get; }

        protected event Action<MachineState>? _stateDone;

        protected static event Action<MachineState>? _timerLifeTimeEnd;
        
        protected Timer _timer;

        protected MachineState(long chatid, long lifeTime=600_000)
        {
            _chatId = chatid; 
            _timer = new Timer(new TimerCallback(objTimerDie), this, lifeTime, lifeTime / 2 + 1);
        }

        public static void invokeDieEvent(MachineState state)
        {
            state.objTimerDie(state);
        }

        protected void invokeStateDone()
        {
            _stateDone?.Invoke(this);
        }

        private void objTimerDie(object? obj) 
        {
            _timerLifeTimeEnd?.Invoke(obj as MachineState ?? this);
        }
        public void addLifeTimeListener(Action<MachineState> listener)
        {
            _timerLifeTimeEnd += listener;

        }

        public void addActionStateDoneListener (Action<MachineState> listener)
        {
            _stateDone += listener;
        }

        public abstract Task MachineStateDo(ITelegramBotClient bot, Message message);

    }
}
