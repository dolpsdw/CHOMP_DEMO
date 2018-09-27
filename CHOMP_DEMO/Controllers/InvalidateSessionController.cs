using CHOMP_DEMO.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CHOMP_DEMO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvalidateSessionController : ControllerBase
    {
        private readonly ICacheManager _cacheManager;

        public InvalidateSessionController(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        [Authorize]
        [HttpPost] //Frombody-> deserializa los argumentos pasados a la api
        public IActionResult InvalidateSession([FromBody] InvalidateSessionRequest request)
        {
            bool result = _cacheManager.Del<string>($"{request.username}_{request.audience}");
            return result ? StatusCode(200) : StatusCode(204);
        }

        public class InvalidateSessionRequest
        {
            public string username;
            public string audience;
            public string sid;
        }
    }
}