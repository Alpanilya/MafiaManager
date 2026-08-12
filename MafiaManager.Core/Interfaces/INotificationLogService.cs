using System.Threading.Tasks;

namespace MafiaManager.Core.Interfaces
{
    public interface INotificationLogService
    {
        public Task NotifyAsync(string id, string message);
    }
}
