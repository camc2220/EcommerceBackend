using System;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EcommerceBackend.Security
{
    public static class JwtKeyProvider
    {
        private const int MinimumKeyBytes = 32;
        public const string DefaultDevelopmentKey = "P#s8!nZ7@wF$hV9gY2kR&qL4jM3pA5uC6bE";

        /// <summary>
        /// A fallback key that can be used during development when no key has been configured.
        /// </summary>
        public static string DevelopmentFallbackKey => DefaultDevelopmentKey;

        public static string GetSigningKey(IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("Jwt__Key");

            if (string.IsNullOrWhiteSpace(key))
            {
                key = DevelopmentFallbackKey;
            }

            if (Encoding.UTF8.GetByteCount(key) < MinimumKeyBytes)
            {
                throw new InvalidOperationException(
                    "The configured JWT signing key must be at least 32 bytes (256 bits) when encoded as UTF-8 for HS256."
                );
            }

            return key;
        }
    }
}
