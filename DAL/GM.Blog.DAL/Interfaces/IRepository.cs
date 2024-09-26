namespace GM.Blog.DAL.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        /// <summary> Получение коллекцию БД</summary>
        IQueryable<T> Items { get; }

        /// <summary>Авто сохранение данных в БД</summary>
        bool AutoSaveChanges { get; set; }

        /// <summary> Получить объект из БД </summary>
        /// <param name="id">Id - объекта</param>
        /// <returns>Возвращаем объект </returns>
        T Get(Guid id);

        /// <summary>Получить объект из БД асинхронно </summary>
        /// <param name="id">Id - объекта</param>
        /// <param name="Cancel">Токен отмены операции</param>
        /// <returns></returns>
        Task<T> GetAsync(Guid id, CancellationToken Cancel = default);

        /// <summary>Добавить в БД объект </summary>
        /// <param name="item">Объект</param>
        /// <returns>Возвращаем добавленный объект</returns>
        T Add(T item);

        /// <summary>Добавить в БД объект асинхронно </summary>
        /// <param name="item">Объект</param>
        /// <param name="Cancel">Токен отмены операции</param>
        /// <returns>Возвращаем добавленный объект</returns>
        Task<T> AddAsync(T item, CancellationToken Cancel = default);

        /// <summary>Добавляем коллекцию в БД</summary>
        /// <param name="item">Передаваемая коллекция</param>
        void AddRange(IEnumerable<T> item);

        /// <summary> Добавляем коллекцию в БД асинхронно </summary>
        /// <param name="item">Передаваемая коллекция</param>
        /// <param name="Cancel">Токен отмены операции</param>
        Task AddRangeAsync(IEnumerable<T> item, CancellationToken Cancel = default);

        /// <summary>Обновление объекта в БД</summary>
        /// <param name="item">Объект</param>
        void Update(T item);

        /// <summary>Обновление объекта в БД асинхронно</summary>
        /// <param name="item">Объект</param>
        /// <param name="Cancel">Токен отмены операции</param>
        Task UpdateAsync(T item, CancellationToken Cancel = default);

        /// <summary>Удаление объекта из БД</summary>
        /// <param name="id">Идентификатор объекта</param>
        void Remove(Guid id);

        /// <summary>Удаление объекта из БД асинхронно</summary>
        /// <param name="item"> объект</param>
        /// <param name="Cancel">Токен отмены операции</param>
        Task RemoveAsync(T item, CancellationToken Cancel = default);

        /// <summary>Сохранение данных в БД если не было авто сохранения</summary>
        void SaveAs();

        /// <summary>Сохранение данных в БД если не было авто сохранения асинхронно</summary>
        Task SaveAsAsync(CancellationToken Cancel = default);

        /// <summary> Очистка таблицы </summary>
        Task ClearAsync(CancellationToken Cancel = default);
    }
}
