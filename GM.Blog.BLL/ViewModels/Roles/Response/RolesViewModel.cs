using GM.Blog.DAL.Entityes;

namespace GM.Blog.BLL.ViewModels.Roles.Response
{
    /// <summary> Модель представления всех ролей</summary>
    public class RolesViewModel
    {
        public ICollection<Role> Roles { get; set; } 
    }
}
