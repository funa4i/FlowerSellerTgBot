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
   
        private void test_one()
        {
            //  var categoryString = _dataBase.GetCategories();

            // Console.WriteLine("Отправка объекта");




            string[] FileIDP = { "Photo-gdsgds322", "Photo-jrt43y4" };
            string[] FileIDV = { "Video-hdh5h35j4", "Video-rhdrhr34", "Video-fsdy32y3" };

            KeyValuePair<string, InputMediaType>[] MediaFiles_Custom = new KeyValuePair<string, InputMediaType>[3];

            for (int i = 0; i < 3; i++) {
                for (int b = 0; b < FileIDP.Count() - 1; b++)
                {
                    var temp = new KeyValuePair<string, InputMediaType>(FileIDP[b], InputMediaType.Photo);
                    MediaFiles_Custom[i] = temp;
                }
            }

            FlowerObject flowerObject = new FlowerObject()
            {
                CategoryName = "Category#1",
                ChatId = "awf20f020lf2flsl463g",
                ProductName = "Лаванда",
                Description = "Отличная лаванда",
                MediaFiles = MediaFiles_Custom,
                Price = "100"
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

            var list_prod = _dataBase.GetProductListPairFromCategory("Цветы");  

            Console.WriteLine("Product List");

            foreach (var item in list_prod)
            {
                Console.WriteLine(item);
            }
            
        }

        private void test_third()
        {
      
            var obj_rel_two = _dataBase.GetCategoryListPair();



            Console.WriteLine("Category list");

            foreach (var item_two in obj_rel_two)
            {
                Console.WriteLine(item_two);
            }
        }



        public void ConsoleOutput()
        {
            //      test_two();
            test_one();
        }


    }
}
