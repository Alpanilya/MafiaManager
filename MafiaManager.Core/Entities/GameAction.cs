using MafiaManager.Shared.Enums;
using System;

namespace MafiaManager.Core.Entities
{
    public class GameAction : Entity<Guid>
    {
        public GamePhase Phase { get; set; }

        public int Day { get; set; }

        public int? RoleID { get; set; }

        public int? PlayerID { get; set; }

        public int? TargetID { get; set; }

        public ActionType Type { get; set; }

        public ActionResult? Result { get; set; }
    }

    public class GameActionBuilder
    {
        private readonly GameAction _gameAction;

        private GameActionBuilder(Game game)
        {
            GameState gameState = game.GameState;

            this._gameAction = new()
            {
                ID = Guid.NewGuid(),
                Phase = gameState.Phase,
                Day = gameState.Day
            };
        }

        public static GameActionBuilder Create(Game game) =>
            new(game);

        public GameActionBuilder SetRole(int? role)
        {
            this._gameAction.RoleID = role;

            return this;
        }

        public GameActionBuilder SetPlayer(int? player)
        {
            this._gameAction.PlayerID = player;

            return this;
        }

        public GameActionBuilder SetTarget(int? targetID)
        {
            this._gameAction.TargetID = targetID;

            return this;
        }

        public GameActionBuilder SetActionType(ActionType actionType)
        {
            this._gameAction.Type = actionType;

            return this;
        }

        public GameActionBuilder SetActionResult(ActionResult? actionResult)
        {
            this._gameAction.Result = actionResult;

            return this;
        }

        public GameActionBuilder SetActionResult(bool result)
        {
            this._gameAction.Result = result ? ActionResult.Success : ActionResult.Fail;

            return this;
        }

        public GameActionBuilder SetGameState(GameState gameState)
        {
            this._gameAction.Phase = gameState.Phase;

            this._gameAction.Day = gameState.Day;

            return this;
        }

        public GameAction Build() =>
            this._gameAction;
    }
}
