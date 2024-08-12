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

        private Dictionary<long, MachineState> _personInMachine = new();

        public FlowerBotModul(IDataBase dataBase)
        {
            _dataBase = dataBase;
        }
        
        // Создание машинного сосотояния, для нового продукта
        public async void startMachineStateProduct(ITelegramBotClient bot, Message message)
        {
            if (!_personInMachine.ContainsKey(message.Chat.Id))
            {
                _personInMachine.Add(message.Chat.Id, new MachineStateProduct(message.Chat.Id, _dataBase.GetCategories()));

                await _personInMachine[message.Chat.Id].MachineStateDo(bot, message);

                _personInMachine[message.Chat.Id].addLifeTimeListener(deleteMashineState);
                _personInMachine[message.Chat.Id].addActionStateDoneListener(saveMachineStateProduct);
            }
        }

        // Создание машинного состояния, для новой категории
        public async void startMachineStateCategory(ITelegramBotClient bot, Message message)
        {
            if (!_personInMachine.ContainsKey(message.Chat.Id))
            {
                _personInMachine.Add(message.Chat.Id, new MachineStateCategory(message.Chat.Id));

                await _personInMachine[message.Chat.Id].MachineStateDo(bot, message);

                _personInMachine[(message.Chat.Id)].addLifeTimeListener(deleteMashineState);
                _personInMachine[message.Chat.Id].addActionStateDoneListener(saveMachineStateCategory);
            }
        }
        // Создание машинного состояния для редактирования (с соответствующего этапа)
        private async void StartRefactorMachineStateProduct(ITelegramBotClient bot, Message message)
        {
            if (_personInMachine.ContainsKey(message.Chat.Id)) //Если уже есть машинное состояния для этого чата - выходим
                return;
            //Ниже создается новый flowerObject. В проде цветок будет получаться из БД по названию из сообщения пользователя
            FlowerObject flower = new FlowerObject("Категория", message.Chat.Id.ToString(), new List<KeyValuePair<string, InputMediaType>>(), 
                "цветок", "Описание", "150", 20);
            flower.AddMediafile("AgACAgIAAxkBAAIINma6GEgx5hwWok267ETFgIN-LqCcAAJi4DEbD_3RSXqRmqu70YP0AQADAgADeAADNQQ", InputMediaType.Photo);
            MachineStateProduct state = new MachineStateProduct(message.Chat.Id, _dataBase.GetCategories(), flower);
            _personInMachine.Add(message.Chat.Id, state);
            await state.DoRefactorState(bot, message);
            //**отправление отредактированного продукта**
        }
        public async Task handleMessage(ITelegramBotClient bot, Message message)
        {
            if (_personInMachine.ContainsKey(message.Chat.Id))
            {
                await _personInMachine[(message.Chat.Id)].MachineStateDo(bot, message);
                return;
            }
            else if (message.Type == MessageType.Text) ;
            {
                switch (message.Text)
                {
                    case "/доб продукт":
                        startMachineStateProduct(bot, message);
                        break;
                    case "/доб категорию":
                        startMachineStateCategory(bot, message);
                        break;
                    case "/ред продукт":
                        StartRefactorMachineStateProduct(bot, message);
                        break;
                    default:
                        await bot.SendTextMessageAsync(message.Chat.Id, "Эхо: " + _dataBase.GetNamesProduct("Семена").Count);
                        break;
                }
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

        private void saveMachineStateProduct(MachineState state)
        {
            if (state is MachineStateProduct s)
            {
                _dataBase.SendToDatabase(s.flowerObject);
            }
        }

        private void saveMachineStateCategory(MachineState state)
        {
            if (state is MachineStateCategory s)
            {
                _dataBase.CreateNewCategory(s.Name);
            }
        }


        
    }
}
