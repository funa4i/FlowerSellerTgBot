namespace FlowerSellerTgBot.ObjectClasses;
/**
 * Класс, описывающий объект-цветок и его методы (IN PROGRESS)
 */
public class FlowerObject
{
    /**
     * ID категории товара товара (цветка)
     */
    public int CategoryId;
    /**
     * ID фотографии(й) товара (цветка)
     */
    public int[] PhotosId;
    /**
     * ID видео товара (цветка)
     */
    public int VideosId;
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
    /// <param name="categoryId">ID категории</param>
    /// <param name="photosId">ID фото</param>
    /// <param name="videosId">ID видео</param>
    /// <param name="productName">Название</param>
    /// <param name="description">Описание</param>
    /// <param name="countOf">Количество</param>
    /// <param name="price">Цена</param>
    protected FlowerObject(int categoryId, string productName, string description, int countOf, int price, int[] photosId = null,  int videosId = -1)
    {
         CategoryId = categoryId;
         PhotosId = photosId;
         VideosId = videosId;
         ProductName = productName;
         Description = description;
         CountOf = countOf;
         Price = price;
    }
}