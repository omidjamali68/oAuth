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
                FullName = string.IsNullOrWhiteSpace(dto.FullName) ? await GetRandomFullName() : dto.FullName,
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

                    return result.CreateError(createUserResult?.Errors?.FirstOrDefault()?.Description ?? "خطا در ثبت کاربر");
                }
            }
            catch (Exception ex)
            {
                return result.CreateError(ex.Message);
            }
        }

        public async Task<ResponseDto> QuickRegister(QuickRegisterDto dto)
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

            var code = await _verificationCodeService.GetByCode(dto.VerificationCode);
            if (code == null || code.PhoneNumber != dto.UserName || !code.IsUsed || 
                DateTime.Compare(code.UsedAt ?? default, DateTime.Now.AddMinutes(-10)) <= 0)
            {
                return result.CreateError("کد تایید صحیح نمی باشد");
            }

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                PhoneNumber = dto.UserName,
                FullName = await GetRandomFullName(),
                CreatedAt = DateTime.Now,
                PhoneNumberConfirmed = true
            };

            try
            {
                var createUserResult = await _userManager.CreateAsync(user);
                if (createUserResult.Succeeded)
                {
                    var tokenResult = await _authService.IssueToken(new IssueTokenRequestDto{
                        PhoneNumber = user.PhoneNumber,
                        UserAgent = dto.UserAgent,
                        UserIp = dto.UserIp
                    });

                    if (!tokenResult.IsSuccess)
                        return result.CreateError(tokenResult.Message);

                    var tokenData = (dynamic)tokenResult.Data;
                    var token = tokenData?.Token;

                    return result.Successful(
                        "کاربر با موفقیت ثبت شد", 
                        new {FullName = user.FullName, Id = user.Id, Token = token});
                }
                else
                {                    
                    return result.CreateError(createUserResult?.Errors?.FirstOrDefault()?.Description ?? "خطا در ثبت کاربر");
                }
            }
            catch (Exception ex)
            {
                return result.CreateError(ex.Message);
            }
        }

        private async Task<string> GetRandomFullName()
        {
            var users = await _unitOfWork.ApplicationUserRepository.GetAll();
            return $"کاربر {users?.Count + 100}";
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
