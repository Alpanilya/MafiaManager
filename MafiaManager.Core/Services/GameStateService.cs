using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Utils;
using MafiaManager.Shared.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MafiaManager.Core.Services
{
    public class StateContext
    {
        public Game Game { get; set; }
    }

    public class NominateContext : StateContext
    {
        public int? NomitantedPlayer { get; set; }
    }

    public class VoteContext : StateContext
    {
        public IReadOnlyDictionary<int, int?> Votes { get; set; }
    }

    public abstract class GameStateService : IGameStateService
    {
        public abstract Game Process(StateContext context, IGameLogService logService);

        /// <summary>
        /// Выбор следующего активного участника
        /// </summary>
        protected virtual int? GetNextActiveID(Game game, IGameLogService logService) =>
            null;

        /// <summary>
        /// Выполняет проверку возможности завершить ход
        /// </summary>
        /// <param name="game">Представление игры (<see cref="IGame"/>)</param>
        /// <param name="logService">Сервис логирования(<see cref="IGame"/>)</param>
        protected abstract bool TryEndPhase(Game game, IGameLogService logService);

        public abstract void EnterPhase(Game game, IGameLogService logService);

        public void TryGameComplete(Game game, IGameLogService logService)
        {
            PlayerCollection players = game.GameSettings.Players;

            Player[] mafiaAlive = [.. Constants.MAFIA_ROLES.SelectMany(m => players.GetPlayersAliveByRole(m))];

            GameAction gameAction = null;

            // Мирные жители (включая шерифа) побеждают, когда все представители мафии выведены из игры (и дон, и рядовые мафиози)
            if (mafiaAlive.Length == 0)
            {
                game.GameState.Phase = GamePhase.GameOver;

                gameAction = this.CreateAndAddAction(
                    game,
                    GameActionBuilder.Create(game)
                        .SetActionType(ActionType.GameOver)
                        .SetRole(Constants.TOWNSMAN_ROLE.ID)
                );
            }

            // Мафия побеждает, когда количество мафии становится равным или превышает количество мирных жителей (мирные + шериф)
            Player[] allAlive = [ ..game.GameSettings.Players.GetPlayersAlive()
                .Except(mafiaAlive) ];

            if (mafiaAlive.Length >= allAlive.Length)
            {
                game.GameState.Phase = GamePhase.GameOver;

                gameAction = this.CreateAndAddAction(
                    game,
                    GameActionBuilder.Create(game)
                        .SetActionType(ActionType.GameOver)
                        .SetRole(Constants.MAFIA_ROLE.ID));
            }

            logService.Log(game, gameAction);
        }

        protected GameAction CreateAndAddAction(Game game, GameActionBuilder builder)
        {
            GameAction gameAction = builder.Build();

            game.Actions.Add(gameAction);

            return gameAction;
        }
    }
}
