using MafiaManager.Api.Requests;
using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Services;
using MafiaManager.Shared.Constants;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MafiaManager.Api.Hubs
{
    public class GameHub(IRepository<Game> repository, GameStateStrategy gameStateStrategy) : Hub
    {
        private readonly IRepository<Game> _repository = repository;

        private readonly GameStateStrategy _gameStateStrategy = gameStateStrategy;

        public async Task Join(string gameId)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, gameId)
                .ConfigureAwait(false);
        }

        public async Task SubmitAction(string sessionID, SubmitActionRequest submitActionRequest)
        {
            if (!Guid.TryParse(sessionID, out Guid gameId))
                return;

            Game game = this._repository[gameId];

            if (game == null)
                await this.Clients.Caller.SendAsync(Constants.ON_REJECTED_ACTION, new RejectAction() { Reason = "Игра не найдена" })
                    .ConfigureAwait(false);

            try
            {
                Game newGame = gameStateStrategy.ProcessPhase(new NominateContext() { Game = game, NomitantedPlayer = submitActionRequest.TargetID });

                await this._repository.UpdateAsync(gameId, newGame, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.Clients.Caller.SendAsync(Constants.ON_REJECTED_ACTION, new RejectAction() { Reason = ex.Message })
                    .ConfigureAwait(false);
            }
        }

        public async Task SubmitVoteAction(string sessionID, SubmitVoteActionRequest submitVoteAction)
        {
            if (!Guid.TryParse(sessionID, out Guid gameId))
                return;

            Game game = this._repository[gameId];

            if (game == null)
                await this.Clients.Caller.SendAsync(Constants.ON_REJECTED_ACTION, new RejectAction() { Reason = "Игра не найдена" })
                    .ConfigureAwait(false);

            try
            {
                Game newGame = gameStateStrategy.ProcessPhase(new VoteContext() { Game = game, Votes = submitVoteAction.Votes });

                await this._repository.UpdateAsync(gameId, newGame, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.Clients.Caller.SendAsync(Constants.ON_REJECTED_ACTION, new RejectAction() { Reason = ex.Message })
                    .ConfigureAwait(false);
            }
        }
    }
}
