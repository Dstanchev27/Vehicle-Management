namespace VMAPP.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using VMAPP.Common;
    using VMAPP.Data.Models;
    using VMAPP.Web.Areas.Administration.Models;

    [Area("Administration")]
    [Authorize(Roles = GlobalConstant.AdministratorRoleName)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
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
                Roles = roles
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateSelectLists();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                City = model.City,
                Address = model.Address,
                EmailConfirmed = true,
                TwoFactorEnabled = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                PopulateSelectLists();
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
            if (!roleResult.Succeeded)
            {
                foreach (var e in roleResult.Errors)
                    ModelState.AddModelError("", e.Description);
                await _userManager.DeleteAsync(user);
                PopulateSelectLists();
                return View(model);
            }

            _logger.LogInformation("User '{Email}' was successfully created with role '{Role}'.", user.Email, model.SelectedRole);
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
                SelectedRole = roles.FirstOrDefault() ?? ""
            };

            PopulateSelectLists();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateSelectLists();
                return View(model);
            }

            var u = await _userManager.FindByIdAsync(model.Id);
            if (u == null || u.IsDeleted) return NotFound();

            u.Email = model.Email;
            u.UserName = model.Email;
            u.City = model.City;
            u.Address = model.Address;

            var currentRoles = await _userManager.GetRolesAsync(u);
            if (currentRoles.Any())
                await _userManager.RemoveFromRoleAsync(u, currentRoles.First());

            await _userManager.AddToRoleAsync(u, model.SelectedRole);
            await _userManager.UpdateAsync(u);

            _logger.LogInformation("User '{Email}' was successfully updated with role '{Role}'.", u.Email, model.SelectedRole);
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
                _logger.LogInformation("User '{Email}' was successfully deleted.", u.Email);
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateSelectLists()
        {
            ViewBag.Roles = _roleManager.Roles
                .Select(r => r.Name)
                .ToList()
                .Select(r => new SelectListItem(r, r));
        }
    }
}
