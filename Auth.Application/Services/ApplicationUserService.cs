using Auth.Application.Dto;
using Auth.Application.Services.Contracts;
using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace Auth.Application.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ResponseDto> CompleteUserInfo(CompleteUserInfoDto dto)
        {
            var result = ResponseDto.Create();

            if (string.IsNullOrWhiteSpace(dto.UserId))
            {
                return result.CreateError("شناسه کاربر ارسال نشده است");
            }

            var user = await _unitOfWork.ApplicationUserRepository.GetById(dto.UserId);
            if (user == null)
            {
                return result.CreateError("کاربری با این مشخصات یافت نشد");
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                user.FullName = dto.FullName;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (!IsValidEmail(dto.Email))
                {
                    return result.CreateError("ایمیل وارد شده معتبر نیست");
                }

                user.Email = dto.Email;
                user.NormalizedEmail = dto.Email.ToUpperInvariant();
            }

            if (!string.IsNullOrWhiteSpace(dto.NationalCode))
            {
                if (!IsValidNationalCode(dto.NationalCode))
                {
                    return result.CreateError("کد ملی وارد شده معتبر نیست");
                }

                user.NationalCode = dto.NationalCode;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var hasPassword = await _userManager.HasPasswordAsync(user);

                if (hasPassword)
                {
                    var removeResult = await _userManager.RemovePasswordAsync(user);
                    if (!removeResult.Succeeded)
                    {
                        var error = removeResult.Errors.FirstOrDefault()?.Description ?? "خطا در حذف کلمه عبور قبلی";
                        return result.CreateError(error);
                    }
                }

                var addResult = await _userManager.AddPasswordAsync(user, dto.Password);
                if (!addResult.Succeeded)
                {
                    var error = addResult.Errors.FirstOrDefault()?.Description ?? "خطا در تنظیم کلمه عبور";
                    return result.CreateError(error);
                }
            }

            await _unitOfWork.CompleteAsync();

            return result.Successful("اطلاعات کاربر با موفقیت به‌روزرسانی شد");
        }

        private bool IsValidEmail(string email)
        {
            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsValidNationalCode(string nationalCode)
        {
            if (string.IsNullOrWhiteSpace(nationalCode))
                return false;

            nationalCode = nationalCode.Trim();

            if (nationalCode.Length != 10 || !nationalCode.All(char.IsDigit))
                return false;

            var invalidCodes = new[]
            {
                "0000000000", "1111111111", "2222222222", "3333333333",
                "4444444444", "5555555555", "6666666666", "7777777777",
                "8888888888", "9999999999"
            };

            if (invalidCodes.Contains(nationalCode))
                return false;

            var check = int.Parse(nationalCode[9].ToString());
            var sum = 0;

            for (var i = 0; i < 9; i++)
            {
                sum += int.Parse(nationalCode[i].ToString()) * (10 - i);
            }

            var remainder = sum % 11;

            return (remainder < 2 && check == remainder) || (remainder >= 2 && check + remainder == 11);
        }
    }
}
