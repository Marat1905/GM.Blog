using AutoMapper;
using GM.Blog.BLL.ViewModels.Comments.Request;
using GM.Blog.BLL.ViewModels.Posts.Request;
using GM.Blog.BLL.ViewModels.Posts.Response;
using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace GM.Blog.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PostController> _logger;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Comment> _commentRepository;

        public PostController(IMapper mapper, ILogger<PostController> logger, IRepository<Post> postRepository, IRepository<Tag>  tagRepository, UserManager<User> userManager,IRepository<Comment> commentRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _userManager = userManager;
            _commentRepository = commentRepository;
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
            var user = User;
            var result = await _userManager.GetUserAsync(user);

            var tags = _tagRepository.Items;

            var model = new PostCreateViewModel
            {
                AllTags = await tags.ToListAsync()
            };
            return View(model);
        }

        /// <summary>Создание статьи</summary>
        [HttpPost]
        [Route("CreatePost")]
        public async Task<IActionResult> Create(PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Создание блога
                var post = _mapper.Map<Post>(model);
                var tags =await SetTagsForPostAsync(model.PostTags);

                var listTag=new List<Tag>();    
                foreach (var tag in tags)
                {
                   listTag.Add(tag);
                }
                post.Tags = listTag;
                var result= await _postRepository!.AddAsync(post);
                //RedirectToAction("View", new { Id = await _postRepository.Items.Where(p => p.UserId == (model.UserId))
                //                                                               .Select(p => p.Id)
                //                                                               .OrderByDescending(id => id)
                //                                                               .FirstOrDefaultAsync(), model.UserId });
                return RedirectToAction("GetPosts");
            }

            model.AllTags ??= await _tagRepository.Items.ToListAsync();
            return View(model);
        }


        /// <summary>Страница всех статей (получения статей, имеющих указанный тег)</summary>
        [HttpGet]
        [Route("GetPosts/{tagId?}")]
        public async Task<IActionResult> GetPosts([FromRoute] Guid? tagId, [FromQuery] Guid? userId)
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

            return View(model);
        }

        /// <summary>Удаление статьи</summary>
        [HttpPost]
        public async Task<IActionResult> Remove([FromRoute] Guid id, [FromForm] Guid userId)
        {
            var access = User.IsInRole("Admin") || User.IsInRole("Moderator");

            var post = await _postRepository!.GetAsync(id);
            if (post == null) return new NotFoundResult();
            if (access || post.UserId == userId)
                await _postRepository.RemoveAsync(post);
            else
                return new ForbidResult();

            return RedirectToAction("GetPosts");
        }

        /// <summary>Страница редактирования статьи</summary>
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            var fullAccess = User.IsInRole("Admin") || User.IsInRole("Moderator");

            var post = await _postRepository!.GetAsync(id);
            if (post == null) return  new NotFoundResult();

            if (fullAccess || post.UserId.ToString() == userId)
            {
                var model = _mapper.Map<PostEditViewModel>(post);
                model.AllTags ??= await _tagRepository.Items.ToListAsync();
                return View(model);
            }
            else
                return new ForbidResult();
        }

        /// <summary>Редактирование статьи</summary>
        [HttpPost]
        public async Task<IActionResult> Edit(PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var post = await _postRepository.GetAsync(model.Id);
                if (post != null)
                {
                    _mapper.Map(model, post);

                    var tags = await SetTagsForPostAsync(model.PostTags);

                    var listTag = new List<Tag>();
                    foreach (var tag in tags)
                    {
                        listTag.Add(tag);
                    }
                    post.Tags = listTag;
                  await  _postRepository.UpdateAsync(post);
                    return RedirectToAction("GetPosts");
                  
                }
                else
                    ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось обновить статью!");
            }
            model.AllTags ??= await _tagRepository.Items.ToListAsync();
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

            var post = await _postRepository.GetAsync(id);
            var user = await _userManager.FindByIdAsync(userId);

            if (post == null || user == null) return NotFound();

            if (!post.Users.Contains(user))
            {
                post.Users.Add(user);
                await _postRepository.UpdateAsync(post);
            }

            var model = _mapper.Map<PostViewModel>(post);
            model.CommentCreateViewModel = new CommentCreateViewModel { PostId = id };


            if (model == null) return NotFound();

            model.Comments = await _commentRepository.Items.Where(c=>c.PostId==id).ToListAsync();
            
            return View(model);
        }


        public async Task<List<Tag>> SetTagsForPostAsync(string? postTags)
        {
            var tags = new List<Tag>();

            if (postTags != null)
            {
                var tagSet = Regex.Replace(postTags, @"\s+", " ").Trim().Split(" ");

                foreach (var tagName in tagSet)
                {
                    var tag = await _tagRepository.Items.Where(t=>t.Name==tagName).FirstOrDefaultAsync();
                    if (tag != null) tags.Add(tag);
                }
            }

            return tags;
        }

    }
}
