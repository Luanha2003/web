using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB.Models;

namespace WEB.Controllers
{
    public class GioiThieuController : Controller
    {
        private readonly NewsDbContext _ctx;
        public GioiThieuController(NewsDbContext ctx) => _ctx = ctx;

        public class IntroItem
        {
            public int BaiVietID { get; set; }
            public string TieuDe { get; set; } = "";
            public DateTime NgayDang { get; set; }
            public string TomTat { get; set; } = "";
        }

        public class IntroListVm
        {
            public List<IntroItem> Items { get; set; } = new();
            public int Page { get; set; }
            public int TotalPages { get; set; }
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
            public int SoBanGhi { get; set; }
        }

        // GET: /GioiThieu
        [HttpGet]
        public IActionResult Index(DateTime? tuNgay, DateTime? denNgay, int page = 1, int soBanGhi = 10)
        {
            // Lấy ID chuyên mục "Giới thiệu" (tạo trước trong Admin > Danh mục)
            var cm = _ctx.ChuyenMucs.FirstOrDefault(x => x.TenChuyenMuc == "Giới thiệu");
            if (cm == null)
            {
                // Nếu chưa có, trả trang rỗng + gợi ý tạo chuyên mục
                return View(new IntroListVm
                {
                    Items = new(),
                    Page = 1,
                    TotalPages = 1,
                    SoBanGhi = soBanGhi
                });
            }

            var q = _ctx.BaiViets
                .Where(x => x.ChuyenMucID == cm.ChuyenMucID)
                .AsQueryable();

            if (tuNgay.HasValue)
            {
                var start = tuNgay.Value.Date;
                q = q.Where(x => x.NgayDang >= start);
            }
            if (denNgay.HasValue)
            {
                var end = denNgay.Value.Date.AddDays(1).AddTicks(-1);
                q = q.Where(x => x.NgayDang <= end);
            }

            q = q.OrderByDescending(x => x.NgayDang);

            if (soBanGhi <= 0) soBanGhi = 10;
            if (page <= 0) page = 1;

            var total = q.Count();
            var totalPages = (int)Math.Ceiling(total / (double)soBanGhi);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var data = q.Skip((page - 1) * soBanGhi).Take(soBanGhi).Select(x => new IntroItem
            {
                BaiVietID = x.BaiVietID,
                TieuDe = x.TieuDe ?? "",
                NgayDang = x.NgayDang,
                TomTat = (x.NoiDung ?? "").Length > 200 ? x.NoiDung!.Substring(0, 200) + "..." : (x.NoiDung ?? "")
            }).ToList();

            var vm = new IntroListVm
            {
                Items = data,
                Page = page,
                TotalPages = totalPages,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                SoBanGhi = soBanGhi
            };

            ViewData["Title"] = "Thông tin giới thiệu";
            return View(vm);
        }
    }
}
