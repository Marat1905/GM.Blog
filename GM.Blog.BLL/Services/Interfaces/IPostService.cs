using GM.Blog.BLL.Result.Posts;
using GM.Blog.BLL.ViewModels.Posts.Request;
using GM.Blog.BLL.ViewModels.Posts.Response;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.BLL.Services.Interfaces
{
    public interface IPostService
    {

        /// <summary>
        /// Создание статьи
        /// </summary>
        /// <param name="model">Модель представления</param>
        /// <returns></returns>
        Task<bool> CreatePostAsync(PostCreateViewModel model);

        /// <summary>
        /// Получение статьи по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор поста</param>
        /// <returns></returns>
        Task<Post?> GetPostByIdAsync(Guid id);

        /// <summary>
        /// Получение модели указной статьи
        /// </summary>
        /// <param name="id">Идентификатор поста</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<PostViewModel?> GetPostAsync(Guid id, string userId);
  

        /// <summary>
        /// Получение модели редактирования статьи
        /// </summary>
        /// <param name="id">Идентификатор поста</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="fullAccess"></param>
        /// <returns></returns>
        Task<PostEditResult> GetPostEditAsync(Guid id, string? userId, bool fullAccess);

        /// <summary>
        /// Обновление статьи
        /// </summary>
        /// <param name="model">Модель представления</param>
        /// <returns></returns>
        Task<bool> UpdatePostAsync(PostEditViewModel model);

        /// <summary>
        /// Получение всех статей
        /// </summary>
        /// <param name="tagId">Идентификатор тега</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<PostsViewModel> GetPostsAsync(Guid? tagId, Guid? userId);

        /// <summary>
        /// Удаление статьи
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="fullAccess"></param>
        /// <returns></returns>
        Task<IActionResult?> DeletePostAsync(Guid id, Guid userId, bool fullAccess);

        /// <summary>
        /// Получение идентификатора последней созданой статьи указанного пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<Guid> GetLastCreatePostIdByUserId(Guid userId);
    }
}
