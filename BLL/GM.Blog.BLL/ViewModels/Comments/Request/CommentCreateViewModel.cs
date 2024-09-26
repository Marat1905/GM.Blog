using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Comments.Request
{
    /// <summary>
    /// Модель представления создания комментария
    /// </summary>
    public class CommentCreateViewModel
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [Required(ErrorMessage = "Добавьте текст комментария!")]
        public Guid  UserId { get; set; }

        /// <summary>
        /// Идентификатор статьи
        /// </summary>
        [Required(ErrorMessage = "Добавьте текст комментария!")]
        public required Guid PostId { get; set; }

        /// <summary>
        /// Текст комментария
        /// </summary>
        [Required(ErrorMessage = "Добавьте текст комментария!")]
        [Display(Name = "Комментарий")]
        public string Text { get; set; }
    }
}
