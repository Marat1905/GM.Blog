using GM.Blog.BLL.Result.Users;
using GM.Blog.BLL.ViewModels.Users.Request;
using GM.Blog.BLL.ViewModels.Users.Response;
using GM.Blog.DAL.Entityes;
using System.Security.Claims;

namespace GM.Blog.BLL.Services.Interfaces
{
    /// <summary>
    /// Интерфейс сервисов сущности пользователя
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <param name="model">Модель представления создания пользователя</param>
        /// <returns></returns>
        Task<UserCreateResult> CreateUserAsync(UserRegisterViewModel model,ICollection<Role>? roles =null);

        /// <summary>
        /// Получение модели всех пользователей
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>Возвращаем модель представления</returns>
        Task<UsersViewModel?> GetUsersAsync(Guid? roleId);

        /// <summary>
        /// Получение модели редактирования пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="fullAccess"></param>
        /// <returns></returns>
        Task<UserEditResult> GetUserEditAsync(Guid id, string? userId, bool fullAccess);

        /// <summary>
        /// Получение модели профиля пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Возвращаем модель представления профиля</returns>
        Task<UserViewModel?> GetUserAsync(Guid id);

        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<User?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Обновление пользователя
        /// </summary>
        /// <param name="model">Модель представления для редактирования пользователя</param>
        /// <returns></returns>
        Task<bool> UpdateUserAsync(UserEditViewModel model);

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="fullAccess"></param>
        /// <returns></returns>
        Task<bool> DeleteByIdAsync(Guid id, Guid? userId, bool fullAccess);


        /// <summary>
        /// Получение утверждений пользователя (роли, идентификатор)
        /// </summary>
        /// <param name="user">модель БД пользователя</param>
        /// <returns>Возвращаем коллекцию клаймов</returns>
        IAsyncEnumerable<Claim> GetUserClaimsAsync(User user);

        /// <summary>
        /// Проверка данных при авторизации пользователя
        /// </summary>
        /// <param name="model">Модель представления</param>
        /// <returns>Возвращаем модель пользователя</returns>
        public Task<User?> CheckDataForLoginAsync(UserLoginViewModel model);

    }
}
