using Auth.Domain.Entities;

namespace Auth.Application.Repositories.Contracts
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser?> GetByUsername(string username);
        Task<bool> IsUserExist(string username);
    }
}
