using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CHOMP_DEMO.DTO;
using CHOMP_DEMO.Managers;
using CHOMP_DEMO.Providers;
using Dapper;
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
        private readonly IAccountProvider _accountProvider;

        public RequestTokenController(ICacheManager cacheManager, IAccountProvider accountProvider)
        {
            _cacheManager = cacheManager;
            _accountProvider = accountProvider;
        }
        //We want clients direct acces to us for JWT passports
        [AllowAnonymous]
        [HttpPost]  //Frombody-> deserializa los argumentos pasados a la api
        public IActionResult RequestToken([FromBody] TokenRequest request)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@customerID", request.username);
            p.Add("@password", request.contra, DbType.String);
            p.Add("@inetTarget", "WL-TG", DbType.String, ParameterDirection.Input);
            p.Add("@clientcode", "BES");
            p.Add("@ipAddress", "0.0.0.1");
            ICollection<dbsp_GetCustomerInfoAtLogin_Result> result = _accountProvider.Procedure<dbsp_GetCustomerInfoAtLogin_Result>("dbsp_GetCustomerInfoAtLogin", p);
            if (result.Count > 0)
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
                    audience: "TG_Web",
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