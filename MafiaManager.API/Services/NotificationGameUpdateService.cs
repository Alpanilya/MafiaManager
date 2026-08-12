using MafiaManager.Api.Hubs;
using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Constants;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace MafiaManager.Api.Services
{
    public class NotificationGameUpdateService(IHubContext<GameHub> context) : SignalRNotificationService(context), INotificationEntityUpdateService<Game>
    {
        public async Task NotifyAsync(Game entity)
        {
            await this._context
                .Clients
                .Group(entity.ID.ToString())
                .SendAsync(Constants.ON_GAMEUPDATED_EVENT, entity)
            .ConfigureAwait(false);
        }

        public async Task NotifyDeletedAsync(Guid sessionId)
        {
            await this._context
                .Clients
                .Group(sessionId.ToString())
                .SendAsync(Constants.ON_GAMEDELETED_EVENT)
            .ConfigureAwait(false);
        }
    }
}
