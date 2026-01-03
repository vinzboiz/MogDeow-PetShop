using MOGDEOW.Models;

namespace MOGDEOW.Services
{
    public interface IFavoriteService
    {
        Task<bool> RemoveFavoriteAsync(int userId, int productId);
        Task<(bool Success, string Message, int TotalFavorite)> AddFavoriteAsync(int userId, int productId);
        Task<(bool IsFavorite, string Message)> ToggleFavoriteAsync(int userId, int productId);
        Task<List<Product>> GetFavoriteProductsByUserAsync(int userId);
    }
}
