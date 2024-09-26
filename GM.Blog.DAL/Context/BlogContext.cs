using GM.Blog.DAL.Configurations;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GM.Blog.DAL.Context
{
    /// <summary>Контекст базы данных </summary>
    public class BlogContext : IdentityDbContext<User,Role,Guid>
    {
       
        public BlogContext(DbContextOptions<BlogContext> options): base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new PostConfiguration());
            builder.ApplyConfiguration(new TagConfiguration());
            builder.ApplyConfiguration(new CommentConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
