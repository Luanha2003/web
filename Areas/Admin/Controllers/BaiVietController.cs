using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB.Models;

namespace WEB.Areas.Admin.Controllers
{
    public class BaiVietController : BaseAdminController
    {
        private readonly NewsDbContext _context;
        public BaiVietController(NewsDbContext context) => _context = context;

        // GET: /Admin/BaiViet
        public IActionResult Index()
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var list = _context.BaiViets
                .OrderByDescending(x => x.NgayDang)
                .ToList();
            return View(list);
        }

        // GET: /Admin/BaiViet/Create
        public IActionResult Create()
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            ViewBag.ChuyenMucID = new SelectList(_context.ChuyenMucs.OrderBy(x => x.TenChuyenMuc), "ChuyenMucID", "TenChuyenMuc");
            return View(new BaiViet());
        }

        // POST: /Admin/BaiViet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BaiViet model)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            if (string.IsNullOrWhiteSpace(model.TieuDe))
                ModelState.AddModelError(nameof(model.TieuDe), "Tiêu đề là bắt buộc");
            if (model.ChuyenMucID <= 0)
                ModelState.AddModelError(nameof(model.ChuyenMucID), "Vui lòng chọn chuyên mục");

            if (!ModelState.IsValid)
            {
                ViewBag.ChuyenMucID = new SelectList(_context.ChuyenMucs.OrderBy(x => x.TenChuyenMuc), "ChuyenMucID", "TenChuyenMuc", model.ChuyenMucID);
                return View(model);
            }

            model.NgayDang = DateTime.Now;
            model.LuotXem = 0;

            // Lấy TacGiaID từ session nếu có (tùy hệ thống)
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (int.TryParse(userIdStr, out var uid)) model.TacGiaID = uid;

            _context.BaiViets.Add(model);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/BaiViet/Edit/5
        public IActionResult Edit(int id)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var item = _context.BaiViets.Find(id);
            if (item == null) return NotFound();

            ViewBag.ChuyenMucID = new SelectList(_context.ChuyenMucs.OrderBy(x => x.TenChuyenMuc), "ChuyenMucID", "TenChuyenMuc", item.ChuyenMucID);
            return View(item);
        }

        // POST: /Admin/BaiViet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, BaiViet model)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var item = _context.BaiViets.Find(id);
            if (item == null) return NotFound();

            if (string.IsNullOrWhiteSpace(model.TieuDe))
                ModelState.AddModelError(nameof(model.TieuDe), "Tiêu đề là bắt buộc");
            if (model.ChuyenMucID <= 0)
                ModelState.AddModelError(nameof(model.ChuyenMucID), "Vui lòng chọn chuyên mục");

            if (!ModelState.IsValid)
            {
                ViewBag.ChuyenMucID = new SelectList(_context.ChuyenMucs.OrderBy(x => x.TenChuyenMuc), "ChuyenMucID", "TenChuyenMuc", model.ChuyenMucID);
                return View(model);
            }

            item.TieuDe = model.TieuDe;
            item.NoiDung = model.NoiDung;
            item.HinhAnh = model.HinhAnh;
            item.ChuyenMucID = model.ChuyenMucID;
            // Giữ nguyên NgayDang, LuotXem, TacGiaID (nếu muốn sửa thì thêm input)

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/BaiViet/Delete/5
        public IActionResult Delete(int id)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var item = _context.BaiViets.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Admin/BaiViet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var bv = _context.BaiViets.Find(id);
            if (bv != null)
            {
                _context.BaiViets.Remove(bv);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
