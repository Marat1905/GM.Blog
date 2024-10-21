using GM.Blog.DAL.Extensions;
using GM.Blog.BLL.Extensions;
using System.Reflection;
using GM.Blog.BLL;
using GM.Blog.DAL.Context;
using GM.Blog.DAL.Entityes;
using GM.Blog.BLL.ViewModels.Users.Response;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(opt =>
{
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xml = $"{Assembly.GetAssembly(typeof(UserViewModel)).GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
    opt.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xml));
    opt.SupportNonNullableReferenceTypes();
});

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
    app.UseSwaggerUI(o =>
    {
        o.InjectStylesheet("/css/swagger-custom.css");
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
