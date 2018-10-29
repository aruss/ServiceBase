namespace WebHost
{
    using System;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using ServiceBase;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");

            WebHostWrapper.Start<Startup>(args, (services) =>
            {
                services
                    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            });
        }
    }
}
