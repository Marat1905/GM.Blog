using GM.Blog.BLL.ViewModels.Posts.Request;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.BLL.Result.Posts
{
    public class PostEditResult
    {
        public PostEditViewModel? Model { get; set; }
        public IActionResult? Result { get; set; }

        public PostEditResult(PostEditViewModel? model, IActionResult? result)
        {
            Model = model;
            Result = result;
        }
    }
}
