using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Comments.Request
{
    /// <summary>Модель представления редактирования комментария </summary>
    public class CommentEditViewModel
    {
        public Guid Id { get; set; }
        public string? ReturnUrl { get; set; }

        [Required(ErrorMessage = "Добавьте текст комментария!")]
        [Display(Name = "Комментарий")]
        public string Text { get; set; }
    }
}
