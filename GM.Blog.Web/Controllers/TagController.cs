using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Tags.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.Web.Controllers
{
    public class TagController : Controller
    {
        private readonly ILogger<TagController> _logger;
        private readonly ITagService _tagService;

        public TagController(ILogger<TagController> logger,ITagService tagService )
        {
            _logger = logger;
            _tagService = tagService;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Страница создания тега
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("CreateTag")]
        public IActionResult Create() => View();

        /// <summary>
        /// Создание тега
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("CreateTag")]
        public async Task<IActionResult> Create(TagCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var check = await _tagService.CheckTagNameAsync(model.Name);
                if (string.IsNullOrEmpty(check))
                {
                    var result = await _tagService.CreateTagAsync(model);
                    if (result)
                        return RedirectToAction("GetTags");
                    else
                        ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось создать тег!");
                }
                else
                    ModelState.AddModelError(string.Empty,check );       
            }
            return View(model);
        }

        /// <summary>
        /// Страница всех тегов (получение тегов для указанной статьи, получение указанного тега)
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("GetTags/{id?}")]
        public async Task<IActionResult> GetTags([FromRoute] Guid? id, [FromQuery] Guid? postId)
        {
            var model = await _tagService.GetTagsAsync(id, postId);
            return View(model);
        }

        /// <summary>
        /// Страница редактирования тега
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var model = await _tagService.GetTagEditAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        /// <summary>
        /// Редактирование тега
        /// </summary>
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> Edit(TagEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var check = await _tagService.CheckTagNameAsync(model.Name);
                if (string.IsNullOrEmpty(check))
                {
                    var result = await _tagService.UpdateTagAsync(model);
                    if (result)
                        return RedirectToAction("GetTags");
                    else
                        ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось обновить тег!");
                }
                else
                    ModelState.AddModelError(string.Empty, check);
            }
            else
                ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось обновить тег!");

            return View(model);
        }

        /// <summary>
        /// Удаление тега
        /// </summary>
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public async Task<IActionResult> Remove(Guid id)
        {
            var result = await _tagService.DeleteTagAsync(id);
            if (!result) return BadRequest();

            return RedirectToAction("GetTags");
        }

        /// <summary>
        /// Страница отображения указанного тега
        /// </summary>
        [Authorize(Roles = "Admin, Moderator")]
        [HttpGet]
        public async Task<IActionResult> View([FromRoute] Guid id)
        {
            var model = await _tagService.GetTagAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }
    }
}
