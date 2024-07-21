using FlowerSellerTgBot.Controllers;
using FlowerSellerTgBot.Model.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace FlowerSellerTgBot.DataBase
{
    public class DatabaseSDK : IDataBase
    {
        private readonly DataContext _db;
        private readonly ILogger _logger;

        public DatabaseSDK()
        {
            _db = new DataContext();

            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(string.Empty);
        }

        public void SetProductObject(ProductObject productObject)
        {
            _db.productObjects.Add(productObject);
            _db.SaveChanges();

            _logger.LogInformation("объект загружен в бд");
        }

        public void SetProductObjectWithCategory(ProductObject productObject, String name_category)
        {
            if (!CategoryDoesExist(name_category))
            {
                CategoryObject category = new CategoryObject() { NameOf = name_category };

                _db.categoryObjects.Add(category);
                _db.SaveChanges();
            }

            productObject.CategoryId = GetIdCategory(name_category);
            _db.productObjects.Add(productObject);
            _db.SaveChanges();
        }

        public void SetNewCategory(String name_category)
        {
            if (!CategoryDoesExist(name_category))
            {
                CategoryObject category = new CategoryObject() { NameOf = name_category };

                _db.categoryObjects.Add(category);
                _db.SaveChanges();
            }
            else
                _logger.LogInformation("Категория уже существует");
     
        }

        public List<string> GetCategories()
        {
            var categoryOBJ = _db.categoryObjects;

            List<string> categories = new List<string>();

            foreach (CategoryObject category in categoryOBJ)
            {
                if (category.NameOf != null)
                {
                    categories.Add(category.NameOf);
                }
            }

            return categories;
        }
        public int GetIdCategory(String name_category)
        {
            var categoryOBJ = _db.categoryObjects;

            foreach (CategoryObject category in categoryOBJ)
            {
                if (category.NameOf == name_category)
                    return category.CategoryId;
            }

            return 0;
        }
        //сомнительная фича, но пусть пока что будет
        public bool CategoryDoesExist(String name_category)
        {
            var categoryString = GetCategories();

            for (int i = 0; i < categoryString.Count; i++)
            {
                if (categoryString[i] == name_category)
                {
                    return true;
                }
            }

            return false;
        }

        public List<KeyValuePair<int, string>> GetCategoryListPair()
        {

            var list_return = new List<KeyValuePair<int, string>>();

            var categoryOBJ = _db.categoryObjects;

            foreach (CategoryObject category in categoryOBJ)
            {
                list_return.Add(new KeyValuePair<int, string>(category.CategoryId, category.NameOf));
            }

            return list_return;
        
        }

        public List<KeyValuePair<int, string>> GetProductListPairFromCategory(String name_category)
        {
            var list_return = new List<KeyValuePair<int, string>>();

            var productsOBJ = _db.productObjects;

            foreach (ProductObject product in productsOBJ)
            {
                if (product.CategoryId == GetIdCategory(name_category))
                {
                    list_return.Add(new KeyValuePair<int, string>(product.ProductId, product.ProductName));

                    return list_return;
                }
            }

            return new List<KeyValuePair<int, string>>();
        
        }

        public ProductObject GetProductObjectFromCategory(String name_category)
        {
            var productsOBJ = _db.productObjects;

            foreach (ProductObject product in productsOBJ)
            {
                if (product.CategoryId == GetIdCategory(name_category))
                {
                    return product;
                }
            }

            return new ProductObject();
        }

    }
}
