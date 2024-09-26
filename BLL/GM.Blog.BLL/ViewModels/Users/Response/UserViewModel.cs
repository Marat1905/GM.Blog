using GM.Blog.DAL.Entityes;
using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Users.Response
{
    /// <summary>Модель представление пользователя</summary>
    public class UserViewModel
    {
        /// <summary>Идентификатор</summary>
        public Guid Id { get; set; }

        /// <summary>Имя</summary>
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        public required string FirstName { get; set; }

        /// <summary>Фамилия</summary>
        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        public required string LastName { get; set; }

        /// <summary>Отчество</summary>
        [Display(Name = "Отчество")]
        public string? MiddleName { get; set; }

        /// <summary>Дата рождения</summary>
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        public DateTime BirthDate { get; set; }

        /// <summary>Фотография</summary>
        [Display(Name = "Фото профиля")]
        public string? Photo { get; set; }


        /// <summary>Адрес электронной почты</summary>
        [EmailAddress]
        [Display(Name = "Электронный адрес")]
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        public required string Email { get; set; }

        /// <summary>Номер телефона</summary>
        [Display(Name = "Номер телефона")]
        public string? Phone { get; set; }

        public ICollection<Role>? Roles { get; set; }
    }
}
