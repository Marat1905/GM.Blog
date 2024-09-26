using GM.Blog.DAL.Entityes;
using GM.Blog.DAL.Interfaces;
using GM.Blog.DAL.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace GM.Blog.DAL.Service
{
    public static class Registrator
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services) => services
            .AddScoped<IRepository<Comment>, CommentRepository>()
            .AddScoped<IRepository<Post>, PostRepository>()
            .AddScoped<IRepository<Tag>, TagRepository>()
            ;
    }
}
