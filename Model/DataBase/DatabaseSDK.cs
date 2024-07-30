using FlowerSellerTgBot.Controllers;
using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.Model.DataBase
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

        T[] InitializeArray<T>(int length) where T : new()
        {
            T[] array = new T[length];
            for (int i = 0; i < length; ++i)
            {
                array[i] = new T();
            }

            return array;
        }

        public void DeleteSeller(string? chatId)
        {
            var removeSeller = from r in _db.sellerObjects where r.ChatId == chatId select r;

            var listNames = GetNamesProductFromSeller(chatId);
            //Удаляем все продукты связанные с продавцом
            for (int i = 0; i < listNames.Count; i++)
            {
                DeleteProduct(listNames[i]);
            }
            //Удаляем самого продавца
            _db.sellerObjects.Remove(removeSeller.FirstOrDefault());
            _db.SaveChanges();
        }

        public void DeleteCategory(string? name_category)
        {
            var removeCategory = from r in _db.categoryObjects where r.NameOf == name_category select r;

            var listNames = GetNamesProduct(name_category);
            //Удаляем все продукты связанные с категорией
            for (int i = 0; i < listNames.Count; i++)
            {
                DeleteProduct(listNames[i]);
            }
            //Удаляем саму категорию
            _db.categoryObjects.Remove(removeCategory.FirstOrDefault());
            _db.SaveChanges();
        }

        public void DeleteProduct(string? name_product)
        {
            if (name_product == null) { _logger.LogWarning("DeleteProduct: name_product==null");  return; }
            
            var removePhoto = from r in _db.photoObjects where r.PhotoId == GetIdNameProduct(name_product) select r;

            var removeVideo = from r in _db.videoObjects where r.VideoId == GetIdNameProduct(name_product) select r;

            var removeProduct = from r in _db.productObjects where r.ProductName == name_product select r;

            //Удаление всех фото привязанных к продукту
            while (removePhoto.Count() > 0)
            {
                _db.photoObjects.Remove(removePhoto.FirstOrDefault());
                _db.SaveChanges();
            }

            //Удаление всех видео привязанных к продукту
            while (removeVideo.Count() > 0)
            {
                _db.videoObjects.Remove(removeVideo.FirstOrDefault());
                _db.SaveChanges();
            }

            //Удаление самого продукта
            _db.productObjects.Remove(removeProduct.FirstOrDefault());
            _db.SaveChanges();
        }

        private void SendToDatabasePhoto(List<KeyValuePair<string, InputMediaType>>? MediaFiles, string? name_product)
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

            if (remove_options.Count() == 0)
            {
                List<string> keyList = new List<string>();

                for (int i = 0; i < MediaFiles.Count(); i++)
                {
                    if (MediaFiles[i].Value == InputMediaType.Photo)
                    {
                        keyList.Add(MediaFiles[i].Key);
                    }
                }

                for (int b = 0; b < keyList.Count(); b++)
                {
                    PhotoObject[] photo = InitializeArray<PhotoObject>(keyList.Count());
                    photo[b].FileId = keyList[b];
                    photo[b].PhotoId = GetIdNameProduct(name_product);

                    _db.photoObjects.Add(photo[b]);
                    _db.SaveChanges();
                }
            }
        }

        private void SendToDatabaseVideo(List<KeyValuePair<string, InputMediaType>>? MediaFiles, string? name_product)
        {
            if (name_product == null)
                return;

            if (MediaFiles == null)
                return;

            var remove_options = from r in _db.videoObjects where r.VideoId == GetIdNameProduct(name_product) select r;

            while (remove_options.Count() > 0)
            {
                _db.videoObjects.Remove(remove_options.FirstOrDefault());
                _db.SaveChanges();
            }

            if (remove_options.Count() == 0)
            {
                List<string> keyList = new List<string>();

                for (int i = 0; i < MediaFiles.Count(); i++)
                {
                    if (MediaFiles[i].Value == InputMediaType.Video)
                    {
                        keyList.Add(MediaFiles[i].Key);
                    }
                }

                for (int b = 0; b < keyList.Count(); b++)
                {
                    VideoObject[] video = InitializeArray<VideoObject>(keyList.Count());
                    video[b].FileId = keyList[b];
                    video[b].VideoId = GetIdNameProduct(name_product);

                    _db.videoObjects.Add(video[b]);
                    _db.SaveChanges();
                }
            }
        }

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
            if (flowerObject.CategoryName == null) { _logger.LogWarning("SendToDatabase: CategoryName==null"); return; }

            if (flowerObject.ChatId == null) { _logger.LogWarning("SendToDatabase: ChatId==null"); return; }

            if (flowerObject.MediaFiles == null) { 
                _logger.LogWarning("SendToDatabase: MediaFiles==null"); 
                return; 
            }

            if (!CategoryDoesExist(flowerObject.CategoryName)) { _logger.LogWarning("SendToDatabase: Category Not Exist!"); return; }

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

            SendToDatabaseVideo(flowerObject.MediaFiles, flowerObject.ProductName);

            _logger.LogInformation("объект загружен в бд");
        }

        public void CreateNewCategory(string? name_category)
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

        public void CreateNewSeller(string? chatId)
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

        public string GetSellerChatId(int id_seller)
        {
            var sellerOBJ = _db.sellerObjects;

            foreach (var seller in sellerOBJ)
            {
                if (seller != null)
                {
                    if (seller.SellerId == id_seller)
                    {
                        return seller.ChatId;
                    }
                }
            }

            return "";
        }

        public List<string> GetNamesProduct(string? name_category)
        {
            if (name_category == null)
                return new List<string>();

            var listNames = new List<string>();

            var categoryId = GetIdCategory(name_category);

            foreach (ProductObject products in _db.productObjects)
            {
                if (products.CategoryId == categoryId)
                    listNames.Add(products.ProductName);
            }

            return listNames;
        }

        public List<string> GetNamesProductFromSeller(string? chatId)
        {
            if (chatId == null)
                return new List<string>();

            var listNames = new List<string>();

            var sellerId = GetIdSeller(chatId);

            foreach (ProductObject products in _db.productObjects)
            {
                if (products.SellerId == sellerId)
                    listNames.Add(products.ProductName);
            }

            return listNames;
        }

        public FlowerObject GetFlowerObject(string? name_product)
        {
            List<KeyValuePair<string, InputMediaType>> mediaFiles = new List<KeyValuePair<string, InputMediaType>>();

            List<PhotoObject> photoObject = new List<PhotoObject>();

            List<VideoObject> videoObject = new List<VideoObject>();

            var categoryId = 0;

            var sellerId = 0;

            foreach (ProductObject products in _db.productObjects)
            {
                if (products.ProductName == name_product)
                {
                    categoryId = products.CategoryId;
                    sellerId = products.SellerId;
                }
            }

            var categoryName = GetNameCategory(categoryId);
            var chatId = GetSellerChatId(sellerId);

            //выгружаем из бд все медиафайлы и записываем в List
            foreach (PhotoObject photo in _db.photoObjects)
            {
                photoObject.Add(photo);
            }

            foreach (VideoObject video in _db.videoObjects)
            {
                videoObject.Add(video);
            }


            foreach (ProductObject products in _db.productObjects)
            {
                if (products.ProductName == name_product)
                {


                    if (photoObject != null)
                    {
                        for (int i = 0; i < photoObject.Count; i++)
                        {
                            if (photoObject[i].PhotoId == products.ProductId)
                            {
                                mediaFiles.Add(new KeyValuePair<string, InputMediaType>(photoObject[i].FileId, InputMediaType.Photo));
                            }
                        }
                    }

                    if (videoObject != null)
                    {
                        for (int i = 0; i < videoObject.Count; i++)
                        {
                            if (videoObject[i].VideoId == products.ProductId)
                            {
                                mediaFiles.Add(new KeyValuePair<string, InputMediaType>(videoObject[i].FileId, InputMediaType.Video));
                            }
                        }
                    }

                    return new FlowerObject()
                    {
                        ProductName = products.ProductName,
                        CategoryName = categoryName,
                        Description = products.Description,
                        Price = products.Price,
                        ChatId = chatId,
                        MediaFiles = mediaFiles

                    };
                }
            }




            return new FlowerObject();
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

        private int GetIdCategory(string name_category)
        {
            var categoryOBJ = _db.categoryObjects;

            foreach (CategoryObject category in categoryOBJ)
            {
                if (category.NameOf == name_category)
                    return category.CategoryId;
            }

            return 0;
        }

        private string GetNameCategory(int categoryId)
        {
            var categoryOBJ = _db.categoryObjects;

            foreach (CategoryObject category in categoryOBJ)
            {
                if (category.CategoryId == categoryId)
                    return category.NameOf != null ? category.NameOf : "";
            }

            return "";
        }

        private int GetIdNameProduct(string name_product)
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
    }
}
