using FlamingoAirwaysAPI.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlamingoAirwaysAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        IUserRepository _repo;
        public RegisterController(IUserRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<UserController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _repo.GetAllUsers();
            return Ok(users);
        }

        // GET api/<UserController>/5
        [HttpGet("GetMyDetails")]
        public async Task<ActionResult<User>> GetUser()
        {
           var UserIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (int.TryParse(UserIdClaim, out int userid))
            {
                var MyDetails =  await _repo.GetUserDetails(userid);  
                return Ok(MyDetails);
            }
            else
            {
                return BadRequest("Wrong User");
            }
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await _repo.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST api/<UserController>
        [HttpPost("register")]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            //Before adding the password to DB Hash the password
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await _repo.AddUser(user);
            return Ok();
        }

        // PUT api/<UserController>/5
        [HttpPut("Update")]
        public async Task<IActionResult> Put([FromBody] UpdateUser user)
        {


            var UseridClaims = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            bool x = int.TryParse(UseridClaims, out int useridClaim);

            var UserRoleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var existingUser = await _repo.GetUserById(useridClaim);
            if (existingUser == null)
            {
                return NotFound($"User not found...");
            }
            existingUser.UserId = useridClaim;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            existingUser.PhoneNo = user.PhoneNo;
            existingUser.Role = UserRoleClaim;

            try
            {
                await _repo.UpdateUser(existingUser);
            }
            catch (DbUpdateException ex)
            {

                Console.WriteLine($"Error updating user: {ex.Message}");
                return StatusCode(500, "An error ocuured while updating the user.");
            }
            return NoContent();

        }

        
    }
}
