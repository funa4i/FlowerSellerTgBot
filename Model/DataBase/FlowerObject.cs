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

public class MediaFilesObject
{
    public string? File { get; set; }
    public InputMediaType Type { get; set; }
    public MediaFilesObject() { }

    public MediaFilesObject(string? _file, InputMediaType _type)
    {
        File = _file;
        Type = _type;
    }

}
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
     * Лист медиафайлов (ключ-значение) с ID файла и Типом файла.
     */
    public List<KeyValuePair<string, InputMediaType>>? MediaFiles;
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
    /// Метод отпраки объекта - медиафайлов и описания
    /// </summary>
    /// <param name="bot">Бот-отправитель</param>
    /// <param name="id">ID чата</param>
   /* public void Send(ITelegramBotClient bot, ChatId id)
    {
        if (MediaFiles == null || MediaFiles.Length == 0)
            return;
        var inputMedia = new List<IAlbumInputMedia>();
        for (var i = 0; i < 3; i++)
        {
            KeyValuePair<string, InputMediaType> keyValuePair = MediaFiles[i];
            if (keyValuePair.Value == InputMediaType.Photo)
                inputMedia[i] = new InputMediaPhoto(keyValuePair.Key);
            else if (keyValuePair.Value == InputMediaType.Video)
                inputMedia[i] = new InputMediaVideo(keyValuePair.Key);
        }
        bot.SendMediaGroupAsync(id, inputMedia);
    }*/
    //TODO: Убрать костыли при добавлении загаловка первому элементу Inputmedia. Протестировать работу! 
    //TODO: Придумать, как выходить из метода при отсутствии медиа. (Exception какой-то, я полагаю)

    //Тимофей(funa4i) - На каждый цветок минимум одна фотка, по другому быть не может. Ошибку выбрасывать самое то!
    //Антон(study) - Ошибку на отсутсвие фото прописал в DatabaseSDK. Подредактировал функцию Send
    
}