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
 public KeyValuePair<string, InputMediaType>[]? MediaFiles;

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
 /// <param name="mediaFiles">Массив Медиафайлов (telegram ID - Type). По умолчанию - null</param>>
 public FlowerObject(int? productId, int categoryId, string productName, string description, int countOf, int price,
  KeyValuePair<string, InputMediaType>[]? mediaFiles = null)
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
      MediaFiles = new KeyValuePair<string, InputMediaType>[3];
      for (int i = 0; i < 3; i++)
      {
       MediaFiles[i] = mediaFiles[i];
      }
     }
 }
 /// <summary>
 /// Метод отпраки объекта - медиафайлов и описания
 /// </summary>
 /// <param name="bot">Бот-отправитель</param>
 /// <param name="id">ID чата</param>
 public void Send(ITelegramBotClient bot, ChatId id)
 {
     if (MediaFiles == null || MediaFiles.Length == 0) 
         return;
     List<IAlbumInputMedia> inputMedia = new();
     if (MediaFiles[0].Value == InputMediaType.Photo)
     {
         InputMediaPhoto firstInputWithDescription = new InputMediaPhoto(MediaFiles[0].Key);
         firstInputWithDescription.Caption = $"{ProductName} - {Price}\n" + $"{Description}";
         inputMedia[0] = firstInputWithDescription;
     }
     else if (MediaFiles[0].Value == InputMediaType.Video)
     {
         InputMediaVideo firstInputWithDescription = new InputMediaVideo(MediaFiles[0].Key);
         firstInputWithDescription.Caption = $"{ProductName} - {Price}\n" + $"{Description}";
         inputMedia[0] = firstInputWithDescription;
     }
     for (var i = 1; i < 3; i++)
     {
         KeyValuePair<string, InputMediaType> keyValuePair = MediaFiles[i];
         if (keyValuePair.Value == InputMediaType.Photo)
             inputMedia[i] = new InputMediaPhoto(keyValuePair.Key);
         else if (keyValuePair.Value == InputMediaType.Video)
             inputMedia[i] = new InputMediaVideo(keyValuePair.Key);
     }
     bot.SendMediaGroupAsync(id, inputMedia);
 }
 //TODO: Убрать костыли при добавлении загаловка первому элементу Inputmedia. Протестировать работу! 
 //TODO: Придумать, как выходить из метода при отсутствии медиа. (Exception какой-то, я полагаю)
}