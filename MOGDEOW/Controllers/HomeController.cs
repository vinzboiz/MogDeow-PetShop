using Microsoft.AspNetCore.Mvc;
using MOGDEOW.Attributes;
using MOGDEOW.Models;
using MOGDEOW.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MOGDEOW.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var dogProducts = await GetRandomProducts("dog", 8);
            var catProducts = await GetRandomProducts("cat", 8);
            var petLoverProducts = await GetRandomProducts("accessories", 8);

            SetSingleImage(dogProducts, "dog");
            SetSingleImage(catProducts, "cat");
            SetSingleImage(petLoverProducts, "accessories");


            ViewBag.DogProducts = dogProducts;
            ViewBag.CatProducts = catProducts;
            ViewBag.PetLoverProducts = petLoverProducts;

            return View();
        }

        private async Task<IEnumerable<Product>> GetRandomProducts(string category, int count)
        {
            var products = await _productService.GetProductsByCategoryNameAsync(category);

            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.ProductImage))
                {
                    product.ProductImage = "/Assets/Img/Home/Category/default-category.png";
                }
                else
                {
                    product.ProductImage = "/" + product.ProductImage.TrimStart('/');
                }
                Console.WriteLine($"Product: {product.ProductName}, Image: {product.ProductImage}");
            }

            return products.OrderBy(p => Guid.NewGuid()).Take(count);
        }

        private void SetSingleImage(IEnumerable<Product> products, string category)
        {
            foreach (var product in products)
            {
                if (category.ToLower() == "accessories")
                {
                    // Xử lý 1 ảnh
                    product.ProductImage = !string.IsNullOrEmpty(product.ProductImage)
                        ? "/" + product.ProductImage.TrimStart('/')
                        : "/default-image.jpg";
                }
                else
                {
                    product.ProductImage = product.Images != null && product.Images.Any()
                        ? product.Images.First().LinkImage
                        : "/default-image.jpg";
                }
            }
        }

    }
}
