using AutoMapper;
using Azure;
using GM.Blog.BLL.ViewModels.Tags.Request;
using GM.Blog.BLL.ViewModels.Tags.Response;
using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace GM.Blog.Web.Controllers
{
    public class TagController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<TagController> _logger;
        private readonly IRepository<Tag> _tagRepository;

        public TagController(IMapper mapper, ILogger<TagController> logger, IRepository<Tag> tagRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _tagRepository = tagRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Страница создания тега
        /// </summary>
        [HttpGet]
        [Route("CreateTag")]
        public IActionResult Create() => View();

        /// <summary>
        /// Создание тега
        /// </summary>
        [HttpPost]
        [Route("CreateTag")]
        public async Task<IActionResult> Create(TagCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tag = _mapper.Map<Tag>(model);
                if (!_tagRepository.Items.Any(x=>x.Name== model.Name))
                    await _tagRepository.AddAsync(tag);
                return RedirectToAction("GetTags");
            }
            return View(model);
        }

        /// <summary>
        /// Страница всех тегов (получение тегов для указанной статьи, получение указанного тега)
        /// </summary>
        [HttpGet]
        [Route("GetTags/{id?}")]
        public async Task<IActionResult> GetTags([FromRoute] Guid? id, [FromQuery] Guid? postId)
        {
            var model = new TagsViewModel();

            if (id == null)
            {
                model.Tags = postId == null
                    ? await _tagRepository.Items.ToListAsync()
                    : await _tagRepository.Items
                                            .SelectMany(t => t.Posts, (t, p) => new { Tag = t, PostId = p.Id })
                                            .Where(o => o.PostId == postId)
                                            .Select(o => o.Tag).ToListAsync();
            }
            else
            {
                var tag = postId == null 
                    ?  _tagRepository.Get( id ?? Guid.Empty) 
                    : await (_tagRepository.Items
                                            .SelectMany(t => t.Posts, (t, p) => new { Tag = t, PostId = p.Id })
                                            .Where(o => o.PostId == postId)
                                            .Select(o => o.Tag)).FirstOrDefaultAsync(t=>t.Id==id);
                if (tag != null) { model.Tags.Add(tag); };
            }
            return View(model);
        }

        /// <summary>
        /// Страница редактирования тега
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);

            var model = tag == null ? null : _mapper.Map<TagEditViewModel>(tag);

            if (model == null) return NotFound();

            return View(model);
        }

        /// <summary>
        /// Редактирование тега
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(TagEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentTag = await _tagRepository.GetAsync(model.Id);
                if (currentTag == null) return NotFound();

                _mapper.Map(model, currentTag);

                await _tagRepository.UpdateAsync(currentTag);

                return RedirectToAction("GetTags");

            }
            else
                ModelState.AddModelError(string.Empty, $"Ошибка! Не удалось обновить тег!");

            return View(model);
        }

        /// <summary>
        /// Удаление тега
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Remove(Guid id)
        {
            var currentTag = await _tagRepository.GetAsync(id);
            if (currentTag == null) return NotFound();

            await _tagRepository.RemoveAsync(currentTag);;

            return RedirectToAction("GetTags");
        }

        /// <summary>
        /// Страница отображения указанного тега
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> View([FromRoute] Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);

            if (tag == null) return NotFound();

            var model = _mapper.Map<TagViewModel>(tag);

            if (model == null) return NotFound();

            return View(model);
        }
    }
}
