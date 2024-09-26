using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Roles.Request
{
    /// <summary> Модель представления редактирования роли</summary>
    public class RoleEditViewModel
    {
        /// <summary>Идентификатор роли</summary>
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        public Guid Id { get; set; }

        /// <summary>Имя роли</summary>
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        /// <summary>писание роли</summary>
        [Display(Name = "Описание")]
        public string? Description { get; set; }
    }
}
