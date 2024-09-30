using GM.Blog.BLL.ViewModels.Users.Request;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.BLL.Result.Users
{
    public class UserEditResult
    {
        public UserEditViewModel? Model { get; set; }
        public IActionResult? Result { get; set; }

        public UserEditResult(UserEditViewModel? model, IActionResult? result)
        {
            Model = model;
            Result = result;
        }
    }
}
