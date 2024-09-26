using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Users.Request
{
    /// <summary>
    /// Модель представления регистрации пользователя
    /// </summary>
    public class UserRegisterViewModel
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "Укажите Имя")]
        [Display(Name = "Имя")]
        public required string FirstName { get; set; }

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        [Required(ErrorMessage = "Укажите Фамилию")]
        [Display(Name = "Фамилия")]
        public required string LastName { get; set; }

        /// <summary>
        /// Отчество пользователя
        /// </summary>
        [Display(Name = "Отчество")]
        public string? MiddleName { get; set; }

        /// <summary>
        /// Email пользователя
        /// </summary>
        /// <example>example@gmail.com</example>
        [Required(ErrorMessage = "Не указан Email")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public required string Email { get; set; }


        /// <summary>
        /// Дата рождения пользователя
        /// </summary>
        [Required(ErrorMessage = "Не указана дата рождения")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Дата рождения")]
        public required DateTime BirthDate { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [StringLength(30, ErrorMessage = "{0} - Минимальная длина пароля: {1}, Максимальная: {2}", MinimumLength = 8)]
        public required string Password { get; set; }

        /// <summary>
        /// Повторный ввод пароля. Должен совпадать с Password
        /// </summary>
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public required string PasswordConfirm { get; set; }
    }
}
