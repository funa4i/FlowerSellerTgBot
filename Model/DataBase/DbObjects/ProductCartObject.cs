using System.ComponentModel.DataAnnotations;

namespace FlowerSellerTgBot.Model.DataBase.DbObjects
{
    public class ProductCartObject
    {
        [Key]
        public int Id { get; set; }

        public string? Cartkey { get; set; } 

        public int ProductId { get; set; }

    }
}
