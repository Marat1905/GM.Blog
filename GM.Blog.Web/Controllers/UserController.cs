using AutoMapper;
using GM.Blog.BLL.Extensions;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Users.Request;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GM.Blog.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly SignInManager<User> _signInManager;

        public UserController( ILogger<UserController> logger, IUserService userService, IRoleService roleService ,SignInManager<User> signInManager)
        {
            _logger = logger;
            _userService = userService;
            _roleService = roleService;
            _signInManager = signInManager;
            logger.LogInformation(nameof(UserController));
        }


        /// <summary>Страница регистрации пользователя</summary>
        [HttpGet]
        [Route("Register")]
        public IActionResult Register() => View();


        /// <summary>Регистрация пользователя</summary>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUserAsync(model);
                if (result.Success!.Succeeded)
                {
                    await _signInManager.SignInWithClaimsAsync(result.User, false, await _userService.GetUserClaimsAsync(result.User).ToListAsync());

                    HttpContext.Session.SetString("username", result.User!.UserName ?? "");

                    return RedirectToAction("Index", "Home");
                }
                else
                    foreach (var error in result.Success.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
            }

            return View(model);
        }




        /// <summary>Страница авторизации пользователя</summary>
        [HttpGet]
        [Route("Login")]
        public IActionResult Login() => View();

        /// <summary>Авторизация пользователя</summary>
        [HttpPost]
        [Route("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.CheckDataForLoginAsync(model);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user!, model.Password, false);
                    if (result.Succeeded)
                    {
                        var claims = await _userService.GetUserClaimsAsync(user!).ToListAsync();
                        await _signInManager.SignInWithClaimsAsync(user!, false, claims);

                        HttpContext.Session.SetString("username", user!.UserName ?? "");

                        var userId = claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl) && userId == Request.Query["UserId"])
                            return Redirect(model.ReturnUrl);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError(string.Empty, "Неверный email или(и) пароль!");
                }
                else
                    ModelState.AddModelError(string.Empty, "Неверный email или(и) пароль!");
            }
            return View(model);
        }

        /// <summary>Выход пользователя из системы </summary>
        [Authorize]
        [HttpPost]
        [Route("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout([FromQuery] string returnUrl)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", new { ReturnUrl = returnUrl, UserId = userId });
        }


        /// <summary>Страница всех пользователей (получение пользователей с указанной ролью)</summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetUsers/{roleId?}")]
        public async Task<IActionResult> GetUsers([FromRoute] Guid? roleId)
        {
            var model = await _userService.GetUsersAsync(roleId);
            return View(model);
        }

        /// <summary> Страница регистрации пользователя</summary>
        [HttpGet]
        public async Task<IActionResult> Create() => View(new UserCreateViewModel(await _roleService.GetAllRolesAsync().ToListAsync()));


        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Roles = (await _roleService.GetEnabledRoleNamesWithRequest(this.Request).ToListAsync()).Select(r => r.Name!).ToList();
                var result = await _userService.CreateUserAsync(model, await _roleService.ConvertRoleNamesInRoles(model.Roles).ToListAsync());

                if (result.Success.Succeeded)
                    return RedirectToAction("GetUsers");
                else
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при создании пользователя!");
            }

            model.AllRoles ??= (await _roleService.GetAllRolesAsync().ToListAsync()).Select(r => r.Name!).ToList();
            return View(model);
        }


        /// <summary>
        /// Страница отображения профиля пользователя
        /// </summary>
        [HttpGet]
        [Route("ViewUser/{id}")]
        public async Task<IActionResult> View([FromRoute] Guid id)
        {
            var model = await _userService.GetUserAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }


        /// <summary>Страница редактирования пользователя</summary>
        [Authorize]
        [HttpGet]
        [Route("EditUser/{id?}")]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            var result = await _userService.GetUserEditAsync(id, userId, User.IsInRole("Admin"));

            if (result.Model == null) return result.Result!;

            return View(result.Model);
        }

        /// <summary>Редактирование пользователя</summary>
        [Authorize]
        [HttpPost]
        [Route("EditUser/{id}")]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Roles = (await _roleService.GetEnabledRoleNamesWithRequest(this.Request).ToListAsync()).Select(r => r.Name!).ToList();
                var result = await _userService.UpdateUserAsync(model);

                if (result)
                    return RedirectToAction("Index", "Home", new { model.ReturnUrl });
                else
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при обновлении пользователя!");
            }

            model.Roles ??= (await _roleService.GetRolesByUserAsync(model.Id).ToListAsync()).Select(r => r.Name!).ToList();
            model.AllRoles ??= (await _roleService.GetAllRolesAsync().ToListAsync()).Select(r => r.Name!).ToList();

            return View(model);
        }

        /// <summary> Удаление пользователя</summary>
        [HttpPost]
        public async Task<IActionResult> Remove(Guid id, [FromForm] Guid? userId)
        {
            var result = await _userService.DeleteByIdAsync(id, userId, User.IsInRole("Admin"));
            if (!result) return BadRequest();

            if (User.IsInRole("Admin"))
                return RedirectToAction("GetUsers");

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
