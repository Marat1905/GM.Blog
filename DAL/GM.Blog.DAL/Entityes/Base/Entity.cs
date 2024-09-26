using GM.Blog.DAL.Interfaces;

namespace GM.Blog.DAL.Entityes.Base
{
    /// <summary>Сущность</summary>
    /// <typeparam name="TKey">Тип первичного ключа</typeparam>
    public abstract class Entity :IEntity
    {
        /// <summary>Первичный ключ</summary>
        public Guid Id { get; set; }
        /// <summary> Инициализация сущности </summary>
       
    }
}
