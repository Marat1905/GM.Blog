using GM.Blog.BLL.ViewModels.Comments.Request;
using Microsoft.AspNetCore.Mvc;

namespace GM.Blog.BLL.Result.Comments
{
    public class CommentEditResult
    {
       public CommentEditViewModel? Model { get; set; }
       public IActionResult?  Result { get; set; }

        public CommentEditResult(CommentEditViewModel? model, IActionResult? result)
        {
            Model = model;
            Result = result;
        }
    }
}
