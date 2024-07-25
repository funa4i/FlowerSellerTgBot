using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Telegram.Bot.Types.Enums;
using static FlowerSellerTgBot.Model.DataBase.DatabaseSDK;

namespace FlowerSellerTgBot.Model.DataBase
{
    public interface IDataBase
    {
        // public List<ProductObject> GetAllPositions(String name_category);
        public List<string> GetCategories();
        public List<string> GetSellers();
        public List<KeyValuePair<int, string>> GetProductListPairFromCategory(string name_category);
        public List<KeyValuePair<int, string>> GetCategoryListPair();
        public ProductObject GetProductObjectFromCategory(string name_category);
        public int GetIdCategory(string name_category);
        public int GetIdNameProduct(string name_product);
        public void SendToDatabase(FlowerObject flowerObject); //Передача в бд FlowerObject
        public void SetProductObjectWithCategory(ProductObject productObject, string name_category);
    }
}
