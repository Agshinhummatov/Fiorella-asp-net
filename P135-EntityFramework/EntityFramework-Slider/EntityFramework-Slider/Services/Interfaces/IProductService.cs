using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetById(int id);
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetFullDataById(int id);
        Task<List<Product>> GetPaginatedDatas(int page, int take);
        Task<int> GetCountAsync();

    }
}
