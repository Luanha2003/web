using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB.Models;                 
using WEB.Areas.Admin.ViewModels; 
using Microsoft.AspNetCore.Http; 


namespace WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaiVietController : BaseAdminController
    {
        private readonly NewsDbContext _context;
        public BaiVietController(NewsDbContext context) => _context = context;

        // ===== Helper =====
        private void PopulateCategories(int? selectedId = null)
        {
            ViewBag.ChuyenMucID = new SelectList(
                _context.ChuyenMucs.AsNoTracking().OrderBy(x => x.TenChuyenMuc),
                "ChuyenMucID", "TenChuyenMuc", selectedId
            );
        }

        // ===== Index =====
        public IActionResult Index(string? q, int? catId, string sort = "new",
                                   string view = "grid", int page = 1, int pageSize = 12)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var categories = _context.ChuyenMucs.AsNoTracking()
                                .OrderBy(x => x.TenChuyenMuc)
                                .Select(x => new BaiVietIndexVM.CategoryOption
                                {
                                    Id = x.ChuyenMucID,
                                    Name = x.TenChuyenMuc
                                })
                                .ToList();

            var query = _context.BaiViets.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(b => b.TieuDe.Contains(q) || b.NoiDung.Contains(q));

            if (catId.HasValue)
                query = query.Where(b => b.ChuyenMucID == catId.Value);

            query = sort switch
            {
                "views" => query.OrderByDescending(b => b.LuotXem).ThenByDescending(b => b.NgayDang),
                "title" => query.OrderBy(b => b.TieuDe),
                _       => query.OrderByDescending(b => b.NgayDang)
            };

            var total = query.Count();
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 6, 48);

            var posts = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var catMap = _context.ChuyenMucs.AsNoTracking()
                            .ToDictionary(c => c.ChuyenMucID, c => c.TenChuyenMuc);

            var vm = new BaiVietIndexVM
            {
                Search     = q,
                CategoryId = catId,
                Sort       = sort,
                ViewMode   = (view == "table") ? "table" : "grid",
                Page       = page,
                PageSize   = pageSize,
                TotalItems = total,
                Categories = categories,
                Items      = posts.Select(b => new BaiVietIndexVM.BaiVietItemVM
                {
                    BaiVietID     = b.BaiVietID,
                    TieuDe        = b.TieuDe,
                    HinhAnh       = string.IsNullOrWhiteSpace(b.HinhAnh) ? null : b.HinhAnh,
                    NgayDang      = b.NgayDang,
                    LuotXem       = b.LuotXem,
                    ChuyenMucID   = b.ChuyenMucID,
                    TenChuyenMuc  = catMap.TryGetValue(b.ChuyenMucID, out var name) ? name : null
                }).ToList()
            };

            return View(vm);
        }

        // ===== Create =====
        public IActionResult Create()
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            PopulateCategories();
            return View(new BaiViet());
        }

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
                PopulateCategories(model.ChuyenMucID);
                return View(model);
            }

            model.NgayDang = DateTime.Now;
            model.LuotXem  = 0;

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (int.TryParse(userIdStr, out var uid)) model.TacGiaID = uid;

            // 3 cờ + HomeOrder đã nằm trong model → EF sẽ lưu
            _context.BaiViets.Add(model);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ===== Edit =====
        public IActionResult Edit(int id)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var item = _context.BaiViets.Find(id);
            if (item == null) return NotFound();

            PopulateCategories(item.ChuyenMucID);
            return View(item);
        }

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
                PopulateCategories(model.ChuyenMucID);
                return View(model);
            }

            // cập nhật dữ liệu chính
            item.TieuDe      = model.TieuDe;
            item.NoiDung     = model.NoiDung;
            item.HinhAnh     = model.HinhAnh;
            item.ChuyenMucID = model.ChuyenMucID;

            // cập nhật 3 cờ + thứ tự
            item.HomeCol1 = model.HomeCol1;
            item.HomeCol2 = model.HomeCol2;
            item.HomeCol3 = model.HomeCol3;
            item.HomeOrder = model.HomeOrder;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ===== Delete =====
        public IActionResult Delete(int id)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var item = _context.BaiViets.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

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
