using MafiaManager.Api.Hubs;
using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Constants;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MafiaManager.Api.Services
{
    public class NotificationLogServiceService(IHubContext<GameHub> context) : SignalRNotificationService(context), INotificationLogService
    {
        public Task NotifyAsync(string id, GameLogMessage log) =>
            this._context
                .Clients
                .Groups(id)
                .SendAsync(Constants.ON_NEW_LOG_EVENT, log);

        public Task NotifyUpdateAsync(string id, GameLogMessage log) =>
            this._context
                .Clients
                .Groups(id)
                .SendAsync(Constants.ON_LOG_UPDATED_EVENT, log);
    }
}
