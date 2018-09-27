using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CHOMP_DEMO.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CHOMP_DEMO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestTokenController : ControllerBase
    {
        private readonly ICacheManager _cacheManager;
        public RequestTokenController(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        //We want clients direct acces to us for JWT passports
        [AllowAnonymous]
        [HttpPost]  //Frombody-> deserializa los argumentos pasados a la api
        public IActionResult RequestToken([FromBody] TokenRequest request)
        {
            if (request.username == "1234" && request.contra == "1234")
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, request.username),
                    new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()), 
                };
                if (true || "isSuperAdmin".IsNormalized())
                {
                    claims.Add(new Claim("SuperAdmin",""));
                }
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("LARGESYMETRICCHUNKOFSTRINGMAYORTHAN128BITSOFLENGHTFORSHAREINSERVERS"/*_configuration["SecurityKey"]*/));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "chomp.chain",
                    audience: "tiggergaming.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);
                _cacheManager.Set($"{request.username}_TGWeb", claims[1].Value);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return BadRequest("Could not verify username and password");
        }

        public class TokenRequest
        {
            public string username;
            public string contra;
        }
    }
}