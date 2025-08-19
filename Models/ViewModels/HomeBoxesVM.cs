using System.Collections.Generic;

namespace WEB.Models.ViewModels
{
    public class HomeBoxesVM
    {
        public List<BaiViet> Box1 { get; set; } = new();
        public List<BaiViet> Box2 { get; set; } = new();
        public List<BaiViet> Box3 { get; set; } = new();
    }
}
