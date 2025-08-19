using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WEB.Models;

namespace WEB.Controllers
{
    // ViewModel CHI TIẾT: phẳng theo schema hiện tại
    public class PostDetailsVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Image { get; set; }
        public string Content { get; set; } = "";
        public DateTime Date { get; set; }
        public int LuotXem { get; set; }
        public string? ChuyenMucTen { get; set; }
        public List<HomeController.PostDto> Related { get; set; } = new();
    }

    public class HomeController : Controller
    {
        private readonly NewsDbContext _ctx;
        public HomeController(NewsDbContext ctx) => _ctx = ctx;

        // ==== VM dùng cho trang chủ / tìm kiếm ====
        public record PostDto(int Id, string Title, string? Image, string Summary, DateTime Date);

        public class HomeVm
        {
            public PostDto? Featured { get; set; }
            public PostDto? RightSpot { get; set; }
            public List<PostDto> LeftList { get; set; } = new();
            public List<PostDto> LatestGrid { get; set; } = new();

            // 3 cột dưới — sẽ đổ theo cờ HomeCol1/2/3
            public List<PostDto> TinTucHoatDong { get; set; } = new();
            public List<PostDto> QuanLyUngDungCNTT { get; set; } = new();
            public List<PostDto> CoSoDuLieuTNMT { get; set; } = new();

            public List<string> TaiLieu { get; set; } = new()
            {
                "Tài liệu phục vụ hội thảo xây dựng dự thảo Kiến trúc Chuyển đổi số ngành tài nguyên và môi trường",
                "Tài liệu xin ý kiến góp ý Dự thảo Kiến trúc CPĐT hướng tới CPS ngành tài nguyên và môi trường, phiên bản 2.1",
                "Dự thảo “Thông tư quy định thủ tục, cơ sở dữ liệu, chế độ báo cáo, cung cấp thông tin ngành tài nguyên và môi trường”",
                "Thông tư ban hành quy định kỹ thuật và định mức kinh tế – kỹ thuật về các hệ thống dữ liệu, bộ dữ liệu, ứng dụng tin học của ngành tài nguyên và môi trường"
            };
        }

        // Map BaiViet -> PostDto
        private static PostDto Map(BaiViet b) =>
            new PostDto(
                b.BaiVietID,
                b.TieuDe ?? "(Không tiêu đề)",
                b.HinhAnh,
                (b.NoiDung ?? string.Empty).Length > 180 ? b.NoiDung!.Substring(0, 180) + "…" : (b.NoiDung ?? string.Empty),
                b.NgayDang
            );

        private IQueryable<BaiViet> BaseQuery() =>
            _ctx.BaiViets.OrderByDescending(x => x.NgayDang);

        private List<PostDto> TakeLatest(int take) =>
            BaseQuery().Take(take).Select(Map).ToList();

        // ===== Trang chủ =====
        public IActionResult Index()
        {
            var latest = TakeLatest(8);
            var vm = new HomeVm();

            // Khu FEATURE + RIGHT + LEFT
            if (latest.Any())
            {
                vm.Featured = latest[0];
                if (latest.Count > 1) vm.RightSpot = latest[1];
                vm.LeftList = latest.Skip(2).Take(5).ToList();
                vm.LatestGrid = TakeLatest(6);
            }

            // ===== 3 CỘT DƯỚI: đổ theo CỜ ẨN HomeCol1/2/3 =====
            vm.TinTucHoatDong = _ctx.BaiViets
                .Where(x => x.HomeCol1)
                .OrderBy(x => x.HomeOrder ?? int.MaxValue)
                .ThenByDescending(x => x.NgayDang)
                .Take(4)
                .Select(Map)
                .ToList();

            vm.QuanLyUngDungCNTT = _ctx.BaiViets
                .Where(x => x.HomeCol2)
                .OrderBy(x => x.HomeOrder ?? int.MaxValue)
                .ThenByDescending(x => x.NgayDang)
                .Take(4)
                .Select(Map)
                .ToList();

            vm.CoSoDuLieuTNMT = _ctx.BaiViets
                .Where(x => x.HomeCol3)
                .OrderBy(x => x.HomeOrder ?? int.MaxValue)
                .ThenByDescending(x => x.NgayDang)
                .Take(4)
                .Select(Map)
                .ToList();

            // (Tùy chọn) Fallback nếu admin chưa tick gì → không để UI trống
            if (vm.TinTucHoatDong.Count == 0) vm.TinTucHoatDong = TakeLatest(4);
            if (vm.QuanLyUngDungCNTT.Count == 0) vm.QuanLyUngDungCNTT = TakeLatest(4);
            if (vm.CoSoDuLieuTNMT.Count == 0) vm.CoSoDuLieuTNMT = TakeLatest(4);

            ViewData["Title"] = "Trang chủ";
            return View(vm);
        }

        // ===== Tìm kiếm =====
        [HttpGet]
        public IActionResult Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return RedirectToAction(nameof(Index));

            var data = _ctx.BaiViets
                .Where(x => (x.TieuDe ?? "").Contains(q) || (x.NoiDung ?? "").Contains(q))
                .OrderByDescending(x => x.NgayDang)
                .Select(Map)
                .ToList();

            ViewData["Title"] = $"Tìm kiếm: {q}";
            return View("SearchResult", data);
        }

        // ===== Chi tiết bài viết =====
        [HttpGet]
        public IActionResult Details(int id)
        {
            var post = _ctx.BaiViets.FirstOrDefault(x => x.BaiVietID == id);
            if (post == null) return NotFound();

            // tăng lượt xem
            post.LuotXem += 1;
            _ctx.SaveChanges();

            string? tenCm = _ctx.ChuyenMucs
                .Where(c => c.ChuyenMucID == post.ChuyenMucID)
                .Select(c => c.TenChuyenMuc)
                .FirstOrDefault();

            var related = _ctx.BaiViets
                .Where(x => x.BaiVietID != id && x.ChuyenMucID == post.ChuyenMucID)
                .OrderByDescending(x => x.NgayDang)
                .Take(5)
                .Select(Map)
                .ToList();

            var vm = new PostDetailsVM
            {
                Id = post.BaiVietID,
                Title = post.TieuDe ?? "(Không tiêu đề)",
                Image = post.HinhAnh,
                Content = post.NoiDung ?? "",
                Date = post.NgayDang,
                LuotXem = post.LuotXem,
                ChuyenMucTen = tenCm,
                Related = related
            };

            ViewData["Title"] = vm.Title;
            return View(vm);
        }
    }
}
