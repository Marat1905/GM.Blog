using AutoMapper;
using GM.Blog.BLL.Extensions;
using GM.Blog.BLL.Result.Users;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Users.Request;
using GM.Blog.BLL.ViewModels.Users.Response;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace GM.Blog.BLL.Services
{
    /// <summary>
    /// Сервис пользователя
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly ILogger<IUserService> _logger;

        public UserService(IMapper mapper, ILogger<IUserService> logger, UserManager<User> userManager, IRoleService roleService)
        {
            _userManager = userManager;
            _roleService = roleService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserCreateResult> CreateUserAsync(UserRegisterViewModel model,ICollection<Role>? roles=null)
        {
            var user = _mapper.Map<User>(model);

            if(roles == null)
            {
                var defaultRole = await _roleService.GetRoleByNameAsync("User");

                user.Roles ??= new List<Role>();

                user.Roles.Add(defaultRole!);
            }
            else
            {
                user.Roles = roles.Distinct().ToList();
            }
            
            var result = await _userManager.CreateAsync(user, model.Password);

            return new UserCreateResult(result,user);
        }


        public async IAsyncEnumerable<Claim> GetUserClaimsAsync(User user)
        {
            var userId = await _userManager.GetUserIdAsync(user);

            foreach (var role in user.Roles)
                yield return new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name!);
            yield return new Claim("UserID", userId);
        }

        public async Task<bool> DeleteByIdAsync(Guid id, Guid? userId, bool fullAccess)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var check = fullAccess
                ? user != null
                : user != null && user.Id == userId;

            if (check) 
                return (await _userManager.DeleteAsync(user!)).Succeeded;
            
            return false;
        }

        public async Task<UsersViewModel?> GetUsersAsync(Guid? roleId)
        {
            var users = _userManager.Users.ToList();
            var model = new UsersViewModel
            {
                Users = await _userManager.Users.Include(u => u.Roles).ToListAsync()
            };

            if (roleId != null)
                model.Users = model.Users
                    .SelectMany(u => u.Roles, (u, r) => new { User = u, RoleId = r.Id })
                    .Where(o => o.RoleId == roleId).Select(o => o.User).ToList();

            return model;
        }

        public async Task<UserEditResult> GetUserEditAsync(Guid id, string? userId, bool fullAccess)
        {
            var user = await _userManager.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id); 
            if (user == null) return new(null, new NotFoundResult());

            if (fullAccess || user.Id.ToString() == userId)
            {
                var model = _mapper.Map<UserEditViewModel>(user);
                model.AllRoles = (await _roleService.GetAllRolesAsync().ToListAsync()).Select(r => r.Name!).ToList();
                return new(model, null);
            }

            return new(null, new ForbidResult());
        }

        public async Task<bool> UpdateUserAsync(UserEditViewModel model)
        {
            //var user = await _userManager.FindByIdAsync(model.Id.ToString());
            var user = await _userManager.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == model.Id);
            if (user == null) return false;

            _mapper.Map(model, user);

            user.Roles = (await _roleService.ConvertRoleNamesInRoles(model.Roles).ToListAsync()).Distinct().ToList();

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var updateSecurityStampRes = await _userManager.UpdateSecurityStampAsync(user);
                return updateSecurityStampRes.Succeeded;
            }

            return false;
        }

        public async Task<UserViewModel?> GetUserAsync(Guid id)
        {
            var user = await _userManager.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;
            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<User?> CheckDataForLoginAsync(UserLoginViewModel model) => await _userManager.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == model.UserEmail);

      
    }
}
