using System;

namespace EcommerceBackend.Models
{
    public class User
    {
        public const string DefaultRole = "customer";

        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NormalizedEmail { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Role { get; set; } = DefaultRole;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
