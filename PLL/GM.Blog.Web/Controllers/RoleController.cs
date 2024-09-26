using AutoMapper;
using GM.Blog.BLL.ViewModels.Roles.Request;
using GM.Blog.BLL.ViewModels.Roles.Response;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GM.Blog.Web.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>Страница отображения всех ролей (получение ролей указанного пользователя) </summary>
        [HttpGet]
        [Route("GetRoles/{userId?}")]
        public async Task<IActionResult> GetRoles([FromRoute] Guid? userId)
        {
            var model = new RolesViewModel();
            if (userId == null)
                model.Roles = await _roleManager.Roles.ToListAsync();
            else
            {
                var user = await _userManager.Users.Include(u => u.Roles)
                  .FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null) return NotFound();
                model.Roles = user.Roles;
            }
            return View(model);
        }

        /// <summary>Страница создания роли</summary>
        [HttpGet]
        [Route("CreateRole")]
        public IActionResult Create() => View();

        /// <summary>Создание роли</summary>
        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> Create(RoleCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = _mapper.Map<Role>(model);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                    return RedirectToAction("GetRoles");
                else
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

            }
            else
                ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось создать роль!");

            return View(model);
        }


        /// <summary> Страница редактирования роли</summary>
        [HttpGet]
        [Route("EditRole")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) return NotFound();

            var model=_mapper.Map<RoleEditViewModel>(role);

            return View(model);
        }

        /// <summary>Редактирование роли </summary>
        [HttpPost]
        [Route("EditRole")]
        public async Task<IActionResult> Edit(RoleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id.ToString());
                if (role != null)
                {
                    _mapper.Map(model, role);

                    var result = await _roleManager.UpdateAsync(role);
                    if(result.Succeeded)
                        return RedirectToAction("GetRoles");
                    else
                        foreach (var error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                }
                else
                    ModelState.AddModelError(string.Empty, $"Ошибка! ");
            }
            return View(model);
        }

        /// <summary>Удаление роли</summary>
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) return NotFound();

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded) return BadRequest();

            return RedirectToAction("GetRoles");
        }

        /// <summary>Страница отображения указанной роли</summary>
        [HttpGet]
        [Route("/ViewRole/{id}")]
        public async Task<IActionResult> View([FromRoute] Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) return NotFound();

            var model =_mapper.Map<RoleViewModel>(role);

            return View(model);
        }
    }
}
