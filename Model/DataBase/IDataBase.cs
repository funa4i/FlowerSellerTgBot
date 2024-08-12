using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Telegram.Bot.Types.Enums;
using static FlowerSellerTgBot.Model.DataBase.DatabaseSDK;

namespace FlowerSellerTgBot.Model.DataBase
{
    public interface IDataBase
    {
        public List<string> GetCategories(); //Получение всех категорий
        public List<string> GetSellers(); //Получение всех продавцов
        public List<int> GetIdProductsFromCategory(string? name_category); //Получение id всех продуктов из выбранной категории
        public List<int> GetIdProductsFromSeller(string? chatId); //Получение id всех продуктов у продавца 
        public FlowerObject GetFlowerObjectFromId(int productId); //Получение объекта FlowerObject по id продукта из бд 
        public void SendToDatabase(FlowerObject flowerObject); //Передача в бд FlowerObject
        public void CreateNewCategory(string? name_category); //Создание новой категории в бд
        public void CreateNewSeller(string? chatId); //Создание нового продавца в бд
        public void DeleteCategory(string? name_category); //Удаление категории вместе с продуктами + медиафайлы
        public void DeleteSeller(string? chatId); //Удаление продавца вместе с продуктами + медиафайлы
        public void DeleteProduct(int productId); //Удаление продукта вместе с медиафайлами
        public void ChangeProduct(FlowerObject flowerObject); //Изменение продукта в бд
    }
}
