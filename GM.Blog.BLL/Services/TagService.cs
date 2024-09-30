using AutoMapper;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Tags.Request;
using GM.Blog.BLL.ViewModels.Tags.Response;
using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GM.Blog.BLL.Services
{
    public class TagService : ITagService
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TagService> _logger;

        public TagService(IRepository<Tag> tagRepository, IMapper mapper,ILogger<TagService> logger)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<bool> CreateTagAsync(TagCreateViewModel model)
        {
            var tag = _mapper.Map<Tag>(model);

            if (await _tagRepository.AddAsync(tag) == null) return false;
            return true;
        }

       
        public async Task<TagsViewModel?> GetTagsAsync(Guid? tagId, Guid? postId)
        {
            var model = new TagsViewModel();

            if (tagId == null)
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
                    ? _tagRepository.Get(tagId ?? Guid.Empty)
                    : await(_tagRepository.Items
                                            .SelectMany(t => t.Posts, (t, p) => new { Tag = t, PostId = p.Id })
                                            .Where(o => o.PostId == postId)
                                            .Select(o => o.Tag)).FirstOrDefaultAsync(t => t.Id == tagId);
                if (tag != null) { model.Tags.Add(tag); };
            }

            return model;
        }

        public async Task<TagEditViewModel?> GetTagEditAsync(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);
            var model = tag == null ? null : _mapper.Map<TagEditViewModel>(tag);

            return model;
        }


        public async Task<bool> UpdateTagAsync(TagEditViewModel model)
        {
            var currentTag = await _tagRepository.GetAsync(model.Id);
            if (currentTag == null)
                return false;

            _mapper.Map(model, currentTag);

            await _tagRepository.UpdateAsync(currentTag);
            return true;
        }

        public async Task<bool> DeleteTagAsync(Guid id)
        {
            var currentTag = await _tagRepository.GetAsync(id);
            if (currentTag == null) return false;

            await _tagRepository.RemoveAsync(currentTag);
            return true;
        }

        public async Task<TagViewModel?> GetTagAsync(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);
            if (tag == null) return null;

            return _mapper.Map<TagViewModel>(tag);
        }

        public async Task<string?> CheckTagNameAsync(string name)
        {
            if (await _tagRepository.Items.AnyAsync(x => x.Name == name))
                return $"Тег с именем [{name}] уже существует!";
            else return null;
        }
    }
}
