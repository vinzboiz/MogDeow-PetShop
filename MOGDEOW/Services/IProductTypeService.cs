using System.Collections.Generic;
using System.Threading.Tasks;
using MOGDEOW.Models;
using MOGDEOW.Data;

namespace MOGDEOW.Services
{
    public interface IProductTypeService
    {
        Task<IEnumerable<ProductType>> GetAllProductTypesAsync();
        Task<ProductType> GetProductTypeByIdAsync(int id);
        Task<IEnumerable<ProductType>> SearchProductTypeByNameAsync(string keyword, bool ignoreCase);
        Task<IEnumerable<ProductType>> GetProductTypesByAllocateAsync(string allocate);
        Task<ProductType> GetProductTypeByNameAsync(string name);

    }
}
