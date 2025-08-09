using Auth.Application.Repositories.Contracts;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories
{
    internal class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly AppDbContext _db;

        public ApplicationUserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApplicationUser?> GetByUsername(string username)
        {
            return await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());
        }

        public async Task<bool> IsUserExist(string username) 
        {
            return await _db.ApplicationUsers.AnyAsync(x => x.UserName == username);
        }
    }
}
