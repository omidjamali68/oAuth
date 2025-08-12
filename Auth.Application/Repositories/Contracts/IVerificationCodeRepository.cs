using Auth.Domain.Entities;

namespace Auth.Application.Repositories.Contracts
{
    public interface IVerificationCodeRepository
    {
        Task Add(IdentityVerificationCode identityVerificationCode);
        Task<IdentityVerificationCode?> GetByCode(uint code);
        Task<List<IdentityVerificationCode>> GetAll();
        Task<int> GetTodaySendCount(string phoneNumber);
        Task<IdentityVerificationCode?> GetUnusedCodFor(string username);
        void DeleteRange(IEnumerable<IdentityVerificationCode> expiredCodes);
    }
}
