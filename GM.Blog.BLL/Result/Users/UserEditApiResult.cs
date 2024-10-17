using GM.Blog.DAL.Entityes;

namespace GM.Blog.BLL.Result.Users
{
    public class UserEditApiResult
    {
        public User? User { get; set; }

        public ICollection<string> Messages { get; set; }

        public UserEditApiResult(User? user, ICollection<string> messages) 
        {
            User = user;
            Messages = messages;
        }
    }
}
