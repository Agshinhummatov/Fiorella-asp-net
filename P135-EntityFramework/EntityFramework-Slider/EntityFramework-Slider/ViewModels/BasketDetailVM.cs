namespace EntityFramework_Slider.ViewModels
{
    public class BasketDetailVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Total { get; set; }
        public string CategoryName { get; set; }
    }
}
