using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TakeawayOrder.Models;

namespace TakeawayOrder.Controllers
{
    public class CustomersController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public CustomersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Index()
        {
            // if app has users, list all users and roles for admin to see
            List<UserWithRoleViewModel> usersWithRoles = new List<UserWithRoleViewModel>();
            var users = _userManager.Users.ToList();

            // users view model to get roles per user
            foreach (var user in users)
            {
                UserWithRoleViewModel userWithRole = new UserWithRoleViewModel();

                userWithRole.UserId = user.Id;
                userWithRole.FullName = user.FullName;
                userWithRole.Username = user.UserName;
                userWithRole.Email = user.Email;
                var roles = await _userManager.GetRolesAsync(user);
                userWithRole.Role = roles.FirstOrDefault();
                userWithRole.CheckoutTotal = user.CheckoutCount;

                if (roles.FirstOrDefault() == "Customer")
                {
                    usersWithRoles.Add(userWithRole);
                }
            }

            return View(usersWithRoles);
        }
        [Authorize(Roles = "Staff,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel userCreateVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser newUser = new ApplicationUser
                {
                    FullName = userCreateVM.FullName,
                    UserName = userCreateVM.UserName,
                    Email = userCreateVM.Email
                };
                IdentityResult result = await _userManager.CreateAsync(newUser, userCreateVM.Password);

                // all users created by admin are staff
                result = await _userManager.AddToRoleAsync(newUser, "Customer");

                if (result.Succeeded)
                {
                    return Redirect("/Customers");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(userCreateVM);
        }

        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            UserEditViewModel userEdit = new(user);

            return View(userEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel userEditVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userToEdit = await _userManager.FindByIdAsync(userEditVM.Id);
                userToEdit.UserName = userEditVM.UserName;
                userToEdit.Email = userEditVM.Email;
                userToEdit.FullName = userEditVM?.FullName;

                IdentityResult result = await _userManager.UpdateAsync(userToEdit);

                if (result.Succeeded && !String.IsNullOrEmpty(userEditVM.Password))
                {
                    await _userManager.RemovePasswordAsync(userToEdit);
                    result = await _userManager.AddPasswordAsync(userToEdit, userEditVM.Password);
                }

                if (result.Succeeded)
                {
                    return Redirect("/Customers");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(userEditVM);
        }

        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return Redirect("/Customers");

        }
    }
}
