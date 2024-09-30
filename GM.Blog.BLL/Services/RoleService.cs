using AutoMapper;
using GM.Blog.BLL.Services.Interfaces;
using GM.Blog.BLL.ViewModels.Roles.Request;
using GM.Blog.BLL.ViewModels.Roles.Response;
using GM.Blog.DAL.Entityes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace GM.Blog.BLL.Services
{
    public class RoleService:IRoleService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<IRoleService> _logger;
        private readonly IMapper _mapper;

        public RoleService(RoleManager<Role> roleManager, UserManager<User> userManager, IMapper mapper,ILogger<IRoleService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<RolesViewModel?> GetRolesAsync(Guid? userId)
        {
            var model = new RolesViewModel();
            if (userId == null)
                model.Roles = await _roleManager.Roles.ToListAsync();
            else
            {
                var user = await _userManager.Users.Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null) return null;

                model.Roles = user.Roles;
            }

            return model;
        }

        public async Task<RoleViewModel?> GetRoleAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) return null;

            return _mapper.Map<RoleViewModel>(role);
        }

        public async Task<IdentityResult?> CreateRoleAsync(RoleCreateViewModel model)
        {
            var role = _mapper.Map<Role>(model);
            var result = await _roleManager.CreateAsync(role);

            return result;
        }

        public async Task<RoleEditViewModel?> GetRoleEditAsync(Guid id)
        {
            var checkRole = await _roleManager.FindByIdAsync(id.ToString());
            if (checkRole == null)
                return null;

            return _mapper.Map<RoleEditViewModel>(checkRole);
        }


        public async Task<IdentityResult?> UpdateRoleAsync(RoleEditViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id.ToString());
            if (role == null) return null;

            _mapper.Map(model, role);

            var result = await _roleManager.UpdateAsync(role);

            return result;
        }

       
        public async Task<IdentityResult?> DeleteRoleAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) return null;

            var result = await _roleManager.DeleteAsync(role);
            return result;
        }

        public  IAsyncEnumerable<Role> GetAllRolesAsync() =>  _roleManager.Roles.AsAsyncEnumerable();

        public async Task<Role?> GetRoleByNameAsync(string roleName) =>await _roleManager.FindByNameAsync(roleName);

        public IAsyncEnumerable<Role> GetRolesByUserAsync(Guid userId) =>
             _roleManager.Roles.Include(r => r.Users)
                .SelectMany(r => r.Users!, (r, u) => new { Role = r, UserId = u.Id })
                .Where(o => o.UserId == userId).Select(o => o.Role).AsAsyncEnumerable();

        public async IAsyncEnumerable<Role> GetEnabledRoleNamesWithRequest(HttpRequest request)
        {
            foreach (var pair in request.Form)
            {
                if (pair.Value == "on")
                {
                    var role = await _roleManager.FindByNameAsync(pair.Key);
                    if (role != null)
                    {
                       yield return  await Task.Run(() => role);
                    }
                }
            }
        }

        public async IAsyncEnumerable<Role> ConvertRoleNamesInRoles(ICollection<string> roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                   yield return await Task.Run(() => role);
            }
        }

        public async Task<string> CheckRoleAsync(string name)
        {
            var role = await _roleManager.FindByNameAsync(name ?? "");
            if (role != null)
                return $"Роль [{name}] уже существует!";

            return string.Empty;
        }
    }
}
