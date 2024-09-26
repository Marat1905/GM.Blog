using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Tags.Request
{
    /// <summary>
    /// Модель представления создания тега
    /// </summary>
    public class TagCreateViewModel
    {
        /// <summary>
        /// Имя тега. Не должно содержать пробельные символы
        /// </summary>
        /// <example>tag_name</example>
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        [Display(Name = "Название")]
        public required string Name { get; set; }
    }
}
