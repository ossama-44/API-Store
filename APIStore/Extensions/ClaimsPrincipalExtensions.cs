using System.Security.Claims;

namespace APIStore.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string RetrieveEmailFromPrincipal(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Email);
    }
}
