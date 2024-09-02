using Microsoft.AspNetCore.Mvc;
using FlamingoAirwaysAPI.Models.Interfaces;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel.Payment;
using FlamingoAirwaysAPI.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace Flamingo_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BookingController : ControllerBase
    {

        //Here we are injecting all the services, repositories that a class needs to perform its functions instead of class creating its own dependencies
        private readonly IBookingRepository _bookingRepo;
        private readonly IFlightRepository _flightRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ITicketRepository _ticketRepo;
        private readonly IUserRepository _userRepo;
        private readonly FlamingoAirwaysDbContext _context;


        public BookingController(IBookingRepository bookingRepo, IFlightRepository flightRepo, IPaymentRepository paymentRepo, ITicketRepository ticketRepo, FlamingoAirwaysDbContext context)
        {
            _bookingRepo = bookingRepo;
            _flightRepo = flightRepo;
            _paymentRepo = paymentRepo;
            _ticketRepo = ticketRepo;
            _context = context;

        }

        [HttpGet]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()

        {
            var bookings = await _bookingRepo.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet("AllMyBookings")]
        [Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllMyBookings()

        {
            var userIDClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            var allmy = await _bookingRepo.GetByUserIdAsync(userIDClaim);
            return Ok(allmy);

        }


        // POST api/Booking

        [HttpPost]
        [Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Booking>> PostBooking([FromBody] BookingRequest request)
        {

            if (request == null ||
            request.Seats <= 0 && request.BSeats <= 0 ||
            request.Payment == null ||
            string.IsNullOrEmpty(request.Payment.CardNumber) ||
            string.IsNullOrEmpty(request.Payment.CardHolderName) ||
            string.IsNullOrEmpty(request.BankName) ||
            request.CVV <= 0) // Assuming CVV should be a positive integer
            {
                return BadRequest("Invalid request data.");
            }

            // Validate CVV
            if (!Regex.IsMatch(request.CVV.ToString(), @"^\d{3}$"))
            {
                return BadRequest("Invalid CVV. It must be 3 digits.");
            }

            // Validate Card Number (must be 13 to 16 digits)
            if (!Regex.IsMatch(request.Payment.CardNumber, @"^\d{13,16}$"))
            {
                return BadRequest("Invalid card number. It must be between 13 and 16 digits.");
            }

            // Validate Bank Name
            var validBanks = new HashSet<string> { "Kotak", "SBI", "Axis", "HDFC" };
            if (!validBanks.Contains(request.BankName))
            {
                return BadRequest("Invalid bank name. Please select a valid bank.");
            }

            // Validate Flight
            var flight = await _flightRepo.GetFlightById(request.FlightId);
            if (flight == null)
            {
                return NotFound("Flight not found.");
            }

            // Validate Payment Type
            var validPaymentTypes = new HashSet<string> { "Credit Card", "Debit Card" };
            if (!validPaymentTypes.Contains(request.Payment.PaymentType))
            {
                return BadRequest("This payment method is not available. Please select Credit Card or Debit Card.");
            }

            // Validate Seat Availability
            if (request.Seats > flight.AvailableSeats)
            {
                return BadRequest("Not enough seats available.");
            }

            // Create booking
            var booking = new Booking
            {

                FlightIdFK = request.FlightId,
                UserIdFK = HttpContext.User.FindFirst("UserId")?.Value,
                BookingDate = DateTime.UtcNow,
                PNR = GeneratePnr(),
                IsCancelled = false
            };

            await _bookingRepo.AddAsync(booking);

            var payment = new Payment
            {
                BookingIdFK = booking.BookingId,
                PaymentType = request.Payment.PaymentType,
                CardNumber = request.Payment.CardNumber,
                CardHolderName = request.Payment.CardHolderName,
                PaymentDate = DateTime.UtcNow,
                Amount = (flight.Price * request.Seats) + (flight.BPrice * request.BSeats)
            };
            await _paymentRepo.AddAsync(payment);
            for (int i = 0; i < request.Seats; i++)
            {
                var ticket = new Ticket
                {
                    BookingIdF = booking.BookingId,
                    SeatNumber = $"E-{flight.TotalNumberOfSeats - flight.AvailableSeats + 1}", // Generate seat number
                    PassengerName = request.PassengerNames[i],
                    Price = flight.Price
                };

                await _ticketRepo.AddAsync(ticket);
                flight.AvailableSeats -= 1;
                await _flightRepo.UpdateAsync(flight);
            }
            
            //return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);

            for (int i = 0; i < request.BSeats; i++)
            {
                var ticket = new Ticket
                {
                    BookingIdF = booking.BookingId,
                    SeatNumber = $"B-{flight.AvailableBSeats - i}", // Generate seat number
                    PassengerName = request.BPassengerNames[i],
                    Price = flight.BPrice
                };

                await _ticketRepo.AddAsync(ticket);
                //Decrease seats from the flight....
                flight.AvailableBSeats -= 1;
                await _flightRepo.UpdateAsync(flight);
            }

            return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);

        }

        // GET api/Booking/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            var ticket = await _ticketRepo.GetByBookingIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var userBooking = await _bookingRepo.GetByIdAsync(id);
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value; //This gets the userid of the current authorized user from the token. User id is used to verify the current user 
            if (userBooking.UserIdFK.ToString() != currentUserId)
            {
                return Forbid("You are not authorized to delete this ticket.");
            }

            var flight = await _flightRepo.GetByBookingIdAsync(id);

            //calculate the difference in days
            TimeSpan difference = flight.DepartureDate - DateTime.Now;
            int daysRemaining = difference.Days;

            var refundPercent = 0m;

            if (daysRemaining >= 30)
            {
                refundPercent = 0.8m; //80% refund
            }
            else if (daysRemaining >= 14)
            {
                refundPercent = 0.5m;
            }
            else if (daysRemaining >= 7)
            {
                refundPercent = 0.3m;
            }
            else if (daysRemaining >= 3)
            {
                refundPercent = 0.1m;
            }
            else if (daysRemaining >= 1)
            {
                refundPercent = 0.05m; // 5% refund
            }
            else
            {
                refundPercent = 0m;
            }

            // Delete tickets
            var tickets = await _ticketRepo.GetByBookingIdAsync(id);
            if (tickets != null)
            {
                foreach (var ticket in tickets)
                {
                    await _ticketRepo.DeleteAsync(ticket.TicketId);
                    flight.AvailableSeats += 1;
                    await _flightRepo.UpdateAsync(flight);
                }
            }

            // Delete booking and setting it's value.....
            await _bookingRepo.CancelAsync(id);

            //update payment

            var payment = await _paymentRepo.getByBookingIdAsync(id);
            if (payment != null)
            {
                payment.Retainer += payment.Amount * (1 - refundPercent);
                payment.Amount = 0;
                payment.RefundAmount += payment.Amount * refundPercent;
                await _paymentRepo.UpdateAsync(payment);
            }

            return NoContent();
        }


        [HttpDelete("{bookingId}/ticket/{ticketId}")]
        [Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteTicket(int bookingId, int ticketId)
        {
            // Fetch the ticket by its ID and Booking ID

            var ticket = await _ticketRepo.GetByBookingIdAndTicketIdAsync(bookingId, ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            //Check if the current user is owner of the booking
            var userBooking = await _bookingRepo.GetByIdAsync(bookingId);
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value; //This gets the userid of the current authorized user from the token. User id is used to verify the current user 
            if (userBooking.UserIdFK.ToString() != currentUserId)
            {
                return Forbid("You are not authorized to delete this ticket.");
            }

            var flight= await _flightRepo.GetByBookingIdAsync(bookingId);

            //calculate the difference in days
            TimeSpan difference = flight.DepartureDate - DateTime.Now;
            int daysRemaining = difference.Days;

            var refundPercent = 0m;

            if (daysRemaining >= 30)
            {
                refundPercent = 0.8m; //80% refund
            }
            else if (daysRemaining >= 14)
            {
                refundPercent = 0.5m;
            }
            else if (daysRemaining >= 7)
            {
                refundPercent = 0.3m;
            }
            else if (daysRemaining >= 3)
            {
                refundPercent = 0.1m;
            }
            else if (daysRemaining >= 1)  
            {
                refundPercent = 0.05m; // 5% refund
            }
            else
            {
                refundPercent = 0m;
            }
            //payment update
            var payment = await _paymentRepo.getByBookingIdAsync(bookingId);
            payment.Amount -= ticket.Price;
            payment.Retainer += ticket.Price * (1 - refundPercent);
            payment.RefundAmount += ticket.Price * refundPercent;

            // Determine seat type and update availability
            if (ticket.SeatNumber.StartsWith("E-")) // Economy seat
            {
                flight.AvailableSeats += 1;
            }
            else if (ticket.SeatNumber.StartsWith("B-")) // Business seat
            {
                flight.AvailableBSeats += 1;
            }
            // Delete the specific ticket
            await _ticketRepo.DeleteAsync(ticket.TicketId);
            await _flightRepo.UpdateAsync(flight);

            // Check if there are any remaining tickets for this booking
            var remainingTickets = await _ticketRepo.GetByBookingIdAsync(bookingId);

            if (remainingTickets == null || !remainingTickets.Any())
            {
                // If no tickets remain, cancel the booking
                await _bookingRepo.CancelAsync(bookingId);
            }

            return Ok($"Ticket Deleted - {payment.RefundAmount} refunded");
        }

        [HttpGet("myTicket")]
        [Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Booking>> GetMyTicket(int bookingId)
        {
            var userBooking = await _bookingRepo.GetByIdAsync(bookingId);
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (userBooking.UserIdFK.ToString() != currentUserId)
            {
                return Forbid("You are not authorized to download this ticket.");
            }

            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            var tickets = await _ticketRepo.GetByBookingIdAsync(bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            var content = new StringBuilder();
            content.AppendLine($"Booking ID: {booking.BookingId}");
            content.AppendLine($"Flight Number:{booking.FlightIdFK}");
            content.AppendLine($"Booking Date:{booking.BookingDate}");

            content.AppendLine($"PNR:{booking.PNR}");

            foreach (var ticket in tickets)
            {
                content.AppendLine($"Ticket ID: {ticket.TicketId}");
                content.AppendLine($"Passenger Name: {ticket.PassengerName}");
                content.AppendLine($"Seat Number: {ticket.SeatNumber}");
                content.AppendLine();
            }


            var TicketPrice = await _paymentRepo.getByBookingIdAsync(bookingId);
            content.AppendLine($"Price of the Ticket: {TicketPrice.Amount}");


            // Define the path to save the file (modify the path as needed)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Exports", $"Booking_{booking.BookingId}.txt");

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Write the string to the file
            await System.IO.File.WriteAllTextAsync(filePath, content.ToString());

            // Optionally return the file for download
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "text/plain", Path.GetFileName(filePath));
        }

        // Helper method to generate a unique PNR (for illustration purposes)
        private string GeneratePnr()
        {
            return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }
        private string UniqueUserID()
        {
            return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }
    }


}