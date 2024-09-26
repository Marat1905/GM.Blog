using System.ComponentModel.DataAnnotations;

namespace GM.Blog.BLL.ViewModels.Tags.Request
{
    /// <summary> Модель представления редактирования тега </summary>
    public class TagEditViewModel
    {
        /// <summary> Идентификатор тега</summary>
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        public Guid Id { get; set; }

        /// <summary>Имя тега. Не должно содержать пробельных символов</summary>
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }
}
