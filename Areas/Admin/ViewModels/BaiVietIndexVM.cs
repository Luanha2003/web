using System;
using System.Collections.Generic;

namespace WEB.Areas.Admin.ViewModels
{
    public class BaiVietIndexVM
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public string Sort { get; set; } = "new"; // new|views|title
        public string ViewMode { get; set; } = "grid"; // grid|table

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / Math.Max(1, PageSize));

        public List<BaiVietItemVM> Items { get; set; } = new();
        public List<CategoryOption> Categories { get; set; } = new();

        public class BaiVietItemVM
        {
            public int BaiVietID { get; set; }              // đổi tên nếu model của bạn khác
            public string TieuDe { get; set; } = "";
            public string? HinhAnh { get; set; }
            public DateTime NgayDang { get; set; }
            public int LuotXem { get; set; }
            public int? ChuyenMucID { get; set; }
            public string? TenChuyenMuc { get; set; }
        }

        public class CategoryOption
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }
    }
}
