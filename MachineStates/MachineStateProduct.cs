using System.Reflection.Metadata.Ecma335;
using FlowerSellerTgBot.MachineStates.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using FlowerSellerTgBot.Model.DataBase;

namespace FlowerSellerTgBot.MachineStates
{
    public class MachineStateProduct : MachineState
    {
        public FlowerObject flowerObject { get; private set; }

        private List<KeyValuePair<string, InputMediaType>> _mediaFiles;
        
        private States _state;
        
        private List<string> _categories;
        
        private string _mes;

        public MachineStateProduct(long ChatId, List<string> categories) : base(ChatId)
        {
            flowerObject = new FlowerObject() { ChatId = ChatId.ToString() };
            _mediaFiles = new ();
            _state = States.None;
            _mes = string.Empty;
            _categories = categories; 
            for (int i = 1; i <= categories.Count; i++)
            {
                _mes += "\n" + $"{i}." + categories[i - 1];
            }
        }

        // TODO: Сделать выбор категории + изменения количества
        override public async Task MachineStateDo(ITelegramBotClient bot, Message message)
        {
            switch (_state)
            {
                case States.None:
                    await bot.SendTextMessageAsync(_chatId, "Введите номер категории" + _mes, replyMarkup: new ReplyKeyboardRemove());
                    _state = States.Category;
                    break;
                case States.Category:
                    int categoryId = -1;
                    if (message.Text == "Оставить прежнее" && !string.IsNullOrEmpty(flowerObject.CategoryName))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Введите название товара");
                        _state = States.Name;
                        break;
                    }
                    if (message.Type != MessageType.Text || !int.TryParse(message.Text, out categoryId))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Пожалуйста, введите номер категории");
                    }
                    if (categoryId > _categories.Count || categoryId < 1)
                    {
                        await bot.SendTextMessageAsync(_chatId, "Такой категории нет, введите номер категории из списка");
                    }
                    else
                    {
                        flowerObject.CategoryName = _categories[categoryId - 1];
                        await bot.SendTextMessageAsync(_chatId, "Введите название товара");
                        _state = States.Name;
                    }
                    break;



