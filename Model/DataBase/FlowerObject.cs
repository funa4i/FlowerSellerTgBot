using Microsoft.EntityFrameworkCore.Design;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.Model.DataBase;
/**
 * Класс, описывающий объект-цветок и его методы
 */
public class FlowerObject
{
    /**
     * Название категории товара
     */
    public string? CategoryName;
    /**
    * ID чата
    */
    public string? ChatId;

    private List<KeyValuePair<string, InputMediaType>>? _mediaFiles;
    /**
     * Список медиафайлов (ключ-значение) с ID файла и Типом файла.
     */
    public List<KeyValuePair<string, InputMediaType>>? MediaFiles
    {
        get { return _mediaFiles; }
        set
        {
            if (value == null)
            {
                _mediaFiles = value;
                return;
            }
            int mediaLength = value.Count < 3 ? value.Count : 3;
            _mediaFiles = new List<KeyValuePair<string, InputMediaType>>(mediaLength);
            for (int i = 0; i < mediaLength; i++)
            {
                _mediaFiles.Add(value[i]);
            }
        }
    }
    /**
     * Название товара товара (цветка)
     */
    public string? ProductName;
    /**
     * Описание товара (цветка)
     */
    public string? Description;
    /**
     * Цена товара (цветка)
     */
    public string? Price;
    /**
     * Количество товара
     */
    public int? Amount;
    
    /// <summary>
    /// Конструктор объекта-цветка
    /// </summary>
    /// <param name="categoryName">Имя категории</param>
    /// <param name="chatId">chatID</param>
    /// <param name="mediaFiles">Список мадиафайлов (ID - Тип)</param>
    /// <param name="productName">Имя цветка</param>
    /// <param name="description">Описание</param>
    /// <param name="price">Цена</param>
    /// <param name="amount">Количество</param>>
    public FlowerObject(string? categoryName, string? chatId, List<KeyValuePair<string, InputMediaType>>? mediaFiles, string? productName, string? description, string? price, int? amount)
    {
        CategoryName = categoryName;
        ChatId = chatId;
        MediaFiles = mediaFiles;
        ProductName = productName;
        Description = description;
        Price = price;
        Amount = amount;
    }
    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public FlowerObject()
    {
    }
    /// <summary>
    /// Метод добавления медиафайла объекту
    /// </summary>
    /// <param name="id">Telegram Id файла </param>
    /// <param name="type">Тип файла</param>
    /// <returns>true - если файл был добавлен, иначе - false</returns>
    public bool AddMediafile(string id, InputMediaType type)
    {
        KeyValuePair<string, InputMediaType> kvpair = new KeyValuePair<string, InputMediaType>(id, type);
        if (MediaFiles == null)
        {
            MediaFiles = new List<KeyValuePair<string, InputMediaType>> { kvpair };
            return true;
        }
        if (MediaFiles.Count >= 3)
            return false;
        MediaFiles.Add(kvpair);
        return true;
    }
    /// <summary>
    /// Метод отправки объекта - медиафайлов и описания
    /// </summary>
    /// <param name="bot">Бот-отправитель</param>
    /// <param name="id">ID чата</param>
    public async Task Send(ITelegramBotClient bot, ChatId id)
    {
        if (MediaFiles == null || MediaFiles.Count == 0)
            throw new NullReferenceException("Отсутствуют медиафайлы");
        List<IAlbumInputMedia> inputMedia = new List<IAlbumInputMedia>(3);
        for (int i = 0; i < (MediaFiles.Count < 3 ? MediaFiles.Count : 3); i++)
        {
            KeyValuePair<string, InputMediaType> keyValuePair = MediaFiles[i];
            if (keyValuePair.Value == InputMediaType.Photo)
                inputMedia.Add(new InputMediaPhoto(keyValuePair.Key));
            else if (keyValuePair.Value == InputMediaType.Video)
                inputMedia.Add(new InputMediaVideo(keyValuePair.Key));
        }
        ((InputMedia)inputMedia[0]).Caption = $"Название: {ProductName}\n" + $"Цена: {Price} руб.\n" + $"Категория: {CategoryName}\n" + $"Количество: {Amount} шт.\n" + $"\n{Description}";
        await bot.SendMediaGroupAsync(id, inputMedia);
    }
}
