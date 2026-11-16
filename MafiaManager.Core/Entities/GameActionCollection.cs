using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MafiaManager.Core.Entities
{
    public class ActionFilterContext
    {
        public GamePhase Phase { get; set; }

        public int Day { get; set; }

        public int? PlayerID { get; set; }

        public int? RoleID { get; set; }

        public ActionType[] Types { get; set; }

        public ActionResult?[] Results { get; set; }
    }

    public class GameActionCollection : EntityCollection<GameAction, Guid>
    {
        private IEnumerable<GameAction> FilterBy(ActionFilterContext filterContext)
        {
            GamePhase gamePhase = filterContext.Phase;

            int day = filterContext.Day;

            IEnumerable<GameAction> filter = this.Where(a => a.Day == day && a.Phase == gamePhase);

            if (filterContext.Types is { Length: > 0 } types)
            {
                filter = filter.Where(a => types.Contains(a.Type));
            }

            if (filterContext.PlayerID.HasValue)
            {
                filter = filter.Where(a => a.PlayerID == filterContext.PlayerID);
            }

            if (filterContext.RoleID.HasValue)
            {
                filter = filter.Where(a => a.RoleID == filterContext.RoleID);
            }

            if (filterContext.Results is { Length: > 0 } results)
            {
                filter = filter.Where(a => results.Contains(a.Result));
            }

            return filter;
        }

        public GameAction[] GetActions(ActionFilterContext context) =>
            [ .. this.FilterBy(context) ];

        public GameAction GetAction(ActionFilterContext context) =>
            this.FilterBy(context).SingleOrDefault();
    }
}
