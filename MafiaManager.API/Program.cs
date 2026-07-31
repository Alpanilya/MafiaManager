using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MafiaManager.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IServiceCollection services = builder.Services;

            services
                .AddControllers();

            services
                .AddDataProtection();

            services
                .AddDistributedMemoryCache()
                .AddHttpContextAccessor();

            var app = builder
                .Build();

            app
                .UseHttpsRedirection();

            app
                .MapControllers();

            app
                .Run();
        }
    }
}

