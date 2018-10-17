using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
        private readonly IASIProvider _asiProvider;
        private readonly IProspectProvider _prospectProvider;

        public RequestTokenController(ICacheManager cacheManager, IAccountProvider accountProvider, IASIProvider asiProvider, IProspectProvider prospectProvider)
        {
            _cacheManager = cacheManager;
            _accountProvider = accountProvider;
            _asiProvider = asiProvider;
            _prospectProvider = prospectProvider;
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
                var user = result.First();
                var prospects = _prospectProvider.Procedure<dbsp_GetProspectCustomerInfoByCustomerID_Result>("dbsp_GetProspectCustomerInfoByCustomerID", new DynamicParameters(new {CustomerID=user.CustomerID} ));
                var customerProperties = _asiProvider.Procedure<dbsp_GetCustomerPropertyByCustomerID_Result>("dbsp_GetCustomerPropertyByCustomerID", new DynamicParameters(new {customerID = user.CustomerID}  ));
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, request.username),
                    new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString())
                };
                if (true || "isSuperAdmin".IsNormalized())
                {
                    claims.Add(new Claim("SuperAdmin",""));
                }

                if (prospects.Count > 0)
                {
                    claims.Add(new Claim("FreePlayValidated", "true"));
                    claims.Add(new Claim("ProspectFreePlayAmount", prospects.First().ProspectFreePlayAmount.ToString("F") ));
                }
                foreach (dbsp_GetCustomerPropertyByCustomerID_Result prop in customerProperties)
                {
                    claims.Add(new Claim(prop.Property, prop.Value));
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