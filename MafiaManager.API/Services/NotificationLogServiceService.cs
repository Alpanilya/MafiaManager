using MafiaManager.Api.Hubs;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Constants;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MafiaManager.Api.Services
{
    public class NotificationLogServiceService(IHubContext<GameHub> context) : SignalRNotificationService(context), INotificationLogService
    {
        public async Task NotifyAsync(string id, string message)
        {
            await this._context
                .Clients
                .Groups(id)
                .SendAsync(Constants.ON_LOGMESSAGE_EVENT, message)
            .ConfigureAwait(false);
        }
    }
}
