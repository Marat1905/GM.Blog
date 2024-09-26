using GM.Blog.DAL.Entityes;

namespace GM.Blog.BLL.ViewModels.Comments.Response
{
    /// <summary> Модель представления комментариев</summary>
    public class CommentsViewModel
    {
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
