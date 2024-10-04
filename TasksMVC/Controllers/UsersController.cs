using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using TasksMVC.Models;
using TasksMVC.Services;

namespace TasksMVC.Controllers
{
    public class UsersController: Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
		private readonly ApplicationDbContext context;

		public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
			this.context = context;
		}

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser() { Email = model.Email, UserName = model.Email };

            var result = await userManager.CreateAsync(user, password: model.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
        }

		[AllowAnonymous]
		public IActionResult Login(string message = null)
		{
            if (message is not null)
            {
                ViewData["Message"] = message;
            }

			return View();
		}

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult ExtLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("RegisterExtUser", values: new {returnUrl});
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterExtUser(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var message = "";

            if (remoteError is not null)
            {
                message = $"Error del provedor externo: {remoteError}";
                return RedirectToAction("login", routeValues: new { message });
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                message = "Error cargando los datos del login externo";
				return RedirectToAction("login", routeValues: new { message });
			}

            var ExtLoginResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (ExtLoginResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            string email = "";

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                message = "Error leyendo el email del usuario del proveedor";
				return RedirectToAction("login", routeValues: new { message });
			}

            var user = new IdentityUser { Email = email, UserName = email };

            var createdUserResult = await userManager.CreateAsync(user);

            if (!createdUserResult.Succeeded)
            {
                message = createdUserResult.Errors.First().Description;
				return RedirectToAction("login", routeValues: new { message });
			}

            var resultAddLogin = await userManager.AddLoginAsync(user, info);

            if (resultAddLogin.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: true, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            message = "Ha ocurrido un error agregando el login";
			return RedirectToAction("login", routeValues: new { message });
		}

        [HttpGet]
        [Authorize(Roles = CONSTANTS.AdminRole)]
        public async Task<IActionResult> UsersList(string message = null)
        {
            var users = await context.Users.Select(u => new UserVM
            {
                Email = u.Email
            }).ToListAsync();

            var model = new UsersListVM();
            model.Users = users;
            model.Message = message;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = CONSTANTS.AdminRole)]
        public async Task<IActionResult> MakeAdmin(string email)
        {
            var user = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user is null)
            {
                return NotFound();
            }

            await userManager.AddToRoleAsync(user, CONSTANTS.AdminRole);

            return RedirectToAction("UsersList", routeValues: new { message = "Rol asignado correctamente a " + email });
        }

		[HttpPost]
        [Authorize(Roles = CONSTANTS.AdminRole)]
        public async Task<IActionResult> RemoveAdmin(string email)
		{
			var user = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
			if (user is null)
			{
				return NotFound();
			}

			await userManager.RemoveFromRoleAsync(user, CONSTANTS.AdminRole);

			return RedirectToAction("UsersList", routeValues: new { message = "Rol quitado correctamente a " + email });
		}
	}
}
