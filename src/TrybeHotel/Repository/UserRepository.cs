using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;
        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }
        public UserDto GetUserById(int userId)
        {
            throw new NotImplementedException();
        }

        public UserDto Login(LoginDto login)
        {
            var userFound = _context.Users.FirstOrDefault(u => u.Email == login.Email && u.Password == login.Password);

            if (userFound != null)
            {
                var content = new UserDto
                {
                    UserId = userFound.UserId,
                    Name = userFound.Name,
                    Email = userFound.Email,
                    UserType = userFound.UserType
                };
                return content;
            }

            return null;
        }
        public UserDto Add(UserDtoInsert user)
        {
            var newUser = new User()
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                UserType = user.Email.Contains("admin") ? "admin" : "client",
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            var content = from u in _context.Users
                          where u.Email == newUser.Email
                          select new UserDto
                          {
                              UserId = u.UserId,
                              Name = u.Name,
                              Email = u.Email,
                              UserType = u.UserType
                          };

            return content.Last();
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            var content = from u in _context.Users
                          where u.Email == userEmail
                          select new UserDto
                          {
                              UserId = u.UserId,
                              Name = u.Name,
                              Email = u.Email,
                              UserType = u.UserType
                          };
            if (content.Count() > 0)
            {
                return content.First();
            }
            return null;

        }

        public IEnumerable<UserDto> GetUsers()
        {
            var content = from u in _context.Users
                          select new UserDto
                          {
                              UserId = u.UserId,
                              Name = u.Name,
                              Email = u.Email,
                              UserType = u.UserType
                          };
            return content;
        }

    }
}