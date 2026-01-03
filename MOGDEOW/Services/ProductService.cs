using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MOGDEOW.Data;
using MOGDEOW.Models;

namespace MOGDEOW.Services
{
    public class ProductService : IProductService
    {
        private readonly MogdeowContext _context;

        public ProductService(MogdeowContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            string baseUrl = "http://localhost:5216";

            return await _context.Products
                .Include(p => p.Images)
                .Select(p => new Product
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductImage = p.ProductImage,
                    ProductDetail = p.ProductDetail,
                    Quantity = p.Quantity,
                    DVT = p.DVT,
                    ProductTypeID = p.ProductTypeID,
                    ProductType = p.ProductType,
                    Images = p.Images.Any()
                        ? new List<Image> { new Image { LinkImage = $"{baseUrl}/{p.Images.First().LinkImage}" } }
                        : new List<Image>()
                })
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Images)
                .Include(p => p.ProductType)
                .Include(p => p.Favorites)
                .FirstOrDefaultAsync(p => p.ProductID == id);
        }

        public async Task<IEnumerable<Product>> SearchProductsByNameAsync(string keyword, bool ignoreCase)
        {
            return await _context.Products
                .Include(p => p.Images) 
                .Include(p => p.ProductType) 
                .Where(p => EF.Functions.Like(p.ProductName, $"%{keyword}%"))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByTypeAsync(int productTypeId)
        {
            return await _context.Products
                .Where(p => p.ProductTypeID == productTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Where(p => p.ProductPrice >= minPrice && p.ProductPrice <= maxPrice)
                .ToListAsync();
        }

        public async Task<IEnumerable<Image>> GetImagesByProductIdAsync(int productId)
        {
            return await _context.Images
                .Where(img => img.ProductID == productId)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetImageLinksByProductIdAsync(int productId)
        {
            return await _context.Images
                .Where(img => img.ProductID == productId)
                .Select(img => img.LinkImage)
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryNameAsync(string categoryName)
        {
            return await _context.Products
                .Include(p => p.Images) 
                .Include(p => p.ProductType)
                .Where(p => p.ProductType.ProductTypeName.ToLower() == categoryName.ToLower())
                .ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task EditProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsForStatsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.BillDetails)
                .Include(p => p.Images)
                .ToListAsync();
        }
        public async Task<IEnumerable<ProductType>> GetAllProductTypesAsync()
        {
            return await _context.ProductTypes.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetTopSellingProductsByDateRangeAsync(DateTime from, DateTime to)
        {
            var billDetailsInRange = await _context.BillDetails
                .Include(b => b.Product)
                    .ThenInclude(p => p.ProductType)
                .Include(b => b.Bill)
                .Where(b => b.Bill.Date >= from && b.Bill.Date <= to)
                .ToListAsync();

            var grouped = billDetailsInRange
                .GroupBy(b => b.Product)
                .Select(g => new
                {
                    Product = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Select(x =>
                {
                    x.Product.BillDetails = new List<BillDetail> { new BillDetail { Quantity = x.TotalSold } };
                    return x.Product;
                });

            return grouped.ToList();
        }
        public async Task<IEnumerable<Product>> GetTopSellingProductsByDateAsync(DateTime selectedDate)
        {
            return await _context.Products
                .Where(p => p.BillDetails.Any(b => b.Bill.Date.Date == selectedDate))
                .ToListAsync();
        }
    }
}
