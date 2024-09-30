using AutoMapper;
using GM.Blog.BLL.Extensions;
using GM.Blog.BLL.Result.Posts;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Comments.Request;
using GM.Blog.BLL.ViewModels.Posts.Request;
using GM.Blog.BLL.ViewModels.Posts.Response;
using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GM.Blog.BLL.Services
{
    public class PostService : IPostService
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ITagService _tagService;
        private readonly ILogger<PostService> _logger;

        public PostService(IRepository<Post> postRepository, IMapper mapper,UserManager<User> userManager, ITagService tagService, ILogger<PostService> logger)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _userManager = userManager;
            _tagService = tagService;
            _logger = logger;
        }

        public async Task<bool> CreatePostAsync(PostCreateViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null) return false;

            var post = _mapper.Map<Post>(model);
            post.User = user;
            post.Tags = (await _tagService.SetTagsForPostAsync(model.PostTags).ToListAsync()).Distinct().ToList();

            await _postRepository.AddAsync(post);
            return true;
        }

        public async Task<IActionResult?> DeletePostAsync(Guid id, Guid userId, bool fullAccess)
        {
            var post = await GetPostByIdAsync(id);
            if (post == null) return new NotFoundResult();

            if (fullAccess || post.UserId == userId)
            {
                await _postRepository.RemoveAsync(post!);
                return null;
            }
            return new ForbidResult();
        }

        public async Task<Post?> GetPostByIdAsync(Guid id) =>await _postRepository.GetAsync(id);

        public async Task<PostsViewModel> GetPostsAsync(Guid? tagId, Guid? userId)
        {
            var model = new PostsViewModel();

            if (userId == null && tagId == null)
                model.Posts = await _postRepository.Items.ToListAsync();
            else if (userId != null && tagId == null)
                model.Posts = await _postRepository.Items.Where(x => x.UserId == userId).ToListAsync();
            else
                model.Posts = await _postRepository.Items
                                                    .SelectMany(p => p.Tags, (p, t) => new { Post = p, TagId = t.Id })
                                                    .Where(o => o.TagId == tagId).Select(o => o.Post)
                                                    .ToListAsync();

            return model;
        }

        public async Task<PostViewModel?> GetPostAsync(Guid id, string userId)
        {
            var post = await _postRepository.GetAsync(id);
            var user = await _userManager.FindByIdAsync(userId);
            if (post == null || user == null) return null;

            if (!post.Users.Contains(user))
            {
                post.Users.Add(user);
                await _postRepository.UpdateAsync(post);
            }

            var model = _mapper.Map<PostViewModel>(post);
            model.CommentCreateViewModel = new CommentCreateViewModel { PostId = id };

            return model;
        }

        public async Task<PostEditResult> GetPostEditAsync(Guid id, string? userId, bool fullAccess)
        {
            var post = await GetPostByIdAsync(id);
            if (post == null) return new(null, new NotFoundResult());

            if (fullAccess || post.UserId.ToString() == userId)
                return new(_mapper.Map<PostEditViewModel>(post), null);

            return new(null, new ForbidResult());
        }

        public async Task<bool> UpdatePostAsync(PostEditViewModel model)
        {
            var post = await _postRepository.GetAsync(model.Id);
            if (post == null) return false;

            _mapper.Map(model, post);

            if (!string.IsNullOrEmpty(model.PostTags))
                post.Tags = (await _tagService.SetTagsForPostAsync(model.PostTags).ToListAsync()).Distinct().ToList();

            await _postRepository.UpdateAsync(post);
            return true;
        }


        public  Task<Guid> GetLastCreatePostIdByUserId(Guid userId) => _postRepository.Items.Where(p => p.UserId == userId).Select(p => p.Id)
            .OrderByDescending(id => id).FirstOrDefaultAsync();
    }
}
