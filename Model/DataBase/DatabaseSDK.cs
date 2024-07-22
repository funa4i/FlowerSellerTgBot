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

        public DatabaseSDK()
        {
            _db = new DataContext();

            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(string.Empty);
        }

        private void SentToDatabasePhoto(KeyValuePair<string, InputMediaType>[] MediaFiles, string name_product)
        {
            var remove_options = from r in _db.photoObjects where r.PhotoId == GetIdNameProduct(name_product) select r;

            while (remove_options.Count() > 0)
            {
                _db.photoObjects.Remove(remove_options.FirstOrDefault());
                _db.SaveChanges();
            }

            if (MediaFiles[MediaFiles.Count()].Value == InputMediaType.Video
                && MediaFiles[MediaFiles.Count()].Key == "")
                return;

            if (remove_options.Count() == 0)
            {
                for (int i = 0; i < MediaFiles.Count(); i++)
                {
                    PhotoObject photo = new PhotoObject() { FileId = MediaFiles[i].Key, PhotoId = GetIdNameProduct(name_product) };
                    _db.photoObjects.Add(photo);
                    _db.SaveChanges();
                }
            }
        }

        private void SentToDatabaseVideo(KeyValuePair<string, InputMediaType>[] MediaFiles, string name_product)
        {
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
        }

        public void SendToDatabaseObject(FlowerObject flowerObject)
        {
            if (flowerObject.CategoryName == null)
            {
                _logger.LogWarning("Не передана CategoryName");
                return;
            }

            if (flowerObject.ChatId == null)
            {
                _logger.LogWarning("Не передан ChatId");
                return;
            }

            if (flowerObject.MediaFiles == null)
            {
                _logger.LogWarning("Не передан MediaFiles");
                return;
            }

            //Если не существует категории в бд, то создаем эту категорию
            if (!CategoryDoesExist(flowerObject.CategoryName))
            {
                CategoryObject categoryObject = new CategoryObject() { NameOf = flowerObject.CategoryName };

                _db.categoryObjects.Add(categoryObject);
                _db.SaveChanges();
            }



            ProductObject productObject = new ProductObject()
            {
                ProductName = flowerObject.ProductName,
                Description = flowerObject.Description,
                CategoryId = GetIdCategory(flowerObject.CategoryName),
                Price = flowerObject.Price,
                SellerId = GetIdSeller(flowerObject.ChatId)

            };


            _db.productObjects.Add(productObject);
            _db.SaveChanges();

         
            SentToDatabasePhoto(flowerObject.MediaFiles, flowerObject.ProductName);

            SentToDatabaseVideo(flowerObject.MediaFiles, flowerObject.ProductName);

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

        public void SetNewCategory(string name_category)
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


        private int GetIdPhoto(KeyValuePair<string, InputMediaType>[]? MediaFiles)
        {
            var photoOBJ = _db.photoObjects;

            foreach (PhotoObject photo in photoOBJ)
            {
                if (MediaFiles[0].Value == InputMediaType.Photo &&
                    photo.FileId == MediaFiles[0].Key)
                {
                    return photo.PhotoId;
                }
            }

            return 0;
        }

        private int GetIdVideo(KeyValuePair<string, InputMediaType>[]? MediaFiles)
        {
            var videoOBJ = _db.videoObjects;

            foreach (VideoObject video in videoOBJ)
            {
                if (MediaFiles[0].Value == InputMediaType.Video &&
                    video.FileId == MediaFiles[0].Key)
                {
                    return video.VideoId;
                }
            }

            return 0;
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

        //сомнительная фича, но пусть пока что будет
        public bool CategoryDoesExist(string name_category)
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
