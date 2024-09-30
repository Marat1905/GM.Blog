using GM.Blog.DAL.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using GM.Blog.DAL.Service;

namespace GM.Blog.DAL.Extensions
{
    public static class DbRegistrator
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration Configuration) => services
           .AddDbContext<BlogContext>(opt =>
           {

               var type = Configuration["Type"];

               var t = Configuration.GetConnectionString(type);

               switch (type)
               {
                   case null: throw new InvalidOperationException("Не определён тип БД");
                   default: throw new InvalidOperationException($"Тип подключения {type} не поддерживается");

                   case "MSSQL":
                       opt.UseSqlServer(Configuration.GetConnectionString(type),
                                                                providerOptions =>
                                                                {
                                                                    providerOptions.CommandTimeout(180);
                                                                }
                                        );
                       break;
                   case "SQLite":
                       opt.UseSqlite(Configuration.GetConnectionString(type), b => b.MigrationsAssembly("GM.Blog.DAL"));
                       break;
               };
               opt.EnableSensitiveDataLogging(false);
           })
            .AddRepositories()
        ;
    }
}
