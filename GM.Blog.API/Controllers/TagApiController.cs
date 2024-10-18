using AutoMapper;
using GM.Blog.BLL.Extensions;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Roles.Response;
using GM.Blog.BLL.ViewModels.Tags.Request;
using GM.Blog.BLL.ViewModels.Tags.Response;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.API.Controllers
{
    /// <summary>
    /// Контроллер тегов
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TagApiController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;

        public TagApiController(ITagService tagService, IMapper mapper)
        {
            _tagService = tagService;
            _mapper = mapper;
        }

        /// <summary>
        /// Получение объекта тега
        /// </summary>
        /// <remarks>Данный метод позволяет получить тег по её идентификатору</remarks>
        /// <param name="id">Идентификатор роли</param>
        /// <response code="200">Получение объекта тега</response>
        /// <response code="404">Не удалось найти тег по указанному идентификатору</response>
        [HttpGet]
        [ProducesResponseType<RoleViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var tag = await _tagService.GetTagAsync(id);
            if (tag == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Не удалось найти тег по указанному идентификатору");

            return StatusCode(StatusCodes.Status200OK, tag);
        }


        /// <summary>
        /// Получение всех тегов. Возможна фильтрация по идентификатору тега
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет получить список всех тегов. Так же, возможно получить
        /// список тегов определённого поста при указании его идентификатора
        /// </remarks>
        /// <param name="postId">
        /// Идентификатор тега. Указать для получения списка тегов определённого пользователя.
        /// Не указывать для получения полного списка тегов
        /// </param>
        /// <response code="200">Получение списка тегов</response>
        [HttpGet]
        [ProducesResponseType<TagViewModel[]>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] Guid? postId)
        {
            var list = new List<TagViewModel>();

            if (postId != null)
                list = _mapper.Map<List<TagViewModel>>(await _tagService.GetTagByPostAsync((Guid)postId).ToListAsync());
            else
                list = _mapper.Map<List<TagViewModel>>(await _tagService.GetAllTagsAsync().ToListAsync());

            return StatusCode(StatusCodes.Status200OK, list);
        }

        /// <summary>
        /// Создание тега
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет создать новый тег. Подробное описание свойств  -  см. схему TagApiCreateModel
        /// </remarks>
        /// <response code="200">Тег успешно создан</response>
        /// <response code="400">Ошибка при создании тега</response>
        /// <response code="409">Имя нового тега совпадает с именем существующего тега</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] TagCreateViewModel model)
        {
            var message = await _tagService.CheckTagNameAsync(model.Name);

            if (string.IsNullOrEmpty(message))
            {
                var result = await _tagService.CreateTagAsync(model);
                if (!result)
                    return StatusCode(StatusCodes.Status400BadRequest, $"Ошибка при создании тега!");

                return StatusCode(StatusCodes.Status200OK, $"Тег успешно создан");
            }

            return StatusCode(StatusCodes.Status409Conflict, message);
        }


        /// <summary>
        /// Обновление тега
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет обновить существующий тег. Подробное описание свойств  -  см. схему RoleEditViewModel
        /// </remarks>
        /// <response code="200">Получение модели тега с обновленными данными</response>
        /// <response code="400">Ошибка при обновлении тега</response>
        /// <response code="404">Не найден тег по указанному идентификатору</response>
        /// <response code="409">Обновление тега невозможно</response>
        [HttpPut]
        [ProducesResponseType<TagEditViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update([FromBody] TagEditViewModel model)
        {
            var tag = await _tagService.GetTagEditAsync(model.Id);
            if (tag == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Не найден тег по указанному идентификатору");

            var message = await _tagService.CheckTagNameAsync(model.Name);

            if (string.IsNullOrEmpty(message))
            {
                var result = await _tagService.UpdateTagAsync(model);
                if (!result)
                    return StatusCode(StatusCodes.Status400BadRequest, $"Ошибка при обновлении тега");

                return StatusCode(StatusCodes.Status200OK, model);
            }

            return StatusCode(StatusCodes.Status409Conflict, message);
        }

        /// <summary>
        /// Удаление тега
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет удалить тег по её идентификатору.
        /// </remarks>
        /// <param name="id">Идентификатор тега, которую необходимо удалить</param>
        /// <response code="200">Получение модели удалённого тега</response>
        /// <response code="400">Ошибка при удалении тега</response>
        /// <response code="404">Не найден тег по указанному идентификатору</response>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType<TagViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var tag = await _tagService.GetTagAsync(id);
            if (tag == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Не найден тег по указанному идентификатору");

            var result = await _tagService.DeleteTagAsync(id);
            if (!result)
                return StatusCode(StatusCodes.Status400BadRequest, $"Ошибка при удалении тега");

            return StatusCode(StatusCodes.Status200OK, tag);
        }
    }
}
