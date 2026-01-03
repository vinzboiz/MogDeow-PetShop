using Microsoft.EntityFrameworkCore;
using MOGDEOW.Data;
using MOGDEOW.Models;

namespace MOGDEOW.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly MogdeowContext _context;

        public FavoriteService(MogdeowContext context)
        {
            _context = context;
        }

        public async Task<bool> RemoveFavoriteAsync(int userId, int productId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserID == userId && f.ProductID == productId);

            if (favorite == null) return false;

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message, int TotalFavorite)> AddFavoriteAsync(int userId, int productId)
        {
            var exists = await _context.Favorites
                .AnyAsync(f => f.UserID == userId && f.ProductID == productId);

            if (exists)
                return (false, "Sản phẩm đã có trong mục yêu thích.", 0);

            _context.Favorites.Add(new Favorite
            {
                UserID = userId,
                ProductID = productId
            });

            await _context.SaveChangesAsync();

            int total = await _context.Favorites.CountAsync(f => f.UserID == userId);
            return (true, "", total);
        }

        public async Task<(bool IsFavorite, string Message)> ToggleFavoriteAsync(int userId, int productId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserID == userId && f.ProductID == productId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                return (false, "Đã xóa khỏi yêu thích.");
            }
            else
            {
                _context.Favorites.Add(new Favorite
                {
                    UserID = userId,
                    ProductID = productId
                });
                await _context.SaveChangesAsync();
                return (true, "✔ Đã thêm vào yêu thích");
            }
        }

        public async Task<List<Product>> GetFavoriteProductsByUserAsync(int userId)
        {
            return await _context.Favorites
                .Where(f => f.UserID == userId)
                .Select(f => f.Product)
                .ToListAsync();
        }

    }
}
