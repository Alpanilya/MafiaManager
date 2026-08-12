using System;

namespace MafiaManager.Shared.Constants
{
    public class Constants
    {
        public static readonly TimeSpan DEFAULT_THRESHOLD = TimeSpan.FromHours(2);

        public const string ON_LOGMESSAGE_EVENT = "OnNewLogMessage";

        public const string ON_GAMEUPDATED_EVENT = "OnGameUpdated";

        public const string ON_GAMEDELETED_EVENT = "OnGameDeleted";

        public const string ON_REJECTED_ACTION = "OnRejectedAction";

        public const string SUBMIT_ACTION = "SubmitAction";

        public const string SUBMIT_VOTE_ACTION = "SubmitVoteAction";

        public const string REJECT_ACTION = "RejectAction";

        public const string ON_START_VOTING = "OnStartVoting";
    }
}
