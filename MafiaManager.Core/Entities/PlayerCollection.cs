using MafiaManager.Core.Utils;
using MafiaManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MafiaManager.Core.Entities
{
    public class PlayerFilterContext
    {
        public int? Role { get; set; }

        public PlayerState[] States { get; set; }

        public PlayerState[] ExceptStates { get; set; }

        public bool? IsSelected { get; set; }
    }

    public class PlayerCollection : EntityCollection<Player, int>
    {
        public PlayerCollection() : base() { }

        public IEnumerable<Player> FilterBy(PlayerFilterContext filterContext)
        {
            IEnumerable<Player> filter = this;

            if (filterContext.States is { Length: > 0 } states)
            {
                filter = filter.Where(p => states.Contains(p.State));
            }

            if (filterContext.ExceptStates is { Length: > 0 } exceptStates)
            {
                filter = filter.Where(p => !exceptStates.Contains(p.State));
            }

            if (filterContext.Role is { } role)
            {
                filter = filter.Where(p => p.Role == role);
            }

            if (filterContext.IsSelected is { } isSelected)
            {
                filter = filter.Where(p => p.IsSelected == isSelected);
            }

            return filter;
        }

        public IEnumerable<Player> GetPlayersByRole(int roleId) => 
            this.FilterBy(new PlayerFilterContext { Role = roleId });

        public IEnumerable<Player> GetPlayersAliveByRole(int roleId) =>
            this.FilterBy(new PlayerFilterContext { Role = roleId, ExceptStates = [ PlayerState.Dead ] });

        public IEnumerable<Player> GetPlayersAlive() =>
            this.FilterBy(new PlayerFilterContext { ExceptStates = [PlayerState.Dead] });

        public bool IsMafia(Player player) =>
            Constants.MAFIA_ROLES.Contains(player.Role);

        public bool IsSheriff(Player player) =>
            Constants.SHERIFF_ROLE.ID == player.Role;

        public Player GetLastAlive()
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (this[i].State != PlayerState.Dead)
                    return this[i];
            }

            return null;
        }

        public void ResetSelection()
        {
            foreach (Player player in this)
            {
                player.IsSelected = false;
            }
        }
    }
}
