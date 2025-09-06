using Auth.Application.Dto;

namespace Auth.Application.Services.Contracts
{
    public interface IAuthService
    {        
        Task<LoginResponseDto> LoginByPassword(LoginRequestDto dto);
        Task<ResponseDto> AssignRole(string userName,  string roleName);
        Task<ResponseDto> ConfirmVerificationCode(ConfirmVerificationCodeDto dto);
        Task<ResponseDto> SendVerificationCode(SendVerificationCodeRequestDto dto);
        Task<ResponseDto> LoginBySms(LoginBySmsRequestDto dto);
        Task<ResponseDto> DeleteExpiredVerificationCodes(int delayInMinute);
        Task<ResponseDto> IssueToken(IssueTokenRequestDto dto);
    }
}
