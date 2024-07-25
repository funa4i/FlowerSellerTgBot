﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerSellerTgBot.Model.DataBase.DbObjects
{
    public class VideoObject
    {
        [Key]
        public string? FileId { get; set; }

        public int VideoId { get; set; }
    }
}