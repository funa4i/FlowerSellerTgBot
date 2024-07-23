using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.EntityFrameworkCore.Design;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.DataBase;

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
    
    private KeyValuePair<string, InputMediaType>[]? _mediaFiles;
    /**
     * Массив медиафайлов (ключ-значение) с ID файла и Типом файла.
     */
    public KeyValuePair<string, InputMediaType>[]? MediaFiles
    {
        get { return _mediaFiles; }
        set
        {
            if (value != null)
            {
                int mediaLength = value.Length < 3 ? value.Length : 3;
                _mediaFiles = new KeyValuePair<string, InputMediaType>[mediaLength];
                for (int i = 0; i < mediaLength; i++)
                {
                    _mediaFiles[i] = value[i];
                }
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
    /// <summary>
    /// Конструктор объекта-цветка
    /// </summary>
    /// <param name="categoryName">Имя категории</param>
    /// <param name="chatId">chatID</param>
    /// <param name="mediaFiles">Массив мадиафайлов (ID - Тип)</param>
    /// <param name="productName">Имя цветка</param>
    /// <param name="description">Описание</param>
    /// <param name="price">Цена</param>
    public FlowerObject(string? categoryName, string? chatId, KeyValuePair<string, InputMediaType>[]? mediaFiles, string? productName, string? description, string? price)
    {
        CategoryName = categoryName;
        ChatId = chatId;
        MediaFiles = mediaFiles;
        ProductName = productName;
        Description = description;
        Price = price;
    }
    /// <summary>
    /// Метод отпраки объекта - медиафайлов и описания
    /// </summary>
    /// <param name="bot">Бот-отправитель</param>
    /// <param name="id">ID чата</param>
    public async Task Send(ITelegramBotClient bot, ChatId id)
    {
        if (MediaFiles == null || MediaFiles.Length == 0)
            throw new NullReferenceException("Отсутствуют медиафайлы");
        List<IAlbumInputMedia> inputMedia = new List<IAlbumInputMedia>(3);
        for (int i = 0; i < (MediaFiles.Length < 3 ? MediaFiles.Length : 3); i++)
        {
            KeyValuePair<string, InputMediaType> keyValuePair = MediaFiles[i];
            if (keyValuePair.Value == InputMediaType.Photo)
                inputMedia.Add(new InputMediaPhoto(keyValuePair.Key));
            else if (keyValuePair.Value == InputMediaType.Video)
                inputMedia.Add(new InputMediaVideo(keyValuePair.Key));
        }
        ((InputMedia)inputMedia[0]).Caption = $"{ProductName} - {Price}\n" + $"{Description}";
        await bot.SendMediaGroupAsync(id, inputMedia);
    }
}
