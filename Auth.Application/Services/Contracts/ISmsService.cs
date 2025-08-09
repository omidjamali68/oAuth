using Auth.Application.Settings;
using Auth.Application.Dto;

namespace Auth.Application.Services.Contracts
{
    public interface ISmsService
    {
        Task<ResponseDto> VerifySendAsync(string mobile, List<SmsParams> parameters);
    }
}
