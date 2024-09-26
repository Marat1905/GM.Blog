using AutoMapper;
using Azure.Core;
using GM.Blog.BLL.ViewModels.Users.Request;
using GM.Blog.BLL.ViewModels.Users.Response;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GM.Blog.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserController(IMapper mapper, ILogger<UserController> logger, SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
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
                var user = _mapper.Map<User>(model);

                var defaultRole = await _roleManager.FindByNameAsync("User");

                user.Roles ??= new List<Role>();

                user.Roles.Add(defaultRole);

                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var claims = new List<Claim>();

                    foreach (var role in user.Roles)
                        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name!));
                    claims.Add(new Claim("UserID", userId));

                    await _signInManager.SignInWithClaimsAsync(user, false, claims);

                    HttpContext.Session.SetString("username", user!.UserName ?? "");

                    return RedirectToAction("Index", "Home");

                }
                else
                    foreach (var error in result.Errors)
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
                var user = await _userManager.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == model.UserEmail);
                if(user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user!, model.Password, false);
                    if (result.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        var claims = new List<Claim>();

                        foreach (var role in user.Roles)
                            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name!));
                        claims.Add(new Claim("UserID", userId));

                        await _signInManager.SignInWithClaimsAsync(user!, false, claims);

                        HttpContext.Session.SetString("username", user!.UserName ?? "");

                        var claimsId = claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
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
            var users = _userManager.Users.ToList();
            var model = new UsersViewModel
            {
                Users = await _userManager.Users.Include(u => u.Roles).ToListAsync()
            };

            if (roleId != null)
                model.Users = model.Users
                    .SelectMany(u => u.Roles, (u, r) => new { User = u, RoleId = r.Id })
                    .Where(o => o.RoleId == roleId).Select(o => o.User).ToList();

            return View(model);
        }

        /// <summary> Страница регистрации пользователя</summary>
        [HttpGet]
        public IActionResult Create() => View();


        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(model);

                var defaultRole = await _roleManager.FindByNameAsync("User");

                user.Roles ??= new List<Role>();

                user.Roles.Add(defaultRole);

                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");

                }
                else
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
            }
            return View(model);
        }


        /// <summary>
        /// Страница отображения профиля пользователя
        /// </summary>
        [HttpGet]
        [Route("ViewUser/{id}")]
        public async Task<IActionResult> View([FromRoute] Guid id)
        {
            var user = await _userManager.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);

            var model = _mapper.Map<UserViewModel>(user);

             if (model == null) return NotFound();

            return View(model);
        }


        /// <summary>Страница редактирования пользователя</summary>
        [Authorize]
        [HttpGet]
        [Route("EditUser/{id?}")]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
           var user = await _userManager.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return new NotFoundResult();

            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;

            if(User.IsInRole("Admin")|| user.Id.ToString()== userId)
            {
                var model = _mapper.Map<UserEditViewModel>(user);
                model.AllRoles = (await _roleManager.Roles.ToListAsync()).Select(r=>r.Name!).ToList();
                return View(model);
            }
            return Forbid();

        }

        /// <summary>Редактирование пользователя</summary>
        [Authorize]
        [HttpPost]
        [Route("EditUser/{id}")]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roles = new List<Role>();
                foreach (var pair in this.Request.Form)
                {
                    if (pair.Value == "on")
                    {
                        var role = await _roleManager.FindByNameAsync(pair.Key);
                        if (role != null)
                        {
                            roles.Add(role);
                        }
                           
                    }
                }

                var user= await _userManager.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == model.Id);
                //var user = await _userManager.FindByIdAsync(model.Id.ToString());
               
                if (user == null)
                    return new NotFoundResult();
                
                _mapper.Map(model, user);

                if(roles.Count!=0)
                    user.Roles=roles.Distinct().ToList();

                //var roleUser = await _roleManager.FindByNameAsync("User");
                 //var roleAdmin = await _roleManager.FindByNameAsync("Moderator");
               // var roleAdmin = await _roleManager.FindByNameAsync("Admin");
               //user.Roles.Add(roleUser);
               // user.Roles.Add(roleAdmin);
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var updateSecurityStampRes = await _userManager.UpdateSecurityStampAsync(user);
                    return RedirectToAction("Index", "Home", new { model.ReturnUrl });
                }             
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
            model.Roles ??= (await _roleManager.Roles.Include(r => r.Users)
                .SelectMany(r => r.Users, (r, u) => new { Role = r, UserId = u.Id })
                .Where(o => o.UserId == model.Id).Select(o => o.Role).ToListAsync()).Select(r=>r.Name).ToList();

            model.AllRoles ??= (await _roleManager.Roles.ToListAsync()).Select(r => r.Name!).ToList();
            return View(model);
        }

        /// <summary> Удаление пользователя</summary>
        [HttpPost]
        public async Task<IActionResult> Remove(Guid id, [FromForm] Guid? userId)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return new NotFoundResult();

            var result = await _userManager.DeleteAsync(user);

           if (!result.Succeeded) return BadRequest();

            if (User.IsInRole("Admin"))
                return RedirectToAction("GetUsers");

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
