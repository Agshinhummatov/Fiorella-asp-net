using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }


        [HttpGet]
        public async Task<IActionResult> AddRoleToUser()
        {
            ViewBag.users = await GetUsersAsync();
            ViewBag.roles = await GetRolesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoleToUser(UserRoleVM model)
        {
            ViewBag.users = await GetUsersAsync();
            ViewBag.roles = await GetRolesAsync();

            AppUser user = await _userManager.FindByIdAsync(model.UserId);

            IdentityRole role = await _roleManager.FindByIdAsync(model.RoleId);

            await _userManager.AddToRoleAsync(user, role.Name);

            return View();
        }


        private async Task<SelectList> GetUsersAsync()
        {
            IEnumerable<AppUser> users = await _userManager.Users.ToListAsync();
            return new SelectList(users, "Id", "FullName");
        }

        private async Task<SelectList> GetRolesAsync()
        {
            IEnumerable<IdentityRole> roles = await _roleManager.Roles.ToListAsync();
            return new SelectList(roles, "Id", "Name");
        }
    }
}
