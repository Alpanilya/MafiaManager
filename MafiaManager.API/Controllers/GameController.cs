using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using MafiaManager.Core.Services;
using MafiaManager.Shared.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MafiaManager.Api.Controllers
{
    public class GameController(IHttpContextAccessor httpContextAccessor) : Controller(httpContextAccessor)
    {
        private readonly Role[] DefaultRoles =
            [
                new() { ID = 1, Name = "Мафия", Color = "#E63946" },
                new() { ID = 2, Name = "Шериф", Color = "#2A9D8F" },
                new() { ID = 3, Name = "Доктор", Color = "#457B9D" },
                new() { ID = 4, Name = "Мирный", Color = "#8D99AE" },
            ];

        [HttpGet("settings")]
        public IActionResult GetSettings()
        {
            var gameSettings = new GameSettings()
            {
                Roles = DefaultRoles,
                TimeDelay = 60,
                Players = [],
                DefaultRole = DefaultRoles[3],
            };

            return this.Ok(gameSettings);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] GameSettings gameSettings, [FromServices] IGameLogService logService)
        {
            if (gameSettings == null)
                return this.BadRequest("Game settings cannot be null.");

            Game game = new()
            {
                GameSettings = gameSettings,
                Players = gameSettings.Players,
                Day = 1,
            };

            logService.Log(game, "Игра началась!");

            return this.Ok(game);
        }

        [HttpGet("playerStates")]
        public IActionResult PlayerStates()
        {
            return this.Ok(Enum.GetValues<PlayerState>());
        }

        [HttpGet("gameStates")]
        public IActionResult GameStates()
        {
            return this.Ok(Enum.GetValues<GameState>());
        }

        [HttpPost("getNextPhase")]
        public IActionResult GetNextPhase([FromBody] Game game, [FromServices] GameStateStrategy gameStateStrategy)
        {
            var gameState = gameStateStrategy.ProcessPhase(game);

            return this.Ok(gameState);
        }
    }
}
