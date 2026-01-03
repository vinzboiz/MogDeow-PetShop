using Microsoft.AspNetCore.Mvc;
using MOGDEOW.Models;
using MOGDEOW.Data;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.IO;


public class UserManageController : Controller
{
    private readonly MogdeowContext _context;

    public UserManageController(MogdeowContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(string searchName, string roleFilter)
    {
        var users = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(searchName))
        {
            users = users.Where(u => u.UserName.Contains(searchName));
        }

        if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "All")
        {
            users = users.Where(u => u.Role == roleFilter);
        }

        ViewBag.CurrentSearch = searchName;
        ViewBag.CurrentRole = roleFilter ?? "All";

        return View(users.ToList());
    }


    [HttpGet]
    public IActionResult EditRole(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    public IActionResult EditRole(int id, string role)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();

        user.Role = role;
        _context.SaveChanges();
        TempData["Success"] = "Cập nhật quyền thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(User user, IFormFile? imageFile)
    {

        if (string.IsNullOrEmpty(user.Password))
        {
            ModelState.AddModelError("Password", "Mật khẩu không được để trống.");
        }

        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState không hợp lệ!");
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"- {modelError.ErrorMessage}");
            }
            return View(user);
        }

        user.Password = HashPassword(user.Password); 
                                                     
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
                imageFile.CopyTo(stream);
            }

            user.UserImg = fileName;
        }
        _context.Users.Add(user);
        _context.SaveChanges();

        TempData["Success"] = "Tạo tài khoản mới thành công!";
        return RedirectToAction("Index");
    }

    // Hàm mã hóa mật khẩu giống trong UserService
    private string HashPassword(string password)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    public IActionResult Edit(User updatedUser, IFormFile? imageFile)
    {
        var user = _context.Users.Find(updatedUser.UserID);
        if (user == null) return NotFound();
        ModelState.Remove("Password");

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Thông tin không hợp lệ!";
            return View(user); 
        }

        user.UserName = updatedUser.UserName;
        user.Email = updatedUser.Email;
        user.Address = updatedUser.Address;
        user.PhoneNumber = updatedUser.PhoneNumber;
        user.Role = updatedUser.Role;

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
                imageFile.CopyTo(stream);
            }

            user.UserImg = fileName;
        }

        _context.SaveChanges();
        TempData["Success"] = "Cập nhật tài khoản thành công!";
        return RedirectToAction("Index");
    }
}
