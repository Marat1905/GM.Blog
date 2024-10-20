using AutoMapper;
using GM.Blog.BLL.Extensions;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Posts.Request;
using GM.Blog.BLL.ViewModels.Posts.Response;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.API.Controllers
{
    /// <summary>
    /// Контроллер статей
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PostApiController: ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;

        public PostApiController(IPostService postService, ITagService tagService, IMapper mapper)
        {
            _postService = postService;
            _tagService = tagService;
            _mapper = mapper;
        }

        /// <summary>
        /// Получение объекта статьи
        /// </summary>
        /// <remarks>Данный метод позволяет получить статью по её идентификатору</remarks>
        /// <param name="id">Идентификатор статьи</param>
        /// <response code="200">Получение объекта статьи</response>
        /// <response code="404">Не удалось найти статью по указанному идентификатору</response>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType<PostViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Статья не найдена!");

            return StatusCode(StatusCodes.Status200OK, _mapper.Map<PostViewModel>(post));
        }

        /// <summary>
        /// Получение всех статей. Возможна фильтрация по идентификаторам пользователя и тега
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет получить список всех статей. Возможно получить
        /// список статей определённого пользователя при указании его идентификатора,
        /// список статей с определённым тегом при указании его идентификатора, либо 
        /// применить сразу два фильтра
        /// </remarks>
        /// <param name="userId">
        /// Идентификатор пользователя. Указать для получения списка статей определённого пользователя.
        /// Не указывать для получения полного списка статей
        /// </param>
        /// <param name="tagId">
        /// Идентификатор тега. Указать для получения списка статей с указанным тегом.
        /// Не указывать для получения полного списка статей
        /// </param>
        /// <response code="200">Получение списка статей</response>
        [HttpGet]
        [ProducesResponseType<PostViewModel>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] Guid? userId, [FromQuery] Guid? tagId)
        {
            var list = await _postService.GetPostsAsync(tagId, userId);

            return StatusCode(StatusCodes.Status200OK, _mapper.Map<List<PostViewModel>>(list.Posts));
        }

        /// <summary>
        /// Создание статьи
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет создать новую статью. Подробное описание свойств  -  см. схему PostCreateViewModel
        /// </remarks>
        /// <response code="200">Статья успешно создана</response>
        /// <response code="400">Ошибка при создании статьи</response>
        /// <response code="404">Не найден пользователь по указанному идентификатору</response>
        /// <response code="422">Указаны несуществующие теги</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Create([FromBody] PostCreateViewModel model)
        {
            var messages = await _postService.CheckByIdAsync(model.UserId).ToListAsync();

            if (messages.Count == 0)
            {
                var errors = await _tagService.CheckTagsForCreatePostAsync(model.PostTags ?? string.Empty).ToListAsync();
                if (errors.Count > 0)
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, errors);

                var result = await _postService.CreatePostAsync(model);
                if (!result)
                    return StatusCode(StatusCodes.Status400BadRequest, $"Произошла ошибка при создании статьи!");

                return StatusCode(StatusCodes.Status200OK, "Статья успешно создана!");
            }
            return StatusCode(StatusCodes.Status404NotFound, messages[0]);
        }

        /// <summary>
        /// Обновление статьи
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет обновить существующую статью. Подробное описание свойств  -  см. схему PostEditViewModel
        /// </remarks>
        /// <response code="200">Получение модели статьи с обновленными данными</response>
        /// <response code="400">Ошибка при обновлении статьи</response>
        /// <response code="404">Не найдена статья по указанному идентификатору</response>
        /// <response code="422">Указаны несуществующие теги</response>
        [HttpPut]
        [ProducesResponseType<PostEditViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update([FromBody] PostEditViewModel model)
        {
            var post = await _postService.GetPostByIdAsync(model.Id);
            if (post == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Статья не найдена!");

            var errors = await _tagService.CheckTagsForCreatePostAsync(model.PostTags ?? string.Empty).ToListAsync();
            if (errors.Count > 0)
                return StatusCode(StatusCodes.Status422UnprocessableEntity, errors);

            var result = await _postService.UpdatePostAsync(model);
            if (!result)
                return StatusCode(StatusCodes.Status400BadRequest, $"Произошла ошибка при обновлении статьи!");

            return StatusCode(StatusCodes.Status200OK, _mapper.Map<PostEditViewModel>(post));
        }

        /// <summary>
        /// Удаление статьи
        /// </summary>
        /// <remarks>
        /// Данный метод позволяет удалить статью по её идентификатору.
        /// </remarks>
        /// <param name="id">Идентификатор статьи, которую необходимо удалить</param>
        /// <response code="200">Получение модели удалённой статьи</response>
        /// <response code="404">Не найдена статья по указанному идентификатору</response>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType<PostViewModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
                return StatusCode(StatusCodes.Status404NotFound, $"Статья не найдена!");
            
            await _postService.DeletePostAsync(post);

            return StatusCode(StatusCodes.Status200OK, _mapper.Map<PostViewModel>(post));
        }
    }
}
