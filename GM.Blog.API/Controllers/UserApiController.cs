using AutoMapper;
using GM.Blog.BLL.Extensions;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Users.Request;
using GM.Blog.BLL.ViewModels.Users.Response;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.API.Controllers
{
    /// <summary>
    /// Контроллер пользователей
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UserApiController(IUserService userService, IRoleService roleService, IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
        }

        /// <summary>
        /// Получение объекта пользователя. Получение списка всех пользователей.
        /// </summary>
        /// <remarks>Данный метод возвращает массив пользователей</remarks>
        /// <param name="id">ID пользователя. Оставить пустым для получения полного списка пользователей</param>

        [HttpGet]
        [ProducesResponseType<UserViewModel[]>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromQuery] Guid? id)
        {
            var response = new List<UserViewModel>();

            if (id == null)
                response = _mapper.Map<List<UserViewModel>>(await _userService.GetAllUsersAsync());
            else
            {
                var user = await _userService.GetUserByIdAsync((Guid)id);
                if (user == null)
                    return StatusCode(StatusCodes.Status404NotFound, $"Пользователь не найден!");

                response.Add(_mapper.Map<UserViewModel>(user));
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет создать нового пользователя
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] UserCreateViewModel model)
        {
            var messages = new List<string>();
            messages.AddRange(await _userService.CheckDataForCreateUserAsync(model));
            messages.AddRange(await _roleService.CheckRolesForUserChanged(model.Roles));

            if (messages.Count == 0)
            {
                var result = await _userService.CreateUserAsync(model, await _roleService.ConvertRoleNamesInRoles(model.Roles).ToListAsync());
                if (!result.Success.Succeeded)
                    return StatusCode(StatusCodes.Status400BadRequest, $"Произошла ошибка при создании пользователя!");

                return StatusCode(StatusCodes.Status200OK, $"Пользователь успешно создан.");
            }

            return StatusCode(StatusCodes.Status409Conflict, messages);
        }

        /// <summary>
        /// Обновление пользователя
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет обновлять информацию существующего пользователя. Подробное описание свойств  -  см. схему UserEditViewModel
        /// </remarks>
        [HttpPut]
        [ProducesResponseType<UserEditViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update([FromBody] UserEditViewModel model)
        {
            var resultApi = await _userService.CheckDataForEditUserAsync(model);
            if (resultApi.User == null)
                return StatusCode(StatusCodes.Status404NotFound, resultApi.Messages.ToArray()[0]);

            var errors = await _roleService.CheckRolesForUserChanged(model.Roles.ToList());
            if (errors.Count > 0)
                return StatusCode(StatusCodes.Status422UnprocessableEntity, errors);

            var result = await _userService.UpdateUserAsync(model);
            if (result)
                return StatusCode(StatusCodes.Status200OK, _mapper.Map<UserEditViewModel>(resultApi.User));

            return StatusCode(StatusCodes.Status400BadRequest, $"Произошла ошибка при обновлении пользователя!");
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <remarks>Данный метод позволяет удалить пользователя</remarks>
        /// <param name="id">Идентификатор пользователя, которого необходимо удалить</param>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType<UserViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedUser = await _userService.GetUserByIdAsync(id);
            if (deletedUser == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Пользователь не найден!");

            var result = await _userService.DeleteByIdAsync(deletedUser);
            if (!result)
                return StatusCode(StatusCodes.Status400BadRequest, $"Ошибка при удалении пользователя!");

            return StatusCode(StatusCodes.Status200OK, _mapper.Map<UserViewModel>(deletedUser));
        }
    }
}
