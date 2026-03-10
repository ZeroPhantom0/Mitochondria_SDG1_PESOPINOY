using PesoPinoy.DAL.Repositories.Base;
using PesoPinoy.Models.Entities;

namespace PesoPinoy.DAL.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
    }
}