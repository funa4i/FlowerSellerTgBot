using FlowerSellerTgBot.DataBase;
using FlowerSellerTgBot.MachineStates;
using System.Collections;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.Model
{
    public class FlowerBotModul : IModulBot
    {

        private readonly IDataBase _dataBase;

        private Dictionary<long, MachineStateProduct> _personInMachine = new ();
        
        public FlowerBotModul(IDataBase dataBase) 
        {
            _dataBase = dataBase;
            _dataBase.connectBase();
        }
        


        // Создание машинного сосотояния, 
        public void StartMachineStateProduct(ITelegramBotClient bot, Message message)
        {
            if (!_personInMachine.ContainsKey(message.Chat.Id))
            {
                
                _personInMachine.Add(message.Chat.Id, new MachineStateProduct(message.Chat.Id));
                
                _personInMachine[message.Chat.Id].MachineStateDo(bot, message);
                
                _personInMachine[message.Chat.Id].addLifeTimeListener(deleteMashineState);
                _personInMachine[message.Chat.Id].addActionStateDoneListener(SaveMachineStateProduct);
            }
        }

        private void deleteMashineState(MachineState state)
        {
            lock (_personInMachine)
            {
                if (_personInMachine.ContainsKey(state._chatId))
                {
                    _personInMachine.Remove(state._chatId);
                }
            }
        }

        private void SaveMachineStateProduct(MachineState state)
        {
            // TODO: Вызвать сохранение в бд
            MachineState.invokeDieEvent(state);
        }


        public async void DoCommand(ITelegramBotClient bot, Message message)
        {
            if (_personInMachine.ContainsKey(message.Chat.Id))
            {
                _personInMachine[(message.Chat.Id)].MachineStateDo(bot, message);
                return;
            }
            else if (message.Type == MessageType.Text && message.Text == "/доб")
            {
                this.StartMachineStateProduct(bot, message);
                return;
            }
            else if (message.Type == MessageType.Text)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Эхо: " + message.Text);
            }
        }
    }
}
