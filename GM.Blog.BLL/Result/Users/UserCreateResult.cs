using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Identity;

namespace GM.Blog.BLL.Result.Users
{
    /// <summary>Сущность возврата данных пользователя </summary>
    public class UserCreateResult
    {
        /// <summary>Успешно создан пользователь</summary>
        public IdentityResult? Success { get; set; }

        /// <summary>Пользователь</summary>
        public User User { get; set; }

        public UserCreateResult(IdentityResult? success, User user)
        {
            Success = success;
            User = user;
        }
    }
}
