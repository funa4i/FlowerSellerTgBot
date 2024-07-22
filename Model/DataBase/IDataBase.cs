using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.Model.DataBase
{
    public interface IDataBase
    {
        // public List<ProductObject> GetAllPositions(String name_category);
        public List<string> GetCategories();
        public List<KeyValuePair<int, string>> GetProductListPairFromCategory(string name_category);
        public List<KeyValuePair<int, string>> GetCategoryListPair();
        public ProductObject GetProductObjectFromCategory(string name_category);
        public bool CategoryDoesExist(string name_category);
        public int GetIdCategory(string name_category);
        public int GetIdNameProduct(string name_product);
        public void SendToDatabaseObject(FlowerObject flowerObject);
        public void SetProductObjectWithCategory(ProductObject productObject, string name_category);
        public void SetNewCategory(string name_category);
    }
}
