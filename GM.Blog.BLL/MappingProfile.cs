using AutoMapper;
using GM.Blog.BLL.ViewModels.Comments.Request;
using GM.Blog.BLL.ViewModels.Posts.Request;
using GM.Blog.BLL.ViewModels.Posts.Response;
using GM.Blog.BLL.ViewModels.Roles.Request;
using GM.Blog.BLL.ViewModels.Roles.Response;
using GM.Blog.BLL.ViewModels.Tags.Request;
using GM.Blog.BLL.ViewModels.Tags.Response;
using GM.Blog.BLL.ViewModels.Users.Request;
using GM.Blog.BLL.ViewModels.Users.Response;
using GM.Blog.DAL.Entityes;

namespace GM.Blog.BLL
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegisterViewModel, User>()
                .ForMember(u => u.Email, opt => opt.MapFrom(m => m.Email))
                .ForMember(u => u.UserName, opt => opt.MapFrom(m => m.Email));

            CreateMap<User, UserViewModel>();
            CreateMap<UserEditViewModel, User>()
                .ForMember(dest => dest.Roles, act => act.Ignore());
            CreateMap<User, UserEditViewModel>()
               .ForMember(u => u.Roles, opt => opt.MapFrom(m => m.Roles.Select(r => r.Name)));

            CreateMap<TagCreateViewModel, Tag>();

            CreateMap<PostCreateViewModel, Post>();
            //.ForMember(u => u.PostTags, config => config.Ignore());

            CreateMap<Post, PostEditViewModel>()
                 .ForMember(m => m.PostTags, opt => opt.MapFrom(p => string.Join(" ", p.Tags.Select(p => p.Name))));

            CreateMap<PostEditViewModel, Post>()
                .ForMember(dest => dest.UserId, act => act.Ignore());
            CreateMap<Post, PostViewModel>();

            CreateMap<Tag, TagEditViewModel>();
            CreateMap<Tag, TagViewModel>();
            CreateMap<TagEditViewModel, Tag>();

            CreateMap<CommentCreateViewModel, Comment>();
            CreateMap<Comment, CommentEditViewModel>();
            CreateMap< CommentEditViewModel, Comment>();

            CreateMap<RoleCreateViewModel, Role>()
                 .ForMember(m => m.NormalizedName, opt => opt.MapFrom(p => p.Name.ToUpper()));
            CreateMap<Role, RoleEditViewModel>();
            CreateMap<Role, RoleViewModel>();
            CreateMap<RoleEditViewModel, Role>();
        }
    }
}
