using GM.Blog.DAL.Entityes;
using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Posts.Request
{
    /// <summary>
    /// Модель представления редактирования статьи
    /// </summary>
    public class PostEditViewModel:PostCreateViewModel
    {
        public Guid Id { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
