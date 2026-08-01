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

            services.AddCors(options =>
            {
                options.AddPolicy("AllowWebClient", policy =>
                {
                    policy.WithOrigins("https://localhost:7256", "http://localhost:5184")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            var app = builder
                .Build();

            app
                .UseHttpsRedirection();

            app
                .UseCors("AllowWebClient");

            app
                .MapControllers();

            app
                .Run();
        }
    }
}

