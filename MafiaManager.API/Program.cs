using MafiaManager.Api.Hubs;
using MafiaManager.Api.Services;
using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Options;
using MafiaManager.Core.Repositories;
using MafiaManager.Core.Services;
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
                .AddHttpContextAccessor()
                .AddSignalR();

            services
                .Configure<RepositorySettings>(builder.Configuration.GetSection("RepositorySettings"));

            services
                .AddSingleton<INotificationLogService, NotificationLogServiceService>()
                .AddSingleton<INotificationEntityUpdateService<Game>, NotificationGameUpdateService>()
                .AddSingleton<IRepository<Game>, GameRepository>()
                .AddSingleton<IGameActionMessage, GameActionMessageStrategy>()
                .AddSingleton<IGameLogService, GameLogService>()
                .AddSingleton<GameStateStrategy>();

            services
                .AddHostedService<RepositoryPollingService>();

            var app = builder
                .Build();

            app
                .UseHttpsRedirection();

            app
                .UseCors("AllowWebClient");

            app
                .MapControllers();

            app
                .MapHub<GameHub>("/notificationRelay");

            app
                .Run();
        }
    }
}

