using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

namespace FlamingoAirwaysAPI.Models.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserById(string id);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserByEmailAsync(string email);
        Task AddUser(User user);
        Task UpdateUser(User user);

        public User? ValidateUser(string email, string password);

        public bool CheckMail(string email);
    }
}
