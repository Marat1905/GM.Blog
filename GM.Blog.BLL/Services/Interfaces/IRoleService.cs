using GM.Blog.BLL.ViewModels.Roles.Request;
using GM.Blog.BLL.ViewModels.Roles.Response;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GM.Blog.BLL.Services.Interfaces
{
    /// <summary>
    /// Интерфейс сервисов ролей
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Получение модели всех ролей пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<RolesViewModel?> GetRolesAsync(Guid? userId);

        /// <summary>
        /// Получение ролей пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        IAsyncEnumerable<Role> GetRolesByUserAsync(Guid userId);

        /// <summary>
        /// Получение списка всех ролей
        /// </summary>
        IAsyncEnumerable<Role> GetAllRolesAsync();

        /// <summary>
        /// Получение роли по названию
        /// </summary>
        /// <param name="roleName">Название роли</param>
        /// <returns></returns>
        Task<Role?> GetRoleByNameAsync(string roleName);

        /// <summary>
        /// Получение модели указанной роли
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns></returns>
        Task<RoleViewModel?> GetRoleAsync(Guid id);

        /// <summary>
        /// Получение модели редактирования роли
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<RoleEditViewModel?> GetRoleEditAsync(Guid id);

        /// <summary>
        /// Обновление роли
        /// </summary>
        /// <param name="model">Модель представления</param>
        /// <returns></returns>
        Task<IdentityResult?> UpdateRoleAsync(RoleEditViewModel model);

        /// <summary>
        /// Создание роли
        /// </summary>
        /// <param name="model">Модель представления</param>
        /// <returns></returns>
        Task<IdentityResult?> CreateRoleAsync(RoleCreateViewModel model);

        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        Task<IdentityResult?> DeleteRoleAsync(Guid id);

        /// <summary>
        /// Получение данных об обновлении ролей пользователя
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <returns></returns>
        IAsyncEnumerable<Role> GetEnabledRoleNamesWithRequest(HttpRequest request);

        /// <summary>
        /// Преобразование списка имён ролей в список ролей 
        /// </summary>
        /// <param name="roleNames"></param>
        /// <returns></returns>
        IAsyncEnumerable<Role> ConvertRoleNamesInRoles(ICollection<string> roleNames);

        /// <summary>
        /// Проверка данных при создании роли
        /// </summary>
        /// <param name="name">Имя роли</param>
        /// <returns></returns>
        Task<string> CheckRoleAsync(string name);
    }
}
