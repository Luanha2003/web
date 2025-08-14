using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB.Models;

namespace WEB.Areas.Admin.Controllers
{
    public class ChuyenMucController : BaseAdminController
    {
        private readonly NewsDbContext _context;
        public ChuyenMucController(NewsDbContext context) => _context = context;

        // GET: /Admin/ChuyenMuc
        public IActionResult Index()
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var list = _context.ChuyenMucs
                .OrderByDescending(x => x.ChuyenMucID)
                .ToList();
            return View(list);
        }

        // GET: /Admin/ChuyenMuc/Create
        public IActionResult Create()
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;
            return View(new ChuyenMuc());
        }

        // POST: /Admin/ChuyenMuc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ChuyenMuc model)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            if (string.IsNullOrWhiteSpace(model.TenChuyenMuc))
                ModelState.AddModelError(nameof(model.TenChuyenMuc), "Tên chuyên mục là bắt buộc");

            if (!ModelState.IsValid) return View(model);

            _context.ChuyenMucs.Add(model);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/ChuyenMuc/Edit/5
        public IActionResult Edit(int id)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var item = _context.ChuyenMucs.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Admin/ChuyenMuc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ChuyenMuc model)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var item = _context.ChuyenMucs.Find(id);
            if (item == null) return NotFound();

            if (string.IsNullOrWhiteSpace(model.TenChuyenMuc))
                ModelState.AddModelError(nameof(model.TenChuyenMuc), "Tên chuyên mục là bắt buộc");

            if (!ModelState.IsValid) return View(model);

            item.TenChuyenMuc = model.TenChuyenMuc;
            item.MoTa = model.MoTa;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/ChuyenMuc/Delete/5
        public IActionResult Delete(int id)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var item = _context.ChuyenMucs.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Admin/ChuyenMuc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var item = _context.ChuyenMucs.Find(id);
            if (item != null)
            {
                // Nếu muốn chặn xóa khi còn bài viết thuộc chuyên mục:
                // if (_context.BaiViets.Any(b => b.ChuyenMucID == id)) {
                //     ModelState.AddModelError("", "Chuyên mục đang có bài viết, không thể xóa.");
                //     return View(item);
                // }

                _context.ChuyenMucs.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
