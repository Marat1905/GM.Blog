using Microsoft.AspNetCore.Identity;

namespace GM.Blog.DAL.Entityes
{
    /// <summary>Сущность роли </summary>
    public class Role: IdentityRole<Guid>
    {
        /// <summary>Список пользователей</summary>
        public ICollection<User>? Users { get; set; }

        /// <summary>Описание роли </summary>
        public string? Description {  get; set; }
    }
}
