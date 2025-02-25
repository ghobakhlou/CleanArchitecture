using Microsoft.AspNetCore.Http;
using Application.Common.Interfaces;

namespace Infrastructure.Services;
public class ContextService : IContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string GetBaseUrl()
    {
        return GetBaseUrl(_httpContextAccessor.HttpContext ?? null);
    }

    private static string GetBaseUrl(HttpContext httpContext)
    {
        if (httpContext == null)
        {
            return null;
        }
        return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}";
    }
}
