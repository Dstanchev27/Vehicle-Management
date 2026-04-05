namespace VMAPP.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using VMAPP.Common;
    using VMAPP.Data.Models;
    using VMAPP.Data.Models.Enums;
    using VMAPP.Web.Areas.Administration.Models;

    [Area("Administration")]
    [Authorize(Roles = GlobalConstant.AdministratorRoleName)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();

            var modelList = new List<UserListViewModel>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                modelList.Add(new UserListViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    City = u.City,
                    Address = u.Address,
                    UserType = u.UserType,
                    Roles = roles.ToList()
                });
            }

            return View(modelList);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var u = await _userManager.FindByIdAsync(id);
            if (u == null || u.IsDeleted) return NotFound();

            var roles = await _userManager.GetRolesAsync(u);
            var vm = new UserDetailsViewModel
            {
                Id = u.Id,
                Email = u.Email,
                City = u.City,
                Address = u.Address,
                UserType = u.UserType,
                Roles = roles
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateSelectLists();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectLists();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                City = model.City,
                Address = model.Address,
                UserType = model.UserType
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                await PopulateSelectLists();
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, model.SelectedRole);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var u = await _userManager.FindByIdAsync(id);
            if (u == null || u.IsDeleted) return NotFound();

            var roles = await _userManager.GetRolesAsync(u);
            var vm = new EditUserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                City = u.City,
                Address = u.Address,
                UserType = u.UserType,
                SelectedRole = roles.FirstOrDefault() ?? ""
            };

            await PopulateSelectLists();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectLists();
                return View(model);
            }

            var u = await _userManager.FindByIdAsync(model.Id);
            if (u == null || u.IsDeleted) return NotFound();

            u.Email = model.Email;
            u.UserName = model.Email;
            u.City = model.City;
            u.Address = model.Address;
            u.UserType = model.UserType;

            var currentRoles = await _userManager.GetRolesAsync(u);
            if (currentRoles.Any())
                await _userManager.RemoveFromRoleAsync(u, currentRoles.First());

            await _userManager.AddToRoleAsync(u, model.SelectedRole);
            await _userManager.UpdateAsync(u);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var u = await _userManager.FindByIdAsync(id);
            if (u == null || u.IsDeleted) return NotFound();

            var roles = await _userManager.GetRolesAsync(u);
            var vm = new UserDetailsViewModel
            {
                Id = u.Id,
                Email = u.Email,
                City = u.City,
                Address = u.Address,
                UserType = u.UserType,
                Roles = roles
            };

            return View(vm);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u != null && !u.IsDeleted)
            {
                u.IsDeleted = true;
                u.DeletedOn = DateTime.UtcNow;
                await _userManager.UpdateAsync(u);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateSelectLists()
        {
            ViewBag.Roles = (await _roleManager.Roles
                    .Select(r => r.Name)
                    .ToListAsync())
                .Select(r => new SelectListItem(r, r));

            ViewBag.UserTypes = Enum.GetValues<UserType>()
                .Select(ut => new SelectListItem(ut.ToString(), ut.ToString()));
        }
    }
}
