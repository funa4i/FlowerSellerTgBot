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
    /**
     * Массив медиафайлов (ключ-значение) с ID файла и Типом файла.
     */
    public KeyValuePair<string, InputMediaType>[]? MediaFiles;
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
    /// Конструктор (будет дезинтегрирован в скором времени)
    /// </summary>
    /// <param name="categoryName">Имя категории</param>
    /// <param name="chatId">ID чата</param>
    /// <param name="mediaFiles">Массив медиафайлов (пары ключ-значение из ID и типа медиафайла)</param>
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
        if (MediaFiles[0].Value == InputMediaType.Photo)
        {
            var firstInputWithDescription = new InputMediaPhoto(MediaFiles[0].Key);
            firstInputWithDescription.Caption = $"{ProductName} - {Price}\n" + $"{Description}";
            inputMedia.Add(firstInputWithDescription);
        }
        else if (MediaFiles[0].Value == InputMediaType.Video)
        {
            var firstInputWithDescription = new InputMediaVideo(MediaFiles[0].Key);
            firstInputWithDescription.Caption = $"{ProductName} - {Price}\n" + $"{Description}";
            inputMedia.Add(firstInputWithDescription);
        }
        
        for (int i = 1; i < (MediaFiles.Length < 3 ? MediaFiles.Length : 3); i++)
        {
            KeyValuePair<string, InputMediaType> keyValuePair = MediaFiles[i];
            if (keyValuePair.Value == InputMediaType.Photo)
                inputMedia.Add(new InputMediaPhoto(keyValuePair.Key));
            else if (keyValuePair.Value == InputMediaType.Video)
                inputMedia.Add(new InputMediaVideo(keyValuePair.Key));
        }
        await bot.SendMediaGroupAsync(id, inputMedia);
    }
    //TODO: Убрать костыли при добавлении загаловка первому элементу Inputmedia. Протестировать работу! 
    //TODO: Сделать пустой конструктор/добавить сеттер к параметру InputMedia.
}
