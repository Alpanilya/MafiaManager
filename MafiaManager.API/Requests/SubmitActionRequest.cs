using MafiaManager.Core.Entities;
using System.Collections.Generic;

namespace MafiaManager.Api.Requests
{
    public class ToggleSelectRequest
    {
        public int? PlayerID { get; set; }
    }

    public class SubmitVoteActionRequest
    {
        public IReadOnlyDictionary<int, int?> Votes { get; set; }
    }

    public class RejectActionResponse
    {
        public string Reason { get; set; }
    }

    public class SwitchGameActionRequest : GameAction
    {
        public int Cursor { get; set; }
    }
}
