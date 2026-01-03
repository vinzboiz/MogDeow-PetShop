using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOGDEOW.Extensions;
using MOGDEOW.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MOGDEOW.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartDetailService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartDetailService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }
        
        // Thêm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                TempData["CartMessage"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!";
                return RedirectToAction("Login", "User");
            }

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Quantity <= 0)
            {
                TempData["CartMessage"] = "Sản phẩm không còn hàng.";
                return RedirectToAction("Cart");
            }

            quantity = Math.Min(quantity, product.Quantity);
            await _cartService.AddToCartAsync(userId.Value, productId, quantity);

            var cartItems = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
            HttpContext.Session.SetObjectAsJson("Cart", cartItems);

            TempData["CartMessage"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            return RedirectToAction("Cart");
        }

        // Thêm vào giỏ hàng có áp dụng Ajax
        [HttpPost]
        public async Task<IActionResult> AddToCartAjax(int productId, int quantity)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!" });
            }

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Quantity <= 0)
            {
                return Json(new { success = false, message = "Sản phẩm không còn hàng." });
            }

            quantity = Math.Min(quantity, product.Quantity);
            await _cartService.AddToCartAsync(userId.Value, productId, quantity);

            var cartItems = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
            HttpContext.Session.SetObjectAsJson("Cart", cartItems);

            int totalQuantity = cartItems.Sum(i => i.Quantity);

            return Json(new
            {
                success = true,
                message = "✔ Sản phẩm đã được thêm vào giỏ hàng!",
                totalQuantity = totalQuantity
            });
        }

        // Giỏ hàng
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var cartItems = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);

            foreach (var item in cartItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductID);
                if (product == null || product.Quantity <= 0)
                {
                    item.IsOutOfStock = true;
                }
                else
                {
                    item.IsOutOfStock = false;
                    item.MaxAvailable = product.Quantity;

                    if (item.Quantity > product.Quantity)
                    {
                        item.Quantity = product.Quantity;
                        await _cartService.UpdateQuantityAsync(userId.Value, item.ProductID, product.Quantity);
                    }
                }
            }

            HttpContext.Session.SetObjectAsJson("Cart", cartItems);
            return View("Cart", cartItems);
        }

        // Xóa khỏi giỏ hàng
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "User");

            await _cartService.RemoveFromCartAsync(userId.Value, productId);
            var cartItems = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
            HttpContext.Session.SetObjectAsJson("Cart", cartItems);

            return RedirectToAction("Cart");
        }

        // Xóa khỏi giỏ hàng có sử dụng Ajax
        [HttpPost]
        public async Task<IActionResult> RemoveFromCartAjax(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập" });
            }

            await _cartService.RemoveFromCartAsync(userId.Value, productId);

            var cartItems = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
            HttpContext.Session.SetObjectAsJson("Cart", cartItems);

            int totalQuantity = cartItems.Sum(i => i.Quantity);

            return Json(new { success = true, totalQuantity });
        }

        // Xử lý tăng, giảm số lượng
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "User");

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Quantity <= 0)
            {
                TempData["CartMessage"] = "Sản phẩm không còn hàng.";
                return RedirectToAction("Cart");
            }

            quantity = Math.Min(quantity, product.Quantity);
            await _cartService.UpdateQuantityAsync(userId.Value, productId, quantity);
            var cartItems = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
            HttpContext.Session.SetObjectAsJson("Cart", cartItems);

            return RedirectToAction("Cart");
        }

        // Xử lý tăng, giảm số lượng có dùng Ajax
        [HttpPost]
        public async Task<IActionResult> UpdateQuantityAjax(int productId, int quantity)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return Json(new { success = false });

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || quantity < 1 || quantity > product.Quantity)
                return Json(new { success = false });

            await _cartService.UpdateQuantityAsync(userId.Value, productId, quantity);

            var cartItems = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
            HttpContext.Session.SetObjectAsJson("Cart", cartItems);

            var updatedItem = cartItems.FirstOrDefault(x => x.ProductID == productId);
            int totalQuantity = cartItems.Sum(x => x.Quantity);
            decimal totalPrice = cartItems.Sum(x => x.ProductPrice * x.Quantity);
            decimal subtotal = updatedItem.ProductPrice * updatedItem.Quantity;

            return Json(new
            {
                success = true,
                quantity = updatedItem.Quantity,
                subtotal = subtotal,
                totalCartPrice = totalPrice,
                totalQuantity = totalQuantity
            });
        }

        // Nút chọn sản phẩm trong giỏ để thanh toán
        [HttpPost]
        public async Task<IActionResult> CheckOutSelected(List<int> selectedProductIds, [FromForm] Dictionary<int, int> quantities)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "User");

            if (selectedProductIds == null || !selectedProductIds.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm để thanh toán.";
                return RedirectToAction("Cart");
            }

            var cartItems = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
            var selectedItems = cartItems
                .Where(c => selectedProductIds.Contains(c.ProductID))
                .Select(c =>
                {
                    if (quantities.TryGetValue(c.ProductID, out int qty))
                    {
                        c.Quantity = qty;
                    }
                    return c;
                })
                .ToList();

            // Cập nhật lại số lượng trong Session & giỏ hàng
            foreach (var item in selectedItems)
            {
                await _cartService.UpdateQuantityAsync(userId.Value, item.ProductID, item.Quantity);
            }

            HttpContext.Session.SetObjectAsJson("SelectedCart", selectedItems);
            return RedirectToAction("CheckOut", "Order");
        }

    }
}
