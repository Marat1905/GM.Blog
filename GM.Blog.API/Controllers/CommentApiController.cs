using AutoMapper;
using GM.Blog.BLL.Extensions;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Comments.Request;
using GM.Blog.BLL.ViewModels.Comments.Response;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.API.Controllers
{
    /// <summary>
    /// Контроллер комментарий
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CommentApiController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentApiController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        /// <summary>
        /// Получение объекта комментария
        /// </summary>
        /// <remarks>Данный метод позволяет получить комментарий по её идентификатору</remarks>
        /// <param name="id">Идентификатор роли</param>
        /// <response code="200">Получение объекта комментария</response>
        /// <response code="404">Не удалось найти комментарий по указанному идентификатору</response>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType<CommentViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Комментарий не найден");

            return StatusCode(StatusCodes.Status200OK, _mapper.Map<CommentViewModel>(comment));
        }


        /// <summary>
        /// Получение всех комментариев. Возможна фильтрация по идентификатору пользователя и статьи
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет получить список всех комментариев. Возможно получить
        /// список комментариев определённого пользователя при указании его идентификатора,
        /// список комментариев с определённым тегом при указании его идентификатора, либо 
        /// применить сразу два фильтра
        /// </remarks>
        /// <param name="userId">
        /// Идентификатор пользователя. Указать для получения списка комментариев определённого пользователя.
        /// Не указывать для получения полного списка комментариев
        /// </param>
        /// <param name="postId">
        /// Идентификатор статьи. Указать для получения списка комментариев с указанной статьей.
        /// Не указывать для получения полного списка комментариев
        /// </param>
        /// <response code="200">Получение списка комментариев</response>
        /// <response code="">Не удалось найти комментарии по указанному идентификатору</response>
        [HttpGet]
        [ProducesResponseType<CommentViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] Guid? postId, [FromQuery] Guid? userId)
        {
            var messages = await _commentService.CheckByIdAsync(postId: postId, userId: userId).ToListAsync();

            if (messages.Count == 0)
            {
                var list = await _commentService.GetCommentsAsync(postId, userId);

                return StatusCode(StatusCodes.Status200OK, _mapper.Map<List<CommentViewModel>>(list.Comments));
            }
            return StatusCode(StatusCodes.Status404NotFound, messages);
        }

        /// <summary>
        /// Создание комментария
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет создать новый комментарий к посту. Подробное описание свойств  -  см. схему CommentCreateViewModel
        /// </remarks>
        /// <response code="200">Комментарий успешно создан</response>
        /// <response code="400">Ошибка при создании комментария</response>
        /// <response code="404">Не найден комментарий по указанному идентификатору</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CommentCreateViewModel model)
        {
            var messages = await _commentService.CheckByIdAsync(model.PostId, model.UserId).ToListAsync();

            if (messages.Count == 0)
            {
                var result = await _commentService.CreateCommentAsync(model);
                if (!result)
                    return StatusCode(StatusCodes.Status400BadRequest, "Ошибка при создании комментария!");

                return StatusCode(StatusCodes.Status200OK, "Комментарий успешно создан.");
            }

            return StatusCode(StatusCodes.Status404NotFound, messages);
        }

        /// <summary>
        /// Обновление комментария
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет обновить существующий комментарий. Подробное описание свойств  -  см. схему CommentEditViewModel
        /// </remarks>
        /// <response code="200">Получение модели комментария с обновленными данными</response>
        /// <response code="400">Ошибка при обновлении комментария</response>
        /// <response code="404">Не найден комментарий по указанному идентификатору</response>
        [HttpPut]
        [ProducesResponseType<CommentEditViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] CommentEditViewModel model)
        {
            var comment = await _commentService.GetCommentByIdAsync(model.Id);
            if (comment == null)
                return StatusCode(StatusCodes.Status404NotFound, "Не найден комментарий по указанному идентификатору!");

            var result = await _commentService.UpdateCommentAsync(model);
            if (!result)
                return StatusCode(StatusCodes.Status400BadRequest, "Ошибка при обновлении комментария!");

            return StatusCode(StatusCodes.Status200OK, _mapper.Map<CommentEditViewModel>(comment));
        }

        /// <summary>
        /// Удаление комментария
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет удалить комментарий по её идентификатору.
        /// </remarks>
        /// <param name="id">Идентификатор комментария, которую необходимо удалить</param>
        /// <response code="200">Получение модели удалённого комментария</response>
        /// <response code="404">Не найден комментарий по указанному идентификатору</response>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType<CommentViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Не найден комментарий по указанному идентификатору!");

             await _commentService.DeleteCommentAsync(comment);
            return StatusCode(StatusCodes.Status200OK, _mapper.Map<CommentViewModel>(comment));
        }
    }
}
