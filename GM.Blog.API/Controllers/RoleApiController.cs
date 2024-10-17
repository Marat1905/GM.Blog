using AutoMapper;
using GM.Blog.BLL.Extensions;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Roles.Request;
using GM.Blog.BLL.ViewModels.Roles.Response;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.API.Controllers
{
    /// <summary>
    /// Контроллер ролей
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RoleApiController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleApiController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        /// <summary>
        /// Получение объекта роли
        /// </summary>
        /// <remarks>Данный метод позволяет получить роль по её идентификатору</remarks>
        /// <param name="id">Идентификатор роли</param>
        /// <response code="200">Получение объекта роли</response>
        /// <response code="404">Не удалось найти роль по указанному идентификатору</response>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType<RoleViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var role = await _roleService.GetRoleAsync(id);
            if (role == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Роль не найдена!");

            return StatusCode(StatusCodes.Status200OK, role);
        }

        /// <summary>
        /// Получение всех ролей. Возможна фильтрация по идентификатору пользователя
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет получить список всех ролей. Так же, возможно получить
        /// список ролей определённого пользователя при указании его идентификатора
        /// </remarks>
        /// <param name="userId">
        /// Идентификатор пользователя. Указать для получения списка ролей определённого пользователя.
        /// Не указывать для получения полного списка ролей
        /// </param>
        /// <response code="200">Получение списка ролей</response>
        [HttpGet]
        [ProducesResponseType<RoleViewModel[]>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] Guid? userId)
        {
            var list = new List<RoleViewModel>();

            if (userId != null)
                list = _mapper.Map<List<RoleViewModel>>(await _roleService.GetRolesByUserAsync((Guid)userId).ToListAsync());
            else
                list = _mapper.Map<List<RoleViewModel>>(await _roleService.GetAllRolesAsync().ToListAsync());

            return StatusCode(StatusCodes.Status200OK, list);
        }

        /// <summary>
        /// Создание роли
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет создать новую роль. Подробное описание свойств  -  см. схему RoleApiCreateModel
        /// </remarks>
        /// <response code="200">Роль успешно создана</response>
        /// <response code="400">Ошибка при создании роли</response>
        /// <response code="409">Имя новой роли совпадает с именем существующей роли</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleCreateViewModel model)
        {
            var message = await _roleService.CheckRoleAsync(model.Name);
            if (message != string.Empty)
                return StatusCode(StatusCodes.Status409Conflict, message);

            var result = await _roleService.CreateRoleAsync(model);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status400BadRequest, $"Произошла ошибка при создании роли!");

            return StatusCode(StatusCodes.Status200OK, $"Роль успешно создана.");
        }

        /// <summary>
        /// Обновление роли
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет обновить существующую роль. Подробное описание свойств  -  см. схему RoleEditViewModel
        /// </remarks>
        /// <response code="200">Получение модели роли с обновленными данными</response>
        /// <response code="400">Ошибка при обновлении роли</response>
        /// <response code="404">Не найдена роли по указанному идентификатору</response>
        [HttpPut]
        [ProducesResponseType<RoleEditViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] RoleEditViewModel model)
        {
            var role = await _roleService.GetRoleByIdAsync(model.Id);
            if (role == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Роль не найдена!");

            var result = await _roleService.UpdateRoleAsync(model);
            if (!result.Succeeded)
                return StatusCode(400, $"Произошла ошибка при обновлении роли!");

            return StatusCode(StatusCodes.Status200OK, _mapper.Map<RoleEditViewModel>(role));
        }

        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет удалить роль по её идентификатору.
        /// </remarks>
        /// <param name="id">Идентификатор роли, которую необходимо удалить</param>
        /// <response code="200">Получение модели удалённой роли</response>
        /// <response code="400">Ошибка при удалении роли</response>
        /// <response code="404">Не найдена роли по указанному идентификатору</response>
        /// <response code="409">Удаление стандартной роли невозможно</response>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(RoleViewModel), 200)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return StatusCode(404, $"Роль не найдена!");

            var result = await _roleService.DeleteRoleAsync(id);
            if (!result.Succeeded)
                return StatusCode(400, $"Произошла ошибка при удалении роли!");

            return StatusCode(200, _mapper.Map<RoleViewModel>(role));
        }
    }
}
