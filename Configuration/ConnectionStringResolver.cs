using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace EcommerceBackend.Configuration;

public static class ConnectionStringResolver
{
    private const string LocalFallback = "Host=localhost;Port=5432;Database=ecommercedb;Username=postgres;Password=postgres";

    public static string Resolve(IConfiguration configuration)
    {
        var envConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (!string.IsNullOrWhiteSpace(envConnection))
        {
            return envConnection;
        }

        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            return BuildConnectionStringFromUrl(databaseUrl);
        }

        var configuredConnection = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(configuredConnection))
        {
            return LocalFallback;
        }

        if (RequiresRailwayFallback(configuredConnection))
        {
            return LocalFallback;
        }

        return configuredConnection;
    }

    private static bool RequiresRailwayFallback(string connectionString)
    {
        if (!TryGetHost(connectionString, out var host))
        {
            return false;
        }

        if (!host.EndsWith(".railway.internal", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !CanResolve(host);
    }

    private static bool TryGetHost(string connectionString, out string host)
    {
        host = string.Empty;
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(builder.Host))
            {
                return false;
            }

            host = builder.Host;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool CanResolve(string host)
    {
        try
        {
            _ = Dns.GetHostEntry(host);
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    public static string BuildConnectionStringFromUrl(string databaseUrl)
    {
        if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri))
        {
            return databaseUrl;
        }

        var host = uri.Host;
        var port = uri.IsDefaultPort ? 5432 : uri.Port;
        var database = uri.AbsolutePath.Trim('/');

        var userInfo = uri.UserInfo.Split(':', 2);
        var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty;
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;

        var sslMode = uri.Scheme.StartsWith("postgres", StringComparison.OrdinalIgnoreCase)
            ? "Require"
            : "Disable";

        return $"Host={host};Port={port};Database={database};Username={username};Password={password};Ssl Mode={sslMode};Trust Server Certificate=true";
    }
}
