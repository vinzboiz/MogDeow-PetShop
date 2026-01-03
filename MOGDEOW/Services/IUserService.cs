using MOGDEOW.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOGDEOW.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(string username, string email, string password);
        Task<User> AuthenticateAsync(string email, string password);
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> UpdateUserAsync(int id, User user);
        Task<User> UpdatePasswordAsync(int id, string newPassword);
        Task<bool> VerifyPasswordAsync(int id, string currentPassword);
        Task<bool> DeleteUserAsync(int id);
    }
}
