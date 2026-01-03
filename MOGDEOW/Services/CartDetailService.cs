using MOGDEOW.Data;
using MOGDEOW.Models;
using Microsoft.EntityFrameworkCore;
using MOGDEOW.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MOGDEOW.Services
{
    public class CartDetailService : ICartDetailService 
    {
        private readonly MogdeowContext _context;

        public CartDetailService(MogdeowContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartDetailDTO>> GetCartDetailsByUserIdAsync(int userId)
        {
            return await _context.CartDetails
               .Where(cd => cd.UserID == userId)
               .Include(cd => cd.Product) 
               .Select(cd => new CartDetailDTO
               {
                   ProductID = cd.ProductID,
                   ProductName = cd.Product.ProductName,
                   ProductPrice = cd.Product.ProductPrice,
                   ProductImage = cd.Product.ProductImage, 
                   Quantity = cd.Quantity
               }).ToListAsync();
        }

        public async Task<CartDetail> AddToCartAsync(int userId, int productId, int quantity)
        {
            var cartItem = await _context.CartDetails
            .FirstOrDefaultAsync(cd => cd.UserID == userId && cd.ProductID == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity; 
            }
            else
            {
                cartItem = new CartDetail
                {
                    UserID = userId,
                    ProductID = productId,
                    Quantity = quantity,
                    Status = "Chưa thanh toán",
                    DateIn = DateTime.Now
                };
                _context.CartDetails.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productId)
        {
            var cartItem = await _context.CartDetails
                .FirstOrDefaultAsync(cd => cd.UserID == userId && cd.ProductID == productId);

            if (cartItem != null)
            {
                _context.CartDetails.Remove(cartItem);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateQuantityAsync(int userId, int productId, int quantity)
        {
            var cartItem = await _context.CartDetails
                .FirstOrDefaultAsync(cd => cd.UserID == userId && cd.ProductID == productId);

            if (cartItem != null)
            {
                if (quantity > 0)
                {
                    cartItem.Quantity = quantity;
                }
                else
                {
                    _context.CartDetails.Remove(cartItem);  
                }

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cartItems = await _context.CartDetails
                .Where(cd => cd.UserID == userId)
                .ToListAsync();

            if (!cartItems.Any()) return false;

            _context.CartDetails.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId, List<int> selectedProductIds)
        {
            var cartItemsToRemove = await _context.CartDetails
                .Where(cd => cd.UserID == userId && selectedProductIds.Contains(cd.ProductID))
                .ToListAsync();

            if (!cartItemsToRemove.Any()) return false;

            _context.CartDetails.RemoveRange(cartItemsToRemove);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId);

            if (product == null)
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm có ID = {productId}");

            return product;
        }
    }
}
