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
            throw new NotImplementedException();
        }
        public UserDto Add(UserDtoInsert user)
        {
            var newUser = new User()
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                UserType = "client",
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            var content = from userInDb in _context.Users
                          where userInDb.Email == userInDb.Email
                          select new UserDto
                          {
                              userId = userInDb.UserId,
                              Name = userInDb.Name,
                              Email = userInDb.Email,
                              userType = userInDb.UserType
                          };

            return content.Last();
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            var content = from u in _context.Users
                          where u.Email == userEmail
                          select new UserDto
                          {
                              userId = u.UserId,
                              Name = u.Name,
                              Email = u.Email,
                              userType = u.UserType
                          };
            return content.Last();
        }

        public IEnumerable<UserDto> GetUsers()
        {
            throw new NotImplementedException();
        }

    }
}