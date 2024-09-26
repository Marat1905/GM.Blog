using GM.Blog.DAL.Entityes;
using Microsoft.EntityFrameworkCore;

namespace GM.Blog.DAL.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Role> builder)
        {
           builder.ToTable("Roles").HasKey(t => t.Id);
            builder.HasData(
                  new Role()
                  {
                      Id = Guid.NewGuid(),
                      Name = "User",
                      NormalizedName = "USER",
                      Description = "Стандартная роль в приложении",
                  },
                  new Role()
                  {
                      Id = Guid.NewGuid(),
                      Name = "Moderator",
                      NormalizedName = "MODERATOR",
                      Description = "Данная роль позволяет выполнять редактирование, удаление комментариев и статей в приложении"
                  },
                  new Role()
                  {
                      Id = Guid.NewGuid(),
                      Name = "Admin",
                      NormalizedName = "ADMIN",
                      Description = "Роль с максимальными возможностями в приложении"
                  }
              );
        }
    }
}
