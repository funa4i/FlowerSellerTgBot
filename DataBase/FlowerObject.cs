using Microsoft.EntityFrameworkCore.Design;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.DataBase;
/**
 * Класс, описывающий объект-цветок и его методы
 */
public class FlowerObject
{
    /**
     * ID товара (цветка)
     */
    public int? ProductId;
    /**
     * ID категории товара товара (цветка)
     */
    public int CategoryId;
    /**
     * Массив медиафайлов (ключ-значение) с ID файла и Типом файла.
     */
    public KeyValuePair<string, Telegram.Bot.Types.Enums.FileType>[]? MediaFiles;
    /**
     * Название товара товара (цветка)
     */
    public string ProductName;
    /**
     * Описание товара (цветка)
     */
    public string Description;
    /**
     * Количество товара (цветка) в наличии
     */
    public int CountOf;
    /**
     * Цена товара (цветка)
     */
    public int Price; //Тут можно заменить на string, в теории(?)
    /// <summary>
    /// Конструктор объекта-цветка
    /// </summary>
    /// <param name="productId">ID объекта</param>
    /// <param name="categoryId">ID категории</param>
    /// <param name="productName">Название</param>
    /// <param name="description">Описание</param>
    /// <param name="countOf">Количество</param>
    /// <param name="price">Цена</param>
    /// <param name="mediaFiles">Массив Медиафайлов (ID - Type). По умолчанию - null</param>>
    public FlowerObject(int? productId, int categoryId, string productName, string description, int countOf, int price, KeyValuePair<string, Telegram.Bot.Types.Enums.FileType>[]? mediaFiles = null)
    {
         ProductId = productId;
         CategoryId = categoryId;
         ProductName = productName;
         Description = description;
         CountOf = countOf;
         Price = price;
         //Если передают медиафайлы - записываем только первые три
         if (mediaFiles != null)
         {
              MediaFiles = new KeyValuePair<string, FileType>[3];
              for (int i = 0; i < 3; i++)
              {
               MediaFiles[i] = mediaFiles[i];
              }
         }
    }
}