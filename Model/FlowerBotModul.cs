using FlowerSellerTgBot.MachineStates;
using FlowerSellerTgBot.Model.DataBase;
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
            
    
        }

        
        public async void StartMachineState(ITelegramBotClient bot, Message message)
        {
            if (!_personInMachine.ContainsKey(message.Chat.Id))
            {
                _personInMachine.Add(message.Chat.Id, new MachineStateProduct(message.Chat.Id));
                _personInMachine[message.Chat.Id].MachineStateDo(bot, message);
                
            }
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
                this.StartMachineState(bot, message);
                return;
            }
        }
    }
}
