using System.Reflection.Metadata.Ecma335;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlowerSellerTgBot.MachineStates
{
    public class MachineStateProduct : MachineState
    {

        public string? ProductName { get; private set; }

        public string? Price { get; private set; }

        public string? Description { get; private set; }

        public FileBase?[] media;

        private MachineStatesEnumProduct machineStates;

        public MachineStateProduct(long ChatId) : base(ChatId)
        {
            
            machineStates = MachineStatesEnumProduct.None;
            media = new FileBase?[3];

        }

        override public async void MachineStateDo(ITelegramBotClient bot, Message message) 
        {
            switch (machineStates)
            {
                case MachineStatesEnumProduct.None:
                    await bot.SendTextMessageAsync(Id, "Введите название товара", replyMarkup: new ReplyKeyboardRemove());
                    machineStates = MachineStatesEnumProduct.Name;
                    break;
                case MachineStatesEnumProduct.Name:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(Id, "Извините, я вас не понял. Пожалуйста, введите название товара", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    ProductName = message.Text;
                    machineStates = MachineStatesEnumProduct.Price;
                    await bot.SendTextMessageAsync(Id, "Какая цена будет у товара?", replyMarkup: new ReplyKeyboardRemove());
                    break;
                case MachineStatesEnumProduct.Price:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(Id, "Извините, я вас не понял. Пожалуйста, введите цену товара", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    Price = message.Text;
                    machineStates = MachineStatesEnumProduct.Desription;

                    await bot.SendTextMessageAsync(Id, "Опишите товар", replyMarkup: new ReplyKeyboardRemove());
                    break;
                case MachineStatesEnumProduct.Desription:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(Id, "Извините, я вас не понял. Пришлите текстовое описание товара", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    Description = message.Text;
                    machineStates = MachineStatesEnumProduct.Media;
                    await bot.SendTextMessageAsync(Id, "Пришлите 1-3 фото или видео", replyMarkup: new ReplyKeyboardRemove());
                    break;
                case MachineStatesEnumProduct.Media:


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

                        await bot.SendTextMessageAsync(Id, "Отлично, проверьте, все ли верно?");
                        await bot.SendTextMessageAsync(Id, "<<Товар>>");
                        await bot.SendTextMessageAsync(Id,
                            "1. Изменить название\n" +
                            "2. Изменить цену\n" +
                            "3. Изменить описние\n" +
                            "4. Изменить фото/видео\n" +
                            "5. Сохранить товар"
                            , replyMarkup: rpk);
                        machineStates = MachineStatesEnumProduct.RefactorState;
                        break;
                    }

                    if (message.Photo == null && message.Video == null)
                    {
                        await bot.SendTextMessageAsync(Id, "Пожалуйста, пришлите фото или видео", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }

                    if (Array.IndexOf(media, null) != -1)
                    {
                        media[Array.IndexOf(media, null)] = message.Photo?.Last() != null ? message.Photo?.Last() : message.Video;
                        var rpk = new ReplyKeyboardMarkup(new KeyboardButton[] { "Да", "Нет" });
                        rpk.ResizeKeyboard = true;
                        await bot.SendTextMessageAsync(Id, Array.IndexOf(media, null) + " из 3, это все?", replyMarkup: rpk);
                    }
                    break;
                case MachineStatesEnumProduct.RefactorState:
                    switch (message.Text)
                    {
                        case "1":
                            await bot.SendTextMessageAsync(Id, "Введите название товара", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = MachineStatesEnumProduct.Name;
                            break;
                        case "2":
                            await bot.SendTextMessageAsync(Id, "Какая цена будет у товара?", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = MachineStatesEnumProduct.Price;
                            break;
                        case "3":
                            await bot.SendTextMessageAsync(Id, "Опишите товар", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = MachineStatesEnumProduct.Desription;
                            break;
                        case "4":
                            await bot.SendTextMessageAsync(Id, "Пришлите 1-3 фото или видео", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = MachineStatesEnumProduct.Media;
                            break;
                        case "5":
                            await bot.SendTextMessageAsync(Id, "Отлично, все сохранено", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = MachineStatesEnumProduct.Done;
                            break;
                        case null:
                            await bot.SendTextMessageAsync(Id, "такого варианта ответа нет", replyMarkup: new ReplyKeyboardRemove());
                            break;
                    }
                    if (machineStates == MachineStatesEnumProduct.Done)
                    {
                        goto case MachineStatesEnumProduct.Done;
                    }
                    break;
                case MachineStatesEnumProduct.Done:
                    // TODO: Вызвать event для сохранения объекта
                    break;

            }
        }


    }
}
