using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Roles.Request;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.Web.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>Страница отображения всех ролей пользователя </summary>
        [HttpGet]
        [Route("GetRoles/{userId?}")]
        public async Task<IActionResult> GetRoles([FromRoute] Guid? userId)
        {
            var model = await _roleService.GetRolesAsync(userId);
            if (model == null) return NotFound();

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
                var check = await _roleService.CheckRoleAsync(model.Name);
                if (string.IsNullOrEmpty(check))
                {
                    var result = await _roleService.CreateRoleAsync(model);
                    if (result.Succeeded)
                        return RedirectToAction("GetRoles");
                    else
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                }
                else
                    ModelState.AddModelError(string.Empty, check);
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
            var model = await _roleService.GetRoleEditAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        /// <summary>Редактирование роли </summary>
        [HttpPost]
        [Route("EditRole")]
        public async Task<IActionResult> Edit(RoleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var check = await _roleService.CheckRoleAsync(model.Name);
                if (string.IsNullOrEmpty(check))
                {
                    var result = await _roleService.UpdateRoleAsync(model);
                    if (result.Succeeded)
                        return RedirectToAction("GetRoles");
                    else
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                }
                else
                    ModelState.AddModelError(string.Empty, check);
            }
            else
                ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось создать роль!");

            return View(model);
        }

        /// <summary>Удаление роли</summary>
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result.Succeeded) return BadRequest();

            return RedirectToAction("GetRoles");
        }

        /// <summary>Страница отображения указанной роли</summary>
        [HttpGet]
        [Route("/ViewRole/{id}")]
        public async Task<IActionResult> View([FromRoute] Guid id)
        {
            var model = await _roleService.GetRoleAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }
    }
}
