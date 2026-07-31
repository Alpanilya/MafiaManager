using MafiaManager.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MafiaManager.Api.Controllers
{
    public class GameController(IHttpContextAccessor httpContextAccessor) : Controller(httpContextAccessor)
    {
        private readonly Role[] DefaultRoles =
            [
                new() { Name = "Mafia", Color = System.Drawing.Color.Red },
                new() { Name = "Sheriff", Color = System.Drawing.Color.Green },
                new() { Name = "Town", Color = System.Drawing.Color.Gray },
                new() { Name = "Doctor", Color = System.Drawing.Color.Green },
            ];

        [HttpGet("getGameSettings")]
        public IActionResult GetGameSettings()
        {
            var gameSettings = new GameSettings()
            {
                Roles = DefaultRoles
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
