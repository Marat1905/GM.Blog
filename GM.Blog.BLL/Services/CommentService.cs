using AutoMapper;
using GM.Blog.BLL.Result.Comments;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Comments.Request;
using GM.Blog.BLL.ViewModels.Comments.Response;
using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Interfaces;
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

        public CommentService(IMapper mapper, ILogger<ICommentService> logger, 
                              IRepository<Comment> commentRepository, IUserService userService,
                              IPostService postService)
        {
            _mapper = mapper;
            _logger = logger;
            _commentRepository = commentRepository;
            _userService = userService;
            _postService = postService;
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

        public IAsyncEnumerable<Comment> GetAllCommentsByPostIdAsync(Guid postId) => _commentRepository.Items.Include(o => o.User).Include(o => o.Post).Where(c => c.PostId == postId).AsAsyncEnumerable();
    }
}
