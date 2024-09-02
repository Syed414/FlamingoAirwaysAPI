using FlamingoAirwaysAPI.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

namespace FlamingoAirwaysAPI.Models
{



    public class BookingRepository : IBookingRepository
    {
        private readonly FlamingoAirwaysDbContext _context;
        public BookingRepository(FlamingoAirwaysDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int id)
        {
            //throw new NotImplementedException();
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                booking.IsCancelled = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.ToListAsync();
        }
        public async Task DeleteTicketsByBookingIdAsync(int bookingId)
        {
            var tickets = await _context.Tickets
                .Where(t => t.BookingIdF == bookingId)
                .ToListAsync();

            if (tickets.Any())
            {
                _context.Tickets.RemoveRange(tickets);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            return await _context.Bookings.FindAsync(id);
        }

        public async Task<Booking> GetByPnrAsync(string pnr)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.PNR == pnr);
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.UserIdFK == userId) // Use UserIdFK instead of UserId
                .ToListAsync();
        }

        public async Task<Booking> GetMyTicketAsync(int bookingId)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }
    }
}
