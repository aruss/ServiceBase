namespace ServiceBase.Multitenancy
{
    using Microsoft.AspNetCore.Http;

    public static class MultitenancyHttpContextExtensions
    {
        public static string GetTenantId(this HttpContext context)
        {
            return context.Request.Headers["x-tenant-id"];
        }
    }
}