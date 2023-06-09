
using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Models
{
    public class Category:BaseEntity
    {
        [Required(ErrorMessage ="Don't be empty")]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
