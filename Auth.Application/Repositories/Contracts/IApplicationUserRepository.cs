using Auth.Domain.Entities;

namespace Auth.Application.Repositories.Contracts
{
    public interface IApplicationUserRepository
    {
        Task<List<ApplicationUser>?> GetAll();
        Task<ApplicationUser?> GetByUsername(string username);
        Task<bool> IsUserExist(string username);
        Task<ApplicationUser?> GetById(string id);
    }
}
