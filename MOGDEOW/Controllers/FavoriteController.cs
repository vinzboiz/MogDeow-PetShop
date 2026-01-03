using Microsoft.AspNetCore.Mvc;
using MOGDEOW.Services;
using MOGDEOW.Models;

namespace MOGDEOW.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        // Thêm vào yêu thích
        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thêm yêu thích." });
            }

            var result = await _favoriteService.AddFavoriteAsync(userId.Value, productId);
            if (!result.Success)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, totalFavorite = result.TotalFavorite });
        }

        // Hiển thị trang yêu thích
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            var products = await _favoriteService.GetFavoriteProductsByUserAsync(userId.Value);
            return View(products);
        }

        // Xử lý icon yêu thích
        [HttpPost]
        public async Task<IActionResult> Toggle(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return Json(new { success = false, message = "Bạn cần đăng nhập để thao tác." });

            var result = await _favoriteService.ToggleFavoriteAsync(userId.Value, productId);
            return Json(new
            {
                success = true,
                isFavorite = result.IsFavorite,
                message = result.Message
            });
        }

        // Xóa khỏi danh sách yêu thích
        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });

            bool removed = await _favoriteService.RemoveFavoriteAsync(userId.Value, productId);
            return Json(new { success = removed });
        }
    }
}
