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
    }
}
