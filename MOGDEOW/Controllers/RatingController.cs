using Microsoft.AspNetCore.Mvc;
using MOGDEOW.Services;
using MOGDEOW.Models;
using System;
using System.Threading.Tasks;

namespace MOGDEOW.Controllers
{
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingService;
        private readonly IProductService _productService;

        public RatingController(IRatingService ratingService, IProductService productService)
        {
            _ratingService = ratingService;
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitRating(int ProductID, int Rate, string Message)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "User");

            if (Rate < 1 || Rate > 5)
            {
                TempData["Error"] = "Vui lòng chọn số sao hợp lệ.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            if (string.IsNullOrWhiteSpace(Message))
            {
                TempData["Error"] = "Nội dung bình luận không được để trống.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var rating = new Rating
            {
                ProductID = ProductID,
                UserID = userId.Value,
                Rate = Rate,
                Message = Message,
                Date = DateTime.Now
            };

            await _ratingService.AddOrUpdateRatingAsync(rating);
            TempData["Success"] = "Đánh giá của bạn đã được cập nhật thành công.";
            return Redirect(Request.Headers["Referer"].ToString());
        }

    }
}
