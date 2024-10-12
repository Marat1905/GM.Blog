using GM.Blog.DAL.Extensions;
using GM.Blog.BLL.Extensions;
using System.Reflection;
using GM.Blog.BLL;
using GM.Blog.DAL.Context;
using GM.Blog.DAL.Entityes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var assembly = Assembly.GetAssembly(typeof(MappingProfile));

builder.Services
    .AddDatabase(builder.Configuration.GetSection("Database"))
    .AddAppServices()
    .AddAutoMapper(assembly)
    .AddIdentity<User, Role>(cfg =>
    {
        cfg.Password.RequiredLength = 8;
        cfg.Password.RequireNonAlphanumeric = false;
        cfg.Password.RequireUppercase = false;
        cfg.Password.RequireLowercase = false;
        cfg.Password.RequireDigit = false;
    })
    .AddEntityFrameworkStores<BlogContext>();
;



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
