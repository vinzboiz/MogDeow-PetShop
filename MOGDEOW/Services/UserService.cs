using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MOGDEOW.Data;
using MOGDEOW.Models;

namespace MOGDEOW.Services
{
    public class UserService : IUserService
    {
        private readonly MogdeowContext _context;
        public UserService(MogdeowContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(string username, string email, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return null; 

            string hashedPassword = HashPassword(password);

            var newUser = new User
            {
                UserName = username,
                Email = email,
                Password = hashedPassword,
                Role = "Customer"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            Console.WriteLine($"Kiểm tra User tồn tại: {email}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                Console.WriteLine("Không tìm thấy email.");
                return null;
            }

            if (!VerifyPassword(password, user.Password))
            {
                Console.WriteLine("Mật khẩu không đúng.");
                return null;
            }

            Console.WriteLine($"Đăng nhập thành công: {user.UserName}");
            return user;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> UpdateUserAsync(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.UserName = updatedUser.UserName ?? user.UserName;
            user.Email = updatedUser.Email ?? user.Email;
            user.Address = updatedUser.Address ?? user.Address;
            user.PhoneNumber = updatedUser.PhoneNumber ?? user.PhoneNumber;
            user.Role = updatedUser.Role ?? user.Role;

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdatePasswordAsync(int id, string newPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.Password = HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        public async Task<bool> VerifyPasswordAsync(int id, string currentPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            return HashPassword(currentPassword) == user.Password;
        }

    }
}
