using GM.Blog.DAL.Entityes;
using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Posts.Request
{
    /// <summary>
    /// Модель представления создания статьи
    /// </summary>
    public class PostCreateViewModel
    {
        public Guid UserId { get; set; }

        public List<Tag>? AllTags { get; set; }

        public string? PostTags { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Добавьте контент!")]
        [Display(Name = "Контент")]
        public string Content { get; set; }
    }
}
