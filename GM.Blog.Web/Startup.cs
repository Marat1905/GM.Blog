using AutoMapper;
using GM.Blog.BLL;
using GM.Blog.DAL.Extensions;
using GM.Blog.DAL.Context;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Http.Extensions;
using GM.Blog.BLL.Extensions;

namespace GM.Blog.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        /// <summary>
        /// Метод вызывается средой ASP.NET.
        /// Используйте его для подключения сервисов приложения
        /// Документация:  https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        public void ConfigureServices( IServiceCollection services)
        {

            var mapperConfig = new MapperConfiguration((v) =>
            {
                v.AddProfile(new MappingProfile());
            });

            IMapper mapper=mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddDatabase(_configuration.GetSection("Database"));

            var t = _configuration.GetConnectionString("Data Source=..\\DAL\\GM.Blog.DAL\\Database\\Blog.db");

            services.AddAppServices();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddIdentity<User, Role>(cfg =>
            {
                cfg.Password.RequiredLength = 8;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<BlogContext>();

            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Login";
                opt.AccessDeniedPath = "/AccessDenied";
            });

            services.AddSession();
        }



        /// <summary>
        /// Метод вызывается средой ASP.NET.
        /// Используйте его для настройки конвейера запросов
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Проверяем, не запущен ли проект в среде разработки
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                app.UseHsts();
            }

            app.UseHttpsRedirection();
            var cachePeriod = "0";
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                }
            });

           

            app.UseStatusCodePages(async statusCodeContext =>
            {
                var response = statusCodeContext.HttpContext.Response;

                response.ContentType = "text/plain; charset=UTF-8";
                if (response.StatusCode == 400)
                    response.Redirect("/BadRequest");

                else if (response.StatusCode == 404)
                    response.Redirect("/NotFound");

                else if (response.StatusCode == 401)
                {
                    var returnUrl = statusCodeContext.HttpContext.Request.GetEncodedPathAndQuery().Replace("/", "%2F");
                    response.Redirect($"/Login?ReturnUrl={returnUrl}");
                }
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();


            //Добавляем компонент с настройкой маршрутов для главной страницы
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

        }
    }
    
}
