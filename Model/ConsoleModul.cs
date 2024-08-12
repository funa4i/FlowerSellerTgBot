using FlowerSellerTgBot.Controllers;
using FlowerSellerTgBot.Model.DataBase;
using FlowerSellerTgBot.Model.DataBase.DbObjects;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.Model
{
    public class ConsoleModul
    {
        private readonly IDataBase _dataBase;


        public ConsoleModul(IDataBase dataBase)
        {
            _dataBase = dataBase;

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

        private void DownloadDBSec()
        {
            var listNames = _dataBase.GetNamesProduct("Цветы");

            for (int i = 0; i < listNames.Count; i++)
            {
                var flowerObject = _dataBase.GetFlowerObject(listNames[i]);

                Console.Write("Category: " + flowerObject.CategoryName);

                Console.WriteLine();

                Console.Write("Name: " + flowerObject.ProductName + " | Description: " + flowerObject.Description);

                Console.WriteLine();

                Console.Write("ChatId: " + flowerObject.ChatId + " | Price: " + flowerObject.Price);

                Console.WriteLine();

                Console.Write("MediaFiles: \nPhotos - ");


                foreach (var media in flowerObject.MediaFiles)
                {
                    if (media.Value == InputMediaType.Photo)
                    {
                        Console.Write(media.Key + " ");
                    }
                }


                Console.WriteLine();
                Console.Write("Videos - ");

                foreach (var media in flowerObject.MediaFiles)
                {
                    if (media.Value == InputMediaType.Video)
                    {
                        Console.Write(media.Key + " ");
                    }
                }


                Console.WriteLine();

                Console.WriteLine();

                Console.WriteLine();

            }

        }
   
        private void LoadToDB()
        {
            //  var categoryString = _dataBase.GetCategories();

            // Console.WriteLine("Отправка объекта");

            _dataBase.CreateNewCategory("Цветы");


            string[] FileIDP = { "Photo-4g-0hgldhl", "Photo-jrjhtgrh35t3", "Photo-he5heh43h4", "Photo-32yh4bedrtbe" };
            string[] FileIDV = { "Video-yhrthr547u4t", "Video-jsy6tj6565", };


            List<KeyValuePair<string, InputMediaType>>? mediaList = new List<KeyValuePair<string, InputMediaType>>();
            
            for (int i = 0; i < FileIDP.Count(); i++)
            {
                mediaList.Add(new KeyValuePair<string, InputMediaType>(FileIDP[i], InputMediaType.Photo));
            }

            for (int i = 0; i < FileIDV.Count(); i++)
            {
                mediaList.Add(new KeyValuePair<string, InputMediaType>(FileIDV[i], InputMediaType.Video));
            }

            FlowerObject flowerObject = new FlowerObject()
            {
                CategoryName = "Цветы",
                ChatId = "awf20f020lf2flsl463g",
                ProductName = "Вторая роза",
                Description = "FlowerRose is good",
                MediaFiles = mediaList,
                Price = "A lot of"
            };

            _dataBase.SendToDatabase(flowerObject);


           // _dataBase.SentToDatabasePhoto(FileID, "Роза");

         //   _dataBase.SentToDatabasePhoto(FileIDSEC, "Крапива");

            //_dataBase.SetProductObjectWithCategory(productObject, "Семена");

            //       Console.WriteLine("Категория создана!");

            /*if (categoryString.Count == 0)
            {
                Console.WriteLine("Нету категорий в бд");
            }
            else
            {
                for (int i = 0; i < categoryString.Count; i++)
                {
                    Console.WriteLine(categoryString[i]);
                }
            }*/
        }

        private void test_two()
        {

        }

        private void test_third()
        {
      

        }

        public void ConsoleOutput()
        {
            LoadToDB();


        }


    }
}
