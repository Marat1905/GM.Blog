using AutoMapper;
using GM.Blog.BLL.Result.Comments;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Comments.Request;
using GM.Blog.BLL.ViewModels.Comments.Response;
using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GM.Blog.BLL.Services
{
    public class CommentService:ICommentService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ICommentService> _logger;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly UserManager<User> _userManager;

        public CommentService(IMapper mapper, ILogger<ICommentService> logger, 
                              IRepository<Comment> commentRepository, IUserService userService,
                              IPostService postService , UserManager<User> userManager)
        {
            _mapper = mapper;
            _logger = logger;
            _commentRepository = commentRepository;
            _userService = userService;
            _postService = postService;
            _userManager = userManager;
        }

        public async Task<bool> CreateCommentAsync(CommentCreateViewModel model)
        {
            var user = await _userService.GetUserByIdAsync(model.UserId);
            if (user == null) return false;

            var post = await _postService.GetPostByIdAsync(model.PostId);
            if (post == null) return false;

            var comment = _mapper.Map<Comment>(model);
            comment.Post = post;
            comment.User = user;

            if (await _commentRepository.AddAsync(comment) == null) return false;
            return true;
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid id) => await _commentRepository.GetAsync(id);

        public async Task<CommentsViewModel> GetCommentsAsync(Guid? postId, Guid? userId)
        {
            var model = new CommentsViewModel();

            if (postId == null && userId == null)
                model.Comments = await _commentRepository.Items.ToListAsync();
            else if (postId != null && userId == null)
                model.Comments = await _commentRepository.Items.Where(c => c.PostId == postId).ToListAsync();
            else if (postId == null && userId != null)
                model.Comments = await _commentRepository.Items.Where(c => c.UserId == userId).ToListAsync();
            else
                model.Comments = (await _commentRepository.Items.Where(c => c.PostId == postId).ToListAsync())
                    .Where(c => c.UserId == userId!).ToList();

            return model;
        }

        public async Task<CommentEditResult> GetCommentEditAsync(Guid id, string? userId, bool fullAccess)
        {
            var comment = await _commentRepository.GetAsync(id);
            if (comment == null) return new(null, new NotFoundResult());

            if (fullAccess || comment.UserId.ToString() == userId)
                return new(_mapper.Map<CommentEditViewModel>(comment), null);

            return new(null, new ForbidResult());

        }

        public async Task<bool> UpdateCommentAsync(CommentEditViewModel model)
        {
            var currentComment = await _commentRepository.GetAsync(model.Id);
            if (currentComment == null)
                return false;

            _mapper.Map(model, currentComment);

            await _commentRepository.UpdateAsync(currentComment);
            return true;
        }

        public async Task<IActionResult?> DeleteCommentAsync(Guid id, Guid? userId, bool fullAccess)
        {
            var deletedComment = await _commentRepository.GetAsync(id);
            if (deletedComment == null) return new NotFoundResult();

            if (fullAccess || deletedComment.UserId == userId)
            {
                await _commentRepository.RemoveAsync(deletedComment!);
                return null;
            }

            return new ForbidResult();
        }

        public async Task DeleteCommentAsync(Comment comment) => await _commentRepository.RemoveAsync(comment);

        public IAsyncEnumerable<Comment> GetAllCommentsByPostIdAsync(Guid postId) => _commentRepository.Items.Include(o => o.User).Include(o => o.Post).Where(c => c.PostId == postId).AsAsyncEnumerable();

        public async IAsyncEnumerable<string> CheckByIdAsync(Guid? postId = null, Guid? userId = null)
        {
            var list = new List<string>();

            if (postId != null)
            {
                var post = await _postService.GetPostByIdAsync((Guid)postId);
                if (post == null) list.Add($"Статья не найдена! Id = [{postId}]");
            }

            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString()!);
                if (user == null) list.Add($"Пользователь не найден! Id = [{userId}]");
            }

            foreach (var item in list)
                yield return item;
        }
    }
}
