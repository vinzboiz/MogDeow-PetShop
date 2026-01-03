using MOGDEOW.Data;
using MOGDEOW.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MOGDEOW.Services
{
    public class RatingService : IRatingService
    {
        private readonly MogdeowContext _context;

        public RatingService(MogdeowContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rating>> GetRatingsByProductIdAsync(int productId)
        {
            return await _context.Ratings
                .Where(r => r.ProductID == productId)
                .Include(r => r.User) 
                .ToListAsync();
        }

        public async Task<Rating> AddRatingAsync(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            return rating;
        }

        public async Task<bool> DeleteRatingAsync(int rateId)
        {
            var rating = await _context.Ratings.FindAsync(rateId);
            if (rating == null) return false;

            _context.Ratings.Remove(rating);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Rating> AddOrUpdateRatingAsync(Rating rating)
        {
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.ProductID == rating.ProductID && r.UserID == rating.UserID);

            if (existingRating != null)
            {
                // Cập nhật đánh giá cũ
                existingRating.Rate = rating.Rate;
                existingRating.Message = rating.Message;
                existingRating.Date = rating.Date;

                _context.Ratings.Update(existingRating);
                await _context.SaveChangesAsync();
                return existingRating;
            }
            else
            {
                // Thêm mới nếu chưa có
                _context.Ratings.Add(rating);
                await _context.SaveChangesAsync();
                return rating;
            }
        }

    }
}
