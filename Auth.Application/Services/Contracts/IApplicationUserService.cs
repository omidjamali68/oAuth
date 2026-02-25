using Auth.Application.Dto;

namespace Auth.Application.Services.Contracts
{
    public interface IApplicationUserService
    {
        Task<ResponseDto> CompleteUserInfo(CompleteUserInfoDto dto);
    }
}
