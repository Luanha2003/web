using System;

namespace WEB.Models
{
    public class BinhLuan
    {
        public int BinhLuanID { get; set; }
        public int BaiVietID { get; set; }
        public int NguoiDungID { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayBinhLuan { get; set; }
    }
}
