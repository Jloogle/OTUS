using Domain.Entities;

namespace Domain.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string roleName);
}

