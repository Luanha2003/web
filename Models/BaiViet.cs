using System;

namespace WEB.Models
{
    public class BaiViet
    {
        public int BaiVietID { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string HinhAnh { get; set; }
        public DateTime NgayDang { get; set; }
        public int LuotXem { get; set; }
        public int ChuyenMucID { get; set; }
        public int TacGiaID { get; set; }
    }
}
