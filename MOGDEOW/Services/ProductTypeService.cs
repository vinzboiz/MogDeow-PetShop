using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MOGDEOW.Models;
using MOGDEOW.Data;

namespace MOGDEOW.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly MogdeowContext _context;

        public ProductTypeService(MogdeowContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductType>> GetAllProductTypesAsync()
        {
            return await _context.ProductTypes.ToListAsync();
        }

        public async Task<ProductType> GetProductTypeByIdAsync(int id)
        {
            return await _context.ProductTypes
                .FirstOrDefaultAsync(pt => pt.ProductTypeID == id);
        }

        public async Task<IEnumerable<ProductType>> SearchProductTypeByNameAsync(string keyword, bool ignoreCase)
        {
            return await _context.ProductTypes
                .Where(pt => ignoreCase
                    ? EF.Functions.Like(pt.ProductTypeName, $"%{keyword}%")
                    : pt.ProductTypeName.Contains(keyword))
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductType>> GetProductTypesByAllocateAsync(string allocate)
        {
            return await _context.ProductTypes
                .Where(pt => pt.Allocate == allocate)
                .ToListAsync();
        }

        public async Task<ProductType> GetProductTypeByNameAsync(string name)
        {
            return await _context.ProductTypes
                .FirstOrDefaultAsync(pt => pt.ProductTypeName.ToLower() == name.ToLower());
        }

    }
}
