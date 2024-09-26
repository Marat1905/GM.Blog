using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Roles.Request
{
    /// <summary>Модель представления создания роли </summary>
    public class RoleCreateViewModel
    {
        /// <summary>Имя роли </summary>
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        [Display(Name = "Название")]
        public required string Name { get; set; }

        /// <summary>Описание роли</summary>
        [Display(Name = "Описание")]
        public string? Description { get; set; }
    }
}
