using System.Reflection.Metadata.Ecma335;
using FlowerSellerTgBot.MachineStates.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlowerSellerTgBot.MachineStates
{
    public class MachineStateProduct : MachineState
    {
        // TODO: Заменить на объект FloweObject после его доработки <<
        public string? ProductName { get; private set; }

        public string? Price { get; private set; }

        public string? Description { get; private set; }

        public FileBase?[] media;
        // Все поля выше >>
        
        private States machineStates;

        public MachineStateProduct(long ChatId) : base(ChatId)
        {

            machineStates = States.None;
            media = new FileBase?[3];
        }


        override public async void MachineStateDo(ITelegramBotClient bot, Message message) 
        {
            switch (machineStates)
            {
                case States.None:

                    await bot.SendTextMessageAsync(_chatId, "Введите название товара", replyMarkup: new ReplyKeyboardRemove());
                    machineStates = States.Name;
                    break;
                    
                  
                case States.Name:
                    
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пожалуйста, введите название товара", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    ProductName = message.Text;
                    machineStates = States.Price;
                    await bot.SendTextMessageAsync(_chatId, "Какая цена будет у товара?", replyMarkup: new ReplyKeyboardRemove());
                    break;
                case States.Price:
                    
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пожалуйста, введите цену товара", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    Price = message.Text;
                    machineStates = States.Desription;

                    await bot.SendTextMessageAsync(_chatId, "Опишите товар", replyMarkup: new ReplyKeyboardRemove());
                    
                    break;
                case States.Desription:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пришлите текстовое описание товара", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    Description = message.Text;
                    machineStates = States.Media;
                    await bot.SendTextMessageAsync(_chatId, "Пришлите 1-3 фото или видео", replyMarkup: new ReplyKeyboardRemove());
                    break;
                case States.Media:


                    if ((message.Text?.Equals("Да") ?? false) && Array.IndexOf(media, null) != 0)
                    {
                        var rpk = new ReplyKeyboardMarkup(new KeyboardButton[]
                        {
                            new KeyboardButton("1"),
                            new KeyboardButton("2"),
                            new KeyboardButton("3"),
                            new KeyboardButton("4"),
                            new KeyboardButton("5")
                        });
                        rpk.ResizeKeyboard = true;

                        await bot.SendTextMessageAsync(_chatId, "Отлично, проверьте, все ли верно?");
                        await bot.SendTextMessageAsync(_chatId, "<<Товар>>");
                        await bot.SendTextMessageAsync(_chatId,
                            "1. Изменить название\n" +
                            "2. Изменить цену\n" +
                            "3. Изменить описние\n" +
                            "4. Изменить фото/видео\n" +
                            "5. Сохранить товар"
                            , replyMarkup: rpk);
                        machineStates = States.RefactorState;
                        break;
                    }

                    if (message.Photo == null && message.Video == null)
                    {
                        await bot.SendTextMessageAsync(_chatId, "Пожалуйста, пришлите фото или видео", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }

                    if (Array.IndexOf(media, null) != -1)
                    {
                        media[Array.IndexOf(media, null)] = message.Photo?.Last() != null ? message.Photo?.Last() : message.Video;
                        var rpk = new ReplyKeyboardMarkup(new KeyboardButton[] { "Да", "Нет" });
                        rpk.ResizeKeyboard = true;
                        await bot.SendTextMessageAsync(_chatId, Array.IndexOf(media, null) + " из 3, это все?", replyMarkup: rpk);
                    }
                    break;
                case States.RefactorState:
                    switch (message.Text)
                    {
                        case "1":
                            await bot.SendTextMessageAsync(_chatId, "Введите название товара", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = States.Name;
                            break;
                        case "2":
                            await bot.SendTextMessageAsync(_chatId, "Какая цена будет у товара?", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = States.Price;
                            break;
                        case "3":
                            await bot.SendTextMessageAsync(_chatId, "Опишите товар", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = States.Desription;
                            break;
                        case "4":
                            await bot.SendTextMessageAsync(_chatId, "Пришлите 1-3 фото или видео", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = States.Media;
                            break;
                        case "5":
                            await bot.SendTextMessageAsync(_chatId, "Отлично, все сохранено", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = States.Done;
                            break;
                        case null:
                            await bot.SendTextMessageAsync(_chatId, "такого варианта ответа нет", replyMarkup: new ReplyKeyboardRemove());
                            break;
                    }
                    if (machineStates == States.Done)
                    {
                        goto case States.Done;
                    }
                    break;
                case States.Done:
                    await bot.SendTextMessageAsync(_chatId, "Объект сохранен\n Надеюсь....");
                    invokeStateDone();
                    // TODO: Вызвать event для сохранения объекта
                    break;

            }
        }


    }
}
