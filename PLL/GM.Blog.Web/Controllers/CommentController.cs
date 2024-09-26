using AutoMapper;
using GM.Blog.BLL.ViewModels.Comments.Request;
using GM.Blog.BLL.ViewModels.Comments.Response;
using GM.Blog.BLL.ViewModels.Posts.Response;
using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Interfaces;
using GM.Blog.DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace GM.Blog.Web.Controllers
{
    /// <summary> Контроллер комментариев</summary>
    public class CommentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CommentController> _logger;
        private readonly IRepository<Comment> _commentRepository;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Post> _postRepository;

        public CommentController(IMapper mapper, ILogger<CommentController> logger, IRepository<Comment> commentRepository, UserManager<User> userManager, IRepository<Post> postRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _commentRepository = commentRepository;
            _userManager = userManager;
            _postRepository = postRepository;
        }
        /// <summary> Создание комментария</summary>
        [HttpPost]
        [Route("CreateComment")]
        public async Task<IActionResult> Create(CommentCreateViewModel model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;

            if (ModelState.IsValid)
            {
                var user = await _userManager.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == model.UserId);
                if (user == null) return new NotFoundResult();

                var post = await _postRepository.GetAsync(model.PostId);
                if (post == null) return new NotFoundResult();

                var comment = _mapper.Map<Comment>(model);
                comment.Post = post;
                comment.User = user;

                await _commentRepository.AddAsync(comment);

                return RedirectToAction("View", "Post", new { Id = model.PostId, UserId = userId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось создать комментарий!");

                var post = await _postRepository.GetAsync(model.PostId);
                var user = await _userManager.FindByIdAsync(userId ?? string.Empty);

                if (post == null || user == null) return NotFound();

                if (!post.Users.Contains(user))
                {
                    post.Users.Add(user);
                    await _postRepository.UpdateAsync(post);
                }

               var postViewModel = _mapper.Map<PostViewModel>(post);
                postViewModel.CommentCreateViewModel = new CommentCreateViewModel { PostId = model.PostId };

                if (postViewModel == null) return NotFound();

                postViewModel.CommentCreateViewModel = model;
                return View("/Views/Post/View.cshtml", postViewModel);
            }
        }


        /// <summary> Страница всех комментариев (получение комментариев для указанной статьи) </summary>
        [HttpGet]
        [Route("GetComments/{postId?}")]
        public async Task<IActionResult> GetComments([FromRoute] Guid? postId, [FromQuery] Guid? userId)
        {
            var model = new CommentsViewModel();

            if (postId == null && userId == null)
                model.Comments = await _commentRepository.Items.ToListAsync();
            else if (postId != null && userId == null)
                model.Comments = await _commentRepository.Items.Where(c=>c.PostId==postId).ToListAsync();
            else if (postId == null && userId != null)
                model.Comments = await _commentRepository.Items.Where(c => c.UserId == userId).ToListAsync();
            else
                model.Comments = (await _commentRepository.Items.Where(c => c.PostId == postId).ToListAsync())
                    .Where(c => c.UserId == userId!).ToList();

            if (model == null) return NotFound();

            return View(model);
        }

        /// <summary> Страница редактирования комментария</summary>
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            var fullAccess = User.IsInRole("Admin") || User.IsInRole("Moderator");

            var comment = await _commentRepository.GetAsync(id);
            if (comment == null) return  new NotFoundResult();

            if (!(fullAccess || comment.UserId.ToString() == userId))
                return new ForbidResult();

            var model = _mapper.Map<CommentEditViewModel>(comment);

            return View(model);
        }

        /// <summary>
        /// Редактирование комментария
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(CommentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentComment = await _commentRepository.GetAsync(model.Id);
                if (currentComment != null)
                {
                    _mapper.Map(model, currentComment);
                    await _commentRepository.UpdateAsync(currentComment);
                    if (model.ReturnUrl != null && Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    return RedirectToAction("GetComments");
                }
                else
                    ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось обновить комментарий!");
            }
            else
                ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось обновить комментарий!");

            return View(model);
        }

        /// <summary>Удаление комментария</summary>
        [HttpPost]
        public async Task<IActionResult> Remove([FromRoute] Guid id, [FromForm] Guid? userId, string? returnUrl)
        {
            var access = User.IsInRole("Admin") || User.IsInRole("Moderator");

            var deletedComment = await _commentRepository.GetAsync(id);
            if (deletedComment == null) return new NotFoundResult();


            if (access || deletedComment.UserId == userId)
                await _commentRepository.RemoveAsync(deletedComment!);
            else
                return new ForbidResult();

            if (returnUrl != null && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl + $"?userId={userId}");
            return RedirectToAction("GetComments");
        }
    }
}
