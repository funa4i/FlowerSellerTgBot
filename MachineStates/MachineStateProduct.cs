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

        private States state;

        public MachineStateProduct(long ChatId) : base(ChatId)
        {

            state = States.None;
            media = new FileBase?[3];
        }

        // TODO: Сделать выбор категории + изменения количества
        override public async Task MachineStateDo(ITelegramBotClient bot, Message message)
        {
            switch (state)
            {
                case States.None:

                    await bot.SendTextMessageAsync(_chatId, "Введите название товара", replyMarkup: new ReplyKeyboardRemove());
                    state = States.Name;
                    break;

                case States.Name:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пожалуйста, введите название товара");
                        break;
                    }
                    if (!message.Text.Equals("Оставить прежнее") && string.IsNullOrEmpty(ProductName))
                    {
                        ProductName = message.Text;
                        
                    }
                    state = States.Price;
                    await bot.SendTextMessageAsync(_chatId, "Какая цена будет у товара?");
                    break;
                case States.Price:

                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пожалуйста, введите цену товара");
                        break;
                    }
                    if (!message.Text.Equals("Оставить прежнее") && string.IsNullOrEmpty(Price))
                    {
                        Price = message.Text;
                        
                    }
                    state = States.Desription;
                    await bot.SendTextMessageAsync(_chatId, "Опишите товар");

                    break;
                case States.Desription:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пришлите текстовое описание товара");
                        break;
                    }
                    if (!message.Text.Equals("Оставить прежнее") && string.IsNullOrEmpty(Description))
                    {
                        Description = message.Text;
                        
                    }
                    state = States.Media;
                    await bot.SendTextMessageAsync(_chatId, "Пришлите 1-3 фото или видео");
                    break;
                case States.Media:
                    if (((message.Text?.Equals("Да") ?? false) || (message.Text?.Equals("Оставить прежнее") ?? false))
                        && Array.IndexOf(media, null) != 0)
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
                        state = States.RefactorState;
                        break;
                    }
                    if (message.Photo == null && message.Video == null)
                    {
                        await bot.SendTextMessageAsync(_chatId, "Пожалуйста, пришлите фото или видео");
                        break;
                    }

                    if (Array.IndexOf(media, null) != -1)
                    {
                        //TODO: Заменить тип массива(посл добавления объекта)
                        media[Array.IndexOf(media, null)] = message.Photo?.Last() != null ? message.Photo?.Last() : message.Video;
                        var rpk = new ReplyKeyboardMarkup(new KeyboardButton[] { "Да", "Нет" });
                        rpk.ResizeKeyboard = true;
                        await bot.SendTextMessageAsync(_chatId, Array.IndexOf(media, null) + " из 3, это все?", replyMarkup: rpk);
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
                            await bot.SendTextMessageAsync(_chatId, "Введите название товара", replyMarkup: rpkRefactor);
                            state = States.Name;
                            break;
                        case "2":
                            await bot.SendTextMessageAsync(_chatId, "Какая цена будет у товара?", replyMarkup: rpkRefactor);
                            state = States.Price;
                            break;
                        case "3":
                            await bot.SendTextMessageAsync(_chatId, "Опишите товар", replyMarkup: rpkRefactor);
                            state = States.Desription;
                            break;
                        case "4":
                            await bot.SendTextMessageAsync(_chatId, "Пришлите 1-3 фото или видео", replyMarkup: rpkRefactor);
                            state = States.Media;
                            break;
                        case "5":
                            await bot.SendTextMessageAsync(_chatId, "Отлично, все сохранено", replyMarkup: rpkRefactor);
                            state = States.Done;
                            break;
                        case null:
                            await bot.SendTextMessageAsync(_chatId, "такого варианта ответа нет", replyMarkup: rpkRefactor);
                            break;
                    }
                    if (state == States.Done)
                    {
                        goto case States.Done;
                    }
                    break;
                case States.Done:
                    await bot.SendTextMessageAsync(_chatId, "Объект сохранен\n Надеюсь....");
                    invokeStateDone();
                    MachineState.invokeDieEvent(this);
                    break;

            }
        }


    }
}
