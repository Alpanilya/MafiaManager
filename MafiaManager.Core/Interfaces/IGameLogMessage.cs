using MafiaManager.Shared.Enums;
using System;

namespace MafiaManager.Core.Interfaces
{
    public interface IGameLogMessage
    {
        public DateTime Date { get; set; }

        public GameState GameState { get; set; }

        public string Message { get; set; }
    }
}
