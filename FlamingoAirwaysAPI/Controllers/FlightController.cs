using FlamingoAirwaysAPI.Models;
using FlamingoAirwaysAPI.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysDbContext;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlamingoAirwaysAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        IFlightRepository _repo;
        public FlightController(IFlightRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<FlamingoAirwaysController>
        [HttpGet]
        public async Task<ActionResult<List<FlamingoAirwaysModel>>> ShowAll()
        {
            var flights = await _repo.GetAllFlights();
            return Ok(flights);
        }

        // GET api/<FlamingoAirwaysController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlamingoAirwaysModel>> FindFlight(int id)
        {
            var flight = await _repo.GetFlightById(id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }

        // POST api/<FlamingoAirwaysController>
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Flight value)
        {
            if (value == null)
            {
                return BadRequest();
            }

            await _repo.AddFlight(value);
            return Ok();
        }

        // PUT api/<FlamingoAirwaysController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Flight value)
        {

            var existingFlight = await _repo.GetFlightById(id);
            if (existingFlight == null)
            {
                return NotFound();
            }

            await _repo.UpdateFlight(id, value);
            return NoContent();
        }

        // DELETE api/<FlamingoAirwaysController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
           await _repo.RemoveFlight(id);
            return NoContent();
        }

        [HttpGet("origin")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights([FromQuery] string origin, [FromQuery] string destination, [FromQuery] DateTime departureDate)
        {
            var flights = await _repo.SearchFlightsAsync(origin, destination, departureDate);
            return Ok(flights);
        }
    }
}
