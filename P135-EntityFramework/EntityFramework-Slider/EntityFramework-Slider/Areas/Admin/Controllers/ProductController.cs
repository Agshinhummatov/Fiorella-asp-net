using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public ProductController(IProductService productService,
                                 ICategoryService categoryService,
                                 IWebHostEnvironment env,
                                 AppDbContext context)
        {
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int take = 4)
        {
            List<Product> products = await _productService.GetPaginatedDatas(page,take);

            List<ProductListVM> mappedDatas = GetMappedDatas(products);

            int pageCount = await GetPageCountAsync(take);

            Paginate<ProductListVM> paginatedDatas = new(mappedDatas, page, pageCount);

            ViewBag.take = take;

            return View(paginatedDatas);
        }

        private async Task<int> GetPageCountAsync(int take)
        {
            var productCount = await _productService.GetCountAsync();
            return (int)Math.Ceiling((decimal)productCount / take);
        }

        private List<ProductListVM> GetMappedDatas(List<Product> products)
        {
            List<ProductListVM> mappedDatas = new();

            foreach (var product in products)
            {
                ProductListVM productVM = new()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Count = product.Count,
                    CategoryName = product.Category.Name,
                    MainImage = product.Images.Where(m => m.IsMain).FirstOrDefault()?.Image
                };

                mappedDatas.Add(productVM);
            }

            return mappedDatas;
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewBag.categories = await GetCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM model)
        {
            try
            {
                ViewBag.categories = await GetCategoriesAsync();

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                foreach (var photo in model.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }

                    if (!photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View();
                    }
                }

                List<ProductImage> productImages = new();

                foreach (var photo in model.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    await FileHelper.SaveFileAsync(path, photo);

                    ProductImage productImage = new()
                    {
                        Image = fileName
                    };

                    productImages.Add(productImage);
                }

                productImages.FirstOrDefault().IsMain = true;

                decimal convertedPrice = decimal.Parse(model.Price.Replace(".", ","));

                Product newProduct = new()
                {
                    Name = model.Name,
                    Price = convertedPrice,
                    Count = model.Count,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Images = productImages
                };

                await _context.ProductImages.AddRangeAsync(productImages);
                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }

     
        }

        private async Task<SelectList> GetCategoriesAsync()
        {
            IEnumerable<Category> categories = await _categoryService.GetAll();
            return new SelectList(categories, "Id", "Name");
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Product product = await _productService.GetFullDataById((int)id);

                if (product == null) return NotFound();

                ViewBag.desc = Regex.Replace(product.Description, "<.*?>", String.Empty);

                return View(product);
            }
            catch (Exception)
            {

                throw;
            }
          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            try
            {
                Product product = await _productService.GetFullDataById((int)id);

                foreach (var item in product.Images)
                {
                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", item.Image);

                    FileHelper.DeleteFile(path);

                }

                _context.Products.Remove(product);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Product product = await _productService.GetFullDataById((int)id);

                if (product == null) return NotFound();

                ViewBag.desc = Regex.Replace(product.Description, "<.*?>", String.Empty);

                ProductDetailVM model = new()
                {
                    Name = product.Name,
                    Count = product.Count,
                    Price = product.Price.ToString("0.#####"),
                    CategoryName = product.Category.Name,
                    Images = product.Images.ToList()
                };

                return View(model);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductImage(int? id)
        {
            if (id == null) return BadRequest();

            bool result = false;

            ProductImage productImage = await _context.ProductImages.Where(m => m.Id == id).FirstOrDefaultAsync();

            if (productImage == null) return NotFound();

            var data = await _context.Products.Include(m => m.Images).FirstOrDefaultAsync(m => m.Id == productImage.ProductId);

            if(data.Images.Count > 1)
            {
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", productImage.Image);

                FileHelper.DeleteFile(path);

                _context.ProductImages.Remove(productImage);

                await _context.SaveChangesAsync();

                result = true;
            }

            data.Images.FirstOrDefault().IsMain = true;

            await _context.SaveChangesAsync();

            return Ok(result);

        }


        [HttpGet]       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            ViewBag.categories = await GetCategoriesAsync();

            Product dbProduct = await _productService.GetFullDataById((int)id);

            if (dbProduct == null) return NotFound();


            ProductEditVM model = new()
            {
                Id = dbProduct.Id,
                Name = dbProduct.Name,
                Count = dbProduct.Count,
                Price = dbProduct.Price.ToString("0.#####"),
                CategoryId = dbProduct.CategoryId,
                Images = dbProduct.Images.ToList(),
                Description = dbProduct.Description
            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ProductEditVM updatedProduct)
        {
            if (id == null) return BadRequest();

            ViewBag.categories = await GetCategoriesAsync();

            Product dbProduct =  await _context.Products.AsNoTracking().Include(m => m.Images).Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id);

            if (dbProduct == null) return NotFound();

            if (!ModelState.IsValid)
            {
                updatedProduct.Images = dbProduct.Images.ToList();
                return View(updatedProduct);
            }

            List<ProductImage> productImages = new();

            if (updatedProduct.Photos is not null)
            {
                foreach (var photo in updatedProduct.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        updatedProduct.Images = dbProduct.Images.ToList();
                        return View(updatedProduct);
                    }

                    if (!photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        updatedProduct.Images = dbProduct.Images.ToList();
                        return View(updatedProduct);
                    }
                }

              

                foreach (var photo in updatedProduct.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    await FileHelper.SaveFileAsync(path, photo);

                    ProductImage productImage = new()
                    {
                        Image = fileName
                    };

                    productImages.Add(productImage);
                }

                await _context.ProductImages.AddRangeAsync(productImages);
            }

            decimal convertedPrice = decimal.Parse(updatedProduct.Price.Replace(".", ","));

            Product newProduct = new()
            {
                Id = dbProduct.Id,
                Name = updatedProduct.Name,
                Price = convertedPrice,
                Count = updatedProduct.Count,
                Description = updatedProduct.Description,
                CategoryId = updatedProduct.CategoryId,
                Images = productImages.Count == 0 ? dbProduct.Images : productImages
            };


            _context.Products.Update(newProduct);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
