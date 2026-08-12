using System;
using System.Threading.Tasks;
using MafiaManager.Core.Entities;

namespace MafiaManager.Core.Interfaces
{
    public interface INotificationEntityUpdateService<TEntity> 
        where TEntity : Entity<Guid>
    {
        public Task NotifyAsync(TEntity entity);

        public Task NotifyDeletedAsync(Guid sessionId);
    }
}
