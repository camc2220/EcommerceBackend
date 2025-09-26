using System.Security.Claims;

namespace System.Security.Claims
{
    public static class ClaimsPrincipalExtensions
    {
        public static string DefaultRole(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Role) ?? "customer";
        }
    }
}
