using FlowerSellerTgBot.MachineStates;
using FlowerSellerTgBot.Model.DataBase;
using System.Collections;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlowerSellerTgBot.Model
{
    public class FlowerBotModul : IModulBot
    {

        private readonly IDataBase _dataBase;

        private Dictionary<long, MachineState> _personInMachine = new();
        /// <summary>
        /// Является ли текущий пользователь продавцом
        /// </summary>
        private bool IsSeller = false;

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
            
            _personInMachine[(message.Chat.Id)].addLifeTimeListener(deleteMashineState);
            _personInMachine[message.Chat.Id].addActionStateDoneListener(SaveMachineStateProductChanges);
        }
        public async Task handleMessage(ITelegramBotClient bot, Message message)
        {
            if (message.Type == MessageType.Text && message.Text.Equals("/start"))
            {
                MakeStartReplyKeyboard(message, bot);
                return;
            }
            if (_personInMachine.ContainsKey(message.Chat.Id)) //Если уже есть текущее состояние - переходим на него
            {
                await _personInMachine[(message.Chat.Id)].MachineStateDo(bot, message);
                return;
            }
            else if (message.Type == MessageType.Text) ;
            {
                switch (message.Text)
                {
                    case "Добавить товар":
                        startMachineStateProduct(bot, message);
                        break;
                    case "Добавить категорию":
                        startMachineStateCategory(bot, message);
                        break;
                    case "Редактировать продукт":
                        StartRefactorMachineStateProduct(bot, message);
                        break;
                    default:
                        await bot.SendTextMessageAsync(message.Chat.Id, $"Эхо: {message.Text}");
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
        /// <summary>
        /// Сохранение изменений(!) отредактированного объекта
        /// </summary>
        private void SaveMachineStateProductChanges(MachineState state)
        {
            if (state is MachineStateProduct s)
            {
                _dataBase.ChangeProduct(s.flowerObject);
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
        /// <summary>
        /// Метод, проверяющий является ли пользователь продавцом
        /// </summary>
        /// <returns>если да - true, иначе - false</returns>
        private bool UserIsSeller(Message message)
        {
            using StreamReader reader = new StreamReader("Sellers.txt");
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Equals(message.Chat.Id.ToString()))
                    return true;
            }
            reader.Dispose();
            return false;
        }
        /// <summary>
        /// Метод, определяющий статус пользователя и создающий клавиатуру
        /// </summary>
        /// <param name="mes">Сообщение, отправляемое пользователю (Опционально. По умолчанию - начальное привествие)</param>
        private async Task MakeStartReplyKeyboard(Message message, ITelegramBotClient bot, string mes = "")
        {
            if (message.Type == MessageType.Text && message.Text.Equals("/start"))
            {
                IsSeller = UserIsSeller(message); //имитация авторизации
                List<KeyboardButton[]> rows = new List<KeyboardButton[]>(); //Колонны. Сделаны для красивого вывода  
                KeyboardButton[] kb = new []
                {
                    new KeyboardButton("Каталог"),
                    new KeyboardButton("Корзина")
                };
                rows.Add(kb);
                if (IsSeller)
                {
                    KeyboardButton[] kb1 = new []
                    {
                        new KeyboardButton("Добавить товар"),
                        new KeyboardButton("Добавить категорию")
                    };
                    rows.Add(kb1);
                }
                var rpk = new ReplyKeyboardMarkup(rows)
                {
                    ResizeKeyboard = true,
                    IsPersistent = true
                };
                await bot.SendTextMessageAsync(message.Chat.Id, string.IsNullOrEmpty(mes.Trim()) ? "Привет! Это бот-магазин цветов, как я могу вам помочь?" : mes, replyMarkup: rpk);
            }
        }
    }
}
