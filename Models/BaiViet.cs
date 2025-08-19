using System;

namespace WEB.Models
{
    public class BaiViet
    {
        public int BaiVietID { get; set; }
        public string? TieuDe { get; set; }
        public string? TomTat { get; set; }
        public string? NoiDung { get; set; }
        public string? HinhAnh { get; set; }
        public DateTime NgayDang { get; set; }
        public int LuotXem { get; set; }
        public int ChuyenMucID { get; set; }
        public int? TacGiaID { get; set; }

        // === 3 cờ ẩn cho trang chủ ===
        public bool HomeCol1 { get; set; }
        public bool HomeCol2 { get; set; }
        public bool HomeCol3 { get; set; }
        public int? HomeOrder { get; set; }
    }
}
