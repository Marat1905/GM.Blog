using GM.Blog.BLL.Services;
using GM.Blog.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GM.Blog.BLL.Extensions
{
    public static class ServicesRegistrator
    {
        /// <summary> Добавление сервисов бизнес-логики в сервисы приложения </summary>
        public static IServiceCollection AddAppServices(this IServiceCollection services) => services
            .AddScoped<IUserService, UserService>()
            .AddScoped<IRoleService, RoleService>()
            .AddScoped<ITagService, TagService>()
            .AddScoped<ICommentService, CommentService>()
            .AddScoped<IPostService, PostService>()
            ;
    }
}
