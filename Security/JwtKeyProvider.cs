using System;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EcommerceBackend.Security
{
    public static class JwtKeyProvider
    {
        private const int MinimumKeyBytes = 32;

        public static string GetSigningKey(IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("Jwt__Key");

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "A JWT signing key was not configured. Provide a value for 'Jwt:Key' or the 'Jwt__Key' environment variable."
                );
            }

            if (Encoding.UTF8.GetByteCount(key) < MinimumKeyBytes)
            {
                throw new InvalidOperationException(
                    "The configured JWT signing key must be at least 32 bytes long when encoded as UTF-8."
                );
            }

            return key;
        }
    }
}
