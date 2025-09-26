using System;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EcommerceBackend.Security
{
    public static class JwtKeyProvider
    {
        private const int MinimumKeyBytes = 16;
        public const string DefaultDevelopmentKey = "development-secret-key-change-me";

        public static string GetSigningKey(IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("Jwt__Key");

            if (string.IsNullOrWhiteSpace(key))
            {
                key = DefaultDevelopmentKey;
            }

            if (Encoding.UTF8.GetByteCount(key) < MinimumKeyBytes)
            {
                throw new InvalidOperationException(
                    "The configured JWT signing key must be at least 16 bytes long when encoded as UTF-8."
                );
            }

            return key;
        }
    }
}
