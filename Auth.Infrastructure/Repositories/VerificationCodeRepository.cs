using Auth.Application.Repositories.Contracts;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories
{
    internal class VerificationCodeRepository : IVerificationCodeRepository
    {
        private readonly AppDbContext _db;

        public VerificationCodeRepository(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        public async Task Add(IdentityVerificationCode identityVerificationCode)
        {
            await _db.VerificationCodes.AddAsync(identityVerificationCode);
        }

        public async Task<IdentityVerificationCode?> GetByCode(uint code)
        {
            return await _db.VerificationCodes.FirstOrDefaultAsync(
                                    x => x.VerificationCode == code);
        }

        public async Task<int> GetTodaySendCount(string phoneNumber)
        {
            return await _db.VerificationCodes.CountAsync( 
                x => x.PhoneNumber == phoneNumber && x.VerificationDate.Date == DateTime.Now.Date);
        }

        public async Task<IdentityVerificationCode?> GetUnusedCodFor(string username)
        {
            return await _db.VerificationCodes
                .Where(_ => _.PhoneNumber == username && _.IsUsed == false)
                .OrderByDescending(_ => _.VerificationDate)
                .FirstOrDefaultAsync();
        }
    }
}
