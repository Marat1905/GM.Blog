using GM.Blog.DAL.Context;
using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GM.Blog.DAL.Repository
{
    public class CommentRepository : BaseRepository<Comment>
    {
        public override IQueryable<Comment> Items => base.Items
            .Include(x => x.User)
            .Include(x => x.Post);
        public CommentRepository(BlogContext db) : base(db) { }
    }
}
