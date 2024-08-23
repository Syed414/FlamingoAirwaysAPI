//using static FlamingoAirwaysAPI.Models.FlamingoAirwaysModel;

//namespace FlamingoAirwaysAPI.Models.Repositories
//{
    
//        public interface ILogin
//        {
//            ValidateUser(string uname, string pwd,string role);
//            int GetUserId(string email);

//        }

    
//        public class LoginRepo : ILogin
//        {

//        FlamingoAirwaysDbContext _context;
//        public LoginRepo(FlamingoAirwaysDbContext context)
//        {
//            _context = context;
//        }

//        public int GetUserId(string email)
//        {
//            //throw new NotImplementedException();
//            var user = _context.Users.FirstOrDefault(u => u.Email == email);
//            return user?.UserId ?? 0;
//        }

//        public User ValidateUser(string uname, string pwd,string role)
//            {
//            //throw new NotImplementedException();
//            User user = _context.Users.Where(u => u.Email == uname).Single();
//            if (user != null && BCrypt.Net.BCrypt.Verify(pwd, user.Password))
//            {
//                return user;
//            }
               
//            return null;

//            }
//        }
    
//}
