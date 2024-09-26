using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Users.Request
{
    /// <summary>
    /// Модель представления авторизации пользователя
    /// </summary>
    public class UserLoginViewModel
    {
        public string? ReturnUrl { get; set; }

        [Required(ErrorMessage = "Не указан Email")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public required string UserEmail { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
