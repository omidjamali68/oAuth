﻿using Microsoft.AspNetCore.Identity;

namespace Auth.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? NationalCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
