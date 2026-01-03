using MOGDEOW.Data;
using MOGDEOW.Models;
using MOGDEOW.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using MOGDEOW.DTOs;
using MOGDEOW.Extensions;

namespace MOGDEOW.Controllers
{
    public class OrderController : Controller
    {
        private readonly ICartDetailService _cartService;
        private readonly IBillService _billService;
        private readonly IBillDetailService _billDetailService;
        private readonly IProductService _productService;
        private readonly MogdeowContext _context;
        private readonly IEmailService _emailService;

        public OrderController(ICartDetailService cartService, IBillService billService, IBillDetailService billDetailService, IProductService productService, IEmailService emailService, MogdeowContext context)
        {
            _cartService = cartService;
            _billService = billService;
            _billDetailService = billDetailService;
            _productService = productService;
            _emailService = emailService;
            _context = context;
        }

        public async Task<IActionResult> CheckOut(int? productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "User");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId.Value);
            ViewBag.User = user;

            var selectedCart = HttpContext.Session.GetObjectFromJson<List<CartDetailDTO>>("SelectedCart");
            if (selectedCart != null && selectedCart.Any())
            {
                return View(selectedCart);
            }

            if (productId.HasValue)
            {
                var product = await _productService.GetProductByIdAsync(productId.Value);
                if (product == null) return RedirectToAction("Index", "Home");

                // Lấy số lượng
                int quantity = HttpContext.Session.GetInt32($"Quantity_{productId.Value}") ?? 1;

                var checkOutItems = new List<CartDetailDTO>
        {
            new CartDetailDTO
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductImage = product.ProductImage,
                Quantity = quantity
            }
        };

