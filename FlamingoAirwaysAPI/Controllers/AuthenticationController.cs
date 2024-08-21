using FlamingoAirwaysAPI.Models;
using FlamingoAirwaysAPI.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlamingoAirwaysAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous()]
    public class AuthenticationController : ControllerBase
    {
        ILogin  _login;
        public AuthenticationController(ILogin login)
        {
            _login = login;
        }
        

        // POST api/<AuthenticationController>
        [HttpPost("login")]
        public IActionResult Post([FromBody] Login user)
        {
            if (user is null)
            {
                return BadRequest("Invalid user request!!!");
            }

            if (_login.ValidateUser(user.Email, user.Password,user.Role))
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Models.ConfigurationManager.AppSetting["JWT:Secret"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: Models.ConfigurationManager.AppSetting["Jwt:ValidIssuer"],
                    audience: Models.ConfigurationManager.AppSetting["Jwt:ValidAudience"],
                    //claims: new List<Claim>(),
                    claims: new List<Claim>(new Claim[] {
                    new Claim(ClaimTypes.Name,user.Email),
                    new Claim(ClaimTypes.Role,user.Role)
                                   }),
                    expires: DateTime.Now.AddMinutes(6),
                    signingCredentials: signinCredentials
                ); ;
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new JWTTokenResponse { Token = tokenString });
            }
            return Unauthorized();
        }

            
    }
}
