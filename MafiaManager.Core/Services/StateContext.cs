using MafiaManager.Core.Entities;
using MafiaManager.Shared.Enums;
using System.Collections.Generic;

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

    public class SwitchContext : StateContext
    {
        public int Cursor { get; set; }
    }

    public class SummaryContext : StateContext
    {
        public GamePhase Phase { get; set; }

        public int Cursor { get; set; }

        public StateContext Owner { get; set; }
    }
}
