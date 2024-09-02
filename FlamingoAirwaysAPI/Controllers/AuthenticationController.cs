using FlamingoAirwaysAPI.Models;
using FlamingoAirwaysAPI.Models.Interfaces;
using FlamingoAirwaysAPI.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlamingoAirwaysAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous()]
    public class AuthenticationController : ControllerBase
    {
        IUserRepository _repo;
        public AuthenticationController(IUserRepository repo)
        {
            _repo = repo;
        }
        

        // POST api/<AuthenticationController>
        [HttpPost("login")]
        public IActionResult Post([FromBody] Login user)
        {
            if (user is null)
            {
                return BadRequest("Invalid user request!!!");
            }
            
            var validUser = _repo.ValidateUser(user.Email, user.Password);

            if (validUser != null)
            {

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Models.ConfigurationManager.AppSetting["Jwt:Secret"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: Models.ConfigurationManager.AppSetting["Jwt:ValidIssuer"],
                    audience: Models.ConfigurationManager.AppSetting["Jwt:ValidAudience"],
                    claims: new List<Claim>(new Claim[] {
                    new Claim(ClaimTypes.Name,validUser.Email),
                    new Claim(ClaimTypes.Role, validUser.Role),
                    new Claim("UserID", validUser.UserId)

                                   }),
                    expires: DateTime.Now.AddMinutes(6),
                    signingCredentials: signinCredentials
                );

                
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new JWTTokenResponse { Token = tokenString });
            }
            return Unauthorized();
        }

            
    }
}
