using System.ComponentModel.DataAnnotations;

namespace FlowerSellerTgBot.Model.DataBase.DbObjects
{
    public class ProductObject
    {

        [Key]
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public int SellerId { get; set; }

        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public string? Price { get; set; }
    }
}
