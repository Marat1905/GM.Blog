using GM.Blog.DAL.Entityes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GM.Blog.DAL.Configurations
{
    /// <summary>Конфигурация для таблицы комментариев</summary>
    /// <typeparam name="T">Тип идентификатора</typeparam>
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments").HasKey(x => x.Id);

            //builder.HasOne(p=>p.User).WithMany(p=>p.Comments).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
