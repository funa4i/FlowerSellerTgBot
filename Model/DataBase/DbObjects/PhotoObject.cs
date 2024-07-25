﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerSellerTgBot.Model.DataBase.DbObjects
{
    public class PhotoObject
    {
        [Key]
        public string? FileId { get; set; }

        public int PhotoId { get; set; }
    }
}