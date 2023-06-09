using EntityFramework_Slider.Data;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;

namespace EntityFramework_Slider.Services
{
    public class LayoutService:ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;

        public LayoutService(AppDbContext context,
                             IBasketService basketService)
        {
            _context = context;
            _basketService = basketService;
        }

        public LayoutVM GetSettingDatas()
        {
            Dictionary<string, string> settings = _context.Settings.AsEnumerable().ToDictionary(m => m.Key, m => m.Value);
            List<BasketVM> basketDatas = _basketService.GetBasketDatas();
            return new LayoutVM { Settings = settings, BasketCount = basketDatas.Sum(m => m.Count) };
        }
    }
}
