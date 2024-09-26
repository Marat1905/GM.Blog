using GM.Blog.DAL.Entityes.Base;
using GM.Blog.DAL.Interfaces;

namespace GM.Blog.DAL.Entityes
{
    /// <summary>Сущность комментарии</summary>
    public class Comment : Entity
    {
        /// <summary>Текст комментария</summary>
        public required string Text { get; set; }

        /// <summary>Дата создания комментария</summary>
        public DateTime DateCreate { get; set; }

        /// <summary>Создатель комментария</summary>
        public required Guid UserId { get; set; }
        /// <summary>Создатель комментария</summary>
        public required User User { get; set; }

        /// <summary>Комментарий к статье</summary>
        public required Guid PostId { get; set; }

        /// <summary>Комментарий к статье</summary>
        public required Post Post { get; set; }


        public Comment()
        {
            DateCreate= DateTime.UtcNow;
        }
    }
}
