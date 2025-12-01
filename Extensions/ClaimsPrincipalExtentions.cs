using System.Security.Claims;

namespace Korvan_API.Extensions
{
    public static class ClaimsPrincipalExtentions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User Id not found in token.");

            return Guid.Parse(id);
        }
    }
}
