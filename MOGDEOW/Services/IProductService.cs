using System.Collections.Generic;
using System.Threading.Tasks;
using MOGDEOW;
using MOGDEOW.Models;

namespace MOGDEOW.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> SearchProductsByNameAsync(string keyword, bool ignoreCase);
        Task<IEnumerable<Product>> GetProductsByTypeAsync(int productTypeId);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Image>> GetImagesByProductIdAsync(int productId);
        Task<IEnumerable<string>> GetImageLinksByProductIdAsync(int productId); 
        Task<IEnumerable<Product>> GetProductsByCategoryNameAsync(string categoryName);
        Task AddProductAsync(Product product);
        Task EditProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task UpdateProductAsync(Product product);
        Task<IEnumerable<Product>> GetProductsForStatsAsync();
        Task<IEnumerable<ProductType>> GetAllProductTypesAsync();
        Task<IEnumerable<Product>> GetTopSellingProductsByDateRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<Product>> GetTopSellingProductsByDateAsync(DateTime selectedDate);

    }
}
