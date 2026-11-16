using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Utils;
using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaManager.Core.Services
{
    public abstract class GameActionMessage : IGameActionMessage
    {
        private const string NON_EXIST_PLAYER = "<b>{0}</b> выбрал(а) несуществующего игрока с ID <b>{1}</b>!";

        private const string SKIP_ACTION = "<b>{0}</b> пропускает ход!";

        public abstract string Format(Game game, GameAction action);

        public virtual string FormatResult(Game game, GameAction action)
        {
            return action.Result switch
            {
                ActionResult.Skip => FormatSkip(game, action),
                ActionResult.Fail => FormatFail(game, action),
                _ => null
            };
        }

        protected virtual string FormatSkip(Game game, GameAction action)
        {
            string actor = this.ResolveActor(game, action);

            return string.Format(SKIP_ACTION, actor);
        }

        protected virtual string FormatFail(Game game, GameAction action)
        {
            string actor = this.ResolveActor(game, action);

            string target = this.ResolveTarget(game, action);

            return string.Format(NON_EXIST_PLAYER, actor, target);
        }

        protected virtual string ResolveActor(Game game, GameAction action)
        {
            string result = null;

            if (action.PlayerID.HasValue)
            {
                Player player = game.GameSettings.Players[action.PlayerID.Value];

                result = player.Name;
            }
            else if (action.RoleID.HasValue)
            {
                Role role = game.GameSettings.Roles[action.RoleID.Value];

                result = role.Name;
            }

            return $"{result}";
        }

        protected virtual string ResolveTarget(Game game, GameAction action)
        {
            string result = null;

            if (action.TargetID.HasValue)
            {
                Player player = game.GameSettings.Players[action.TargetID.Value];

                result = player.Name;
            }

            return result;
        }
    }

    public class GameActionCheck : GameActionMessage
    {
        private const string START_CHECK = "<b>{0}</b> начинает проверку <b>{1}</b>. <b>{1}</b><b>{2}</b><b>{3}</b>!";

        private const string SEPARATOR = " ";

        private const string NO = " НЕ ";

        public override string Format(Game game, GameAction action)
        {
            if (!action.Result.HasValue)
                throw new InvalidOperationException(nameof(action.Result));

            bool isGF = game.GameSettings.Roles[action.RoleID.Value]?.ID == Constants.GOD_FATHER_ROLE.ID;

            string checkedFor = isGF ? Constants.SHERIFF_ROLE.Name : Constants.MAFIA_ROLE.Name;

            string status = action.CheckResult is true ? SEPARATOR : NO;

            string actor = this.ResolveActor(game, action);

            string target = this.ResolveTarget(game, action);

            return string.Format(START_CHECK, actor, target, status, checkedFor);
        }
    }

    public class GameActionNominate : GameActionMessage
    {
        private const string NOMINATE = "<b>{0}</b> номинирует <b>{1}</b> на выбывание!";

        public override string Format(Game game, GameAction action)
        {
            string actor = this.ResolveActor(game, action);

            string target = this.ResolveTarget(game, action);

            return string.Format(NOMINATE, actor, target);
        }
    }

    public class GameActionKill : GameActionMessage
    {
        private const string NIGHT_KILL = "<b>{0}</b> убивает <b>{1}</b>!";

        public override string Format(Game game, GameAction action)
        {
            StringBuilder sb = new();

            string actor = this.ResolveActor(game, action);

            string target = this.ResolveTarget(game, action);

            sb.AppendFormat(NIGHT_KILL, actor, target);

            PlayerCollection players = game.GameSettings.Players;

            Player player = players[action.TargetID.Value];

            var role = game.GameSettings.Roles[player.Role];

            sb.Append($" <b>{target}</b> <b>{role.Name}</b>!");

            return sb.ToString();
        }
    }

    public class GameActionGameOver : GameActionMessage
    {
        private const string GAME_OVER = "Игра завершена победой <b>{0}</b>!";

        public override string Format(Game game, GameAction action)
        {
            bool isMafiaWin = action.RoleID == Constants.MAFIA_ROLE.ID;

            string role = isMafiaWin ? "мафии" : "мирных жителей";

            return string.Format(GAME_OVER, role);
        }
    }

    public class GameActionPhaseChanged : GameActionMessage
    {
        private const string NIGHT_WAKEUP = "Город засыпает! Просыпается <b>{0}</b>";

        public override string Format(Game game, GameAction action)
        {
            string actor = this.ResolveActor(game, action);

            if (action.Phase == GamePhase.Night)
            {
                return string.Format(NIGHT_WAKEUP, actor);
            }

            return null;
        }
    }

    public class GameActionHandoff : GameActionMessage
    {
        private const string NIGHT_HANDOFF = "<b>{0}</b> засыпает! Просыпается <b>{1}!</b>";

        private const string NIGHT_OVER = "Все роли сделали свой выбор!";

        private const string DAY_OVER = "Номинирование завершено!";

        private const string DAY_HANDOFF = "<b>{0}</b> завершил речь.";

        private const string DAY_NEXT_PLAYER = "Свою речь говорит: <b>{0}</b>!";

        private const string NON_EXIST_PLAYER = "<b>{0}</b> выбрал(а) несуществующего игрока с ID <b>{1}</b>";

        private const string SKIP_TURN = "<b>{0}</b> пропускает ход";

        public override string Format(Game game, GameAction action) => action.Phase == GamePhase.Night
            ? this.FormatNightHandoff(game, action)
            : this.FormatDayHandoff(game, action);

        private string FormatNightHandoff(Game game, GameAction action)
        {
            if (!action.TargetID.HasValue)
                return NIGHT_OVER;

            string actor = ResolveActor(game, action);
            string target = ResolveTarget(game, action);

            return string.Format(NIGHT_HANDOFF, actor, target);
        }

        private string FormatDayHandoff(Game game, GameAction action)
        {
            if (!action.TargetID.HasValue)
                return DAY_OVER;

            string actor = ResolveActor(game, action);
            string target = ResolveTarget(game, action);

            StringBuilder handoff = new();

            if (!string.IsNullOrWhiteSpace(actor))
                handoff.AppendFormat(DAY_HANDOFF, actor);

            if (handoff.Length > 0)
                handoff.Append(' ');

            return handoff.AppendFormat(DAY_NEXT_PLAYER, target).ToString();
        }

        protected override string ResolveTarget(Game game, GameAction action)
        {
            string result = null;

            if (action.TargetID.HasValue)
            {
                if (action.Phase == GamePhase.Night)
                {
                    Role role = game.GameSettings.Roles[action.TargetID.Value];

                    result = role.Name;
                }
                else
                {
                    Player player = game.GameSettings.Players[action.TargetID.Value];

                    result = player.Name;
                }
            }

            return result;
        }
    }

    public class GameActionVote : GameActionMessage
    {
        private const string START_VOTE = "Все кандидаты выбраны. Голосование начинается!";

        private const string SKIP_VOTE = "Голосование было пропущено!";

        private const string VOTE_KILL = "По результату голосования выбывает игрок <b>{0}</b>!";

        public override string Format(Game game, GameAction action)
        {
            if (action.Result == null)
                return START_VOTE;

            return string.Format(VOTE_KILL, this.ResolveTarget(game, action));
        }

        protected override string FormatSkip(Game game, GameAction action) =>
            SKIP_VOTE;
    }

    public class GameActionMessageStrategy : GameActionMessage
    {
        private static readonly Dictionary<ActionType, IGameActionMessage> _actions = new()
        {
            [ActionType.Check] = new GameActionCheck(),
            [ActionType.Nominate] = new GameActionNominate(),
            [ActionType.Kill] = new GameActionKill(),
            [ActionType.PhaseChanged] = new GameActionPhaseChanged(),
            [ActionType.GameOver] = new GameActionGameOver(),
            [ActionType.Handoff] = new GameActionHandoff(),
            [ActionType.Vote] = new GameActionVote(),
        };

        public override string Format(Game game, GameAction action)
        {
            if (!_actions.TryGetValue(action.Type, out IGameActionMessage actionMessage))
                throw new InvalidOperationException(nameof(action.Type));

            if (actionMessage.FormatResult(game, action) is string formattedMessage)
                return formattedMessage;

            return actionMessage.Format(game, action);
        }
    }
}
