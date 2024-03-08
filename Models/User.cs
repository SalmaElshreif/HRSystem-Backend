﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [ForeignKey("role")]
        public int? Role_Id { get; set; }
        public virtual Role? role { get; set; }

        public string? Token {get; set;}
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
