using Auth.Application.Repositories.Contracts;
using Auth.Domain.Entities;

namespace Auth.Infrastructure.Repositories
{
    internal class LoginLoggerRepository : ILoginLoggerRepository
    {
        private readonly AppDbContext _appDbContext;

        public LoginLoggerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Add(UserLoginLog log)
        {
            await _appDbContext.UserLoginLogs.AddAsync(log);
        }
    }
}
