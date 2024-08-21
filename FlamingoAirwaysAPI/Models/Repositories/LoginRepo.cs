using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

namespace FlamingoAirwaysAPI.Models.Repositories
{
    
        public interface ILogin
        {
            bool ValidateUser(string uname, string pwd,string role);
        }

    
        public class LoginRepo : ILogin
        {

        FlamingoAirwaysDbContext _context;
        public LoginRepo(FlamingoAirwaysDbContext context)
        {
            _context = context;
        }
            public bool ValidateUser(string uname, string pwd,string role)
            {
            //throw new NotImplementedException();
            User user = _context.Users.Where(u => u.Email == uname).Single();
           
                if (user.Password == pwd)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            

            }
        }
    
}
