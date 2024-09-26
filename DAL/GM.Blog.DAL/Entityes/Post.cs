using GM.Blog.DAL.Entityes.Base;

namespace GM.Blog.DAL.Entityes
{
    /// <summary>Сущность статьи </summary>
    public class Post : Entity
    {
        /// <summary>Заголовок</summary>
        public required string Title { get; set; }

        /// <summary>Описание</summary>
        public required string Content { get; set; }

        /// <summary>Дата публикации </summary>
        public DateTime DatePublic { get; set; }

        /// <summary>Дата модификации</summary>
        public DateTime? DateModified { get; set; }

        /// <summary>Создатель Поста</summary>
        public required Guid UserId { get; set; }
        /// <summary>Создатель Поста</summary>
        public required User User { get; set; }


        /// <summary>Теги статьи</summary>
        public ICollection<Tag>? Tags { get; set; }

        /// <summary>Комментарии статьи</summary>
        public ICollection<Comment>? Comments { get; set; }

        public ICollection<User>? Users { get; set; }

        public  Post() 
        {
            DatePublic = DateTime.UtcNow;
        }
    }
}
