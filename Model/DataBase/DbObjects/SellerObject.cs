using System.ComponentModel.DataAnnotations;

namespace FlowerSellerTgBot.Model.DataBase.DbObjects
{
    public class SellerObject
    {
        [Key]
        public int SellerId { get; set; }

        public string? ChatId { get; set; }
    }
}
