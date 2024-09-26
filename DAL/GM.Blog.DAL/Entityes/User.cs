using Microsoft.AspNetCore.Identity;

namespace GM.Blog.DAL.Entityes
{
    /// <summary>Сущность пользователя</summary>
    public class User : IdentityUser<Guid>
    {
        /// <summary>Имя</summary>
        public required string FirstName { get; set; }

        /// <summary>Фамилия</summary>
        public required string LastName { get; set; }

        /// <summary>Отчество</summary>
        public string? MiddleName { get; set; }

        /// <summary>Дата рождения</summary>
        public DateTime BirthDate { get; set; }

        /// <summary>Фотография</summary>
        public string? Photo { get; set; }

        /// <summary>Дата регистрации</summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>Номер телефона</summary>
        public string? Phone { get; set; }
 
        
        /// <summary>Роли пользователя </summary>
        public ICollection<Role> Roles { get; set; }

        /// <summary>Посты пользователя </summary>
        public ICollection<Post>? Posts { get; set; }

        /// <summary>Комментарии пользователя</summary>
        public ICollection<Comment>? Comments { get; set; }

        /// <summary>Количество просмотров</summary>
        public ICollection<Post>? VisitedPosts { get; set; }

        public string GetFullName()
        {
            return FirstName + " " + MiddleName + " " + LastName;
        }
    }
}
