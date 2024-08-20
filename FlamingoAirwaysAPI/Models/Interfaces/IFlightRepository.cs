using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

namespace FlamingoAirwaysAPI.Models.Interfaces
{
    public interface IFlightRepository
    {
        Task<IEnumerable<Flight>> SearchFlightsAsync(string origin, string destination, DateTime departureDate);
        Task<Flight> GetFlightById(int id);
        Task<IEnumerable<Flight>> GetAllFlights();
        Task AddFlight(Flight flight);
        Task UpdateFlight(int id, Flight flight);
        Task UpdateAsync(Flight flight);
        Task RemoveFlight(int id);
        Task<Flight> GetByBookingIdAsync(int Bookingid);
    }
}
