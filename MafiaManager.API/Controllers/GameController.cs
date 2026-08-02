using MafiaManager.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("getGameSettings")]
        public IActionResult GetGameSettings()
        {
            var gameSettings = new GameSettings()
            {
                Roles = DefaultRoles,
                TimeDelay = 60,
                Players = []
            };

            return this.Ok(gameSettings);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] GameSettings gameSettings)
        {
            if (gameSettings == null)
                return this.BadRequest("Game settings cannot be null.");

            Game game = new()
            {
                GameSettings = gameSettings,
                Players = [],
            };

            return this.Ok(game);
        }
    }
}
