using Auth.Application.Dto;

namespace Auth.Application.Services.Contracts
{
    public interface IRegisterUserService
    {
        Task<ResponseDto> Register(RegisterRequestDto dto);
        Task<ResponseDto> QuickRegister(QuickRegisterDto dto);
        Task<ResponseDto> RegisterOrLogin(RegisterOrLoginDto dto);
    }
}
