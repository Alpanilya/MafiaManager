using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MafiaManager.Core.Repositories
{
    public class GameRepository(
        INotificationEntityUpdateService<Game> notificationUpdateService,
        IOptions<RepositorySettings> options) 
            : IRepository<Game>, IDisposable
    {
        private readonly ConcurrentDictionary<Guid, Game> _items = [];

        public Game this[Guid sessionID]
        {
            get
            {
                this._items.TryGetValue(sessionID, out Game game);

                return game;
            }
        }

        private readonly TimeSpan _dateThreshold = TimeSpan.FromMilliseconds(options.Value.Threshold);

        private readonly INotificationEntityUpdateService<Game> _notificationUpdateService = notificationUpdateService;

        private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(options.Value.TimerDelay));

        public async Task AddAsync(Game game, CancellationToken cancellationToken)
        {
            Guid sessionID = game.ID;

            this._items.TryAdd(sessionID, game);

            await this._notificationUpdateService.NotifyAsync(game)
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(Guid sessionID, CancellationToken cancellationToken)
        {
            this._items.TryRemove(sessionID, out Game game);

            await this._notificationUpdateService.NotifyAsync(game)
                .ConfigureAwait(false);
        }

        public async Task PollingAsync(CancellationToken cancellationToken)
        {
            while (await this._timer.WaitForNextTickAsync(cancellationToken))
            {
                var items = this._items.Values.ToList();

                DateTime threshold = DateTime.UtcNow
                    .AddTicks(-this._dateThreshold.Ticks);

                foreach (var item in items)
                {
                    if (item.DateLastConnection < threshold)
                    {
                        await this.DeleteAsync(item.ID, cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
            }
        }

        public async Task UpdateAsync(Guid sessionID, Game newEntity, CancellationToken cancellationToken)
        {
            this._items.AddOrUpdate(sessionID, newEntity, (k, v) => newEntity);

            await this._notificationUpdateService.NotifyAsync(newEntity)
                .ConfigureAwait(false);
        }

        public void Dispose() =>
            this._timer.Dispose();
    }
}
