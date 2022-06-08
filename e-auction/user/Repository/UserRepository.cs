using Microsoft.EntityFrameworkCore;
using System.Linq;
using user.Models;

namespace user.Repository
{
    public class UserRepository : IUserRepository
    {
        readonly UserDbContext context;
        public UserRepository(UserDbContext dbContext)
        {
            context = dbContext;
        }

        public void AddUser(UserDetails user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }
        public UserDetails GetUser(UserDetails user)
        {
            return context.Users.FirstOrDefault(u => u.Email == user.Email);
        }

        public void ChangePassword(UserDetails user)
        {
            context.Entry<UserDetails>(user).State = EntityState.Modified;
            context.SaveChanges();
        }
        public UserDetails GetUserByMail(string email)
        {
            return context.Users.Find(email);
        }

        public UserDetails GetUserByCredential(string email, string password){
            return context.Users.FirstOrDefault(u => u.Email == email && u.Password==password);
         }

    }
}
