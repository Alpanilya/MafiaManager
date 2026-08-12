using System.Collections.Generic;

namespace MafiaManager.Api.Requests
{
    public class SubmitActionRequest
    {
        public int? TargetID { get; set; }
    }

    public class SubmitVoteActionRequest
    {
        public IReadOnlyDictionary<int, int?> Votes { get; set; }
    }

    public class RejectAction
    {
        public string Reason { get; set; }
    }
}
