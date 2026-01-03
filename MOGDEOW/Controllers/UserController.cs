using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using MOGDEOW.Services;
using MOGDEOW.Models;
using Microsoft.EntityFrameworkCore;
using MOGDEOW.Extensions;
using MOGDEOW.Attributes;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using MOGDEOW.Helpers;

namespace MOGDEOW.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICartDetailService _cartDetailService;

        public UserController(IUserService userService, ICartDetailService cartDetailService)
        {
            _userService = userService;
            _cartDetailService = cartDetailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string UserName, string Email, string Password, string? otpInput = null)
        {
            ViewBag.ShowOTP = false;

            // Nếu chưa nhập OTP: Gửi OTP
            if (string.IsNullOrEmpty(otpInput))
            {
                if (!Regex.IsMatch(Email ?? "", @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
                {
                    ViewData["UserName"] = UserName;
                    ViewData["Email"] = Email;
                    ViewBag.ErrorMessage = "Vui lòng nhập Gmail hợp lệ.";
                    return View("Signin");
                }

                if (!Regex.IsMatch(Password ?? "", @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$"))
                {
                    ViewData["UserName"] = UserName;
                    ViewData["Email"] = Email;
                    ViewBag.ErrorMessage = "Mật khẩu phải từ 6 ký tự gồm chữ hoa, chữ thường, số và ký tự.";
                    return View("Signin");
                }

                var existingUser = await _userService.GetUserByEmailAsync(Email);
                if (existingUser != null)
                {
                    ViewData["UserName"] = UserName;
                    ViewData["Email"] = Email;
                    ViewBag.ErrorMessage = "Email đã được sử dụng.";
                    return View("Signin");
                }
                var otp = new Random().Next(100000, 999999).ToString();
                TempData["OTP"] = otp;
                TempData["RegisterUser"] = JsonConvert.SerializeObject(new User
                {
                    UserName = UserName,
                    Email = Email,
                    Password = Password,
                    Role = "Customer"
                });
                ViewData["UserName"] = UserName;
                ViewData["Email"] = Email;
                ViewData["Password"] = Password;
                await EmailHelper.SendEmailAsync(Email, "Mã xác nhận đăng ký", $"Mã xác nhận của bạn là: <b>{otp}</b>");
                ViewBag.ShowOTP = true;
                ViewBag.EmailSent = Email;
                return View("Signin");
            }
            var storedOTP = TempData["OTP"] as string;
            var jsonUser = TempData["RegisterUser"] as string;

            if (otpInput != storedOTP || string.IsNullOrEmpty(jsonUser))
            {
                ViewBag.ShowOTP = true;
                ViewBag.EmailSent = Email;
                ViewBag.ErrorMessage = "Mã xác nhận không đúng. Vui lòng thử lại.";
                TempData.Keep(); 
                return View("Signin");
            }
            var user = JsonConvert.DeserializeObject<User>(jsonUser);
            var newUser = await _userService.RegisterAsync(user.UserName, user.Email, user.Password);
            HttpContext.Session.SetInt32("UserID", newUser.UserID);
            HttpContext.Session.SetString("UserRole", newUser.Role);
            HttpContext.Session.SetString("UserName", newUser.UserName);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            Console.WriteLine($"Đang kiểm tra đăng nhập: {email} - {password}");

            var user = await _userService.AuthenticateAsync(email, password);
            if (user == null)
            {
                Console.WriteLine("Đăng nhập thất bại: Sai email hoặc mật khẩu");
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu.");
                return View();
            }

            // Lưu thông tin vào session
            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("UserImg", user.UserImg ?? "");

            var cartItems = await _cartDetailService.GetCartDetailsByUserIdAsync(user.UserID);
            HttpContext.Session.SetObjectAsJson("Cart", cartItems);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login");

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null) return RedirectToAction("Login");

            return View(user);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
