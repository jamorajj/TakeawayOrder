using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TakeawayOrder.Models;

namespace TakeawayOrder.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            // check if there are users, if none, redirect to initialize
            int numUsers = _userManager.Users.ToList().Count();

            if (numUsers == 0)
            {
                return Redirect("/Admin/Users/Initialize");
            }

            if (!User.IsInRole("Admin"))
            {
                return Redirect("/");
            }

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

                usersWithRoles.Add(userWithRole);
            }

            return View(usersWithRoles);
        }

        // will be only used to create the first user
        public IActionResult Initialize()
        {
            int numUsers = _userManager.Users.ToList().Count();

            // check if there are users, if yes, then there is already an admin
            if (numUsers > 0)
            {
                TempData["val"] = "Not allowed";

                return Redirect("/Account/Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Initialize(User user)
        {
            int numUsers = _userManager.Users.ToList().Count();

            if (ModelState.IsValid)
            {
                ApplicationUser newUser = new ApplicationUser { 
                    UserName = user.UserName,
                    Email = user.Email, 
                    FullName = user.UserName,
                };
                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                if (numUsers == 0)
                {
                    // initialize other roles
                    result = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    result = await _roleManager.CreateAsync(new IdentityRole("Staff"));
                    result = await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    result = await _userManager.AddToRoleAsync(newUser, "Admin");
                }

                if (result.Succeeded)
                {
                    return Redirect("/Account/Login");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel userCreateVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser newUser = new ApplicationUser {
                    FullName = userCreateVM.FullName,
                    UserName = userCreateVM.UserName,
                    Email = userCreateVM.Email
                };
                IdentityResult result = await _userManager.CreateAsync(newUser, userCreateVM.Password);

                // all users created by admin are staff
                result = await _userManager.AddToRoleAsync(newUser, "Staff");

                if (result.Succeeded)
                {
                    return Redirect("/Admin");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(userCreateVM);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

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
                    return RedirectToAction("Index");
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

            if (user == null)
            {
                return NotFound();
            }

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");

        }
    }
}
