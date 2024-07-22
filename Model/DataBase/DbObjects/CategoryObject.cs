using System.ComponentModel.DataAnnotations;

namespace FlowerSellerTgBot.Model.DataBase.DbObjects
{
    public class CategoryObject
    {
        [Key]
        public int CategoryId { get; set; }

        public string? NameOf { get; set; }
    }

}
