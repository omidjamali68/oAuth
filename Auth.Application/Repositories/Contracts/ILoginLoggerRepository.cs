using Auth.Domain.Entities;

namespace Auth.Application.Repositories.Contracts
{
    public interface ILoginLoggerRepository
    {
        Task Add(UserLoginLog log);
    }
}
