using FlamingoAirwaysAPI.Models.Interfaces;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;
using static FlamingoAirwaysAPI.Models.FlamingoAirwaysDbContext;
using Microsoft.EntityFrameworkCore;

namespace FlamingoAirwaysAPI.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        FlamingoAirwaysDbContext _context;
        public UserRepository(FlamingoAirwaysDbContext context)
        {
            _context = context; 
        }

        public async Task AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task RemoveUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateUser(User user)
        {
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
    }
}
