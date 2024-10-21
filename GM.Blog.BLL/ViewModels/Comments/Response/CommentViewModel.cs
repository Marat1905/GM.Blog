namespace GM.Blog.BLL.ViewModels.Comments.Response
{
    /// <summary>
    /// Модель комментария
    /// </summary>
    public class CommentViewModel
    {
        /// <summary>
        /// Идентификатор комментария
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Текст комментария
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Дата создания комментария
        /// </summary>
        public string CreatedDate { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор статьи
        /// </summary>
        public Guid PostId { get; set; }
    }
}
