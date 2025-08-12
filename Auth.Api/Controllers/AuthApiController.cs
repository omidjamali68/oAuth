using Auth.Application.Dto;
using Auth.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRegisterUserService _registerUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public AuthApiController(
            IAuthService authService, IHttpContextAccessor httpContextAccessor, IRegisterUserService registerUserService, IConfiguration configuration)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _registerUserService = registerUserService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            dto.UserIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?
                .MapToIPv4().ToString();

            var result = await _registerUserService.Register(dto);

            if (!result.IsSuccess) 
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("verify-code")]
        public async Task<IActionResult> ConfirmVerificationCode(ConfirmVerificationCodeDto dto)
        {
            dto.UserIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?
                .MapToIPv4().ToString();

            var result = await _authService.ConfirmVerificationCode(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode(SendVerificationCodeRequestDto dto)
        {
            var result = await _authService.SendVerificationCode(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("login-by-sms")]
        public async Task<IActionResult> LoginBySms(LoginBySmsRequestDto dto)
        {
            dto.UserIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?
                .MapToIPv4().ToString();

            dto.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _authService.LoginBySms(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login-by-password")]
        public async Task<IActionResult> LoginByPassword([FromBody] LoginRequestDto dto)
        {
            dto.UserIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?
                .MapToIPv4().ToString();

            dto.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _authService.LoginByPassword(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var result = await _authService.AssignRole(dto.UserName, dto.RoleName);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("expired-verification-codes")]
        public async Task<IActionResult> DeleteExpiredVerificationCodes(DeleteExpiredVerificationCodeDto dto)
        {
            if (Request.Headers["X-App-Secret"] != _configuration.GetValue<string>("ApiSettings:InternalApiSecret"))
                return Unauthorized();

            var result = await _authService.DeleteExpiredVerificationCodes(dto.DelayInMinute);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
