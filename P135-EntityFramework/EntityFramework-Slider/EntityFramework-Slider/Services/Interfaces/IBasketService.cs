using EntityFramework_Slider.Models;
using EntityFramework_Slider.ViewModels;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface IBasketService
    {
        List<BasketVM> GetBasketDatas();
        void AddProductToBasket(BasketVM? existProduct, Product product, List<BasketVM> basket);
        void DeleteProductFromBasket(int id);
    }
}
