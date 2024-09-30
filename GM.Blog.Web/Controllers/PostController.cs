using GM.Blog.BLL.Extensions;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Posts.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GM.Blog.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly ITagService _tagService;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;

        public PostController( ILogger<PostController> logger, ITagService tagService, IPostService postService, ICommentService commentService)
        {
            _logger = logger;
            _tagService = tagService;
            _postService = postService;
            _commentService = commentService;
        }

        public IActionResult Index()
        {
            return View();
        }
        /// <summary>Страница создания статьи</summary>
        [HttpGet]
        [Route("CreatePost")]
        public async Task<IActionResult> Create()
        {
            var model = new PostCreateViewModel { AllTags = await _tagService.GetAllTagsAsync().ToListAsync() };
            return View(model);
        }

        /// <summary>Создание статьи</summary>
        [HttpPost]
        [Route("CreatePost")]
        public async Task<IActionResult> Create(PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _postService.CreatePostAsync(model);

                if (result)
                    return RedirectToAction("View", new { Id = await _postService.GetLastCreatePostIdByUserId(model.UserId), model.UserId });
                else
                    ModelState.AddModelError(string.Empty, "Ошибка! Не удалось создать статью!");
            }

            model.AllTags ??= await _tagService.GetAllTagsAsync().ToListAsync();
            return View(model);
        }


        /// <summary>Страница всех статей</summary>
        [HttpGet]
        [Route("GetPosts/{tagId?}")]
        public async Task<IActionResult> GetPosts([FromRoute] Guid? tagId, [FromQuery] Guid? userId)
        {
            var model = await _postService.GetPostsAsync(tagId, userId);
            return View(model);
        }

        /// <summary>Удаление статьи</summary>
        [HttpPost]
        public async Task<IActionResult> Remove([FromRoute] Guid id, [FromForm] Guid userId)
        {
            var access = User.IsInRole("Admin") || User.IsInRole("Moderator");
            var result = await _postService.DeletePostAsync(id, userId, access);

            if (result==null)
                return RedirectToAction("GetPosts");
            else
                return result;
        }

        /// <summary>Страница редактирования статьи</summary>
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            var fullAccess = User.IsInRole("Admin") || User.IsInRole("Moderator");

            var result = await _postService.GetPostEditAsync(id, userId, fullAccess);

            if (result.Model == null) return result.Result!;

            result.Model.AllTags = await _tagService.GetAllTagsAsync().ToListAsync();
            return View(result.Model);
        }

        /// <summary>Редактирование статьи</summary>
        [HttpPost]
        public async Task<IActionResult> Edit(PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _postService.UpdatePostAsync(model);
                if (result)
                {
                    if (model.ReturnUrl != null && Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    return RedirectToAction("GetPosts");
                }
                else
                    ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось обновить статью!");
            }

            model.AllTags ??= await _tagService.GetAllTagsAsync().ToListAsync();
            return View(model);
        }

        /// <summary>
        /// Страница отображения указанной статьи
        /// </summary>
        [HttpGet]
        [Route("ViewPost/{id}")]
        public async Task<IActionResult> View([FromRoute] Guid id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            var model = await _postService.GetPostAsync(id, userId ?? string.Empty);

            if (model == null) return NotFound();

            model.Comments = await _commentService.GetAllCommentsByPostIdAsync(id).ToListAsync();
            return View(model);
        }
    }
}
