using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.ViewModels
{
    public class BlogVM
    {
        public IEnumerable<Blog> Blogs { get; set; }
        public BlogHeader BlogHeader { get; set; }
    }
}
