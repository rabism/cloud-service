using user.Models;
using user.Repository;

namespace user.Services
{
    public class UserService : IUserService
    {
        readonly IUserRepository repository;
        public UserService(IUserRepository userRepository)
        {
            repository = userRepository;
        }

        public void Register(UserDetails user)
        {
            repository.AddUser(user);
        }

        public void ChangePassword(UserDetails user)
        {
            var _user = repository.GetUser(user);
            if (_user != null)
            {
                _user.Password = user.Password;
                repository.ChangePassword(_user);
            }
            else
                throw new UserNotFoundException("This user id does not exists");
        }

        public UserDetails Login(string email, string password)
        {
            var _user = repository.GetUserByCredential(email,password);
            if (_user != null)
                return _user;
            else
                throw new UserNotFoundException("Invalid credentials");
        }
        public void AddUser(UserDetails usr)
        {
            if (repository.GetUserByMail(usr.Email) != null)
            {
                throw new UserAlreadyExistsException("User with given email exists");
            }
            repository.AddUser(usr);
        }

    }
}
