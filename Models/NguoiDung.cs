using System;

namespace WEB.Models
{
    public class NguoiDung
    {
        public int NguoiDungID { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string VaiTro { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
