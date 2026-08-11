using MafiaManager.Core.Interfaces;
using MafiaManager.Shared.Enums;
using System;

namespace MafiaManager.Core.Entities
{
    public class GameLogMessage : IGameLogMessage
    {
        public DateTime Date { get; set; }

        public GameState GameState { get; set; }

        public string Message { get; set; }
    }
}
