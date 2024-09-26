using GM.Blog.DAL.Entityes;

namespace GM.Blog.BLL.ViewModels.Posts.Response
{
    /// <summary>
    /// Модель представления всез статей
    /// </summary>
    public class PostsViewModel
    {
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
