using Microsoft.EntityFrameworkCore.Design;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.Bot.Types.Enums;

namespace FlowerSellerTgBot.DataBase;
/**
 * Класс, описывающий объект-цветок и его методы
 */

public class CategoryObject
{
    [Key]
    public int CategoryId { get; set; }

    public string? NameOf { get; set; }
}

public class ProductObject
{

    [Key] 
    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    public string? ProductName { get; set; }

    public string? Description { get; set; }

    public int CountOf { get; set; }

    public int Price { get; set; }

}