﻿using FlowerSellerTgBot.MachineStates;
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


        public async Task handleMessage(ITelegramBotClient bot, Message message)
        {
            if (_personInMachine.ContainsKey(message.Chat.Id))
            {
                await _personInMachine[(message.Chat.Id)].MachineStateDo(bot, message);
                return;
            }
            else if (message.Type == MessageType.Text) ;
            {
                if (message.Text.Equals("/доб продукт"))
                {
                    this.startMachineStateProduct(bot, message);
                }
                else if(message.Text == "/доб категорию") {
                    this.startMachineStateCategory(bot, message);
                }
                else
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Эхо: " + _dataBase.GetIdProductsFromCategory("Семена").Count);
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
