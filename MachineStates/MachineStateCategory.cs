using FlowerSellerTgBot.MachineStates.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlowerSellerTgBot.MachineStates
{
    public class MachineStateCategory : MachineState
    {
        //TODO: Заменить на объект Category после его доработки
        public string? Name { get; private set; }
        //
        private States state;

        public MachineStateCategory(long chatId) : base(chatId, 300_000) 
        {
             state = States.None;   
        }

        public override async Task MachineStateDo(ITelegramBotClient bot, Message message)
        {
            switch (state)
            {
                case States.None:
                    await bot.SendTextMessageAsync(_chatId, "Введите название события", replyMarkup: new ReplyKeyboardRemove());
                    state = States.Name;
                    break;
                case States.Name:
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Извините, я вас не понял. Пожалуйста, введите название события", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    Name = message.Text;
                    state = States.RefactorState;
                    await bot.SendTextMessageAsync(_chatId,
                        "Названием нового события будет:\n"
                        + Name + "\n" +
                        "Все верно?", 
                        replyMarkup: new ReplyKeyboardMarkup(
                            new KeyboardButton[] { "Да", "Нет" })
                            {
                                ResizeKeyboard = true
                            }) ;
                    break;
                case States.RefactorState:
                    if (string.IsNullOrEmpty(message.Text) || (!message.Text.Equals("Да") && !message.Text.Equals("Нет")))
                    {
                        await bot.SendTextMessageAsync(_chatId, "Такого варинта ответа нет.", replyMarkup: new ReplyKeyboardRemove());
                    }
                    else if (message.Text.Equals("Нет"))
                    {
                        state = States.None;
                        goto case States.None;
                    }
                    else if (message.Text.Equals("Да"))
                    {
                        state = States.Done;
                        goto case States.Done;
                    }
                    break;
                case States.Done:
                    await bot.SendTextMessageAsync(_chatId, "Сохранено", replyMarkup: new ReplyKeyboardRemove());
                    invokeStateDone();
                    MachineState.invokeDieEvent(this);
                    break;
            }
           
        }
    }
}
