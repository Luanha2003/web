using Microsoft.AspNetCore.Mvc;
using WEB.Models;
using Microsoft.EntityFrameworkCore;

namespace WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly NewsDbContext _context;

        public AccountController(NewsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(NguoiDung model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_context.NguoiDungs.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email đã tồn tại");
                return View(model);
            }

            model.VaiTro = string.IsNullOrWhiteSpace(model.VaiTro) ? "DocGia" : model.VaiTro;
            model.NgayTao = DateTime.Now;

            _context.NguoiDungs.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(matKhau))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ Email và Mật khẩu");
                return View();
            }

            var user = _context.NguoiDungs.FirstOrDefault(u => u.Email == email && u.MatKhau == matKhau);
            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.NguoiDungID);
            HttpContext.Session.SetString("UserName", user.HoTen ?? string.Empty);
            HttpContext.Session.SetString("UserRole", user.VaiTro ?? "DocGia");

            if ((user.VaiTro ?? "").Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
