using Microsoft.AspNetCore.Mvc;
using MOGDEOW.Services;
using MOGDEOW.Models;
using System;
using System.Threading.Tasks;
using MOGDEOW.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using MOGDEOW.Attributes;
using Microsoft.IdentityModel.Tokens;

namespace MOGDEOW.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBillService _billService;
        private readonly IProductService _productService;

        public AccountController(IUserService userService, IBillService billService, IProductService productService)
        {
            _userService = userService;
            _billService = billService;
            _productService = productService;
        }

        // Hiển thị trang tài khoản với dữ liệu cần thiết
        public async Task<IActionResult> Index(string section = "profile", DateTime? selectedDate = null, int? billId = null, int? selectedTypeId = null, string topRange = null)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var billDetails = billId != null ? await _billService.GetBillDetailsByBillIdAsync(billId.Value) : new List<BillDetail>();
            var bill = billId != null? await _billService.GetBillByIdAsync(billId.Value): new Bill();

            var model = new AccountViewModel
            {
                User = user,
                Bills = await _billService.GetBillsByUserIdAsync(user.UserID),
                TodayOrders = selectedDate != null ? await _billService.GetOrdersByDateAsync(selectedDate.Value) : new List<Bill>(),
                SelectedSection = section,
                SelectedDate = selectedDate ?? DateTime.Today,
                BillDetails = billDetails,
                Bill = bill
            };

            if (section == "productstats")
            {
                var allTypes = await _productService.GetAllProductTypesAsync();
                model.ProductTypes = allTypes.ToList();

                if (int.TryParse(HttpContext.Request.Query["selectedTypeId"], out var typeId))
                    selectedTypeId = typeId;
                model.SelectedTypeId = selectedTypeId;

                topRange = HttpContext.Request.Query["topRange"];
                if (DateTime.TryParse(HttpContext.Request.Query["selectedDate"], out var parsedDate))
                    selectedDate = parsedDate;

                ViewBag.SelectedDate = selectedDate?.ToString("yyyy-MM-dd");
                ViewBag.TopRange = topRange;

                var allProducts = await _productService.GetProductsForStatsAsync();
                var filtered = allProducts.AsQueryable();

                if (selectedTypeId.HasValue)
                    filtered = filtered.Where(p => p.ProductTypeID == selectedTypeId.Value);

                if (selectedDate.HasValue)
                {
                    filtered = filtered.Where(p => p.BillDetails != null &&
                        p.BillDetails.Any(b => b.Bill != null && b.Bill.Date.Date == selectedDate.Value.Date));
                }

                // Tổng số lượng đã bán theo loại đã lọc
                int totalSold = 0;
                foreach (var product in filtered)
                {
                    if (product.BillDetails != null)
                        totalSold += product.BillDetails.Sum(b => b.Quantity);
                }
                ViewBag.TotalSold = totalSold;

                // Xử lý top bán chạy
                if (!string.IsNullOrEmpty(topRange))
                {
                    DateTime from, to;
                    if (topRange == "today")
                    {
                        from = DateTime.Today;
                        to = DateTime.Today;
                    }
                    else if (topRange == "month")
                    {
                        from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        to = DateTime.Today;
                    }
                    else if (topRange == "year")
                    {
                        from = new DateTime(DateTime.Today.Year, 1, 1);
                        to = DateTime.Today;
                    }
                    else
                    {
                        from = to = DateTime.MinValue;
                    }

                    var productsForTop = allProducts.Where(p => p.BillDetails != null &&
                        p.BillDetails.Any(b => b.Bill != null &&
                                               b.Bill.Date.Date >= from &&
                                               b.Bill.Date.Date <= to));

                    if (selectedTypeId.HasValue)
                        productsForTop = productsForTop.Where(p => p.ProductTypeID == selectedTypeId.Value);

                    var topProducts = productsForTop
                        .OrderByDescending(p => p.BillDetails
                            .Where(b => b.Bill != null &&
                                        b.Bill.Date.Date >= from &&
                                        b.Bill.Date.Date <= to)
                            .Sum(b => b.Quantity))
                        .Take(5)
                        .ToList();

                    ViewBag.TopProducts = topProducts;
                }

                model.ProductStats = filtered.ToList();
            }


            if (section == "revenuestats")
            {
                topRange = HttpContext.Request.Query["topRange"];
                DateTime? fromDate = null, toDate = null;

                if (!string.IsNullOrEmpty(HttpContext.Request.Query["fromDate"]) &&
                    DateTime.TryParse(HttpContext.Request.Query["fromDate"], out var parsedFrom))
                {
                    fromDate = parsedFrom;
                }

                if (!string.IsNullOrEmpty(HttpContext.Request.Query["toDate"]) &&
                    DateTime.TryParse(HttpContext.Request.Query["toDate"], out var parsedTo))
                {
                    toDate = parsedTo;
                }

                if (!string.IsNullOrEmpty(HttpContext.Request.Query["selectedTypeId"]) &&
                    int.TryParse(HttpContext.Request.Query["selectedTypeId"], out var parsedType))
                {
                    selectedTypeId = parsedType;
                }

                var (totalRevenue, totalCompletedOrders, revenueDates, revenueValues) =
                    await _billService.GetRevenueStatsAsync(fromDate, toDate, topRange, selectedTypeId);

                model.TotalRevenue = totalRevenue;
                model.TotalCompletedOrders = totalCompletedOrders;
                model.RevenueDates = revenueDates;
                model.RevenueValues = revenueValues;

                model.ProductTypes = (await _productService.GetAllProductTypesAsync()).ToList();
                model.SelectedTypeId = selectedTypeId;
            }


            ViewBag.Role = user.Role;
            return View(model);
        }

        // Cập nhật thông tin người dùng
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User updatedUser, IFormFile? imageFile)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var existingUser = await _userService.GetUserByIdAsync(userId.Value);
            if (existingUser == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng!";
                return RedirectToAction("Index", new { section = "profile" });
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(updatedUser.PhoneNumber ?? "", @"^0\d{9}$"))
            {
                TempData["Error"] = "Số điện thoại phải có đúng 10 chữ số và bắt đầu bằng số 0!";
                return RedirectToAction("Index", new { section = "profile" });
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/src/Assets/Img/User");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                existingUser.UserImg = fileName;
                HttpContext.Session.SetString("UserImg", fileName);
            }

            // Cập nhật thông tin khác
            existingUser.UserName = updatedUser.UserName;
            existingUser.PhoneNumber = updatedUser.PhoneNumber;
            existingUser.Address = updatedUser.Address;

            await _userService.UpdateUserAsync(existingUser.UserID, existingUser);

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index", new { section = "profile" });
        }


        // Cập nhật mật khẩu
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp!";
                return RedirectToAction("Index", new { section = "profile" });
            }

            // Kiểm tra độ mạnh của mật khẩu
            if (!System.Text.RegularExpressions.Regex.IsMatch(newPassword ?? "", @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$"))
            {
                TempData["Error"] = "Mật khẩu mới phải từ 6 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt!";
                return RedirectToAction("Index", new { section = "profile" });
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null || !(await _userService.VerifyPasswordAsync(userId.Value, currentPassword)))
            {
                TempData["Error"] = "Mật khẩu hiện tại không đúng!";
                return RedirectToAction("Index", new { section = "profile" });
            }

            await _userService.UpdatePasswordAsync(userId.Value, newPassword);
            TempData["Success"] = "Mật khẩu đã được thay đổi!";

            return RedirectToAction("Index", new { section = "profile" });
        }

        [AuthorizeRole("Admin", "Staff")]
        [HttpGet]
        public async Task<IActionResult> TodayOrders(DateTime? selectedDate)
        {
            var orders = selectedDate != null ? await _billService.GetOrdersByDateAsync(selectedDate.Value) : new List<Bill>();
            return View(orders);
        }

        public async Task<IActionResult> BillDetail(int billId)
        {
            var billDetails = await _billService.GetBillDetailsByBillIdAsync(billId);

            if (billDetails == null || !billDetails.Any())
            {
                TempData["Error"] = "Không tìm thấy chi tiết hóa đơn!";
                return RedirectToAction("Index", new { section = "bills" });
            }

            var bill = await _billService.GetBillByIdAsync(billId);
            if (bill == null)
            {
                TempData["Error"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("Index", new { section = "bills" });
            }

            var user = await _userService.GetUserByIdAsync(bill.UserID);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người mua của hóa đơn!";
                return RedirectToAction("Index", new { section = "bills" });
            }

            var model = new AccountViewModel
            {
                User = user,
                Bill = bill,
                BillDetails = billDetails,
                SelectedSection = "billDetail"
            };

            return View("_BillDetail", model);
        }
        [HttpPost]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> ConfirmCodPayment(int billId, DateTime? selectedDate)
        {
            var bill = await _billService.GetBillByIdAsync(billId);

            if (bill != null && bill.Status == "Chờ xác nhận" && bill.PaymentMethod == "COD")
            {
                bill.Status = "Đã thanh toán";
                await _billService.UpdateBillAsync(bill);
                TempData["Success"] = $"Đã xác nhận đơn hàng #{bill.BillID}!";
            }
            else
            {
                TempData["Error"] = "Không thể xác nhận đơn hàng này.";
            }
            return RedirectToAction("Index", new { section = "orders", selectedDate = selectedDate?.ToString("yyyy-MM-dd") });
        }

    }
}
