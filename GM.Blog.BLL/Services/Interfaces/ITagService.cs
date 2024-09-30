using GM.Blog.BLL.ViewModels.Tags.Request;
using GM.Blog.BLL.ViewModels.Tags.Response;
using GM.Blog.DAL.Entityes;

namespace GM.Blog.BLL.Services.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса тега
    /// </summary>
    public interface ITagService
    {

        /// <summary>
        /// Создание тега
        /// </summary>
        /// <param name="model"> Модель представления</param>
        /// <returns></returns>
        Task<bool> CreateTagAsync(TagCreateViewModel model);

        /// <summary>
        /// Получение модели всех тегов
        /// </summary>
        /// <param name="tagId">Идентификатор Тега</param>
        /// <param name="postId">Идентификатор Поста</param>
        /// <returns></returns>
        Task<TagsViewModel?> GetTagsAsync(Guid? tagId, Guid? postId);

        /// <summary>
        /// Получение модели редактирования тега
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        Task<TagEditViewModel?> GetTagEditAsync(Guid id);

        /// <summary>
        /// Обновление тега
        /// </summary>
        /// <param name="model">Модель представления тега</param>
        /// <returns></returns>
        Task<bool> UpdateTagAsync(TagEditViewModel model);

        /// <summary>
        /// Удаление тега
        /// </summary>
        /// <param name="id">Идентификатор тега</param>
        /// <returns></returns>
        Task<bool> DeleteTagAsync(Guid id);

        /// <summary>
        /// Получение модели указанного тега
        /// </summary>
        /// <param name="id">Идентификатор тега</param>
        /// <returns></returns>
        Task<TagViewModel?> GetTagAsync(Guid id);

        /// <summary>
        /// Проверка данных о теге
        /// </summary>
        /// <param name="name">Имя тега</param>
        /// <returns></returns>
        Task<string?> CheckTagNameAsync(string name);

        /// <summary>
        /// Присвоение тегов посту
        /// </summary>
        /// <param name="postTags"></param>
        /// <returns></returns>
        IAsyncEnumerable<Tag> SetTagsForPostAsync(string? postTags);

        /// <summary>
        /// Получение списка всех тегов
        /// </summary>
        IAsyncEnumerable<Tag> GetAllTagsAsync();
    }
}
