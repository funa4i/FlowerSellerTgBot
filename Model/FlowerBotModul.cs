using FlowerSellerTgBot.MachineStates;
using FlowerSellerTgBot.Model.DataBase;
using System.Collections;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FlowerSellerTgBot.Model
{
    public class FlowerBotModul : IModulBot
    {
        private readonly IDataBase _dataBase;

        private Dictionary<long, MachineState> _personInMachine = new();
        /// <summary>
        /// Является ли текущий пользователь продавцом
        /// </summary>
        public FlowerBotModul(IDataBase dataBase)
        {
            _dataBase = dataBase;
        }
        
        // Создание машинного сосотояния, для нового продукта
        private async void StartMachineStateProduct(ITelegramBotClient bot, Message message)
        {
            if (!_personInMachine.ContainsKey(message.Chat.Id))
            {
                _personInMachine.Add(message.Chat.Id, new MachineStateProduct(message.Chat.Id, _dataBase.GetCategories()));

                await _personInMachine[message.Chat.Id].MachineStateDo(bot, message);

                _personInMachine[message.Chat.Id].addLifeTimeListener(DeleteMashineState);
                _personInMachine[message.Chat.Id].addActionStateDoneListener(SaveMachineStateProduct);
            }
        }

        // Создание машинного состояния, для новой категории
        private async void StartMachineStateCategory(ITelegramBotClient bot, Message message)
        {
            if (!_personInMachine.ContainsKey(message.Chat.Id))
            {
                _personInMachine.Add(message.Chat.Id, new MachineStateCategory(message.Chat.Id));

                await _personInMachine[message.Chat.Id].MachineStateDo(bot, message);

                _personInMachine[(message.Chat.Id)].addLifeTimeListener(DeleteMashineState);
                _personInMachine[message.Chat.Id].addActionStateDoneListener(SaveMachineStateCategory);
            }
        }
        // Создание машинного состояния для редактирования (с соответствующего этапа)
        private async void StartRefactorMachineStateProduct(ITelegramBotClient bot, Message message, int id)
        {
            if (_personInMachine.ContainsKey(message.Chat.Id)) //Если уже есть машинное состояния для этого чата - выходим
                return;
            FlowerObject flower = _dataBase.GetFlowerObjectFromId(id);
            MachineStateProduct state = new MachineStateProduct(message.Chat.Id, _dataBase.GetCategories(), flower);
            
            _personInMachine.Add(message.Chat.Id, state);
            await state.DoRefactorState(bot, message);
            _personInMachine[(message.Chat.Id)].addLifeTimeListener(DeleteMashineState);
            _personInMachine[message.Chat.Id].addActionStateDoneListener(SaveMachineStateProductChanges);
        }
        public async Task handleMessage(ITelegramBotClient bot, Message message)
        {
            if (message.Type == MessageType.Text && message.Text.Equals("/start"))
            {
                if (_personInMachine.ContainsKey(message.Chat.Id))
                    DeleteMashineState(_personInMachine[message.Chat.Id]); //Пользователь нажал start -> удаляем предыдущее MachineState
                //Сделано это для того, чтобы в случае подвисания/ошибки бота можно было вернуться в начало (Проверено на личном опыте)
                await MakeStartReplyKeyboard(message, bot);
                return;
            }
            if (_personInMachine.ContainsKey(message.Chat.Id)) //Если уже есть текущее состояние - переходим на него
            {
                await _personInMachine[(message.Chat.Id)].MachineStateDo(bot, message);
                return;
            }
            if (message.Type == MessageType.Text && message.Text.Equals("/possibilities"))
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Я умею собирать понравившиеся товары в корзину," +
                    " а так же связывать вас с продавцом");
                return;
            }
            if (message.Type == MessageType.Text && message.Text.Equals("/about"))
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Я предоставляю каталоги цветков, с удобной компановкой\n\n" +
                    "Здесь вы можете найти саженцы на ваш вкус");
                return;
            }
            if (message.Type == MessageType.Text)
            {
                switch (message.Text)
                {
                    case "Добавить товар":
                        if (UserIsSeller(message.Chat.Id))
                            StartMachineStateProduct(bot, message);
                        break;
                    case "Добавить категорию":
                        if (UserIsSeller(message.Chat.Id))
                            StartMachineStateCategory(bot, message);
                        break;
                    case ("Каталог"):
                        await ShowCatalog(bot, message);
                        break;
                    case ("Корзина"):
                        await ShowUserCart(bot, message); //TODO Тимофей, тут надо сделать открытие самой корзины
                        break;
                    default:
                        await bot.SendTextMessageAsync(message.Chat.Id, $"Эхо: {message.Text}");
                        break;
                }
            }
        }
        // РЕКЕОМЕНДАЦИЯ? Вместо query.Message.Chat.Id, можно использовать query.From.Id
        public async Task handleCallbackQuery(ITelegramBotClient bot, CallbackQuery query)
        {
            if (query.Data == null || query.Message == null)
                return;
            
            if (query.Data.Equals("requestSeller")) // Выполняем действия на покупку из корзины
            {
                await bot.DeleteMessageAsync(chatId: query.From.Id, query.Message.MessageId);
                await CallSellers(bot, query);
                await ShowCatalog(bot, query.Message);
                return;
            }
            if (query.Data.Equals("returnToCategory")) // Возвращаемся к категориям
            {
                await bot.EditMessageTextAsync(query.From.Id, 
                    query.Message.MessageId,
                    "Каталог\nКакая категория вас интересует?", 
                    replyMarkup: CreateCategoryKeyBoard());
                return;
            }
            //ОБРАБОТКА КНОПКИ КАТЕГОРИИ
            if (_dataBase.GetCategories().Contains(query.Data)) //Если в запросе название категории - выводим продукты этой категории
            {
                List<InlineKeyboardButton[]> ink = new List<InlineKeyboardButton[]>();
                int[] products = _dataBase.GetIdProductsFromCategory(query.Data).ToArray();
                for (int i = 0; i < products.Length; i+=2)
                {
                    var flower = _dataBase.GetFlowerObjectFromId(products[i]);
                    List<InlineKeyboardButton> arr = new List<InlineKeyboardButton>
                    {
                        new InlineKeyboardButton
                        {
                            Text = flower.ProductName ?? string.Empty,
                            //Строкой ниже я передаю id товара и действие, которое будет сделано
                            CallbackData = flower.ProductId + "|" + "Show"
                        }
                    };
                    if (i + 1 < products.Length)
                    {
                        flower = _dataBase.GetFlowerObjectFromId(products[i+1]);
                        arr.Add(new InlineKeyboardButton
                        {
                            Text = flower.ProductName ?? string.Empty,
                            CallbackData = flower.ProductId + "|" + "Show"
                        });
                    }
                    ink.Add(arr.ToArray());
                } //Заморочки с массивами нужны для демонстрации в 2 колонны
                ink.Add(new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton
                    {
                        Text = "Вернуться к категориям",
                        CallbackData = "returnToCategory"
                    }
                });
                
                var inkm = new InlineKeyboardMarkup(ink);
                await bot.EditMessageTextAsync(chatId: query.Message.Chat.Id,
                    messageId: query.Message.MessageId,
                    text: $"Каталог\nТовары категории '{query.Data}'",
                    replyMarkup: inkm);
                return;
            }
            //ОБРАБОТКА КНОПКИ ТОВАРА
            (int id, string action) = ParseIdAction(query.Data);
            if ((id, action) != (-1, string.Empty)) //Если все нормально отпарсилось, то делаем
            {
                //Проверка, есть ли объект с таким ID в бд 
                if (_dataBase.GetFlowerObjectFromId(id).ProductId != -1) //Если такой товар есть - выполняем action
                {
                    //Здесь я вывел все действия в отдельные функции для чистоты
                    switch (action)
                    {
                        case "Show": //Показать товар (приходит от страницы с категорией, остальные - от самого товара)
                            await CallbackQuerryShow(id, query, bot);
                            break;
                        case "AddCart": //Добавить в корзину
                            _dataBase.SendToDatabaseCart(query.From.Id.ToString(), id);
                            await bot.SendTextMessageAsync(query.From.Id, "Товар добавлен в корзину");
                            await bot.DeleteMessageAsync(chatId: query.Message.Chat.Id, query.Message.MessageId);
                            await ShowCatalog(bot,query.Message);
                            break;
                        case "DelCart": //Убрать из корзины
                            _dataBase.DeleteCart(query.From.Id.ToString(), id);
                            await bot.SendTextMessageAsync(query.From.Id, "Товар удален из корзины");
                            await bot.DeleteMessageAsync(chatId: query.Message.Chat.Id, query.Message.MessageId);
                            await ShowUserCart(bot, query.Message);
                            break;
                        case "Refactor": //Редактировать товар
                            StartRefactorMachineStateProduct(bot, query.Message, id);
                            break;
                        case "PreDel":
                            List<InlineKeyboardButton> list = new List<InlineKeyboardButton>
                            {
                                new InlineKeyboardButton
                                {
                                    Text = "Да",
                                    CallbackData = id + "|" + "Delete"
                                },
                                new InlineKeyboardButton
                                {
                                    Text = "Нет",
                                    CallbackData = _dataBase.GetFlowerObjectFromId(id).CategoryName
                                }
                            };
                            var ikbm = new InlineKeyboardMarkup(list);
                            await bot.EditMessageTextAsync(chatId: query.Message.Chat.Id,
                                messageId: query.Message.MessageId,
                                "Вы уверены, что хотите удалить этот товар?",
                                replyMarkup: ikbm);
                            break;
                        case "Delete":
                            var keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton
                            {
                                Text = "Вернутся к товарам категории",
                                CallbackData = _dataBase.GetFlowerObjectFromId(id).CategoryName
                            });
                            _dataBase.DeleteProduct(id);
                            await bot.EditMessageTextAsync(chatId: query.Message.Chat.Id,
                                messageId: query.Message.MessageId,
                                "Объект удален",
                                replyMarkup: keyboard);
                            break;
                    }
                }
            }
            
        }

        private void DeleteMashineState(MachineState state)
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

        private void SaveMachineStateProduct(MachineState state)
        {
            if (state is MachineStateProduct s)
            {
                _dataBase.SendToDatabase(s.flowerObject);
            }
        }

        private void SaveMachineStateCategory(MachineState state)
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
        private bool UserIsSeller(ChatId chatId)
        {
            using (StreamReader reader = new StreamReader("Sellers.txt"))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Equals(chatId.ToString()))
                    {
                        reader.Close();
                        return true;
                    }
                }
                reader.Close();
            }
            return false;
        }
        /// <summary>
        /// Метод, создающий клавиатуру
        /// </summary>
        /// <param name="mes">Сообщение, отправляемое пользователю (Опционально. По умолчанию - начальное привествие)</param>
        private async Task MakeStartReplyKeyboard(Message message, ITelegramBotClient bot, string mes = "")
        {
            if (message.Type == MessageType.Text && message.Text.Equals("/start"))
            {
                List<KeyboardButton[]> rows = new List<KeyboardButton[]>(); //Колонны. Сделаны для красивого вывода  
                KeyboardButton[] kb = new []
                {
                    new KeyboardButton("Каталог"),
                    new KeyboardButton("Корзина")
                };
                rows.Add(kb);
                if (UserIsSeller(message.Chat.Id))
                {
                    KeyboardButton[] kb1 = 
                    {
                        new KeyboardButton("Добавить товар"),
                        new KeyboardButton("Добавить категорию")
                    };
                    rows.Add(kb1);
                }
                var rpk = new ReplyKeyboardMarkup(rows)
                {
                    ResizeKeyboard = true
                };
                rpk.ResizeKeyboard = true;
                await bot.SendTextMessageAsync(message.Chat.Id, string.IsNullOrEmpty(mes.Trim()) ? "Привет! Это бот-магазин цветов, как я могу вам помочь?" : mes, replyMarkup: rpk);
            }
        }
        /// <summary>
        /// Метод отправки каталога, с Inline кнопками катгорий
        /// </summary>
        private async Task ShowCatalog(ITelegramBotClient bot, Message message)
        {
            InlineKeyboardMarkup inkm = CreateCategoryKeyBoard();
            await bot.SendTextMessageAsync(message.Chat.Id, "Каталог\nКакая категория вас интересует?", replyMarkup: inkm);
        }
        /// <summary>
        /// Метод разделяет строку с Id и действием через "|" на кортеж
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Кортеж (int, string) с Id и действием.
        /// Если разделить не удалось - возвращет -1 и пустую строку</returns>
        private (int, string) ParseIdAction(string str)
        {
            string[] arr = str.Split("|");
            if (!int.TryParse(arr[0], out var id) || arr.Length < 2) //Если не могу отпарсить - выхожу
                return (-1, string.Empty);
            return (id, arr[1]);
        }
        /// <summary>
        /// Метод вывода конкретного товара по Inline запросу
        /// </summary>
        /// <param name="id">id товара</param>
        /// <param name="query">запрос</param>
        /// <param name="bot">бот</param>
        private async Task CallbackQuerryShow(int id, CallbackQuery query, ITelegramBotClient bot)
        {
            _dataBase.GetFlowerObjectFromId(id);
            FlowerObject flower = _dataBase.GetFlowerObjectFromId(id);
            List<InlineKeyboardButton[]> ink = new List<InlineKeyboardButton[]>();
            //Ниже каждую кнопку я оборачиваю в массив, чтобы они выводились по одной (в 2 не помещаются)
            var el1 = new []
            { new InlineKeyboardButton
                {
                    Text = "Добавить в корзину",
                    CallbackData = flower.ProductId + "|" + "AddCart"
                }
            };
            var el2 = new[]
            { new InlineKeyboardButton
                {
                    Text = "Убрать из корзины",
                    CallbackData = flower.ProductId + "|" + "DelCart"
                }
            };
            ink.Add(el1);
            ink.Add(el2);
            if (long.Parse(flower.ChatId) == query.From.Id)
            {
                var el3 = new[]
                { new InlineKeyboardButton
                    {
                        Text = "Редактировать товар",
                        CallbackData = flower.ProductId + "|" + "Refactor"
                    }
                };
                var el4 = new[]
                { new InlineKeyboardButton
                    {
                        Text = "Удалить товар",
                        CallbackData = flower.ProductId + "|" + "PreDel"
                    } 
                };
                ink.Add(el3);
                ink.Add(el4);
            }
            var el5 = new[]
            { new InlineKeyboardButton
                {
                    Text = "К товарам категории",
                    CallbackData = flower.CategoryName
                } 
            };
            ink.Add(el5);
            var inkm = new InlineKeyboardMarkup(ink);
            await flower.Send(bot, query.Message.Chat.Id);
            await bot.SendTextMessageAsync(query.Message.Chat.Id, "Что будем делать?", replyMarkup: inkm);
        }
    
    
        private async Task ShowUserCart(ITelegramBotClient bot, Message message)
        {
            var ids = _dataBase.GetIdProductsFromCart(message.Chat.Id.ToString());

            List<InlineKeyboardButton[]> ink = new List<InlineKeyboardButton[]>();
            for (int i = 0; i < ids.Count; i += 2)
            {
                var flower = _dataBase.GetFlowerObjectFromId(ids[i]);
                List<InlineKeyboardButton> arr = new List<InlineKeyboardButton>
                    {
                        new InlineKeyboardButton
                        {
                            Text = flower.ProductName ?? string.Empty,
                            //Строкой ниже я передаю id товара и действие, которое будет сделано
                            CallbackData = flower.ProductId + "|" + "Show"
                        }
                    };
                if (i + 1 < ids.Count)
                {
                    flower = _dataBase.GetFlowerObjectFromId(ids[i + 1]);
                    arr.Add(new InlineKeyboardButton
                    {
                        Text = flower.ProductName ?? string.Empty,
                        CallbackData = flower.ProductId + "|" + "Show"
                    });
                }
                ink.Add(arr.ToArray());
            }
            InlineKeyboardMarkup inkm = new InlineKeyboardMarkup(ink);
            await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                   text: $"Ваша корзина",
                   replyMarkup: inkm);
            await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                text: "Отправить заявку?",
                replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton
                {
                    Text = "Отправить заявку на товары",
                    CallbackData = "requestSeller"
                }
                ));
            return;
        }


        private async Task CallSellers(ITelegramBotClient bot, CallbackQuery query)
        {
            var ids = _dataBase.GetIdProductsFromCart(query.From.Id.ToString());
            if (ids.Count == 0)
            {
                await bot.SendTextMessageAsync(query.From.Id, "Корзина пуста");
                return;
            }
            await bot.SendTextMessageAsync(query.From.Id, "Заявка отправлена.\n" +
                "Продавец свяжется с вами как сможет\n" +
                "Удостовертесь что вам можно написать :)");
            Dictionary<string, List<string>> valuePairs = new Dictionary<string, List<string>>();
            for (int i = 0; i < ids.Count; i++)
            {
                var obj = _dataBase.GetFlowerObjectFromId(ids[i]);
                if (!string.IsNullOrEmpty(obj.ProductName)) 
                {
                    if (!valuePairs.ContainsKey(obj.ChatId))
                    {
                        valuePairs[obj.ChatId] = new List<string>();
                    }
                    valuePairs[obj.ChatId].Add("Категория: " + obj.CategoryName + "\n" + "Товар: " + obj.ProductName);
                }
            }

            foreach (var pair in valuePairs)
            {
                string message = $"Пользователь @{query.From.Username}, отправил заявку на данные товары\n";
                foreach (var item in pair.Value)
                {
                    message += "\n" + item  + "\n";
                }
                await bot.SendTextMessageAsync(long.Parse(pair.Key), message);
            }
            _dataBase.DeleteAllCart(query.From.Id.ToString());
        }

        private InlineKeyboardMarkup CreateCategoryKeyBoard()
        {
            List<InlineKeyboardButton[]> ink = new List<InlineKeyboardButton[]>();
            var categories = _dataBase.GetCategories();
            for (int i = 0; i < categories.Count; i += 2)
            {
                List<InlineKeyboardButton> arr = new List<InlineKeyboardButton>
                    {
                        new InlineKeyboardButton
                        {
                            Text = categories[i],
                            //Строкой ниже я передаю id товара и действие, которое будет сделано
                            CallbackData = categories[i]
                        }
                    };
                if (i + 1 < categories.Count)
                {
                    arr.Add(new InlineKeyboardButton
                    {
                        Text = categories[i + 1],
                        CallbackData = categories[i + 1]
                    });
                }
                ink.Add(arr.ToArray());
            }
            return new InlineKeyboardMarkup(ink);
        }
    }
}