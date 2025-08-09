using Auth.Application.Repositories.Contracts;
using Auth.Application.Services.Contracts;
using Auth.Domain.Entities;

namespace Auth.Application.Services
{
    internal class VerificationCodeService : IVerificationCodeService
    {
        private readonly IVerificationCodeRepository _verificationCodeRepository;

        public VerificationCodeService(IVerificationCodeRepository verificationCodeRepository)
        {
            _verificationCodeRepository = verificationCodeRepository;
        }

        public async Task Add(IdentityVerificationCode identityVerificationCode)
        {
            await _verificationCodeRepository.Add(identityVerificationCode);
        }

        public async Task<IdentityVerificationCode?> GetByCode(uint code)
        {
            return await _verificationCodeRepository.GetByCode(code); 
        }

        public async Task<int> GetTodaySendCount(string phoneNumber)
        {
            return await _verificationCodeRepository.GetTodaySendCount(phoneNumber);
        }

        public async Task<IdentityVerificationCode?> GetUnusedCodFor(string username)
        {
            return await _verificationCodeRepository.GetUnusedCodFor(username);
        }


    }
}
