namespace GM.Blog.DAL.Interfaces
{
    /// <summary>Сущность</summary>
    /// <typeparam name="TKey">Тип первичного ключа</typeparam>
    public interface IEntity<out TKey>
    {
       /// <summary>Идентификатор</summary>
        TKey Id { get; }
    }

    /// <summary>Сущность</summary>
    public interface IEntity : IEntity<Guid> { }
}
