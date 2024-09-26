using GM.Blog.DAL.Entityes.Base;

namespace GM.Blog.DAL.Entityes
{
    /// <summary>Сущность тега</summary>
    public class Tag: Entity
    {
        /// <summary>Имя тега</summary>
        public required string Name { get; set; }


        /// <summary>Теги статьи</summary>
        public ICollection<Post>? Posts { get; set; }
    }
}
