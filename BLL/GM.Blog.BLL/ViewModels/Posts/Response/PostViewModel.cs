using GM.Blog.BLL.ViewModels.Comments.Request;
using GM.Blog.DAL.Entityes;

namespace GM.Blog.BLL.ViewModels.Posts.Response
{
    /// <summary>
    /// Модель представления статьи
    /// </summary>
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public User? User { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<Comment>? Comments { get; set; }

        public CommentCreateViewModel? CommentCreateViewModel { get; set; }
    }
}
