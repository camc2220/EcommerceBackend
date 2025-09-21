using System;

namespace EcommerceBackend.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; }
        public string Role { get; set; } = "customer";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
