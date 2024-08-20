using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

namespace FlamingoAirwaysAPI.Models.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserByEmailAsync(string email);
        Task AddUser(User user);
        Task UpdateUser(User user);
        Task RemoveUser(int id);
    }
}
