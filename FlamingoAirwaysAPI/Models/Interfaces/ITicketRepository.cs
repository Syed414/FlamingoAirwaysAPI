using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

namespace FlamingoAirwaysAPI.Models.Interfaces
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetByBookingIdAsync(int bookingId);
        Task AddAsync(Ticket ticket);
        Task DeleteAsync(int ticketId);
        Task<Ticket> GetByBookingIdAndTicketIdAsync(int bookingId, int ticketId);
        Task UpdateAsync(Ticket ticket);
    }
}
