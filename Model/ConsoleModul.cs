using FlowerSellerTgBot.Controllers;
using FlowerSellerTgBot.Model.DataBase;
using FlowerSellerTgBot.Model.DataBase.DbObjects;

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

           // string[] FileID = { "5hr55rh5", "5a4y4h4" };

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