                return View(checkOutItems);
            }
            else
            {
                var cartItems = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
                if (!cartItems.Any()) return RedirectToAction("Cart", "Cart");

                return View(cartItems);
            }
        }

        // Xử lý khi đặt hàng từ giỏ hàng
        [HttpPost]
        public async Task<IActionResult> PlaceOrderFromCart([FromForm] List<int> selectedProductIds, [FromForm] string paymentMethod, [FromForm] string receiverName,[FromForm] string receiverPhone, [FromForm] string shippingAddress)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "User");
            if (selectedProductIds == null || !selectedProductIds.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm để đặt hàng.";
                return RedirectToAction("Cart", "Cart");
            }

            var allCartItems = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
            var cartItems = allCartItems
                .Where(i => selectedProductIds.Contains(i.ProductID)).ToList();


            // Kiểm tra tồn kho trước khi tạo hóa đơn
            foreach (var item in cartItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductID);
                if (product == null || product.Quantity < item.Quantity)
                {
                    TempData["Error"] = $"Sản phẩm \"{item.ProductName}\" không đủ số lượng hoặc đã hết hàng.";
                    return RedirectToAction("Cart", "Cart");
                }
            }

            // Tạo hóa đơn
            var bill = new Bill
            {
                UserID = userId.Value,
                Date = DateTime.Now,
                Status = paymentMethod == "Chuyển khoản" ? "Đã thanh toán" : "Chờ xác nhận",
                PaymentMethod = paymentMethod,
                Total = cartItems.Sum(i => i.ProductPrice * i.Quantity),
                ReceiverName = receiverName,
                ReceiverPhone = receiverPhone,
                ShippingAddress = shippingAddress
            };
            await _billService.AddBillAsync(bill);

            // Tạo chi tiết hóa đơn & trừ số lượng sản phẩm
            foreach (var item in cartItems)
            {
                var billDetail = new BillDetail
                {
                    BillID = bill.BillID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.ProductPrice
                };
                await _billDetailService.AddBillDetailAsync(billDetail);

                var product = await _productService.GetProductByIdAsync(item.ProductID);
                product.Quantity -= item.Quantity;
                await _productService.UpdateProductAsync(product);
            }
            Console.WriteLine("Danh sách sản phẩm cần xóa: " + string.Join(", ", selectedProductIds));
            Console.WriteLine($"[DEBUG] Bill created at: {bill.Date}");

            // Sau khi tạo hóa đơn & bill detail & trừ số lượng
            foreach (var productId in selectedProductIds)
            {
                var result = await _cartService.RemoveFromCartAsync(userId.Value, productId);
                Console.WriteLine($"XÓA sản phẩm {productId}: {(result ? "THÀNH CÔNG" : "THẤT BẠI")}");
            }

            // Cập nhật lại Session
            var updatedCart = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
            Console.WriteLine("Còn lại trong giỏ: " + string.Join(", ", updatedCart.Select(x => x.ProductID)));
            HttpContext.Session.SetObjectAsJson("Cart", updatedCart);
            HttpContext.Session.Remove("SelectedCart");

            // Gửi email xác nhận đơn hàng
            var billDetails = await _billDetailService.GetBillDetailsByBillIdAsync(bill.BillID);
            var user = await _context.Users.FindAsync(userId);
            string toEmail = user.Email;
            // Tạo bảng HTML từ billDetails
            string productTable = "<table style='width:100%; border-collapse:collapse; margin-top:20px;'>"
                                + "<tr>"
                                + "<th style='border:1px solid #ccc; padding:8px; background-color:#f2f2f2;'>Sản phẩm</th>"
                                + "<th style='border:1px solid #ccc; padding:8px; background-color:#f2f2f2;'>Số lượng</th>"
                                + "<th style='border:1px solid #ccc; padding:8px; background-color:#f2f2f2;'>Đơn giá</th>"
                                + "<th style='border:1px solid #ccc; padding:8px; background-color:#f2f2f2;'>Thành tiền</th>"
                                + "</tr>";

            foreach (var item in billDetails)
            {
                var prod = await _productService.GetProductByIdAsync(item.ProductID);
                productTable += "<tr>"
                              + $"<td style='border:1px solid #ccc; padding:8px;'>{prod.ProductName}</td>"
                              + $"<td style='border:1px solid #ccc; padding:8px; text-align:center;'>{item.Quantity}</td>"
                              + $"<td style='border:1px solid #ccc; padding:8px;'>{item.Price:N0}đ</td>"
                              + $"<td style='border:1px solid #ccc; padding:8px;'>{(item.Price * item.Quantity):N0}đ</td>"
                              + "</tr>";
            }
            productTable += "</table>";
            string subject = "Xác nhận đơn hàng MOGDEOW";
            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            background-color: #f8f8f8;
            padding: 20px;
        }}
        .email-container {{
            background-color: #ffffff;
            padding: 20px;
            border-radius: 10px;
            border: 1px solid #ddd;
            max-width: 600px;
            margin: auto;
        }}
        h2 {{
            color: #ff6b6b;
        }}
        p {{
            font-size: 16px;
            color: #333;
        }}
        .highlight {{
            font-weight: bold;
            color: #2c3e50;
        }}
        .footer {{
            margin-top: 30px;
            font-size: 14px;
            color: #888;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <h2>🛒 Xác nhận đơn hàng - MOGDEOW</h2>
        <p>Xin chào <span class='highlight'>{user.UserName}</span>,</p>
        <p>Đơn hàng của bạn đã được tạo thành công lúc <span class='highlight'>{bill.Date:dd/MM/yyyy HH:mm}</span>.</p>
        <p><strong>Mã đơn hàng:</strong> #{bill.BillID}</p>
        <p><strong>Tổng tiền:</strong> {bill.Total:N0}đ</p>
       <h3>Chi tiết đơn hàng</h3>
        {productTable}
        <p style='margin-top:20px;'>Cảm ơn bạn đã mua sắm tại <strong>MOGDEOW PETSHOP</strong>! 🐾</p>
        <div class='footer'>MOGDEOW Team ♥</div>
    </div>
</body>
</html>
";

            try
            {
                await _emailService.SendEmailAsync(toEmail, subject, body);
                TempData["Success"] = "Đặt hàng thành công!";
                return RedirectToAction("OrderSuccess", new { billId = bill.BillID });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đơn hàng đã tạo nhưng không thể gửi email xác nhận: " + ex.Message;
                return RedirectToAction("OrderSuccess", new { billId = bill.BillID });
            }

        }

        [HttpPost]
        public async Task<IActionResult> BuyNow(int productId, int quantity, [FromForm] string paymentMethod, [FromForm] string receiverName, [FromForm] string receiverPhone, [FromForm] string shippingAddress)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Quantity < quantity)
            {
                TempData["Error"] = "Sản phẩm không còn đủ số lượng.";
                return RedirectToAction("ProductDetail", "Product", new { id = productId });
            }

            // Tạo hóa đơn
            var bill = new Bill
            {
                UserID = userId.Value,
                Date = DateTime.Now,
                Status = paymentMethod == "Chuyển khoản" ? "Đã thanh toán" : "Chờ xác nhận",
                PaymentMethod = paymentMethod,
                Total = product.ProductPrice * quantity,
                ReceiverName = receiverName,
                ReceiverPhone = receiverPhone,
                ShippingAddress = shippingAddress
            };
            await _billService.AddBillAsync(bill);

            var billDetail = new BillDetail
            {
                BillID = bill.BillID,
                ProductID = product.ProductID,
                Quantity = quantity,
                Price = product.ProductPrice
            };
            await _billDetailService.AddBillDetailAsync(billDetail);

            product.Quantity -= quantity;
            await _productService.UpdateProductAsync(product);

            await _cartService.RemoveFromCartAsync(userId.Value, productId);
            var updatedCart = await _cartService.GetCartDetailsByUserIdAsync(userId.Value);
            HttpContext.Session.SetObjectAsJson("Cart", updatedCart);

            var billDetails = await _billDetailService.GetBillDetailsByBillIdAsync(bill.BillID);
            var user = await _context.Users.FindAsync(userId);
            string toEmail = user.Email;
            // Tạo bảng HTML từ billDetails
            string productTable = "<table style='width:100%; border-collapse:collapse; margin-top:20px;'>"
                                + "<tr>"
                                + "<th style='border:1px solid #ccc; padding:8px; background-color:#f2f2f2;'>Sản phẩm</th>"
                                + "<th style='border:1px solid #ccc; padding:8px; background-color:#f2f2f2;'>Số lượng</th>"
                                + "<th style='border:1px solid #ccc; padding:8px; background-color:#f2f2f2;'>Đơn giá</th>"
                                + "<th style='border:1px solid #ccc; padding:8px; background-color:#f2f2f2;'>Thành tiền</th>"
                                + "</tr>";

            foreach (var item in billDetails)
            {
                var prod = await _productService.GetProductByIdAsync(item.ProductID);
                productTable += "<tr>"
                              + $"<td style='border:1px solid #ccc; padding:8px;'>{prod.ProductName}</td>"
                              + $"<td style='border:1px solid #ccc; padding:8px; text-align:center;'>{item.Quantity}</td>"
                              + $"<td style='border:1px solid #ccc; padding:8px;'>{item.Price:N0}đ</td>"
                              + $"<td style='border:1px solid #ccc; padding:8px;'>{(item.Price * item.Quantity):N0}đ</td>"
                              + "</tr>";
            }
            productTable += "</table>";

            string subject = "Xác nhận đơn hàng MOGDEOW";
            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: 'Segoe UI', sans-serif;
            background-color: #f8f8f8;
            padding: 20px;
        }}
        .email-container {{
            background-color: #ffffff;
            padding: 20px;
            border-radius: 10px;
            border: 1px solid #ddd;
            max-width: 600px;
            margin: auto;
        }}
        h2 {{
            color: #ff6b6b;
        }}
        p {{
            font-size: 16px;
            color: #333;
        }}
        .highlight {{
            font-weight: bold;
            color: #2c3e50;
        }}
        .footer {{
            margin-top: 30px;
            font-size: 14px;
            color: #888;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <h2>🛒 Xác nhận đơn hàng - MOGDEOW</h2>
        <p>Xin chào <span class='highlight'>{user.UserName}</span>,</p>
        <p>Đơn hàng của bạn đã được tạo thành công lúc <span class='highlight'>{bill.Date:dd/MM/yyyy HH:mm}</span>.</p>
        <p><strong>Mã đơn hàng:</strong> #{bill.BillID}</p>
        <p><strong>Tổng tiền:</strong> {bill.Total:N0}đ</p>
        <h3>Chi tiết đơn hàng</h3>
        {productTable}
        <p style='margin-top:20px;'>Cảm ơn bạn đã mua sắm tại <strong>MOGDEOW PETSHOP</strong>! 🐾</p>
        <div class='footer'>MOGDEOW Team ♥</div>
    </div>
</body>
</html>
";

            try
            {
                await _emailService.SendEmailAsync(toEmail, subject, body);
                TempData["Success"] = "Đặt hàng thành công!";
                return RedirectToAction("OrderSuccess", new { billId = bill.BillID });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đơn hàng đã tạo nhưng không thể gửi email xác nhận: " + ex.Message;
                return RedirectToAction("OrderSuccess", new { billId = bill.BillID });
            }

        }

        public async Task<IActionResult> OrderSuccess(int billId)
        {
            var bill = await _billService.GetBillByIdAsync(billId);
            if (bill == null) return RedirectToAction("Index", "Home");

            // Gửi dữ liệu đơn hàng qua ViewBag
            ViewBag.BillID = bill.BillID;
            ViewBag.ReceiverName = bill.ReceiverName ?? "Không rõ";
            ViewBag.ReceiverPhone = bill.ReceiverPhone ?? "Không rõ";
            ViewBag.ShippingAddress = bill.ShippingAddress ?? "Không rõ";
            ViewBag.TotalPrice = bill.Total;

            return View();
        }

    }
}
