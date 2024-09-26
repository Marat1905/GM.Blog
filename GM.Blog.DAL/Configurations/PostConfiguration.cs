using GM.Blog.DAL.Entityes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GM.Blog.DAL.Configurations
{
    /// <summary>Конфигурация для таблицы статей</summary>
    /// <typeparam name="T">Тип идентификатора</typeparam>
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts").HasKey(x => x.Id);

            builder
                .HasMany(e => e.Users)
                .WithMany(e => e.VisitedPosts)
                .UsingEntity("UserToVisitedPost");
        }
    }
}
