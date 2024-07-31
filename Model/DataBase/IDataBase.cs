using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Telegram.Bot.Types.Enums;
using static FlowerSellerTgBot.Model.DataBase.DatabaseSDK;

namespace FlowerSellerTgBot.Model.DataBase
{
    public interface IDataBase
    {
        public List<string> GetCategories(); //Получение всех категорий
        public List<string> GetSellers(); //Получение всех продавцов
        public List<string> GetNamesProduct(string? name_category); //Получение всех названий продуктов по категории
        public List<string> GetNamesProductFromSeller(string? chatId); //Получение всех названий продуктов у продавца 
        public FlowerObject GetFlowerObject(string? name_product); //Получение объекта FlowerObject из бд
        public void SendToDatabase(FlowerObject flowerObject); //Передача в бд FlowerObject
        public void CreateNewCategory(string? name_category); //Создание новой категории в бд
        public void CreateNewSeller(string? chatId); //Создание нового продавца в бд
        public void DeleteCategory(string? name_category); //Удаление категории вместе с продуктами + медиафайлы
        public void DeleteSeller(string? chatId); //Удаление продавца вместе с продуктами + медиафайлы
        public void DeleteProduct(string? name_product); //Удаление продукта вместе с медиафайлами

    }
}
