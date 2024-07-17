using FlowerSellerTgBot.DataBase;

namespace FlowerSellerTgBot.Model
{
    public class FlowerBotModul : IModulBot
    {

        private readonly IDataBase _dataBase;

        public FlowerBotModul(IDataBase dataBase) 
        {
            _dataBase = dataBase;
            _dataBase.connectBase();
        }

    }
}
