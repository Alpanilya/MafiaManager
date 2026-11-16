using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MafiaManager.Core.Services
{
    public class GamePhaseSummaryStrategy(IGameActionMessage gameActionMessage)
    {
        private readonly Dictionary<GamePhase, IGamePhaseSummaryService> _services = new()
        {
            { GamePhase.Night, new NightGamePhaseSummaryService(gameActionMessage) },
            { GamePhase.Vote, new VoteGamePhaseSummaryService(gameActionMessage) },
        };

        public GamePhaseSummary ProcessPhaseSummary(SummaryContext stateContext)
        {
            ArgumentNullException.ThrowIfNull(stateContext.Game);

            Game game = stateContext.Game;

            GamePhase phase = stateContext.Phase;

            if (this._services.TryGetValue(phase, out IGamePhaseSummaryService summaryService))
                return summaryService.ProcessSummary(stateContext);

            return null;
        }
    }

    public class NightGamePhaseSummaryService(IGameActionMessage gameActionMessage) : IGamePhaseSummaryService
    {
        private readonly IGameActionMessage _gameActionMessage = gameActionMessage ??
            throw new ArgumentNullException(nameof(gameActionMessage));

        public GamePhaseSummary ProcessSummary(SummaryContext stateContext)
        {
            Game game = stateContext.Game;

            PlayerCollection players = game.GameSettings.Players;

            GameActionCollection gameActions = game.Actions;

            Role[] roles = [ .. game.GameSettings.Roles.Where(r => r.Phase == GamePhase.Night &&
                players.GetPlayersAliveByRole(r.ID).Any()) ];

            GameState gameState = game.GameState;

            StringBuilder stringBuilder = new();

            foreach (var role in roles)
            {
                stringBuilder.AppendLine($"<p><b>{role.Name}:</b></p>");

                stringBuilder.AppendLine("<ul>");

                GameAction[] actions = gameActions.GetActions(new()
                {
                    Day = gameState.Day - 1,
                    Phase = stateContext.Phase,
                    RoleID = role.ID,
                    Types = [ActionType.Kill, ActionType.Check],
                });

                foreach (GameAction action in actions)
                {
                    stringBuilder.AppendLine(this._gameActionMessage.Format(game, action));
                }

                stringBuilder.AppendLine("</ul>");
                stringBuilder.AppendLine("<hr />");
            }

            return new()
            { 
                ID = Guid.NewGuid(), 
                Phase = stateContext.Phase,
                Day = gameState.Day - 1,
                Message = stringBuilder.ToString() 
            };
        }
    }

    public class VoteGamePhaseSummaryService(IGameActionMessage gameActionMessage) : IGamePhaseSummaryService
    {
        private readonly IGameActionMessage _gameActionMessage = gameActionMessage ??
            throw new ArgumentNullException(nameof(gameActionMessage));

        public GamePhaseSummary ProcessSummary(SummaryContext stateContext)
        {
            Game game = stateContext.Game;

            PlayerCollection players = game.GameSettings.Players;

            GameActionCollection gameActions = game.Actions;

            GameState gameState = game.GameState;

            StringBuilder stringBuilder = new();

            int[] nominateActions = [ .. gameActions.GetActions(new()
            {
                Day = gameState.Day,
                Phase =  GamePhase.Day,
                Types = [ActionType.Nominate],
            })
            .Select(t => t.TargetID)
            .OfType<int>() ];


            if (stateContext.Owner is VoteContext voteContext && voteContext.Votes is IReadOnlyDictionary<int, int?> votes)
            {
                foreach (int id in nominateActions)
                {
                    if (players.GetOrDefault(id) is not Player player)
                        continue;

                    if (!votes.TryGetValue(player.ID, out int? value))
                        value = 0;

                    stringBuilder.AppendLine($"<p>За <b>{player.Name}:</b> {value ?? 0} голос(ов).</p>");
                }
            }

            if (stringBuilder.Length > 0)
                stringBuilder.AppendLine("<hr />");

            GameAction resultAction = gameActions.GetAction(new()
            {
                Day = gameState.Day,
                Phase = stateContext.Phase,
                Types = [ActionType.Vote],
                Results = [ActionResult.Success, ActionResult.Skip]
            });

            if (resultAction is not null)
            {
                stringBuilder.AppendLine(this._gameActionMessage.Format(game, resultAction));
            }

            return new()
            {
                ID = Guid.NewGuid(),
                Phase = stateContext.Phase,
                Day = gameState.Day,
                Message = stringBuilder.ToString()
            };
        }
    }

}
