namespace GM.Blog.BLL.ViewModels.Roles.Response
{
    /// <summary>Модель представления роли</summary>
    public class RoleViewModel
    {
        /// <summary>Идентификатор роли</summary>
        public Guid Id { get; set; }

        /// <summary>Имя роли</summary>
        public required string Name { get; set; }

        /// <summary>Описание роли</summary>
        public string? Description { get; set; }
    }
}
