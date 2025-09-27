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
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var key = configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(key))
            {
                key = Environment.GetEnvironmentVariable("Jwt__Key");
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                key = DevelopmentFallbackKey;
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                key = DefaultDevelopmentKey;
            }

            if (Encoding.UTF8.GetByteCount(key) < MinimumKeyBytes)
            {
                throw new InvalidOperationException(
                    "The configured JWT signing key must be at least 32 bytes long when encoded as UTF-8."
                );
            }

            configuration["Jwt:Key"] = key;

            return key;
        }
    }
}
