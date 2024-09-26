using GM.Blog.DAL.Context;
using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GM.Blog.DAL.Repository
{
    public class TagRepository : BaseRepository<Tag>
    {
        public override IQueryable<Tag> Items => base.Items.Include(t=>t.Posts);
        public TagRepository(BlogContext db) : base(db) { }
    
    }
}
