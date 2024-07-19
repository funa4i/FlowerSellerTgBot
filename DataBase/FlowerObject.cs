using Microsoft.EntityFrameworkCore.Design;

namespace FlowerSellerTgBot.DataBase;
/**
 * Класс, описывающий объект-цветок и его методы
 */
public class FlowerObject
{
    /**
     * ID товара (цветка)
     */
    public int ProductId;
    /**
     * ID категории товара товара (цветка)
     */
    public int CategoryId;
    /**
     * ID фотографии(й) товара (цветка)
     */
    public int[]? PhotosId;
    /**
     * ID видео товара (цветка)
     */
    public int? VideosId;
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
    /// <param name="photosId">ID фото (необязательно)</param>
    /// <param name="videosId">ID видео (необязательно)</param>
    public FlowerObject(int productId, int categoryId, string productName, string description, int countOf, int price,
     int[]? photosId = null, int? videosId = null)
    {
         ProductId = productId;
         CategoryId = categoryId;
         PhotosId = photosId;
         VideosId = videosId;
         ProductName = productName;
         Description = description;
         CountOf = countOf;
         Price = price;
    }
    
}