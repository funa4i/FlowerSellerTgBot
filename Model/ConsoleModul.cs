using FlowerSellerTgBot.Controllers;
using FlowerSellerTgBot.Model.DataBase;
using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.Model
{
    public class ConsoleModul
    {
        private readonly IDataBase _dataBase;


        public ConsoleModul(IDataBase dataBase)
        {
            _dataBase = dataBase;
        }
    }
}
