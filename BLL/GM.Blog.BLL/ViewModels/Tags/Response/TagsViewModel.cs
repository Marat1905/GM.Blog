using GM.Blog.DAL.Entityes;

namespace GM.Blog.BLL.ViewModels.Tags.Response
{
    /// <summary>Модель представления всех тегов</summary>
    public class TagsViewModel
    {
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
