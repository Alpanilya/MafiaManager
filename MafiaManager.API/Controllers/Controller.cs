using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MafiaManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class Controller(IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        protected readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    }
}
