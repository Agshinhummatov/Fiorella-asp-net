using EntityFramework_Slider.Data;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;

namespace EntityFramework_Slider.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IProductService _productService;

        public CartController(AppDbContext context,
                              IBasketService basketService,
                              IProductService productService)
        {
            _context = context;
            _basketService = basketService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {

            List<BasketVM> basketProducts = _basketService.GetBasketDatas();

            List<BasketDetailVM> basketDetails = new();


            foreach (var product in basketProducts)
            {
                var dbProduct = await _productService.GetFullDataById(product.Id);
                basketDetails.Add(new BasketDetailVM
                {
                    Id = dbProduct.Id,
                    Name = dbProduct.Name,
                    CategoryName = dbProduct.Category.Name,
                    Description = dbProduct.Description,
                    Price = dbProduct.Price,
                    Image = dbProduct.Images.Where(m => m.IsMain).FirstOrDefault().Image,
                    Count = product.Count,
                    Total = dbProduct.Price * product.Count
                });
            }

            return View(basketDetails);
        }

        [ActionName("Delete")]
        public IActionResult DeleteProductFromBasket(int? id)
        {
            if (id is null) return BadRequest();
            _basketService.DeleteProductFromBasket((int)id);
            return RedirectToAction("Index");
        }
    }
}
