using AutoMapper;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Comments.Request;
using GM.Blog.BLL.ViewModels.Comments.Response;
using GM.Blog.BLL.ViewModels.Posts.Response;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GM.Blog.Web.Controllers
{
    /// <summary> Контроллер комментариев</summary>
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentService _commentService;
        private readonly IPostService _postService;

        public CommentController(ILogger<CommentController> logger,ICommentService commentService, IPostService postService)
        {

            _logger = logger;
            _commentService = commentService;
            _postService = postService;
        }
        /// <summary> Создание комментария</summary>
        [HttpPost]
        [Route("CreateComment")]
        public async Task<IActionResult> Create(CommentCreateViewModel model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;

            if (!ModelState.IsValid)
                return await GetPostViewModel(model,userId);


            var result = await _commentService.CreateCommentAsync(model);
            if (result)
                return RedirectToAction("View", "Post", new { Id = model.PostId, UserId = userId });
            else
            {
                ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось создать комментарий!");
                return await GetPostViewModel(model, userId);
            }
        }

       

        /// <summary> Страница всех комментариев (получение комментариев для указанной статьи) </summary>
        [HttpGet]
        [Route("GetComments/{postId?}")]
        public async Task<IActionResult> GetComments([FromRoute] Guid? postId, [FromQuery] Guid? userId)
        {
            var model = await _commentService.GetCommentsAsync(postId, userId);
            if (model == null) return NotFound();

            return View(model);
        }

        /// <summary> Страница редактирования комментария</summary>
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            var fullAccess = User.IsInRole("Admin") || User.IsInRole("Moderator");

            var result = await _commentService.GetCommentEditAsync(id, userId, fullAccess);

            if (result.Model == null) return result.Result!;

            return View(result.Model);
        }

        /// <summary>
        /// Редактирование комментария
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(CommentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _commentService.UpdateCommentAsync(model);
                if (result)
                {
                    if (model.ReturnUrl != null && Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    return RedirectToAction("GetComments");
                }
                else
                    ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось обновить комментарий!");
            }

            return View(model);
        }

        /// <summary>Удаление комментария</summary>
        [HttpPost]
        public async Task<IActionResult> Remove([FromRoute] Guid id, [FromForm] Guid? userId, string? returnUrl)
        {
            var access = User.IsInRole("Admin") || User.IsInRole("Moderator");
            var result= await _commentService.DeleteCommentAsync(id, userId, access);

            if(result==null) 
            {
                if (returnUrl != null && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl + $"?userId={userId}");
                return RedirectToAction("GetComments");
            }
            else
                return result;
        }

        async Task<IActionResult> GetPostViewModel(CommentCreateViewModel model, string userId)
        {
            var postViewModel = await _postService.GetPostAsync(model.PostId, userId ?? string.Empty);
            if (postViewModel == null) return NotFound();

            postViewModel.CommentCreateViewModel = model;
            return View("/Views/Post/View.cshtml", postViewModel);
        }
    }
}
