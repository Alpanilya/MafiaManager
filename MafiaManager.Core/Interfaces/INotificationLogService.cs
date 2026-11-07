using MafiaManager.Core.Entities;
using System.Threading.Tasks;

namespace MafiaManager.Core.Interfaces
{
    public interface INotificationLogService
    {
        public Task NotifyAsync(string id, GameLogMessage log);

        public Task NotifyUpdateAsync(string id, GameLogMessage log);
    }
}
