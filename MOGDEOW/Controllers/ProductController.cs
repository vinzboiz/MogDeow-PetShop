using Microsoft.AspNetCore.Mvc;
using MOGDEOW.Services;
using MOGDEOW.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MOGDEOW.Models;
using MOGDEOW.Attributes;

namespace MOGDEOW.Controllers
{
    [Route("products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IRatingService _ratingService;
        private readonly IProductTypeService _productTypeService;

        public ProductController(IProductService productService, IRatingService ratingService, IProductTypeService productTypeService)
        {
            _productService = productService;
            _ratingService = ratingService;
            _productTypeService = productTypeService;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index(string search = "", string category = "all", decimal? minPrice = null, decimal? maxPrice = null, string sortOrder = "none")
        {
            var products = await _productService.GetAllProductsAsync();
            if (!string.IsNullOrEmpty(search))
            {
                products = await _productService.SearchProductsByNameAsync(search, true);
                ViewBag.SearchTerm = search;
            }
            else if (!string.IsNullOrEmpty(category) && category.ToLower() != "all")
            {
                products = await _productService.GetProductsByCategoryNameAsync(category);
            }
            else
            {
                products = await _productService.GetAllProductsAsync();
            }
            if (category != "all")
            {
                products = products
                    .Where(p => p.ProductType.ProductTypeName.ToLower() == category.ToLower())
                    .ToList();
            }
            if (!string.IsNullOrEmpty(sortOrder) && sortOrder.StartsWith("range-"))
            {
                var parts = sortOrder.Replace("range-", "").Split('-');
                if (parts.Length == 2 && int.TryParse(parts[0], out int min) && int.TryParse(parts[1], out int max))
                {
                    minPrice = min;
                    maxPrice = max;
                    products = products.Where(p => p.ProductPrice >= min && p.ProductPrice <= max).ToList();
                }
            }
            else
            {
                switch (sortOrder)
                {
                    case "asc":
                        products = products.OrderBy(p => p.ProductPrice).ToList();
                        break;
                    case "desc":
                        products = products.OrderByDescending(p => p.ProductPrice).ToList();
                        break;
                }
            }
            ViewData["SelectedCategory"] = category;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortOrder = sortOrder;

            return View(products);
        }

        [HttpGet("productDetail/{id}")]
        public async Task<IActionResult> ProductDetail(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound("Sản phẩm không tồn tại.");
            ViewBag.Quantity = HttpContext.Session.GetInt32($"Quantity_{id}") ?? 1;
            product.Ratings = (await _ratingService.GetRatingsByProductIdAsync(id)).ToList();
            return View(product);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, string quantityChange)
        {
            int currentQuantity = HttpContext.Session.GetInt32($"Quantity_{productId}") ?? 1;

            var product = _productService.GetProductByIdAsync(productId).Result;
            if (product == null || product.Quantity <= 0)
            {
                TempData["Error"] = "Sản phẩm không còn hàng.";
                return RedirectToAction("ProductDetail", new { id = productId });
            }

            if (quantityChange == "1" && currentQuantity < product.Quantity)
                currentQuantity++;
            else if (quantityChange == "-1" && currentQuantity > 1)
                currentQuantity--;

            if (currentQuantity == product.Quantity)
                TempData["Warning"] = "Bạn đã chọn số lượng tối đa.";

            HttpContext.Session.SetInt32($"Quantity_{productId}", currentQuantity);
            return RedirectToAction("ProductDetail", new { id = productId });
        }

        [HttpGet("checkout")]
        public IActionResult CheckOut()
        {
            return View();
        }

        [HttpGet("cart")]
        public IActionResult Cart()
        {
            return View();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            if (!products.Any())
                return NotFound("Không có sản phẩm nào trong cơ sở dữ liệu.");

            return Ok(FormatProductData(products));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Sản phẩm không tồn tại.");

            return Ok(FormatProductData(new List<Product> { product }).First());
        }
        
        [HttpGet("search")]
        public async Task<IActionResult> SearchProductsByName([FromQuery] string keyword)
        {
            var products = await _productService.SearchProductsByNameAsync(keyword, ignoreCase: true);
            return Ok(FormatProductData(products));
        }

        [HttpGet("category/{categoryName}")]
        public async Task<IActionResult> GetProductsByCategory(string categoryName)
        {
            var products = await _productService.GetAllProductsAsync();
            products = products
                .Where(p => p.ProductType.ProductTypeName.ToLower() == categoryName.ToLower())
                .ToList();

            return Ok(FormatProductData(products));
        }

        [HttpGet("price-range")]
        public async Task<IActionResult> GetProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var products = await _productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
            return Ok(FormatProductData(products));
        }

        private List<object> FormatProductData(IEnumerable<Product> products)
        {
            return products.Select(product => (object)new
            {
                product.ProductID,
                product.ProductName,
                product.ProductPrice,
                product.ProductDetail,
                product.Quantity,
                product.DVT,
                ProductType = new
                {
                    product.ProductType.ProductTypeID,
                    product.ProductType.ProductTypeName
                },
                Images = product.Images?.Select(img =>
                    img.LinkImage.StartsWith("http") ? img.LinkImage : $"http://localhost:5216/{img.LinkImage}"
                ).ToList() ?? new List<string>()
            }).ToList();
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSearchSuggestions([FromQuery] string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return Ok(new List<string>());
            }

            var products = await _productService.SearchProductsByNameAsync(keyword, true);
            var suggestions = products
                .Select(p => p.ProductName)
                .Distinct()
                .Take(7)
                .ToList();

            return Ok(suggestions);
        }

        [AuthorizeRole("Admin", "Staff")]
        [HttpGet("manage")]
        public async Task<IActionResult> Manage(string search = "", string productType = "")
        {
            var products = await _productService.GetAllProductsAsync();
            if (!string.IsNullOrEmpty(search))
            {
                products = products
                    .Where(p => p.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                ViewBag.Search = search;
            }
            if (!string.IsNullOrEmpty(productType))
            {
                products = products
                    .Where(p => p.ProductType?.ProductTypeName == productType)
                    .ToList();
                ViewBag.ProductType = productType;
            }
            var allTypes = (await _productService.GetAllProductsAsync())
                .Select(p => p.ProductType)
                .Where(pt => pt != null)
                .Distinct()
                .ToList();
            ViewBag.ProductTypes = allTypes;

            return View("~/Views/ProductManage/Index.cshtml", products);
        }

        [AuthorizeRole("Admin")]
        [HttpGet("add")]
        public async Task<IActionResult> Add()
        {
            var productTypes = await _productTypeService.GetAllProductTypesAsync();
            ViewBag.ProductTypes = productTypes;
            return View("~/Views/ProductManage/Add.cshtml");
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(Product product)
        {
            ModelState.Remove("ProductImage");
            ModelState.Remove("ProductTypeID");
            ModelState.Remove("ProductType");
            if (!ModelState.IsValid || product.Upload == null || string.IsNullOrEmpty(product.SelectedTypeName))
            {
                Console.WriteLine("ModelState Invalid:");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                foreach (var err in errors)
                {
                    Console.WriteLine(" - " + err);
                }
                ViewBag.ProductTypes = await _productTypeService.GetAllProductTypesAsync();
                return View("~/Views/ProductManage/Add.cshtml", product);
            }
            var selectedType = await _productTypeService.GetProductTypeByNameAsync(product.SelectedTypeName);
            if (selectedType == null)
            {
                ModelState.AddModelError("", "Danh mục không hợp lệ.");
                ViewBag.ProductTypes = await _productTypeService.GetAllProductTypesAsync();
                return View("~/Views/ProductManage/Add.cshtml", product);
            }
            string subFolder = Path.Combine("wwwroot", "src", "Assets", "Img", "Product", selectedType.ProductTypeName);
            if (!Directory.Exists(subFolder))
            {
                Directory.CreateDirectory(subFolder);
                Console.WriteLine("Đã tạo thư mục: " + subFolder);
            }
            string fileName = Path.GetFileName(product.Upload.FileName);
            string filePath = Path.Combine(subFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await product.Upload.CopyToAsync(stream);
                Console.WriteLine("Đã lưu ảnh tại: " + filePath);
            }

            product.ProductImage = $"/src/Assets/Img/Product/{selectedType.ProductTypeName}/{fileName}";
            product.ProductTypeID = selectedType.ProductTypeID;
            try
            {
                await _productService.AddProductAsync(product);
                Console.WriteLine("Đã thêm sản phẩm thành công.");
                TempData["Success"] = "Đã thêm sản phẩm thành công!";
                return RedirectToAction("Manage");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu vào CSDL: " + ex.Message);
                ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu sản phẩm.");
                ViewBag.ProductTypes = await _productTypeService.GetAllProductTypesAsync();
                return View("~/Views/ProductManage/Add.cshtml", product);
            }
        }

        [AuthorizeRole("Admin")]
        [HttpGet("edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Manage");
            }
            product.SelectedTypeName = product.ProductType?.ProductTypeName;
            ViewBag.ProductTypes = await _productTypeService.GetAllProductTypesAsync();
            return View("~/Views/ProductManage/Edit.cshtml", product);
        }
        [AuthorizeRole("Admin")]
        [HttpPost("edit")]
        public async Task<IActionResult> Edit(Product product)
        {
            ModelState.Remove("ProductImage");
            ModelState.Remove("ProductTypeID");
            ModelState.Remove("ProductType");
            ModelState.Remove("Upload"); 
            if (!ModelState.IsValid || string.IsNullOrEmpty(product.SelectedTypeName))
            {
                Console.WriteLine("ModelState Invalid:");
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();
                foreach (var err in errors)
                {
                    Console.WriteLine(" - " + err);
                }

                ViewBag.ProductTypes = await _productTypeService.GetAllProductTypesAsync();
                return View("~/Views/ProductManage/Edit.cshtml", product);
            }
            var selectedType = await _productTypeService.GetProductTypeByNameAsync(product.SelectedTypeName);
            if (selectedType == null)
            {
                ModelState.AddModelError("", "Danh mục không hợp lệ.");
                ViewBag.ProductTypes = await _productTypeService.GetAllProductTypesAsync();
                return View("~/Views/ProductManage/Edit.cshtml", product);
            }

            if (product.Upload != null)
            {
                string subFolder = Path.Combine("wwwroot", "src", "Assets", "Img", "Product", selectedType.ProductTypeName);
                if (!Directory.Exists(subFolder))
                {
                    Directory.CreateDirectory(subFolder);
                    Console.WriteLine("Đã tạo thư mục: " + subFolder);
                }

                string fileName = Path.GetFileName(product.Upload.FileName);
                string filePath = Path.Combine(subFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await product.Upload.CopyToAsync(stream);
                    Console.WriteLine("Đã lưu ảnh tại: " + filePath);
                }

                product.ProductImage = $"/src/Assets/Img/Product/{selectedType.ProductTypeName}/{fileName}";
            }

            product.ProductTypeID = selectedType.ProductTypeID;

            try
            {
                await _productService.UpdateProductAsync(product);
                Console.WriteLine("Đã cập nhật sản phẩm thành công.");
                TempData["Success"] = "Đã cập nhật sản phẩm thành công!";
                return RedirectToAction("Manage", "Product");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật sản phẩm: " + ex.Message);
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật sản phẩm.");
                ViewBag.ProductTypes = await _productTypeService.GetAllProductTypesAsync();
                return View("~/Views/ProductManage/Edit.cshtml", product);
            }
        }

        [AuthorizeRole("Admin")]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Manage", "Product");
            }

            await _productService.DeleteProductAsync(id);
            TempData["Success"] = "Đã xóa sản phẩm!";
            return RedirectToAction("Manage", "Product");
        }
    }
}
