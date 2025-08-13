using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB.Models;

namespace WEB.Areas.Admin.Controllers
{
    public class BaiVietController : BaseAdminController
    {
        private readonly NewsDbContext _context;
        public BaiVietController(NewsDbContext context) => _context = context;

        public IActionResult Index()
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var list = _context.BaiViets
                .OrderByDescending(x => x.NgayDang)
                .ToList();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;
            return View(new BaiViet { NgayDang = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BaiViet model)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            if (!ModelState.IsValid) return View(model);
            model.NgayDang = DateTime.Now;
            _context.BaiViets.Add(model);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var bv = _context.BaiViets.Find(id);
            if (bv == null) return NotFound();
            return View(bv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BaiViet model)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            if (!ModelState.IsValid) return View(model);
            _context.BaiViets.Update(model);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var gate = OnlyAdmin();
            if (gate != null) return gate;

            var bv = _context.BaiViets.Find(id);
            if (bv == null) return NotFound();
            return View(bv);
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
