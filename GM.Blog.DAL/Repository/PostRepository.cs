using GM.Blog.DAL.Context;
using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GM.Blog.DAL.Repository
{
    public class PostRepository : BaseRepository<Post>
    {
        public override IQueryable<Post> Items => base.Items
            .Include(x => x.Users)
            .Include(x => x.Comments)
            .Include(x => x.Tags)
            .Include(p => p.User);
        public PostRepository(BlogContext db) : base(db) { }
    
    }
}
