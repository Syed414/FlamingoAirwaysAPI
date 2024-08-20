using FlamingoAirwaysAPI.Models.Interfaces;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;
using Microsoft.EntityFrameworkCore;

namespace FlamingoAirwaysAPI.Models.Repositories
{
    public class FlightRepository : IFlightRepository
    {

        FlamingoAirwaysDbContext _context;
        public FlightRepository(FlamingoAirwaysDbContext context)
        {
            _context = context;
        }
        public async Task AddFlight(Flight flight)
        {
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();  // This is crucial to save the data
        }

        public async Task<IEnumerable<Flight>> GetAllFlights()
        {
            // Ensure to use ToListAsync() for asynchronous operation
            return await _context.Flights.ToListAsync();
        }

        public async Task<FlamingoAirwaysModel.Flight> GetFlightById(int id)
        {
            //throw new NotImplementedException();
            return await _context.Flights.FindAsync(id);

        }

        public async Task RemoveFlight(int id)
        {
            //throw new NotImplementedException();

            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Flight not found");
            }
        }

        public async Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime departureDate)
        {
            return await _context.Flights
                .Where(f => f.Origin == origin && f.Destination == destination && f.DepartureDate.Date == departureDate.Date)
                .ToListAsync();
        }

        

        public async Task UpdateFlight(int id, Flight flight)
        {
            Flight f = _context.Flights.Find(id);
            f.Origin = flight.Origin;
            f.Destination = flight.Destination;
            f.ArrivalDate = flight.ArrivalDate;
            f.DepartureDate = flight.DepartureDate;
            f.Price = flight.Price;
            f.AvailableSeats = flight.AvailableSeats;
            f.TotalNumberOfSeats = flight.TotalNumberOfSeats;
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Flight flight)
        {
            _context.Flights.Update(flight);
            await _context.SaveChangesAsync();
        }

        public async Task<Flight> GetByBookingIdAsync(int Bookingid)
        {
            var bookingX = _context.Bookings.Find(Bookingid);
            var FlightIdX = bookingX.FlightIdFK;
            //throw new NotImplementedException();

            return await _context.Flights.SingleOrDefaultAsync(f => f.FlightId == FlightIdX);
        }
    }
}
