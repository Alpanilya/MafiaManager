using MafiaManager.Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MafiaManager.Core.Interfaces
{
    public interface IRepository<T>
        where T : Entity<Guid>
    {
        T this[Guid sessionID] { get; }

        public Task AddAsync(T entity, CancellationToken cancellationToken);

        public Task UpdateAsync(Guid sessionID, T newEntity, CancellationToken cancellationToken);

        public Task DeleteAsync(Guid sessionID, CancellationToken cancellationToken);

        public Task PollingAsync(CancellationToken cancellationToken);
    }
}