                case States.Name:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пожалуйста, введите название товара");
                        break;
                    }
                    if (!message.Text.Equals("Оставить прежнее") || string.IsNullOrEmpty(flowerObject.ProductName))
                    {
                        flowerObject.ProductName = message.Text;
                        
                    }
                    _state = States.Price;
                    await bot.SendTextMessageAsync(_chatId, "Какая цена будет у товара?");
                    break;
                case States.Price:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пожалуйста, введите цену товара");
                        break;
                    }
                    if (!message.Text.Equals("Оставить прежнее") || string.IsNullOrEmpty(flowerObject.Price))
                    {
                        flowerObject.Price = message.Text;
                    }
                    _state = States.Amount;
                    await bot.SendTextMessageAsync(_chatId, "Введите количество товара в наличии");
                    break;
                case States.Amount:
                    if (message.Type != MessageType.Text)
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пожалуйста, введите количество товара");
                        break;
                    }
                    if (!message.Text.Equals("Оставить прежнее"))
                    {
                        if (!string.IsNullOrEmpty(message.Text) && int.TryParse(message.Text, out int result))
                            flowerObject.Amount = result;
                        else
                        {
                            await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пожалуйста, введите количество товара числом");
                            break;
                        }
                    }
                    _state = States.Desription;
                    await bot.SendTextMessageAsync(_chatId, "Опишите товар");
                    break;
                case States.Desription:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пришлите текстовое описание товара");
                        break;
                    }
                    if (!message.Text.Equals("Оставить прежнее") || string.IsNullOrEmpty(flowerObject.Description))
                    {
                        flowerObject.Description = message.Text;
                    }
                    _state = States.Media;
                    await bot.SendTextMessageAsync(_chatId, "Пришлите 1-3 фото или видео");
                    break;
                case States.Media:
                    if ((_mediaFiles.Count == 3 || 
                        (message.Text?.Equals("Да") ?? false) || 
                        (message.Text?.Equals("Оставить прежнее") ?? false)) && 
                        flowerObject.MediaFiles?.Count != 0)
                    {
                        if (_mediaFiles.Count != 0)
                        {
                            flowerObject.MediaFiles = _mediaFiles;
                            _mediaFiles.Clear();
                        }
                        var rpk = new ReplyKeyboardMarkup(new KeyboardButton[]
                        {
                            new KeyboardButton("1"),
                            new KeyboardButton("2"),
                            new KeyboardButton("3"),
                            new KeyboardButton("4"),
                            new KeyboardButton("5"),
                            new KeyboardButton("6"),
                            new KeyboardButton("7")
                        });
                        rpk.ResizeKeyboard = true;

                        await bot.SendTextMessageAsync(_chatId, "Отлично, проверьте, все ли верно?");
                        await flowerObject.Send(bot, _chatId);
                        await bot.SendTextMessageAsync(_chatId,
                            "1. Выбор категории\n" +
                            "2. Изменить название\n" +
                            "3. Изменить цену\n" +
                            "4. Изменить количество\n" +
                            "5. Изменить описание\n" +
                            "6. Изменить фото/видео\n" +
                            "7. Сохранить товар"
                            , replyMarkup: rpk);
                        _state = States.RefactorState;
                        break;
                    }
                    if (message.Photo == null && message.Video == null)
                    {
                        await bot.SendTextMessageAsync(_chatId, "Пожалуйста, пришлите фото или видео");
                        break;
                    }

                    if (_mediaFiles.Count < 3)
                    {
                        
                        InputMediaType inputMediaType = 
                            message.Type == MessageType.Video ? InputMediaType.Video : InputMediaType.Photo;

                        string fileId =
                            inputMediaType == InputMediaType.Video ? message.Video.FileId : message.Photo.Last().FileId;
                            
                            _mediaFiles.Add(new(fileId, inputMediaType));
                            var rpk = new ReplyKeyboardMarkup(new KeyboardButton[] { "Да"});
                            rpk.ResizeKeyboard = true;
                            await bot.SendTextMessageAsync(_chatId, _mediaFiles.Count + " из 3, это все?", replyMarkup: rpk);
                        
                    }
                    break;
                case States.RefactorState:
                    var rpkRefactor = new ReplyKeyboardMarkup(new KeyboardButton("Оставить прежнее")) 
                    {
                        ResizeKeyboard = true,
                        IsPersistent = true
                    };
                    switch (message.Text)
                    {
                        case "1":
                            await bot.SendTextMessageAsync(_chatId, "Введите номер категории" + _mes, replyMarkup: rpkRefactor);
                            _state = States.Category;
                            break;
                        case "2":
                            await bot.SendTextMessageAsync(_chatId, "Введите название товара", replyMarkup: rpkRefactor);
                            _state = States.Name;
                            break;
                        case "3":
                            await bot.SendTextMessageAsync(_chatId, "Какая цена будет у товара?", replyMarkup: rpkRefactor);
                            _state = States.Price;
                            break;
                        case "4":
                            await bot.SendTextMessageAsync(_chatId, "Введите количество товара в наличии", replyMarkup: rpkRefactor);
                            _state = States.Amount;
                            break;
                        case "5":
                            await bot.SendTextMessageAsync(_chatId, "Опишите товар", replyMarkup: rpkRefactor);
                            _state = States.Desription;
                            break;
                        case "6":
                            await bot.SendTextMessageAsync(_chatId, "Пришлите 1-3 фото или видео", replyMarkup: rpkRefactor);
                            flowerObject.MediaFiles = null;
                            _state = States.Media;
                            break;
                        case "7":
                            await bot.SendTextMessageAsync(_chatId, "Сохраняем", replyMarkup: new ReplyKeyboardRemove());
                            _state = States.Done;
                            break;
                        case null:
                            await bot.SendTextMessageAsync(_chatId, "Такого варианта ответа нет");
                            break;
                    }
                    if (_state == States.Done)
                    {
                        goto case States.Done;
                    }
                    break;
                case States.Done:
                    invokeStateDone();
                    MachineState.invokeDieEvent(this);
                    break;
            }
        }


    }
}
