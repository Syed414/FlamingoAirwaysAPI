//using Microsoft.EntityFrameworkCore;
//using static FlamingoAirwaysAPI.Models.FlamingoAirwayModel;
//using static FlamingoAirwaysAPI.Models.FlamingoAirwaysDbContext;

//namespace FlamingoAirwaysAPI.Models
//{
//    public interface IFlamingoAirwaysUser
//    {

//        //User methods
//        Task<User> GetUserById (int id);
//        Task<IEnumerable<User>> GetAllUsers ();
//        Task<User> GetUserByEmail (string email);
//        Task AddUser (User user);
//        Task UpdateUser (User user);
//        Task RemoveUser(int id);


//    }

//    public interface IFlamingoAirwaysFlight
//    {
//        Task<Flight> GetFlightById(int id);
//        Task<IEnumerable<Flight>> GetAllFlights();
//        Task AddFlight(Flight flight);
//        Task UpdateFlight(Flight flight);
//        Task RemoveFlight(int id);
//    }

//    public interface IFlamingoAirwaysBooking
//    {
//        Task<Booking> GetBookingById(int id);
//        Task<IEnumerable<Booking>> GetAllBooking();
//        Task AddBooking(Booking booking);
//        Task UpdateBooking(Booking booking);
//        Task RemoveBooking(Booking booking);
//    }

//    public interface IFlamingoAirwaysTicket
//    {
//        Task<Ticket> GetTicketById(int id);
//        Task<IEnumerable<Ticket>> GetAllTicket();
//        Task AddTicket(Ticket ticket);
//        Task UpdateTicket(Ticket ticket);
//        Task RemoveTicket(Ticket ticket);

//    }

//    public class FlamingoAirwaysUserRepository : IFlamingoAirwaysUser
//    {
//        FlamingoAirwaysDB _context;
//        public FlamingoAirwaysUserRepository(FlamingoAirwaysDB context)
//        {
//            _context = context;
//        }
//        public async Task AddUser(User user)
//        {
//            //throw new NotImplementedException();
//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//        }

//        public async Task<IEnumerable<User>> GetAllUsers()
//        {
//            //throw new NotImplementedException();
//            return await _context.Users.ToListAsync();
//        }

//        public async Task<User> GetUserByEmail(string email)
//        {
//            //throw new NotImplementedException();
//            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
//        }

//        public async Task<User> GetUserById(int id)
//        {
//            // throw new NotImplementedException();
//            return await _context.Users.FindAsync(id);
//        }

//        public async Task RemoveUser(int id)
//        {
//            //throw new NotImplementedException();
//            var User = await _context.Users.FindAsync(id);
//            _context.Users.Remove(User);
//            await _context.SaveChangesAsync();

//        }

//        public async Task UpdateUser(User user)
//        {
//            //throw new NotImplementedException();
//            _context.Users.Update(user);
//            await _context.SaveChangesAsync();
//        }
//    }

//    public class FlightRepository : IFlamingoAirwaysFlight
//    {
//        FlamingoAirwaysDB _context;
//        public FlightRepository(FlamingoAirwaysDB context)
//        {
//            _context = context;
//        }
//        public async Task AddFlight(Flight flight)
//        {
//            _context.Flights.Add(flight);
//            await _context.SaveChangesAsync();  // This is crucial to save the data
//        }


//        public Task<IEnumerable<Flight>> GetAllFlights()
//        {
//            throw new NotImplementedException();
//        }

//        public Task<Flight> GetFlightById(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveFlight(int id)
//        {
//            //throw new NotImplementedException();
//            FlamingoAirwayModel m = _context.Flights.Find(id);
//        }

//        public Task UpdateFlight(Flight flight)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class BookingRepository : IFlamingoAirwaysBooking
//    {
//        public Task AddBooking(Booking booking)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Booking>> GetAllBooking()
//        {
//            throw new NotImplementedException();
//        }

//        public Task<Booking> GetBookingById(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveBooking(Booking booking)
//        {
//            throw new NotImplementedException();
//        }

//        public Task UpdateBooking(Booking booking)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class TicketRepository : IFlamingoAirwaysTicket
//    {
//        public Task AddTicket(Ticket ticket)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Ticket>> GetAllTicket()
//        {
//            throw new NotImplementedException();
//        }

//        public Task<Ticket> GetTicketById(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task RemoveTicket(Ticket ticket)
//        {
//            throw new NotImplementedException();
//        }

//        public Task UpdateTicket(Ticket ticket)
//        {
//            throw new NotImplementedException();
//        }
//    }

//}
