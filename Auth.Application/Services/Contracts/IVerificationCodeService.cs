using Auth.Domain.Entities;

namespace Auth.Application.Services.Contracts
{
    public interface IVerificationCodeService
    {
        Task<IdentityVerificationCode?> GetUnusedCodFor(string username);
        Task<IdentityVerificationCode?> GetByCode(uint code);
        Task<int> GetTodaySendCount(string phoneNumber);
        Task Add(IdentityVerificationCode identityVerificationCode);
        Task<List<IdentityVerificationCode>> GetAll();
        void DeleteRange(IEnumerable<IdentityVerificationCode> expiredCodes);
    }
}
