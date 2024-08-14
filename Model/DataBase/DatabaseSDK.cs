using FlowerSellerTgBot.Controllers;
using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.Model.DataBase
{
    public class DatabaseSDK : IDataBase
    {

        private readonly DataContext _db;
        private readonly ILogger _logger;

        private enum prop_e
        { 
            MIN_SIZE,
            CHANGED_NAME,
            CHANGED_DESC,
            CHANGED_AMOUNT,
            CHANGED_PRICE,
            CHANGED_CATEGORY,
            CHANGED_MEDIAFILES_VIDEO,
            CHANGED_MEDIAFILES_PHOTO,
            MAX_SIZE
        };

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

        private bool IsEqualsTwoLStrings(List<string> firstString, List<string> secondString)
        {
            for (int i = 0; i < firstString.Count; i++)
            {
                if (firstString[i].Equals(secondString[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private List<KeyValuePair<prop_e, bool>> ChangePropeties(FlowerObject firstObject)
        {
            FlowerObject secondObject = GetFlowerObjectFromId(firstObject.ProductId);

            List<KeyValuePair<prop_e, bool>> changed_ret = new List<KeyValuePair<prop_e, bool>>();

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
               
                if (firstObject.MediaFiles[i].Value == InputMediaType.Video)
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

                if (secondObject.MediaFiles[i].Value == InputMediaType.Video)
                {
                    databaseVideo.Add(secondObject.MediaFiles[i].Key);
                }
            }

            changed_ret.Add
            (
                firstObject.ProductName != secondObject.ProductName ?
                new KeyValuePair<prop_e, bool>(prop_e.CHANGED_NAME, true) : new KeyValuePair<prop_e, bool>(prop_e.CHANGED_NAME, false)
            );

            changed_ret.Add
            (
                firstObject.Description != secondObject.Description ?
                new KeyValuePair<prop_e, bool>(prop_e.CHANGED_DESC, true) : new KeyValuePair<prop_e, bool>(prop_e.CHANGED_DESC, false)
            );

            changed_ret.Add
            (
                firstObject.CategoryName != secondObject.CategoryName ?
                new KeyValuePair<prop_e, bool>(prop_e.CHANGED_CATEGORY, true) : new KeyValuePair<prop_e, bool>(prop_e.CHANGED_CATEGORY, false)
            );

            changed_ret.Add
            (
                firstObject.Amount != secondObject.Amount ?
                new KeyValuePair<prop_e, bool>(prop_e.CHANGED_AMOUNT, true) : new KeyValuePair<prop_e, bool>(prop_e.CHANGED_AMOUNT, false)
            );

            changed_ret.Add
            (
                firstObject.Price != secondObject.Price ?
                new KeyValuePair<prop_e, bool>(prop_e.CHANGED_PRICE, true) : new KeyValuePair<prop_e, bool>(prop_e.CHANGED_PRICE, false)
            );


            if (localPhoto.Count == databasePhoto.Count)
            {
                changed_ret.Add
                (
                    !IsEqualsTwoLStrings(localPhoto, databasePhoto) ?
                    new KeyValuePair<prop_e, bool>(prop_e.CHANGED_MEDIAFILES_PHOTO, true) : new KeyValuePair<prop_e, bool>(prop_e.CHANGED_MEDIAFILES_PHOTO, false)
                );
            }
            else
            {
                changed_ret.Add(new KeyValuePair<prop_e, bool>(prop_e.CHANGED_MEDIAFILES_PHOTO, true));
            }

            if (localVideo.Count == databaseVideo.Count)
            {
                changed_ret.Add
                (
                    !IsEqualsTwoLStrings(localVideo, databaseVideo) ?
                    new KeyValuePair<prop_e, bool>(prop_e.CHANGED_MEDIAFILES_VIDEO, true) : new KeyValuePair<prop_e, bool>(prop_e.CHANGED_MEDIAFILES_VIDEO, false)
                );
            }
            else
            {
                changed_ret.Add(new KeyValuePair<prop_e, bool>(prop_e.CHANGED_MEDIAFILES_VIDEO, true));
            }

            return changed_ret;
        }

        private bool ChangeRequired(List<KeyValuePair<prop_e, bool>> listed)
        {
            for (int i = 0; i < listed.Count; i++)
            {
                if (listed[i].Value)
                    return true;
            }

            return false;
        }

        public void ChangeProduct(FlowerObject flowerObject)
        {
            if (flowerObject.ProductId == -1) { _logger.LogError("ChangeProduct: ProductId==-1"); return; }

            var productObject = _db.productObjects.Where(c => c.ProductId == flowerObject.ProductId).FirstOrDefault();

            if (productObject == null)
                return;

            var changed = ChangePropeties(flowerObject);

            if (!ChangeRequired(changed))
            {
                _logger.LogWarning("Изменений продукта не найдено!");
                return;
            }

            foreach (var change in changed)
            {
                if (change.Key == prop_e.CHANGED_NAME && change.Value == true)
                {
                    productObject.ProductName = flowerObject.ProductName;
                    _db.SaveChanges();
                }

                if (change.Key == prop_e.CHANGED_DESC && change.Value == true)
                {
                    productObject.Description = flowerObject.Description;
                    _db.SaveChanges();
                }

                if (change.Key == prop_e.CHANGED_AMOUNT && change.Value == true)
                {
                    productObject.Amount = flowerObject.Amount;
                    _db.SaveChanges();
                }

                if (change.Key == prop_e.CHANGED_PRICE && change.Value == true)
                {
                    productObject.Price = flowerObject.Price;
                    _db.SaveChanges();
                }

                if (change.Key == prop_e.CHANGED_CATEGORY && change.Value == true)
                {
                    productObject.CategoryId = GetIdCategory(flowerObject.CategoryName);
                    _db.SaveChanges();
                }

                if (change.Key == prop_e.CHANGED_MEDIAFILES_PHOTO && change.Value == true)
                {
                    SendToDatabasePhoto(flowerObject.MediaFiles, GetMediaKeyFromId(flowerObject.ProductId));
                }

                if (change.Key == prop_e.CHANGED_MEDIAFILES_VIDEO && change.Value == true)
                {
                    SendToDatabaseVideo(flowerObject.MediaFiles, GetMediaKeyFromId(flowerObject.ProductId));
                }
            }

            _logger.LogInformation("Продукт успешно изменен!");
        }
        
        private bool UserChatIdCartExist(string? chatId)
        {
            foreach(var cart in _db.cartObjects)
            {
                if (cart.ChatId == chatId)
                {
                    return true;
                }
            }

            return false;
        }

        public void DeleteAllCart(string? chatId)
        {
            if (chatId == null) { _logger.LogError("DeleteAllCart: chatId==null"); return; }

            if (!UserChatIdCartExist(chatId))
            {
                _logger.LogError("DeleteAllCart: пользователя не существует!"); 
                return;
            }

            var removeCartProductOBJ = from r in _db.cartproductObjects where r.Cartkey == GetCartKey(chatId) select r;

            var removeUser = from r in _db.cartObjects where r.ChatId == chatId select r;

            //удаление всех товаров в корзине по выбранному ключу
            while (removeCartProductOBJ.Count() > 0)
            {
                _db.cartproductObjects.Remove(removeCartProductOBJ.FirstOrDefault());
                _db.SaveChanges();
            }

            _db.cartObjects.Remove(removeUser.FirstOrDefault());
            _db.SaveChanges();
        }

        private bool CartProductExist(string? cartKey, int productId)
        {
            foreach(var cartProduct in _db.cartproductObjects)
            {
                if (cartProduct.Cartkey == cartKey && cartProduct.ProductId == productId)
                {
                    return true;
                }
            }

            return false;
        }

        public void DeleteCart(string? chatId, int productId)
        {

            if (chatId == null) { _logger.LogError("DeleteCart: chatId==null"); return; }

            var removeUser = from r in _db.cartObjects where r.ChatId == chatId select r;

            var cartKey = GetCartKey(chatId);

            if (!CartProductExist(cartKey, productId))
            {
                _logger.LogError("DeleteCart: товара не существует!"); 
                return;
            }

            foreach (var cartProduct in _db.cartproductObjects)
            {
                if (cartProduct.Cartkey == cartKey && cartProduct.ProductId == productId)
                {
                    _db.cartproductObjects.Remove(cartProduct);
                    break;
                }
            }

            _db.SaveChanges();
        }

        public void DeleteSeller(string? chatId)
        {
            var removeSeller = from r in _db.sellerObjects where r.ChatId == chatId select r;

            var listId = GetIdProductsFromSeller(chatId);
            //Удаляем все продукты связанные с продавцом
            for (int i = 0; i < listId.Count; i++)
            {
                DeleteProduct(listId[i]);
            }
            //Удаляем самого продавца
            _db.sellerObjects.Remove(removeSeller.FirstOrDefault());
            _db.SaveChanges();
        }

        

        public void DeleteCategory(string? name_category)
        {
            if (name_category == null)
            {
                _logger.LogError("DeleteCategory: name_category == null");
                return;
            }

            if (!CategoryDoesExist(name_category))
            {
                _logger.LogError("DeleteCategory: Категории не существует!");
                return;
            }

            var removeCategory = from r in _db.categoryObjects where r.NameOf == name_category select r;

            var listId = GetIdProductsFromCategory(name_category);
            //Удаляем все продукты связанные с категорией
            for (int i = 0; i < listId.Count; i++)
            {
                DeleteProduct(listId[i]);
            }
            //Удаляем саму категорию
            _db.categoryObjects.Remove(removeCategory.FirstOrDefault());
            _db.SaveChanges();
        }

        public void DeleteProduct(int productId)
        {
            if (productId == -1) { _logger.LogError("DeleteProduct: productId==-1");  return; }

            if (!ProductIdDBExist(productId))
            {
                _logger.LogError("DeleteProduct: Продукта не существует!");
                return;
            }

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

        public List<int> GetIdProductsFromCart(string? chatId)
        {
            List<int> list_ret = new List<int>();

            string cartKeyDB = GetCartKey(chatId);

            var cartproductOBJ = _db.cartproductObjects;

            foreach (var cartk in cartproductOBJ)
            {
                if (cartk.Cartkey == cartKeyDB)
                {
                    list_ret.Add(cartk.ProductId);
                }
            }

            return list_ret;
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
        
        private List<string> GetAllMediaKeys()
        {
            List<string> keys = new List<string>();

            var productOBJ = _db.productObjects;

            foreach (var product in productOBJ)
            {
                keys.Add(product.MediaKey);
            }

            return keys;
        }

        private List<string> GetAllCartKeys()
        {
            List<string> keys = new List<string>();

            var cartOBJ = _db.cartObjects;

            foreach (var cart in cartOBJ)
            {
                keys.Add(cart.CartKey);
            }

            return keys;
        }

        private bool SimilarKeyMedia(string key)
        {
            var mediaKeys = GetAllMediaKeys();

            for (int i = 0; i < mediaKeys.Count; i++)
            {
                if (mediaKeys[i] == key)
                    return true;
            }

            return false;
        }

        private bool SimilarKeyCart(string key)
        {
            var cartKeys = GetAllCartKeys();

            for (int i = 0; i < cartKeys.Count; i++)
            {
                if (cartKeys[i] == key)
                    return true;
            }

            return false;
        }

        private string GenerateMediaKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            const int length = 10;

            Random random = new Random();

            string key_ret = new string(Enumerable.Repeat(chars, length)
                                .Select(s => s[random.Next(s.Length)]).ToArray());

            while (SimilarKeyMedia(key_ret))
            {
                key_ret = new string(Enumerable.Repeat(chars, length)
                                .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return "MK-" + key_ret;
        }

        private string GenerateCartKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            const int length = 10;

            Random random = new Random();

            string key_ret = new string(Enumerable.Repeat(chars, length)
                                .Select(s => s[random.Next(s.Length)]).ToArray());

            while (SimilarKeyCart(key_ret))
            {
                key_ret = new string(Enumerable.Repeat(chars, length)
                                .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return "CK-" + key_ret;
        }

        private bool CartChatIdExist(string? chatId)
        {
            var cartOBJ = _db.cartObjects;

            foreach (var cart in cartOBJ)
            {
                if (cart.ChatId == chatId)
                    return true;
            }

            return false;
        }

        private string GetCartKey(string? chatId)
        {
            var cartOBJ = _db.cartObjects;

            foreach (var cart in cartOBJ)
            {
                if (cart.ChatId == chatId)
                    return cart.CartKey;
            }

            return "";
        }

        private bool ProductIdCartExist(int productId, string? cartKey)
        {
            var cartproductOBJ = _db.cartproductObjects;

            foreach(var cart in cartproductOBJ)
            {
                if (cart.Cartkey == cartKey && cart.ProductId == productId)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ProductIdDBExist(int productId)
        {
            var allCategories = GetCategories();

            for (int i = 0; i < allCategories.Count(); i++)
            {
                var allProducts = GetIdProductsFromCategory(allCategories[i]);

                for (int b = 0; b < allProducts.Count(); b++)
                {
                    if (allProducts[b] == productId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SendToDatabaseCart(string? chatId, int productId)
        {
            if (productId == -1)
            {
                _logger.LogError("SendToDatabase cart: productId==-1");
                return;
            }

            if (chatId == null)
            {
                _logger.LogError("SendToDatabase cart: chatId==null");
                return;
            }

            var cartOBJ = _db.cartObjects;

            var cartproductOBJ = _db.cartproductObjects;

            if (!CartChatIdExist(chatId))
            {
                string cartKey = GenerateCartKey();

                CartObject cart = new CartObject()
                {
                    ChatId = chatId,
                    CartKey = cartKey
                };

                cartOBJ.Add(cart);
                _db.SaveChanges();
            }
            else
            {
                _logger.LogInformation("SendToDatabase cart: пользователь уже существует");
            }

            string cartKeyDB = GetCartKey(chatId);

            if (ProductIdCartExist(productId, cartKeyDB))
            {
                _logger.LogError("SendToDatabase cart: такой товар уже есть в корзине!");
                return;
            }
            else if (!ProductIdDBExist(productId))
            {
                _logger.LogError("SendToDatabase cart: id продукта не существует!");
                return;
            }
            else
            {  
                ProductCartObject cartk = new ProductCartObject()
                {
                    Cartkey = cartKeyDB,
                    ProductId = productId
                };

                cartproductOBJ.Add(cartk);
                _db.SaveChanges();

                _logger.LogInformation("SendToDatabase cart: товар успешно добавлен в корзину");
            }
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
            if (flowerObject.CategoryName == null) { _logger.LogError("SendToDatabase: CategoryName==null"); return; }

            if (flowerObject.ChatId == null) { _logger.LogError("SendToDatabase: ChatId==null"); return; }

            if (flowerObject.MediaFiles == null) { 
                _logger.LogError("SendToDatabase: MediaFiles==null"); 
                return; 
            }

            if (!CategoryDoesExist(flowerObject.CategoryName)) { _logger.LogError("SendToDatabase: Category Not Exist!"); return; }

            CreateNewSeller(flowerObject.ChatId);

            string MediaKey = GenerateMediaKey();

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
                _logger.LogError("SendToDatabase: MediaKey==empty");
            }
        }

        public void CreateNewCategory(string? name_category)
        {
            if (CategoryDoesExist(name_category))
            {
                _logger.LogInformation("Категория уже существует");
                return;
            }

            CategoryObject category = new CategoryObject() { NameOf = name_category };

            _db.categoryObjects.Add(category);
            _db.SaveChanges();
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

        public FlowerObject GetFlowerObjectFromId(int productId)
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
            var mediaKey = GetMediaKeyFromId(productId); 

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
                            if (photoObject[i].PhotoKey == mediaKey)
                            {
                                mediaFiles.Add(new KeyValuePair<string, InputMediaType>(photoObject[i].FileId, InputMediaType.Photo));
                            }
                        }
                    }

                    if (videoObject != null)
                    {
                        for (int i = 0; i < videoObject.Count; i++)
                        {
                            if (videoObject[i].VideoKey == mediaKey)
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

        public List<int> GetIdProductsFromCategory(string? name_category)
        {
            if (name_category == null)
                return new List<int>();

            var listNames = new List<int>();

            var categoryId = GetIdCategory(name_category);

            foreach (ProductObject products in _db.productObjects)
            {
                if (products.CategoryId == categoryId)
                    listNames.Add(products.ProductId);
            }

            return listNames;
        }


        public List<int> GetIdProductsFromSeller(string? chatId)
        {
            if (chatId == null)
                return new List<int>();

            var listNames = new List<int>();

            var sellerId = GetIdSeller(chatId);

            foreach (ProductObject products in _db.productObjects)
            {
                if (products.SellerId == sellerId)
                    listNames.Add(products.ProductId);
            }

            return listNames;
        }

        public List<string> GetCategories()
        {
            var categories = new List<string>();

            foreach (CategoryObject category in _db.categoryObjects)
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
