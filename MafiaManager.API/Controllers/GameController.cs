using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Utils;
using MafiaManager.Shared.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MafiaManager.Api.Controllers
{
    public class GameController(IHttpContextAccessor httpContextAccessor, IRepository<Game> gameRepository) : Controller(httpContextAccessor)
    {
        private readonly IRepository<Game> _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));

        private static readonly Role[] DefaultRoles =
            [
                Constants.MAFIA_ROLE,
                Constants.GOD_FATHER_ROLE,
                Constants.SHERIFF_ROLE,
                Constants.TOWNSMAN_ROLE
            ];

        [HttpGet("settings")]
        public IActionResult GetSettings()
        {
            var gameSettings = new GameSettings()
            {
                Roles = [.. DefaultRoles],
                TimeDelay = 60,
                Players = [],
                DefaultRole = Constants.TOWNSMAN_ROLE.ID,
            };

            return this.Ok(gameSettings);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] GameSettings gameSettings, CancellationToken cancellationToken)
        {
            if (gameSettings == null)
                return this.BadRequest("Game settings cannot be null.");

            Game game = new()
            {
                ID = Guid.NewGuid(),
                GameSettings = gameSettings,
                GameState = new GameState() { Day = 1, Phase = GamePhase.Meeting },
                DateLastConnection = DateTime.Now,
            };

            await this._gameRepository.AddAsync(game, cancellationToken)
                .ConfigureAwait(false);

            return this.Ok(game);
        }

        [HttpGet("load")]
        public IActionResult Load([FromQuery] Guid? sessionID)
        {
            if (!sessionID.HasValue)
                return this.BadRequest("SessionID cannot be null.");

            Game game = this._gameRepository[sessionID.Value];

            return this.Ok(game);
        }

        [HttpGet("playerStates")]
        public IActionResult PlayerStates()
        {
            return this.Ok(Enum.GetValues<PlayerState>());
        }

        [HttpGet("gamePhases")]
        public IActionResult GamePhases()
        {
            return this.Ok(Enum.GetValues<GamePhase>());
        }

        //[HttpPost("getNextPhase")]
        //public async Task<IActionResult> GetNextPhase([FromBody] GetNextPhaseRequest requestBody, 
        //    [FromServices] GameStateStrategy gameStateStrategy,
        //    CancellationToken cancellationToken)
        //{
        //    Guid? sessionID = requestBody.SessionID;

        //    if (!sessionID.HasValue)
        //        return this.BadRequest("SessionID cannot be null.");

        //    Game game = this._gameRepository[sessionID.Value];

        //    if (game == null)
        //        return this.BadRequest("Game not found");

        //    Game newGame = gameStateStrategy.ProcessPhase(game);

        //    await this._gameRepository.UpdateAsync(sessionID.Value, newGame, cancellationToken)
        //        .ConfigureAwait(false);

        //    return this.Ok(newGame);
        //}

        //[HttpPost("getVoteResult")]
        //public async Task<IActionResult> GetVoteResult([FromBody] GetVoteResultRequest requestBody,
        //    [FromServices] GameStateStrategy gameStateStrategy,
        //    CancellationToken cancellationToken)
        //{
        //    Guid? sessionID = requestBody.SessionID;

        //    if (!sessionID.HasValue)
        //        return this.BadRequest("SessionID cannot be null.");

        //    Game game = this._gameRepository[sessionID.Value];

        //    if (game == null)
        //        return this.BadRequest("Game not found");

        //    game = gameStateStrategy.ProcessPhase(game, requestBody.VotedPlayers);

        //    await this._gameRepository.UpdateAsync(sessionID.Value, game, cancellationToken)
        //        .ConfigureAwait(false);

        //    return this.Ok(game);
        //}
    }
}
