using MafiaManager.Core.Entities;
using System;

namespace MafiaManager.Api.Requests
{
    public class GetVoteResultRequest
    {
        public Guid? SessionID { get; set; }

        public GameState GameState { get; set; }

        public NominatedPlayer[] VotedPlayers { get; set; }
    }
}
