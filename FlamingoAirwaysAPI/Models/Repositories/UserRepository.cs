using FlamingoAirwaysAPI.Models.Interfaces;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;
using Microsoft.EntityFrameworkCore;

namespace FlamingoAirwaysAPI.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        FlamingoAirwaysDbContext _context;    //contains the dbset prop for each entity in your model
        public UserRepository(FlamingoAirwaysDbContext context) //user repository has a dependency on FlamingoAirwaysDbContext
        {
            _context = context;                      //_context is used throughout the UserRepository to interact with the database
        }

        public async Task AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); //commits this change to the database
        }

        public async Task<IEnumerable<User>> GetAllUsers() //Async way to retreive all users by not blocking threads during database execution
        {
            return await _context.Users
                .Where(u => u.Role=="User")
                .ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email); //Finds the user with a specific email address

            //the _context translates this LINQ expression into a sql query sends it to the database and returns the result as entity object
        }

        public async Task<User> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }
        
        public async Task UpdateUser(User user)
        {
            //throw new NotImplementedException();
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public User? ValidateUser(string email, string password)
        {
            //throw new NotImplementedException();
            User user = _context.Users.SingleOrDefault(u => u.Email == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }
            return null;
        }

        public bool CheckMail(string email)
        {
            User myUser = _context.Users.SingleOrDefault(u => u.Email == email);
            if(myUser != null)
                return true;
            return false;
            
        }
    }
}
