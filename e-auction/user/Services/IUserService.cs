using user.Models;

namespace user.Services
{
    public interface IUserService
    {
        void ChangePassword(UserDetails user);
        UserDetails Login(string email, string password);
        void Register(UserDetails user);
        void AddUser(UserDetails usr);
    }
}