using FlamingoAirwaysAPI.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;
namespace FlamingoAirwaysAPI.Models.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        FlamingoAirwaysDbContext _context;
        public PaymentRepository(FlamingoAirwaysDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<Payment> GetByIdAsync(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment> getByBookingIdAsync(int bookingId)
        {
            //throw new NotImplementedException();
            return await _context.Payments
            .FirstOrDefaultAsync(p => p.BookingIdFK == bookingId);
        }

        public async Task UpdateAsync(Payment payment)
        {
            //throw new NotImplementedException();
            _context.Payments.Update(payment);
            _context.SaveChangesAsync();
        }

        public async Task<Payment> GetByBookingIdAsync(int Bookingid)
        {
            //throw new NotImplementedException();
            return await _context.Payments.SingleOrDefaultAsync(p => p.BookingIdFK == Bookingid);
        }
    }
}
