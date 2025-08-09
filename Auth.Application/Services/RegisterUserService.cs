using Auth.Application.Common;
using Auth.Application.Dto;
using Auth.Application.Services.Contracts;
using Auth.Application.Settings;
using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Services
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserService(UserManager<ApplicationUser> userManager, IVerificationCodeService verificationCodeService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _verificationCodeService = verificationCodeService;
            _unitOfWork = unitOfWork;
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

            var confirmCodeResult = await ConfirmRegisteringVerificationCode(new ConfirmVerificationCodeDto
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

        private async Task<ResponseDto> ConfirmRegisteringVerificationCode(ConfirmVerificationCodeDto dto)
        {
            var result = ResponseDto.Create();

            var userVerification = await _verificationCodeService.GetUnusedCodFor(dto.PhoneNumber);

            if (userVerification == null)
            {
                result.CreateError("کد تایید برای این کاربر یافت نشد");
                return result;
            }

            var expireTime = DateTime.Now.Subtract(new TimeSpan(0, Setting.EXPIRE_VERIFICATIONCODE_TIME_MINUTE, 0));

            if (userVerification.VerificationDate < expireTime)
            {
                result.CreateError("کد تایید منقضی شده است");
                return result;
            }

            if (userVerification.TryCount >= 5)
            {
                result.CreateError("تعداد تلاش‌های ناموفق بیش از حد مجاز است. لطفاً کد جدید دریافت کنید.");
                return result;
            }

            userVerification.SentFromIP = dto.UserIp;
            userVerification.TryCount += 1;

            if (userVerification.VerificationCode != dto.VerificationCode)
            {                
                await _unitOfWork.CompleteAsync();
                result.CreateError("کد تایید وارد شده صحیح نمی‌باشد");
                return result;
            }

            userVerification.IsUsed = true;
            userVerification.UsedAt = DateTime.Now;

            await _unitOfWork.CompleteAsync();

            return result.Successful();
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
