using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Entities;
using ShoppingList.Models;
using ShoppingList.Services;

namespace ShoppingList.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AuthController : Controller
	{
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
		private readonly ListContext _context;

		public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ListContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string? ReturnUrl)
        {
            if(_roleManager.Roles.Count() == 0)
            {
                var fullname = "admin";
                var email = "admin@admin.com";
                var role = "Admin";

                var result1 = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                var result2 = await _roleManager.CreateAsync(new IdentityRole("User"));

                if (result1.Succeeded && result2.Succeeded)
                {
                    if (await _userManager.FindByNameAsync(fullname) == null)
                    {
                        var user = new User()
                        {
                            UserName = "admin",
                            FullName = fullname,
                            Email = email,
                        };

                        var result3 = await _userManager.CreateAsync(user, "Admin123");

                        if (result3.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }
                    
                }

                
            } 
            

            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Bu kullanıcı adı kayıtlı değil.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                if (user.FullName == "admin")
                {
                    return Redirect("/admin/desk");
                }
                model.ReturnUrl = "/list/getlists/" + user.Id;
                return Redirect(model.ReturnUrl ?? "/");
            }

            ModelState.AddModelError("", "Kullanıcı adı veya parola yanlış");

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                UserName = model.Email,
                FullName = model.FullName,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");

                return RedirectToAction("Login", "Auth");
            }

            ModelState.AddModelError("", "Bir hata oluştu.Lütfen Tekrar Deneyiniz");
            return View();
        }

        public async Task<IActionResult> Logout(string? ReturnUrl)
        {
            await _signInManager.SignOutAsync();

            return Redirect("/");
        }

        public IActionResult AccessDenied()
        {
            TempData["Message"] = "Sayfaya erişmek için yetkiniz yok";
            return Redirect("/auth/logout");
        }

	}
}
