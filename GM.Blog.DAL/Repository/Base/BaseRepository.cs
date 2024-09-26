using GM.Blog.DAL.Context;
using GM.Blog.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GM.Blog.DAL.Repository.Base
{
    public class BaseRepository<T> : IRepository<T> where T : class,IEntity
    {
        private readonly BlogContext _db;
        private readonly DbSet<T> _Set;

        public bool AutoSaveChanges { get; set; } = true;

        public BaseRepository(BlogContext db)
        {
            _db = db;
            _Set = db.Set<T>();
        }

        public virtual IQueryable<T> Items => _Set;

        public T? Get(Guid id) => Items.SingleOrDefault(i => i.Id.Equals(id));

        public async Task<T?> GetAsync(Guid id, CancellationToken Cancel = default) => await Items.
            SingleOrDefaultAsync(i => i.Id.Equals(id), Cancel);

        public T Add(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            //_db.Add(item);
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                _db.SaveChanges();
            return item;
        }

        public async Task<T> AddAsync(T item, CancellationToken Cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
            return item;
        }

        public void AddRange(IEnumerable<T> item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.AddRange(item);
            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task AddRangeAsync(IEnumerable<T> item, CancellationToken Cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            await _db.AddRangeAsync(item, Cancel).ConfigureAwait(false);
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        }

        public void Update(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task UpdateAsync(T item, CancellationToken Cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        }

        public void Remove(Guid id)
        {
            var item = _Set.Local.FirstOrDefault(i => i.Id.Equals(id));
            if(item != null)
                _db.Remove(item);
            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task RemoveAsync(T item, CancellationToken Cancel = default)
        {
            _db.Remove(item);
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        }

        public void SaveAs()
        {
            if (!AutoSaveChanges)
                _db.SaveChanges();
        }

        public async Task SaveAsAsync(CancellationToken Cancel = default)
        {
            if (!AutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        }

        public async Task ClearAsync(CancellationToken Cancel = default)
        {
            _db.RemoveRange(Items);
            await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        }
    }
}
