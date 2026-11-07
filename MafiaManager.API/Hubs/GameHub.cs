using MafiaManager.Api.Requests;
using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Services;
using MafiaManager.Shared.Constants;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
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

        public async Task ToggleSelectPlayer(string sessionID, ToggleSelectRequest toggleSelect)
        {
            try
            {
                Game game = this.GetGame(sessionID);

                int? playerId = toggleSelect.PlayerID;

                if (!playerId.HasValue)
                    return;

                Player player = game.GameSettings.Players[playerId.Value];

                if (player.State == Shared.Enums.PlayerState.Dead)
                    return;

                bool isSelected = player.IsSelected;

                foreach (Player selectedPlayer in game.GameSettings.Players.Where(p => p.IsSelected))
                {
                    selectedPlayer.IsSelected = !selectedPlayer.IsSelected;
                }

                player.IsSelected = !isSelected;

                await this._repository.UpdateAsync(game.ID, game, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.Clients.Caller.SendAsync(Constants.ON_REJECTED_ACTION, new RejectActionResponse() { Reason = ex.Message })
                    .ConfigureAwait(false);
            }
        }

        public async Task SubmitAction(string sessionID)
        {
            try
            {
                Game game = this.GetGame(sessionID);

                Player target = game.GameSettings.Players.FirstOrDefault(p => p.IsSelected);

                Game newGame = this._gameStateStrategy.ProcessPhase(new NominateContext() { Game = game, NomitantedPlayer = target?.ID });

                await this._repository.UpdateAsync(game.ID, newGame, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.Clients.Caller.SendAsync(Constants.ON_REJECTED_ACTION, new RejectActionResponse() { Reason = ex.Message })
                    .ConfigureAwait(false);
            }
        }

        public async Task SubmitVoteAction(string sessionID, SubmitVoteActionRequest submitVoteAction)
        {
            try
            {
                Game game = this.GetGame(sessionID);

                Game newGame = this._gameStateStrategy.ProcessPhase(new VoteContext() { Game = game, Votes = submitVoteAction.Votes });

                await this._repository.UpdateAsync(game.ID, newGame, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.Clients.Caller.SendAsync(Constants.ON_REJECTED_ACTION, new RejectActionResponse() { Reason = ex.Message })
                    .ConfigureAwait(false);
            }
        }

        public async Task SwitchGameAction(string sessionID, SwitchGameActionRequest switchGameAction)
        {
            try
            {
                Game game = this.GetGame(sessionID);

                Game newGame = this._gameStateStrategy.ProcessSwitch(new SwitchContext() { Game = game, Cursor = switchGameAction.Cursor });

                await this._repository.UpdateAsync(game.ID, newGame, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.Clients.Caller.SendAsync(Constants.ON_REJECTED_ACTION, new RejectActionResponse() { Reason = ex.Message })
                    .ConfigureAwait(false);
            }
        }

        private Game GetGame(string sessionID)
        {
            if (!Guid.TryParse(sessionID, out Guid gameId))
                return null;

            return this._repository[gameId] ??
                throw new InvalidOperationException("Игра не найдена");
        }
    }
}
