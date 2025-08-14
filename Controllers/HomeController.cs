using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB.Models;

namespace WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly NewsDbContext _db;
        public HomeController(NewsDbContext db) => _db = db;

        // Trang chủ: có tìm kiếm, lọc danh mục, phân trang
        public IActionResult Index(int page = 1, int pageSize = 6, int? categoryId = null, string? q = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 36) pageSize = 6;

            var query = _db.BaiViets.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(x => x.ChuyenMucID == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var key = q.Trim();
                query = query.Where(x =>
                    (x.TieuDe ?? "").Contains(key) ||
                    (x.NoiDung ?? "").Contains(key));
            }

            query = query.OrderByDescending(x => x.NgayDang);

            var total = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Danh mục cho dropdown lọc
            var categories = _db.ChuyenMucs.OrderBy(x => x.TenChuyenMuc).ToList();

            var vm = new HomeIndexVM
            {
                Items = items,
                Categories = categories,
                CurrentCategoryId = categoryId,
                Query = q,
                Page = page,
                PageSize = pageSize,
                Total = total
            };
            return View(vm);
        }

        // Trang xem chi tiết bài viết
        public IActionResult Details(int id)
        {
            var post = _db.BaiViets.FirstOrDefault(x => x.BaiVietID == id);
            if (post == null) return NotFound();

            // tăng lượt xem
            post.LuotXem += 1;
            _db.SaveChanges();

            // gợi ý bài viết mới nhất bên phải
            var latest = _db.BaiViets
                .OrderByDescending(x => x.NgayDang)
                .Take(6)
                .ToList();

            var vm = new PostDetailsVM
            {
                Post = post,
                Latest = latest
            };
            return View(vm);
        }
    }

    // ===== ViewModels đơn giản cho View =====
    public class HomeIndexVM
    {
        public List<BaiViet> Items { get; set; } = new();
        public List<ChuyenMuc> Categories { get; set; } = new();
        public int? CurrentCategoryId { get; set; }
        public string? Query { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)Total / Math.Max(PageSize, 1));
    }

    public class PostDetailsVM
    {
        public BaiViet Post { get; set; } = new();
        public List<BaiViet> Latest { get; set; } = new();
    }
}
