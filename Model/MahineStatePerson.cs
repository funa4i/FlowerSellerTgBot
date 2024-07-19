using System.Reflection.Metadata.Ecma335;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlowerSellerTgBot.Model
{
    public class MahineStatePerson
    {
        public long Id { get; }

        public string ProductName { get; private set; }

        public string Price { get; private set; }

        public string Description { get; private set; }

        public FileBase?[] media; 

      
        private MachineStates machineStates;

        public MahineStatePerson(long ChatId) {
            Id = ChatId;
            machineStates = MachineStates.None;
            media = new FileBase?[3];
            
        }
        
        public async void MachineStateDo(ITelegramBotClient bot, Message message) 
        {
            switch (machineStates)
            {
                case MachineStates.None:
                    await bot.SendTextMessageAsync(Id, "Введите название товара", replyMarkup: new ReplyKeyboardRemove());
                    machineStates = MachineStates.Name;
                    break;
                case MachineStates.Name:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(Id, "Извините, я вас не понял. Пожалуйста, введите название товара", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    ProductName = message.Text;
                    machineStates = MachineStates.Price;
                    await bot.SendTextMessageAsync(Id, "Какая цена будет у товара?", replyMarkup: new ReplyKeyboardRemove());
                    break;
                case MachineStates.Price:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(Id, "Извините, я вас не понял. Пожалуйста, введите цену товара", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    Price = message.Text;
                    machineStates = MachineStates.Desription;

                    await bot.SendTextMessageAsync(Id, "Опишите товар", replyMarkup: new ReplyKeyboardRemove());
                    break;
                case MachineStates.Desription:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(Id, "Извините, я вас не понял. Пришлите текстовое описание товара", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    Description = message.Text;
                    machineStates = MachineStates.Media;
                    await bot.SendTextMessageAsync(Id, "Пришлите 1-3 фото или видео", replyMarkup: new ReplyKeyboardRemove());
                    break;
                case MachineStates.Media:
                    
                    
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
                        machineStates = MachineStates.RefactorState;
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
                 case MachineStates.RefactorState:
                    switch (message.Text)
                        {
                        case "1":
                            await bot.SendTextMessageAsync(Id, "Введите название товара", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = MachineStates.Name;
                            break;
                        case "2":
                            await bot.SendTextMessageAsync(Id, "Какая цена будет у товара?", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = MachineStates.Price;
                            break;
                        case "3":
                            await bot.SendTextMessageAsync(Id, "Опишите товар", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = MachineStates.Desription;
                            break;
                        case "4":
                            await bot.SendTextMessageAsync(Id, "Пришлите 1-3 фото или видео", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = MachineStates.Media;
                            break;
                        case "5":
                            await bot.SendTextMessageAsync(Id, "Отлично, все сохранено", replyMarkup: new ReplyKeyboardRemove());
                            machineStates = MachineStates.Done;
                            break;
                        case null:
                            await bot.SendTextMessageAsync(Id, "такого варианта ответа нет", replyMarkup: new ReplyKeyboardRemove());
                            break;
                    }
                    if (machineStates == MachineStates.Done)
                    {
                        goto case MachineStates.Done;
                    }
                    break;
                case MachineStates.Done:
                    // TODO: Вызвать event для сохранения объекта
                    break;
                 
            }
        }

        
        private void ChooseName()
        {

        }


    }
}
