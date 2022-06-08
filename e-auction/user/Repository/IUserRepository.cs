using user.Models;

namespace user.Repository
{
    public interface IUserRepository
    {
        void AddUser(UserDetails user);
        void ChangePassword(UserDetails user);
        UserDetails GetUser(UserDetails user);
        UserDetails GetUserByMail(string email);
        UserDetails GetUserByCredential(string email, string password);
    }
}