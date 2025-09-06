using Auth.Application.Common;
using Auth.Application.Dto;
using Auth.Application.Services.Contracts;
using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Services
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserService(
            UserManager<ApplicationUser> userManager, IVerificationCodeService verificationCodeService, IUnitOfWork unitOfWork, IAuthService authService)
        {
            _userManager = userManager;
            _verificationCodeService = verificationCodeService;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<ResponseDto> Register(RegisterRequestDto dto)
        {
            var result = ResponseDto.Create();

            if (string.IsNullOrEmpty(dto.UserName))
            {
                return result.CreateError("لطفا شماره همراه را وارد کنید");
            }

            if (!dto.UserName.IsValidMobile())
            {
                return result.CreateError("شماره همراه باید به صورت 09120000000 وارد شود");
            }            

            var confirmCodeResult = await _authService.ConfirmVerificationCode(new ConfirmVerificationCodeDto
            {
                PhoneNumber = dto.UserName,
                UserIp = dto.UserIp,
                VerificationCode = dto.VerificationCode,
            });

            if (confirmCodeResult.IsSuccess == false)
            {
                return confirmCodeResult;
            }

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.UserName,
                NormalizedEmail = dto.Email?.ToUpper(),
                FullName = dto.FullName,
                NationalCode = dto.NationalCode,
                CreatedAt = DateTime.Now,
                PhoneNumberConfirmed = true
            };

            try
            {
                var createUserResult = await _userManager.CreateAsync(user, dto.Password);
                if (createUserResult.Succeeded)
                {
                    return result.Successful("کاربر با موفقیت ثبت شد", new {UserId = user.Id});
                }
                else
                {
                    await UndoUsedVerificationCode(dto);

                    return result.CreateError(
                        createUserResult.Errors.FirstOrDefault()?.Description);
                }
            }
            catch (Exception ex)
            {
                return result.CreateError(ex.Message);
            }
        }                

        private async Task UndoUsedVerificationCode(RegisterRequestDto dto)
        {
            var verificationCode = await _verificationCodeService.GetByCode(dto.VerificationCode);
            if (verificationCode != null)
            {
                verificationCode.IsUsed = false;
                verificationCode.UsedAt = null;

                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
