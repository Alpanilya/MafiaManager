using MafiaManager.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MafiaManager.Api.Services
{
    public abstract class SignalRNotificationService(IHubContext<GameHub> context)
    {
        protected readonly IHubContext<GameHub> _context = context;
    }
}
