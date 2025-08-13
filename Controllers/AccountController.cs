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
// [ValidateAntiForgeryToken]
public IActionResult Register(NguoiDung model)
{
    // Bỏ validate các field không nhập từ form
    ModelState.Remove(nameof(NguoiDung.VaiTro));
    ModelState.Remove(nameof(NguoiDung.NgayTao)); // nếu property này có [Required]

    if (ModelState.IsValid)
    {
        if (_context.NguoiDungs.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError("", "Email đã tồn tại");
            return View(model);
        }

        var user = new NguoiDung
        {
            HoTen   = model.HoTen,
            Email   = model.Email,
            MatKhau = model.MatKhau, // TODO: hash
            VaiTro  = "DocGia",      // gán mặc định phía server
            NgayTao = DateTime.Now
        };

        _context.NguoiDungs.Add(user);
        _context.SaveChanges();
        return RedirectToAction("Login", "Account");
    }

    return View(model);
}


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
// [ValidateAntiForgeryToken]
public IActionResult Login(NguoiDung model)
{
    // Bỏ validate các field không nhập từ form
    ModelState.Remove(nameof(NguoiDung.HoTen));
    ModelState.Remove(nameof(NguoiDung.VaiTro));
    ModelState.Remove(nameof(NguoiDung.NgayTao)); // nếu có

    if (ModelState.IsValid)
    {
        var user = _context.NguoiDungs
            .FirstOrDefault(u => u.Email == model.Email && u.MatKhau == model.MatKhau);

        if (user != null)
        {
            HttpContext.Session.SetString("UserId", user.NguoiDungID.ToString());
            HttpContext.Session.SetString("UserName", user.HoTen ?? "");
            HttpContext.Session.SetString("UserRole", user.VaiTro ?? "");
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
    }

    // ModelState không hợp lệ hoặc đăng nhập sai -> trả lại form
    return View(model);
}

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
