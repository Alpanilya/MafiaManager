using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace MafiaManager.Core.Services
{
    public class RepositoryPollingService(IRepository<Game> repository) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await repository.PollingAsync(stoppingToken)
                .ConfigureAwait(false);
        }
    }
}
