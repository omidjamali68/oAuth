using Auth.Application.Dto;

namespace Auth.Application.Services.Contracts
{
    public interface IRegisterUserService
    {
        Task<ResponseDto> Register(RegisterRequestDto dto);
    }
}
