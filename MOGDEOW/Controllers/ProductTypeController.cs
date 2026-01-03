using Microsoft.AspNetCore.Mvc;
using MOGDEOW.Services;
using MOGDEOW.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using MOGDEOW.Models;

namespace MOGDEOW.Controllers
{
    [Route("api/product-types")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;

        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetAllProductTypes()
        {
            var productTypes = await _productTypeService.GetAllProductTypesAsync();
            return Ok(productTypes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductType>> GetProductTypeById(int id)
        {
            var productType = await _productTypeService.GetProductTypeByIdAsync(id);
            if (productType == null)
                return NotFound("Loại sản phẩm không tồn tại.");
            return Ok(productType);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductType>>> SearchProductTypeByName([FromQuery] string keyword, [FromQuery] bool ignoreCase = true)
        {
            var productTypes = await _productTypeService.SearchProductTypeByNameAsync(keyword, ignoreCase);
            return Ok(productTypes);
        }

        [HttpGet("allocate")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductTypesByAllocate([FromQuery] string allocate)
        {
            var productTypes = await _productTypeService.GetProductTypesByAllocateAsync(allocate);
            return Ok(productTypes);
        }
    }
}
