using FlowerSellerTgBot.Controllers;
using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.Model.DataBase
{
    public class DatabaseSDK : IDataBase
    {
        private readonly DataContext _db;
        private readonly ILogger _logger;

        public enum EnumDB
        {
            WITHOUTPHOTO = 1,
            WITHOUTVIDEO
        }

        public DatabaseSDK()
        {
            _db = new DataContext();

            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(string.Empty);
        }

        private void SendToDatabasePhoto(KeyValuePair<string, InputMediaType>[]? MediaFiles, string? name_product)
        {
            if (name_product == null)
                return;

            if (MediaFiles == null)
                return;

            var remove_options = from r in _db.photoObjects where r.PhotoId == GetIdNameProduct(name_product) select r;

            while (remove_options.Count() > 0)
            {
                _db.photoObjects.Remove(remove_options.FirstOrDefault());
                _db.SaveChanges();
            }

            PhotoObject[] photo = new PhotoObject[2];

            if (remove_options.Count() == 0)
            {
       

                for (int i = 0; i < 2; i++)
                {
                    

                    photo[i].FileId = MediaFiles[i].Key;
                    photo[i].PhotoId = GetIdNameProduct(name_product);

                    _db.photoObjects.Add(photo[i]);
                    _db.SaveChanges();
                }
            }
        }

     /* private void SendToDatabaseVideo(KeyValuePair<string, InputMediaType>[] MediaFiles, string? name_product)
        {
            if (name_product == null)
                return;

            var remove_options = from r in _db.videoObjects where r.VideoId == GetIdNameProduct(name_product) select r;

            while (remove_options.Count() > 0)
            {
                _db.videoObjects.Remove(remove_options.FirstOrDefault());
                _db.SaveChanges();
            }

            if (MediaFiles[MediaFiles.Count()].Value == InputMediaType.Photo
                && MediaFiles[MediaFiles.Count()].Key == "")
                return;

            if (remove_options.Count() == 0)
            {
                for (int i = 0; i < MediaFiles.Count(); i++)
                {
                    VideoObject video = new VideoObject() { FileId = MediaFiles[i].Key, VideoId = GetIdNameProduct(name_product) };
                    _db.videoObjects.Add(video);
                    _db.SaveChanges();
                }
            }
        }*/

        private void SendToDatabaseProduct(string? _productName, string? _description, int _categoryId, string? _price, int _sellerId)
        {
            ProductObject productObject = new ProductObject()
            {
                ProductName = _productName,
                Description = _description,
                CategoryId = _categoryId,
                Price = _price,
                SellerId = _sellerId
            };

            _db.productObjects.Add(productObject);
            _db.SaveChanges();
        }

        public void SendToDatabase(FlowerObject flowerObject)
        {
            if (flowerObject.CategoryName == null) { _logger.LogWarning("SendToDatabase: CategoryName=null"); return; }

            if (flowerObject.ChatId == null) { _logger.LogWarning("SendToDatabase: ChatId=null"); return; }

            if (flowerObject.MediaFiles == null) {
                _logger.LogWarning("SendToDatabase: MediaFiles=null");
                return;
            }

            CreateNewCategory(flowerObject.CategoryName);

            CreateNewSeller(flowerObject.ChatId);

            SendToDatabaseProduct
            (
                flowerObject.ProductName, 
                flowerObject.Description, 
                GetIdCategory(flowerObject.CategoryName), 
                flowerObject.Price, 
                GetIdSeller(flowerObject.ChatId)
            );

        
                SendToDatabasePhoto(flowerObject.MediaFiles, flowerObject.ProductName);

        //    if (e_args != EnumDB.WITHOUTVIDEO)
        //        SendToDatabaseVideo(flowerObject.MediaFiles, flowerObject.ProductName);

            _logger.LogInformation("объект загружен в бд");
        }

        public void SetProductObjectWithCategory(ProductObject productObject, string name_category)
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

        private void CreateNewCategory(string name_category)
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

        private void CreateNewSeller(string chatId)
        {
            if (!SellerDoesExist(chatId))
            {
                SellerObject seller = new SellerObject() { ChatId = chatId };

                _db.sellerObjects.Add(seller);
                _db.SaveChanges();
            }
            else
                _logger.LogInformation("Продавец уже существует");

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

        public List<string> GetSellers()
        {
            var sellerOBJ = _db.sellerObjects;

            List<string> sellers = new List<string>();

            foreach (SellerObject seller in sellerOBJ)
            {
                if (seller.ChatId != null)
                {
                    sellers.Add(seller.ChatId);
                }
            }

            return sellers;
        }

        private int GetIdSeller(string chatId)
        {
            var sellerOBJ = _db.sellerObjects;

            foreach (SellerObject seller in sellerOBJ)
            {
                if (seller.ChatId == chatId)
                {
                    return seller.SellerId;
                }
            }

            return 0;
        }

        public int GetIdCategory(string name_category)
        {
            var categoryOBJ = _db.categoryObjects;

            foreach (CategoryObject category in categoryOBJ)
            {
                if (category.NameOf == name_category)
                    return category.CategoryId;
            }

            return 0;
        }

        public int GetIdNameProduct(string name_product)
        {
            var productsOBJ = _db.productObjects;

            foreach (ProductObject product in productsOBJ)
            {
                if (product.ProductName == name_product)
                    return product.ProductId;
            }

            return 0;
        }

        private bool CategoryDoesExist(string name_category)
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

        private bool SellerDoesExist(string chatId)
        {
            var sellerString = GetSellers();

            for (int i = 0; i < sellerString.Count; i++)
            {
                if (sellerString[i] == chatId)
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

        public List<KeyValuePair<int, string>> GetProductListPairFromCategory(string name_category)
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

        public ProductObject GetProductObjectFromCategory(string name_category)
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
