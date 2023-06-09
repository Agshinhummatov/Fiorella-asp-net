using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class ProductDetailVM
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public int Count { get; set; }
        public string CategoryName { get; set; }
        public List<ProductImage> Images { get; set; }
    }
}
