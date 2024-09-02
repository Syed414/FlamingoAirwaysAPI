using Microsoft.EntityFrameworkCore;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

namespace FlamingoAirwaysAPI.Models
{
    public class FlamingoAirwaysDbContext : DbContext //DbContext is responsible for interacting with the database 
    {
        public FlamingoAirwaysDbContext(DbContextOptions<FlamingoAirwaysDbContext> options) : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
    }
}
