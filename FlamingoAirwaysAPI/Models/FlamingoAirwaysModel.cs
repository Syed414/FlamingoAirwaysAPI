using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlamingoAirwaysAPI.Models
{
    public class FlamingoAirwaysModel
    {

        public class Login
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        [Table("Users")]
        public class User
        {
            [Key]

            public string UserId { get; set; }

            [Required]
            [StringLength(50)]
            [MinLength(2)]
            public string FirstName { get; set; }

            [Required]
            [StringLength(50)]
            [MinLength(2)]
            public string LastName { get; set; }

            [Required]
            [DataType(DataType.EmailAddress)]
            [StringLength(100)]
            public string Email { get; set; }

            [Required]
            [StringLength(64)] // Assuming a fixed length for the hash
            public string Password { get; set; }

            public string PhoneNo { get; set; }

            [Required]
            [StringLength(20)]
            public string Role { get; set; } // Admin or Customer


        }

        public class UpdateUser
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string PhoneNo { get; set; }
        }

        [Table("Flights")]
        public class Flight
        {
            [Key]

            public int FlightId { get; set; }

            [Required]
            [StringLength(100)]
            public string Origin { get; set; }

            [Required]
            [StringLength(100)]
            public string Destination { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            public DateTime DepartureDate { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            public DateTime ArrivalDate { get; set; }

            [Required]
            [DataType(DataType.Currency)]
            [Column(TypeName = "decimal(18, 2)")] // Adjust based on your needs
            public decimal Price { get; set; }

            [Required]
            [Range(0, int.MaxValue)]
            public int TotalNumberOfSeats { get; set; }

            [Required]
            [Range(0, int.MaxValue)]
            public int AvailableSeats { get; set; }

            [Required]
            [DataType(DataType.Currency)]
            [Column(TypeName = "decimal(18, 2)")]
            public decimal BPrice { get; set; }

            [Required]
            [Range(0, int.MaxValue)]
            public int TotalNumberOfBSeats { get; set; }

            [Required]
            [Range(0, int.MaxValue)]
            public int AvailableBSeats { get; set; }

        }

        [Table("Bookings")]
        public class Booking
        {
            [Key]
            public int BookingId { get; set; }

            [ForeignKey(nameof(Flight))]
            public int FlightIdFK { get; set; }

            [ForeignKey(nameof(User))]
            public string UserIdFK { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            public DateTime BookingDate { get; set; }

            [Required]
            [StringLength(20)]
            public string PNR { get; set; }

            [Required]
            public bool IsCancelled { get; set; }


            public User User { get; set; }

            public Flight Flight { get; set; }


        }

        [Table("Tickets")]
        public class Ticket
        {
            [Key]

            public int TicketId { get; set; }

            [ForeignKey(nameof(Booking))]
            public int BookingIdF { get; set; }

            [Required]
            [StringLength(50)]
            public string SeatNumber { get; set; }

            [Required]
            [StringLength(50)]
            public string PassengerName { get; set; }

            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal Price { get; set; }


            public Booking Booking { get; set; }
        }

        [Table("Payments")]
        public class Payment
        {
            [Key]

            public int PaymentId { get; set; }

            [ForeignKey(nameof(Booking))]
            public int BookingIdFK { get; set; }

            [Required]
            [StringLength(20)]
            public string PaymentType { get; set; } // CreditCard or DebitCard

            [Required]
            [StringLength(16, MinimumLength = 13)]
            [DataType(DataType.CreditCard)]
            public string CardNumber { get; set; }
            [Required]
            [StringLength(16, MinimumLength = 5)]
            public string CardHolderName { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            public DateTime PaymentDate { get; set; }

            [Required]
            [DataType(DataType.Currency)]
            [Column(TypeName = "decimal(18, 2)")] 
            public decimal Amount { get; set; }

            [DataType(DataType.Currency)]
            [Column(TypeName = "decimal(18, 2)")] 
            public decimal Retainer { get; set; }

            [DataType(DataType.Currency)]
            [Column(TypeName = "decimal(18, 2)")]
            public decimal RefundAmount { get; set; }

            public Booking Booking { get; set; }

            //public int CVV { get; set; }

            public class BookingRequest
            {
                public int FlightId { get; set; }
                public int Seats { get; set; }
                public List<string> PassengerNames { get; set; }
                public int BSeats { get; set; }
                public List<string> BPassengerNames { get; set; }
                public int CVV { get; set; }
                public string BankName { get; set; }
                public PaymentRequest Payment { get; set; }

            }

            // DTO for payment details
            public class PaymentRequest
            {
                public string PaymentType { get; set; } // CreditCard or DebitCard
                public string CardNumber { get; set; }
                public string CardHolderName { get; set; }
                //public int CVV { get; set; }

            }

            public class PassengerDetails
            {
                public string PassengerName { get; set; }
                public bool HasLegSpace { get; set; }
                public bool HasLuxury { get; set; }
            }
        }
    }
}
