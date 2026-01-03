using MOGDEOW.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOGDEOW.Services
{
    public interface IRatingService
    {
        Task<IEnumerable<Rating>> GetRatingsByProductIdAsync(int productId);
        Task<Rating> AddRatingAsync(Rating rating);
        Task<bool> DeleteRatingAsync(int rateId);
        Task<Rating> AddOrUpdateRatingAsync(Rating rating);

    }
}
