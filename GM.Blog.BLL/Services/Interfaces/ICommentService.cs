using GM.Blog.BLL.Result.Comments;
using GM.Blog.BLL.ViewModels.Comments.Request;
using GM.Blog.BLL.ViewModels.Comments.Response;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.BLL.Services.Interfaces
{
    public interface ICommentService
    {
        /// <summary>
        /// Создание комментария
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Модель представления</returns>
        Task<bool> CreateCommentAsync(CommentCreateViewModel model);

        /// <summary>
        /// Получение модели всех комментариев
        /// </summary>
        /// <param name="postId">Идентификатор поста</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<CommentsViewModel> GetCommentsAsync(Guid? postId, Guid? userId);

        /// <summary>
        /// Получение модели редактирования комментария
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="fullAccess"></param>
        /// <returns></returns>
        Task<CommentEditResult> GetCommentEditAsync(Guid id, string? userId, bool fullAccess);

        /// <summary>
        /// Обновление комментария
        /// </summary>
        /// <param name="model">Модель представления</param>
        /// <returns></returns>
        Task<bool> UpdateCommentAsync(CommentEditViewModel model);

        /// <summary>
        /// Удаление комментария
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="fullAccess"></param>
        /// <returns></returns>
        Task<IActionResult?> DeleteCommentAsync(Guid id, Guid? userId, bool fullAccess);

        /// <summary>
        /// Получение всех комментариев для указанной статьи
        /// </summary>
        /// <param name="postId">Идентификатор поста</param>
        /// <returns></returns>
        IAsyncEnumerable<Comment> GetAllCommentsByPostIdAsync(Guid postId);
    }
}
