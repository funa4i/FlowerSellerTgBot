using System.ComponentModel.DataAnnotations;

namespace FlowerSellerTgBot.Model.DataBase.DbObjects
{
    public class CartObject
    {
        [Key]
        public string? ChatId { get; set; }

        public string? CartKey { get; set; }
    }
}
