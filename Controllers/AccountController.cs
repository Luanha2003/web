using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB.Models;

namespace WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly NewsDbContext _context;
        public AccountController(NewsDbContext context) => _context = context;

        // ======= ĐĂNG KÝ =======
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(NguoiDung model)
        {
            if (ModelState.IsValid)
            {
                if (_context.NguoiDungs.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại");
                    return View(model);
                }

                var user = new NguoiDung
                {
                    HoTen = model.HoTen,
                    Email = model.Email,
                    MatKhau = model.MatKhau, // TODO: Hash khi dùng thật
                    VaiTro = "DocGia",
                    NgayTao = DateTime.Now
                };
                _context.NguoiDungs.Add(user);
                _context.SaveChanges();

                // Tự đăng nhập sau khi đăng ký (tuỳ chọn)
                HttpContext.Session.SetString("UserId", user.NguoiDungID.ToString());
                HttpContext.Session.SetString("UserName", user.HoTen ?? user.Email);
                HttpContext.Session.SetString("UserRole", user.VaiTro ?? "DocGia");

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // ======= ĐĂNG NHẬP =======
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(NguoiDung model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.MatKhau))
            {
                ModelState.AddModelError("", "Vui lòng nhập Email và Mật khẩu");
                return View(model);
            }

            var user = _context.NguoiDungs
                .FirstOrDefault(u => u.Email == model.Email && u.MatKhau == model.MatKhau);

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc Mật khẩu không đúng");
                return View(model);
            }

            HttpContext.Session.SetString("UserId", user.NguoiDungID.ToString());
            HttpContext.Session.SetString("UserName", user.HoTen ?? user.Email);
            HttpContext.Session.SetString("UserRole", user.VaiTro ?? "DocGia");

            // Nếu là Admin thì đi vào Dashboard, không thì về trang chủ
            if ((user.VaiTro ?? "").Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return RedirectToAction("Index", "Home");
        }

        // ======= ĐĂNG XUẤT (POST) =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
