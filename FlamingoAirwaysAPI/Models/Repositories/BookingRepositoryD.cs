//using FlamingoAirwaysAPI.Models.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

//namespace FlamingoAirwaysAPI.Models.Repositories
//{
//    public class BookingRepository : IBookingRepository, IFlightRepository
//    {
//        FlamingoAirwaysDbContext _context;
//        public BookingRepository(FlamingoAirwaysDbContext context)
//        {
//            _context = context;
//        }

//        //public async Task<(List<PassTicket> Tickets, decimal TotalPrice)> AddBooking(Booking booking,Flight flight)
//        //{
//        //    // Retrieve available seats for the flight
//        //    var flightdata = await _context.Flights
//        //        .Where(f => f.FlightId == booking.FlightId)
//        //        .SingleOrDefaultAsync();

//        //    if (flightdata == null)
//        //        throw new Exception("Flight not found");

//        //    var availableSeats = flightdata.AvailableSeats;
//        //    var totalPrice = 0m;

//        //    // Retrieve tickets that need to be updated
//        //    var tickets = await _context.Tickets
//        //        .Where(t => t.BookingId == booking.BookingId)
//        //        .OrderBy(t => t.TicketId) // Assuming TicketId is an auto-increment primary key
//        //        .ToListAsync();

//        //    if (tickets.Count == 0)
//        //        throw new Exception("No tickets found for the given booking");

//        //    foreach (Ticket ticket in tickets)
//        //    {
//        //        ticket.SeatNumber = flight.AvailableSeats;
//        //        ticket.BookingId =booking.BookingId;
//        //        totalPrice += ticket.Price;
//        //        availableSeats--;
//        //    }

//        //    // Update the tickets in the database
//        //    await _context.SaveChangesAsync();

//        //    // Retrieve the updated ticket details
//        //    var result = await _context.Tickets
//        //        .Where(t => t.BookingId == booking.BookingId)
//        //        .Join(_context.Bookings, t => t.BookingId, b => b.BookingId, (t, b) => new { t, b })
//        //        .Join(_context.Flights, tb => tb.b.FlightId, f => f.FlightId, (tb, f) => new { tb.t, tb.b, f })
//        //        .Join(_context.Users, tfb => tfb.b.UserId, u => u.UserId, (tfb, u) => new PassTicket
//        //        {
//        //            UserId = u.UserId,
//        //            FlightId = tfb.f.FlightId,
//        //            BookingId = tfb.b.BookingId,
//        //            PassengerName = tfb.t.PassengerName,
//        //            Price = tfb.t.Price,
//        //            PNR = tfb.b.PNR,
//        //            SeatNumber = tfb.t.SeatNumber
//        //        })
//        //        .OrderByDescending(t => t.SeatNumber)
//        //        .ToListAsync();

//        //    return (result, totalPrice);
//        //}
    


//    public Task AddFlight(FlamingoAirwaysModel.Flight flight)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<FlamingoAirwaysModel.Booking>> GetAllBooking()
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<FlamingoAirwaysModel.Flight>> GetAllFlights()
//        {
//            throw new NotImplementedException();
//        }

//        public Task<Booking> GetBookingById(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<List<Booking>> GetBookingByIdAsync(int id)
//        {
//            return await _context.Bookings
//                                 .Where(b => b.BookingId == id)
//                                 .ToListAsync();
//        }

//        public Task<FlamingoAirwaysModel.Flight> GetFlightById(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveBooking(FlamingoAirwaysModel.Booking booking)
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveFlight(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<FlamingoAirwaysModel.Flight>> SearchFlightsAsync(string origin, string destination, DateTime departureDate)
//        {
//            throw new NotImplementedException();
//        }

//        public Task UpdateBooking(FlamingoAirwaysModel.Booking booking)
//        {
//            throw new NotImplementedException();
//        }

//        public Task UpdateFlight(int id, FlamingoAirwaysModel.Flight flight)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
