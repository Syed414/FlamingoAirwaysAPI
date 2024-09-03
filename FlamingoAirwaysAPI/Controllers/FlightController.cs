using FlamingoAirwaysAPI.Models;
using FlamingoAirwaysAPI.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysDbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlamingoAirwaysAPI.Controllers
{
    [Route("api/Flight")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        IFlightRepository _repo;
        public FlightController(IFlightRepository repo)
        {
            _repo = repo;
        }


        // GET: api/<FlamingoAirwaysController>
        [HttpGet("getAllFlights")]
        [AllowAnonymous()]
        public async Task<ActionResult<List<Flight>>> ShowAll()
        {
            var flights = await _repo.GetAllFlights();
            return Ok(flights);
        }


        // GET api/<FlamingoAirwaysController>/5
        [AllowAnonymous()]
        [HttpGet("getFlightByID/{id}")]
        public async Task<ActionResult<Flight>> FindFlight(int id)
        {
            var flight = await _repo.GetFlightById(id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }


        [AllowAnonymous()]
        [HttpGet("getFlight")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights([FromQuery] string origin, [FromQuery] string destination, [FromQuery] DateTime departureDate)
        {
            var flights = await _repo.SearchFlightsAsync(origin, destination, departureDate);
            return Ok(flights);
        }





        // POST api/<FlamingoAirwaysController>
        [Authorize(Roles="Admin",AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("postFlight")]
        public async Task<IActionResult> Post([FromBody] Flight value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            await _repo.AddFlight(value);
            return Ok($"Flight {value.FlightId} added");
        }



        // PUT api/<FlamingoAirwaysController>/5
        [HttpPut("putFlight/{id}")]
        [Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
        public async Task<IActionResult> Put(int id, [FromBody] Flight value)
        {

            var existingFlight = await _repo.GetFlightById(id);
            if (existingFlight == null)
            {
                return NotFound();
            }

            await _repo.UpdateFlight(id, value);
            return Ok("Flight Updation Successful!!");
        }

        // DELETE api/<FlamingoAirwaysController>/5
        [HttpDelete("deleteFlight/{id}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int id)
        {
           await _repo.RemoveFlight(id);
            return Ok($"Flight {id} deleted");
        }

        
    }
}
