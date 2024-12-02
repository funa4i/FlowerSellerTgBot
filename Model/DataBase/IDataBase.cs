using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Telegram.Bot.Types.Enums;
using static FlowerSellerTgBot.Model.DataBase.DatabaseSDK;

namespace FlowerSellerTgBot.Model.DataBase
{
    public interface IDataBase
    {
        public List<string> GetCategories(); //Получение всех категорий
        public List<string> GetSellers(); //Получение всех продавцов
        public List<int> GetIdProductsFromCategory(string? name_category); //Получение всех id продуктов из выбранной категории
        public List<int> GetIdProductsFromSeller(string? chatId); //Получение всех id продуктов от продавца 
        public List<int> GetIdProductsFromCart(string? chatId); //Получение всех id продуктов из корзины по chatId
        public FlowerObject GetFlowerObjectFromId(int productId); //Получение объекта FlowerObject по id продукта из бд 
        public void SendToDatabase(FlowerObject flowerObject); //Передача в бд FlowerObject
        public void SendToDatabaseCart(string? chatId, int productId); //Добавление продукта в корзину(бд) по chatId, productId
        public void CreateNewCategory(string? name_category); //Создание новой категории в бд
        public void CreateNewSeller(string? chatId); //Создание нового продавца в бд
        public void DeleteCategory(string? name_category); //Удаление категории вместе с продуктами + медиафайлы
        public void DeleteSeller(string? chatId); //Удаление продавца вместе с продуктами + медиафайлы
        public void DeleteProduct(int productId); //Удаление продукта вместе с медиафайлами
        public void DeleteAllCart(string? chatId); //Полное удаление корзины по chatId
        public void DeleteCart(string? chatId, int productId); //Удаление одного товара из корзины 
        public void ChangeProduct(FlowerObject flowerObject); //Изменение продукта в бд
  
    }
}
