using GM.Blog.DAL.Entityes;

namespace GM.Blog.BLL.ViewModels.Users.Request
{
    /// <summary>Модель представления создания пользователя</summary>
    public class UserCreateViewModel : UserRegisterViewModel
    {
        public List<string>? Roles { get; set; } = new List<string> { "User" };
        public List<string>? AllRoles { get; set; }

        public UserCreateViewModel(List<Role> roles)
        {
            AllRoles = roles.Select(r => r.Name!).ToList();
        }

        public UserCreateViewModel() { }
    }
}
