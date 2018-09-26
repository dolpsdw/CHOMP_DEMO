using CHOMP_DEMO.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CHOMP_DEMO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckAuthController : ControllerBase
    {
        private readonly ICacheManager _cacheManager;
        public CheckAuthController(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        [AllowAnonymous]
        [HttpPost] //Frombody-> deserializa los argumentos pasados a la api
        public IActionResult CheckAuth([FromBody] CheckAuthRequest request)
        {
            var cached = _cacheManager.Get<string>($"{request.username}_{request.audience}");
            if (cached == request.sid)
            {
                return StatusCode(200);
            }

            return StatusCode(204);
        }
        public class CheckAuthRequest
        {
            public string username;
            public string audience;
            public string sid;
        }
    }
}