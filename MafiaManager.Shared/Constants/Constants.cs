using System;

namespace MafiaManager.Shared.Constants
{
    public class Constants
    {
        public static readonly TimeSpan DEFAULT_THRESHOLD = TimeSpan.FromHours(2);

        public const string ON_NEW_LOG_EVENT = "OnNewLogMessage";

        public const string ON_LOG_UPDATED_EVENT = "OnLogUpdated";

        public const string ON_GAMEUPDATED_EVENT = "OnGameUpdated";

        public const string ON_GAMEDELETED_EVENT = "OnGameDeleted";

        public const string ON_REJECTED_ACTION = "OnRejectedAction";
    }
}
