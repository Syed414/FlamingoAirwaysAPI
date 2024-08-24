﻿using Microsoft.AspNetCore.Mvc;
using FlamingoAirwaysAPI.Models.Interfaces;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel.Payment;
using FlamingoAirwaysAPI.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace Flamingo_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IFlightRepository _flightRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ITicketRepository _ticketRepo;
        private readonly IUserRepository _userRepo;
        private readonly FlamingoAirwaysDbContext _context;
        private readonly IConverter _pdfconverter;
        private readonly PdfService _pdfService;

        public BookingController(IBookingRepository bookingRepo, IFlightRepository flightRepo, IPaymentRepository paymentRepo, ITicketRepository ticketRepo, FlamingoAirwaysDbContext context, IConverter pdfConverter, PdfService pdfService)
        {
            _bookingRepo = bookingRepo;
            _flightRepo = flightRepo;
            _paymentRepo = paymentRepo;
            _ticketRepo = ticketRepo;
            _context = context;
            _pdfconverter = pdfConverter;
            _pdfService = pdfService;
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
            if (int.TryParse(userIDClaim, out int userid))
            {
                var allmy = await _bookingRepo.GetByUserIdAsync(userid);
                return Ok(allmy);
            }
            else
            {
                return BadRequest("Invalid Userid");
            }
        }
   

    // POST api/Booking
    [HttpPost]
        [Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Booking>> PostBooking([FromBody] BookingRequest request)
        {
            
            if (!Regex.IsMatch(request.CVV.ToString() , @"^\d{3}$"))
            {
                return BadRequest("Invalid CVV. It must be 3 digits.");
            }
            if (!Regex.IsMatch(request.Payment.CardNumber, @"^\d{12}$"))
            {
                return BadRequest("Invalid card number. It must be 12 digits.");
            }

            var validBank = new List<string> { "Kotak", "SBI", "Axis", "HDFC" };
            if (!validBank.Contains(request.BankName))
            {
                return BadRequest("Invalid bank name. Please select a valid bank.");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            if (!int.TryParse(userIdClaim.Value, out var UserID))
            {
                return Unauthorized("Invalid User ID format in token.");
            }

            var flight = await _flightRepo.GetFlightById(request.FlightId);

            if (flight == null)
            {
                return NotFound("Flight not found.");
            }

            if (request.Seats > flight.AvailableSeats)
            {
                return BadRequest("Not enough seats available.");
            }

            // Create booking
            var booking = new Booking
            {
                FlightIdFK = request.FlightId,
                UserIdFK = UserID, // Assuming user ID is provided in the request
                BookingDate = DateTime.UtcNow,
                PNR = GeneratePnr(), 
                IsCancelled = false
            };

            await _bookingRepo.AddAsync(booking);

            // Process payment
            var payment = new Payment
            {
                BookingIdFK = booking.BookingId,
                PaymentType = request.Payment.PaymentType,
                CardNumber = request.Payment.CardNumber,
                CardHolderName = request.Payment.CardHolderName,
                PaymentDate = DateTime.UtcNow,
                Amount = flight.Price * request.Seats
            };

            await _paymentRepo.AddAsync(payment);

            // Create tickets
            for (int i = 0; i < request.Seats; i++)
            {
                var ticket = new Ticket
                {
                    BookingIdF = booking.BookingId,
                    SeatNumber = $"Seat-{flight.AvailableSeats-i}", // Generate seat number
                    PassengerName = request.PassengerNames[i],
                    Price = flight.Price
                };

                await _ticketRepo.AddAsync(ticket);

                
                
                
            }
            //decrease seat from flight 
            flight.AvailableSeats = flight.AvailableSeats - request.Seats;
            await _flightRepo.UpdateAsync(flight);
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
                payment.Retainer += payment.Amount * 0.5m;
                payment.Amount = 0;

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

           //payment update
            var payment = await _paymentRepo.getByBookingIdAsync(bookingId);
            payment.Amount -= ticket.Price;
            payment.Retainer += ticket.Price * 0.5m;


            // Delete the specific ticket
            await _ticketRepo.DeleteAsync(ticket.TicketId);

            //update the number of seats in flight
            var flight = await _flightRepo.GetByBookingIdAsync(bookingId);
            flight.AvailableSeats += 1;
            await _flightRepo.UpdateAsync(flight);

            // Check if there are any remaining tickets for this booking
            var remainingTickets = await _ticketRepo.GetByBookingIdAsync(bookingId);

            if (remainingTickets == null || !remainingTickets.Any())
            {
                // If no tickets remain, cancel the booking
                await _bookingRepo.CancelAsync(bookingId);
            }

            return NoContent();
        }

        [HttpGet("{bookingId}/ticket/{ticketId}/download")]
        public async Task<IActionResult> DownloadTicket(int bookingId, int ticketId)
        {
            var ticket = await _ticketRepo.GetByBookingIdAndTicketIdAsync(bookingId, ticketId);

            if (ticket == null)
            {
                return NotFound("Ticket not found....");
            }

            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return NotFound("Booking not found...");
            }

            // Step 3: Create the HTML content for the PDF
            var htmlContent = $@"
        <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    .ticket-details {{ margin: 20px; }}
                    h1 {{ color: #333; }}
                    p {{ font-size: 14px; color: #555; }}
                </style>
            </head>
            <body>
                <div class='ticket-details'>
                    <h1>Flamingo Airways - Ticket</h1>
                    <p><strong>Booking ID:</strong> {booking.BookingId}</p>
                    <p><strong>Ticket ID:</strong> {ticket.TicketId}</p>
                    <p><strong>Flight ID:</strong> {booking.FlightIdFK}</p>
                    <p><strong>Passenger Name:</strong> {ticket.PassengerName}</p>
                    <p><strong>Seat Number:</strong> {ticket.SeatNumber}</p>
                    <p><strong>PNR:</strong> {booking.PNR}</p>
                    
                </div>
            </body>
        </html>";

            // Step 4: Generate the PDF using the IConverter service
            var pdfBytes = _pdfService.GeneratePdf(htmlContent);

            // Step 5: Return the PDF as a file download
            return File(pdfBytes, "application/pdf", $"Ticket_{ticketId}.pdf");
        }

        

        // Helper method to generate a unique PNR (for illustration purposes)
        private string GeneratePnr()
        {
            return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }
    }

    
}