using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repository;

public class RoleRepository(IAdapterApplicationContext context) : IRoleRepository
{
    private readonly ApplicationContext _ctx = context.getContext();
    public async Task<Role?> GetByNameAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName)) return null;
        return await _ctx.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
    }
}

