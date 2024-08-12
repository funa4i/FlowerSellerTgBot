using FlowerSellerTgBot.Controllers;
using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.Model.DataBase
{
    public class DatabaseSDK : IDataBase
    {

        private readonly DataContext _db;
        private readonly ILogger<BotController> _logger;

        private enum prop_e
        { 
            CHANGED_NAME, 
            CHANGED_DESC,
            CHANGED_AMOUNT,
            CHANGED_PRICE,
            CHANGED_CATEGORY,
            CHANGED_MEDIAFILES_VIDEO,
            CHANGED_MEDIAFILES_PHOTO,
            MAX_SIZE
        };

        public DatabaseSDK(ILogger<BotController> logger)
        {
            _db = new DataContext();
            _logger = logger;
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

        private bool[] ChangePropeties(FlowerObject firstObject)
        {
            FlowerObject secondObject = GetFlowerObjectFromId(firstObject.ProductId);

            bool[] changed = new bool[(int)(prop_e.MAX_SIZE)] { false, false, false, false, false, false, false };

            List<string> localPhoto = new List<string>();
            List<string> localVideo = new List<string>();

            List<string> databasePhoto = new List<string>();
            List<string> databaseVideo = new List<string>();

            for (int i = 0; i < firstObject.MediaFiles.Count(); i++)
            {
                if (firstObject.MediaFiles[i].Value == InputMediaType.Photo)
                {
                    localPhoto.Add(firstObject.MediaFiles[i].Key);
                }
                else if (firstObject.MediaFiles[i].Value == InputMediaType.Video)
                {
                    localVideo.Add(firstObject.MediaFiles[i].Key);
                }
            }

            for (int i = 0; i < secondObject.MediaFiles.Count(); i++)
            {
                if (secondObject.MediaFiles[i].Value == InputMediaType.Photo)
                {
                    databasePhoto.Add(secondObject.MediaFiles[i].Key);
                }
                else if (secondObject.MediaFiles[i].Value == InputMediaType.Video)
                {
                    databaseVideo.Add(secondObject.MediaFiles[i].Key);
                }
            }

            if (firstObject.ProductName != secondObject.ProductName)
            {
                changed[(int)(prop_e.CHANGED_NAME)] = true;
            }
            else if (firstObject.Description != secondObject.Description)
            {
                changed[(int)(prop_e.CHANGED_DESC)] = true;
            }
            else if (firstObject.CategoryName != secondObject.CategoryName)
            {
                changed[(int)(prop_e.CHANGED_CATEGORY)] = true;
            }
            else if (firstObject.Amount != secondObject.Amount)
            {
                changed[(int)(prop_e.CHANGED_AMOUNT)] = true;
            }
            else if (firstObject.Price != secondObject.Price)
            {
                changed[(int)(prop_e.CHANGED_PRICE)] = true;
            }
            else if (localPhoto != databasePhoto)
            {
                changed[(int)(prop_e.CHANGED_MEDIAFILES_PHOTO)] = true;
            }
            else if (localVideo != databaseVideo)
            {
                changed[(int)(prop_e.CHANGED_MEDIAFILES_VIDEO)] = true;
            }

            return changed;
        }

        private bool ChangeRequired(bool[] changed)
        {
            for (int i = 0; i < (int)(prop_e.MAX_SIZE); i++)
            {
                if (changed[i])
                    return true;
            }

            return false;
        }

        public void ChangeProduct(FlowerObject flowerObject)
        {
            var productObject = _db.productObjects.Where(c => c.ProductId == flowerObject.ProductId).FirstOrDefault();

            var photoObject = _db.photoObjects;

            if (productObject == null)
                return;

            var changed = ChangePropeties(flowerObject);

            if (ChangeRequired(changed))
            {
                if (changed[(int)(prop_e.CHANGED_NAME)])
                {
                    productObject.ProductName = flowerObject.ProductName;
                }

                if (changed[(int)(prop_e.CHANGED_DESC)])
                {
                    productObject.Description = flowerObject.Description;
                }

                if (changed[(int)(prop_e.CHANGED_AMOUNT)])
                {
                    productObject.Amount = flowerObject.Amount;
                }

                if (changed[(int)(prop_e.CHANGED_PRICE)])
                {
                    productObject.Price = flowerObject.Price;
                }

                if (changed[(int)(prop_e.CHANGED_CATEGORY)])
                {
                    productObject.CategoryId = GetIdCategory(flowerObject.CategoryName);
                }

                if (changed[(int)(prop_e.CHANGED_MEDIAFILES_PHOTO)])
                {
                    var removePhoto = from r in _db.photoObjects where r.PhotoKey == GetMediaKeyFromId(flowerObject.ProductId) select r;

                    //Удаление всех видео привязанных к продукту
                    while (removePhoto.Count() > 0)
                    {
                        _db.photoObjects.Remove(removePhoto.FirstOrDefault());
                        _db.SaveChanges();
                    }

                    if (flowerObject.MediaFiles != null)
                    {
                        List<string> keyList = new List<string>();

                        for (int i = 0; i < flowerObject.MediaFiles.Count(); i++)
                        {
                            if (flowerObject.MediaFiles[i].Value == InputMediaType.Photo)
                            {
                                keyList.Add(flowerObject.MediaFiles[i].Key);
                            }
                        }

                        for (int b = 0; b < keyList.Count(); b++)
                        {
                            PhotoObject[] photo = InitializeArray<PhotoObject>(keyList.Count());
                            photo[b].FileId = keyList[b];
                            photo[b].PhotoKey = GetMediaKeyFromId(flowerObject.ProductId);

                            _db.photoObjects.Add(photo[b]);
                            _db.SaveChanges();
                        }
                    }
                }

                if (changed[(int)(prop_e.CHANGED_MEDIAFILES_VIDEO)])
                {
                    var removeVideo = from r in _db.videoObjects where r.VideoKey == GetMediaKeyFromId(flowerObject.ProductId) select r;

                    //Удаление всех видео привязанных к продукту
                    while (removeVideo.Count() > 0)
                    {
                        _db.videoObjects.Remove(removeVideo.FirstOrDefault());
                        _db.SaveChanges();
                    }

                    if (flowerObject.MediaFiles != null)
                    {
                        List<string> keyList = new List<string>();

                        for (int i = 0; i < flowerObject.MediaFiles.Count(); i++)
                        {
                            if (flowerObject.MediaFiles[i].Value == InputMediaType.Video)
                            {
                                keyList.Add(flowerObject.MediaFiles[i].Key);
                            }
                        }

                        for (int b = 0; b < keyList.Count(); b++)
                        {
                            VideoObject[] video = InitializeArray<VideoObject>(keyList.Count());
                            video[b].FileId = keyList[b];
                            video[b].VideoKey = GetMediaKeyFromId(flowerObject.ProductId);

                            _db.videoObjects.Add(video[b]);
                            _db.SaveChanges();
                        }
                    }
                }

               


            }
            else
            {
                _logger.LogInformation("ChangeProduct: No parameters changed");
                return;
            }



            _db.SaveChanges();

        }

        public void DeleteSeller(string? chatId)
        {
            var removeSeller = from r in _db.sellerObjects where r.ChatId == chatId select r;

            var listNames = GetNamesProductFromSeller(chatId);
            //Удаляем все продукты связанные с продавцом
            for (int i = 0; i < listNames.Count; i++)
            {
                DeleteProductFromName(listNames[i]);
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
                DeleteProductFromName(listNames[i]);
            }
            //Удаляем саму категорию
            _db.categoryObjects.Remove(removeCategory.FirstOrDefault());
            _db.SaveChanges();
        }

        private void DeleteProductFromName(string? name_product)
        {
            if (name_product == null) { _logger.LogWarning("DeleteProduct: name_product==nullptr"); return; }

            var removePhoto = from r in _db.photoObjects where r.PhotoKey == GetMediaKeyFromId(GetIdNameProduct(name_product)) select r;

            var removeVideo = from r in _db.videoObjects where r.VideoKey == GetMediaKeyFromId(GetIdNameProduct(name_product)) select r;

            var removeProduct = from r in _db.productObjects where r.ProductId == GetIdNameProduct(name_product) select r;

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

        public void DeleteProduct(int productId)
        {
            if (productId == -1) { _logger.LogWarning("DeleteProduct: productId==-1");  return; }

            var removePhoto = from r in _db.photoObjects where r.PhotoKey == GetMediaKeyFromId(productId) select r;

            var removeVideo = from r in _db.videoObjects where r.VideoKey == GetMediaKeyFromId(productId) select r;

            var removeProduct = from r in _db.productObjects where r.ProductId == productId select r;

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

        private string GetMediaKeyFromId(int productId)
        {
            var productOBJ = _db.productObjects;

            foreach (var product in productOBJ)
            {
                if (product.ProductId == productId)
                {
                    return product.MediaKey;
                }
            }

            return "";
        }

        private void GenerateMediaKey(string? MediaKey)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            const int length = 6;

            Random random = new Random();

            string key_ret = new string(Enumerable.Repeat(chars, length)
                                .Select(s => s[random.Next(s.Length)]).ToArray()); ;

            MediaKey = "MK-" + key_ret;
        }

        private void SendToDatabasePhoto(List<KeyValuePair<string, InputMediaType>>? MediaFiles, string? MediaKey)
        {
            if (MediaKey == null)
                return;

            if (MediaFiles == null)
                return;

            var remove_options = from r in _db.photoObjects where r.PhotoKey == MediaKey select r;

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
                    photo[b].PhotoKey = MediaKey;

                    _db.photoObjects.Add(photo[b]);
                    _db.SaveChanges();
                }
            }
        }

        private void SendToDatabaseVideo(List<KeyValuePair<string, InputMediaType>>? MediaFiles, string? MediaKey)
        {
            if (MediaKey == null)
                return;

            if (MediaFiles == null)
                return;

            var remove_options = from r in _db.videoObjects where r.VideoKey == MediaKey select r;

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
                    video[b].VideoKey = MediaKey;

                    _db.videoObjects.Add(video[b]);
                    _db.SaveChanges();
                }
            }
        }

        private void SendToDatabaseProduct(string? _productName, string? _description, int _categoryId, string? _mediaKey, string? _price, int _sellerId, int _amount)
        {
            ProductObject productObject = new ProductObject()
            {
                ProductName = _productName,
                Description = _description,
                CategoryId = _categoryId,
                MediaKey = _mediaKey,
                Price = _price,
                SellerId = _sellerId, 
                Amount = _amount
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

            string MediaKey = "empty";

            GenerateMediaKey(MediaKey);

            if (MediaKey != "empty")
            {
                SendToDatabaseProduct
                (
                    flowerObject.ProductName,
                    flowerObject.Description,
                    GetIdCategory(flowerObject.CategoryName),
                    MediaKey,
                    flowerObject.Price,
                    GetIdSeller(flowerObject.ChatId),
                    flowerObject.Amount
                );

                SendToDatabasePhoto(flowerObject.MediaFiles, MediaKey);

                SendToDatabaseVideo(flowerObject.MediaFiles, MediaKey);
           
                _logger.LogInformation("Объект загружен в бд");
            }
            else
            {
                _logger.LogWarning("SendToDatabase: MediaKey==empty");
            }
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

        private FlowerObject GetFlowerObjectFromId(int productId)
        {
            List<KeyValuePair<string, InputMediaType>> mediaFiles = new List<KeyValuePair<string, InputMediaType>>();

            List<PhotoObject> photoObject = new List<PhotoObject>();

            List<VideoObject> videoObject = new List<VideoObject>();

            var categoryId = 0;

            var sellerId = 0;

            foreach (ProductObject products in _db.productObjects)
            {
                if (products.ProductId == productId)
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
                if (products.ProductId == productId)
                {
                    if (photoObject != null)
                    {
                        for (int i = 0; i < photoObject.Count; i++)
                        {
                            if (photoObject[i].PhotoKey == GetMediaKeyFromId(products.ProductId))
                            {
                                mediaFiles.Add(new KeyValuePair<string, InputMediaType>(photoObject[i].FileId, InputMediaType.Photo));
                            }
                        }
                    }

                    if (videoObject != null)
                    {
                        for (int i = 0; i < videoObject.Count; i++)
                        {
                            if (videoObject[i].VideoKey == GetMediaKeyFromId(products.ProductId))
                            {
                                mediaFiles.Add(new KeyValuePair<string, InputMediaType>(videoObject[i].FileId, InputMediaType.Video));
                            }
                        }
                    }

                    return new FlowerObject()
                    {
                        ProductId = products.ProductId,
                        ProductName = products.ProductName,
                        CategoryName = categoryName,
                        Description = products.Description,
                        Price = products.Price,
                        ChatId = chatId,
                        MediaFiles = mediaFiles,
                        Amount = products.Amount
                    };
                }
            }

            return new FlowerObject();
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
                            if (photoObject[i].PhotoKey == GetMediaKeyFromId(products.ProductId))
                            {
                                mediaFiles.Add(new KeyValuePair<string, InputMediaType>(photoObject[i].FileId, InputMediaType.Photo));
                            }
                        }
                    }

                    if (videoObject != null)
                    {
                        for (int i = 0; i < videoObject.Count; i++)
                        {
                            if (videoObject[i].VideoKey == GetMediaKeyFromId(products.ProductId))
                            {
                                mediaFiles.Add(new KeyValuePair<string, InputMediaType>(videoObject[i].FileId, InputMediaType.Video));
                            }
                        }
                    }

                    return new FlowerObject()
                    {
                        ProductId = products.ProductId,
                        ProductName = products.ProductName,
                        CategoryName = categoryName,
                        Description = products.Description,
                        Price = products.Price,
                        ChatId = chatId,
                        MediaFiles = mediaFiles,
                        Amount = products.Amount
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
