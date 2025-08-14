using Microsoft.AspNetCore.Mvc;
using WEB.Models;

namespace WEB.Areas.Admin.Controllers
{
    public class NguoiDungController : BaseAdminController
    {
        private readonly NewsDbContext _ctx;
        public NguoiDungController(NewsDbContext ctx) => _ctx = ctx;

        // GET: /Admin/NguoiDung
        public IActionResult Index()
        {
            var gate = OnlyAdmin(); if (gate != null) return gate;
            var list = _ctx.NguoiDungs
                .OrderByDescending(x => x.NgayTao)
                .ToList();
            return View(list);
        }

        // GET: /Admin/NguoiDung/Create
        public IActionResult Create()
        {
            var gate = OnlyAdmin(); if (gate != null) return gate;
            return View(new NguoiDung { VaiTro = "DocGia" });
        }

        // POST: /Admin/NguoiDung/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NguoiDung model)
        {
            var gate = OnlyAdmin(); if (gate != null) return gate;

            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError(nameof(model.Email), "Email là bắt buộc");
            if (string.IsNullOrWhiteSpace(model.MatKhau))
                ModelState.AddModelError(nameof(model.MatKhau), "Mật khẩu là bắt buộc");

            if (_ctx.NguoiDungs.Any(u => u.Email == model.Email))
                ModelState.AddModelError(nameof(model.Email), "Email đã tồn tại");

            if (!ModelState.IsValid) return View(model);

            var user = new NguoiDung
            {
                HoTen = model.HoTen,
                Email = model.Email,
                MatKhau = model.MatKhau, // PROD: hash password
                VaiTro = string.IsNullOrWhiteSpace(model.VaiTro) ? "DocGia" : model.VaiTro,
                NgayTao = DateTime.Now
            };
            _ctx.NguoiDungs.Add(user);
            _ctx.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/NguoiDung/Edit/5
        public IActionResult Edit(int id)
        {
            var gate = OnlyAdmin(); if (gate != null) return gate;

            var user = _ctx.NguoiDungs.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Admin/NguoiDung/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string? HoTen, string? Email, string? VaiTro, string? NewPassword)
        {
            var gate = OnlyAdmin(); if (gate != null) return gate;

            var user = _ctx.NguoiDungs.Find(id);
            if (user == null) return NotFound();

            if (string.IsNullOrWhiteSpace(Email))
                ModelState.AddModelError(nameof(Email), "Email là bắt buộc");
            else if (_ctx.NguoiDungs.Any(u => u.Email == Email && u.NguoiDungID != id))
                ModelState.AddModelError(nameof(Email), "Email đã tồn tại");

            if (!ModelState.IsValid)
            {
                // Trả lại model hiện có để hiển thị lỗi
                user.HoTen = HoTen;
                user.Email = Email;
                user.VaiTro = VaiTro;
                return View(user);
            }

            user.HoTen = HoTen;
            user.Email = Email;
            user.VaiTro = string.IsNullOrWhiteSpace(VaiTro) ? user.VaiTro : VaiTro;

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                user.MatKhau = NewPassword; // PROD: hash password
            }

            _ctx.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/NguoiDung/Delete/5
        public IActionResult Delete(int id)
        {
            var gate = OnlyAdmin(); if (gate != null) return gate;

            var user = _ctx.NguoiDungs.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Admin/NguoiDung/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var gate = OnlyAdmin(); if (gate != null) return gate;

            // Không cho xóa chính mình
            var currentIdStr = HttpContext.Session.GetString("UserId");
            if (int.TryParse(currentIdStr, out var currentId) && currentId == id)
            {
                TempData["Err"] = "Không thể xóa tài khoản đang đăng nhập.";
                return RedirectToAction(nameof(Index));
            }

            var user = _ctx.NguoiDungs.Find(id);
            if (user != null)
            {
                _ctx.NguoiDungs.Remove(user);
                _ctx.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/NguoiDung/SetAdmin/5  (toggle Admin/DocGia nhanh)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetAdmin(int id, bool makeAdmin = true)
        {
            var gate = OnlyAdmin(); if (gate != null) return gate;

            var user = _ctx.NguoiDungs.Find(id);
            if (user == null) return NotFound();

            user.VaiTro = makeAdmin ? "Admin" : "DocGia";
            _ctx.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
