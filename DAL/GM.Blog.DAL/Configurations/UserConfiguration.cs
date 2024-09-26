using GM.Blog.DAL.Entityes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GM.Blog.DAL.Configurations
{
    /// <summary>Конфигурация для таблицы пользователя</summary>
    /// <typeparam name="T">Тип идентификатора</typeparam>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users").HasKey(x => x.Id);

            builder
                .HasMany(e => e.Posts)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();
        }
    }
}
