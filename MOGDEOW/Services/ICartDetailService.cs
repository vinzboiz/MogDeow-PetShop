using MOGDEOW.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MOGDEOW.DTOs;

namespace MOGDEOW.Services
{
    public interface ICartDetailService
    {
        Task<IEnumerable<CartDetailDTO>> GetCartDetailsByUserIdAsync(int userId);
        Task<CartDetail> AddToCartAsync(int userId, int productId, int quantity);
        Task<bool> RemoveFromCartAsync(int userId, int productId);
        Task<bool> UpdateQuantityAsync(int userId, int productId, int quantity);
        Task<Product> GetProductByIdAsync(int productId);
        Task<bool> ClearCartAsync(int userId);
        Task<bool> ClearCartAsync(int userId, List<int> productIds);

    }
}
