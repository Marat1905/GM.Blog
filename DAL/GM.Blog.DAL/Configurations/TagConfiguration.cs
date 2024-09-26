using GM.Blog.DAL.Entityes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GM.Blog.DAL.Configurations
{
    /// <summary>Конфигурация для таблицы тегов</summary>
    /// <typeparam name="T">Тип идентификатора</typeparam>
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags").HasKey(x => x.Id);
        }
    }
}
