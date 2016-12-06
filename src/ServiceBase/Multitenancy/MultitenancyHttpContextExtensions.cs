using Microsoft.AspNetCore.Http;

namespace ServiceBase.Multitenancy
{
    public static class MultitenancyHttpContextExtensions
    {
        public static string GetTenantId(this HttpContext context)
        {
            return context.Request.Headers["x-tenant-id"];
        }
    }
}
