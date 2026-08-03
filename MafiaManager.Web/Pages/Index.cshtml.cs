using HttpServices;
using HttpServices.Abstractions;
using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MafiaManager.Web.Pages;

public class IndexModel(IHttpClientService httpClientService, JsonSerializerOptions options) : PageModel
{
    private class GameSettingsBody : HttpContentCreator
    {
        public GameSettingsBody() : base("api/game/getGameSettings", HttpMethod.Get) { }

        public override HttpContent ToHttpContent() =>
            null;
    }

    public IGameSettings Settings { get; set; }

    private readonly IHttpClientService _httpClientService = httpClientService;

    private readonly JsonSerializerOptions _options = options;

    public async Task OnGet()
    {
        this.Settings = await _httpClientService.GetResponseAsync<GameSettings>(new GameSettingsBody(), this._options, default);
    }
}
