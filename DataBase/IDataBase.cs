namespace FlowerSellerTgBot.DataBase
{
    public interface IDataBase
    {
       // public List<ProductObject> GetAllPositions(String name_category);
        public List<string> GetCategories();
        public List<KeyValuePair<int, string>> GetProductListPairFromCategory(String name_category);
        public List<KeyValuePair<int, string>> GetCategoryListPair();
        public ProductObject GetProductObjectFromCategory(String name_category);
        public bool CategoryDoesExist(String name_category);
        public int GetIdCategory(String name_category);
        public void SetProductObject(ProductObject productObject);
        public void SetProductObjectWithCategory(ProductObject productObject, String name_category);
        public void SetNewCategory(String name_category);
    }
}
