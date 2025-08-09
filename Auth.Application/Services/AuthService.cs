using Auth.Application.Common;
using Auth.Application.Dto;
using Auth.Application.Repositories.Contracts;
using Auth.Application.Services.Contracts;
using Auth.Application.Settings;
using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Services
{
    public class AuthService : IAuthService
    {        
        private readonly IApplicationUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ISmsService _smsService;
        private readonly IloginLoggerService _loginLoggerService;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IJwtTokenGenerator jwtTokenGenerator,
            ISmsService smsService,
            IApplicationUserRepository userRepository,
            IloginLoggerService loginLoggerService,
            IVerificationCodeService verificationCodeService,
            IUnitOfWork unitOfWork,
            IRegisterUserService registerUserService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _smsService = smsService;
            _userRepository = userRepository;
            _loginLoggerService = loginLoggerService;
            _verificationCodeService = verificationCodeService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto> AssignRole(string userName, string roleName)
        {
            var result = ResponseDto.Create();

            var user = await _userRepository.GetByUsername(userName);
            if (user == null)
            {
                result.CreateError("کاربری با این نام کاربری یافت نشد");
                return result;
            }

            if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                result.CreateError($"نقش {roleName} در سیستم وجود ندارد. لطفا با مدیر سیستم تماس بگیرید");
                return result;
            }
            //await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, roleName);

            return result.Successful();
        }

        public async Task<LoginResponseDto> LoginByPassword(LoginRequestDto dto)
        {
            var result = new LoginResponseDto();

            var user = await _userRepository.GetByUsername(dto.UserName);
            if (user == null)
            {
                result.CreateError("کاربری با این نام کاربر یافت نشد");
                await _loginLoggerService.LogLoginAsync(
                    dto.UserName, dto.UserIp, dto.UserAgent, LoginStatus.Failed, LoginType.Password,LoginSource.Web, "کاربری با این نام کاربر یافت نشد");
                return result;
            }

            if (user.PhoneNumberConfirmed == false)
            {
                result.CreateError("کاربر احراز هویت نشده است");
                await _loginLoggerService.LogLoginAsync(
                    user.UserName, dto.UserIp, dto.UserAgent, LoginStatus.Failed, LoginType.Password,LoginSource.Web, "کاربر احراز هویت نشده است");
                return result;
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (isValid == false)
            {
                result.CreateError("کلمه عبور صحیح نمیباشد");
                await _loginLoggerService.LogLoginAsync(
                    user.UserName, dto.UserIp, dto.UserAgent, LoginStatus.Failed, LoginType.Password, LoginSource.Web, "کلمه عبور صحیح نمیباشد");
                return result;
            }            

            var token = _jwtTokenGenerator.GenerateToken(user);
            result.Successful($"{user.FullName} خوش آمدید", new { Token = token });

            await _loginLoggerService.LogLoginAsync(
                    user.UserName, dto.UserIp, dto.UserAgent, LoginStatus.Success, LoginType.Password, LoginSource.Web, $"{user.FullName} خوش آمدید");

            return result;
        }        

        public async Task<ResponseDto> LoginBySms(LoginBySmsRequestDto dto)
        {
            var result = ResponseDto.Create();

            var user = await _userRepository.GetByUsername(dto.PhoneNumber);
            if (user == null)
            {
                result.CreateError("کاربری با این نام کاربر یافت نشد");
                await _loginLoggerService.LogLoginAsync(
                    dto.PhoneNumber, dto.UserIp, dto.UserAgent, LoginStatus.Failed, LoginType.Sms, LoginSource.Web, "کاربری با این نام کاربر یافت نشد");
                return result;
            }

            var confirmCodeResult = await ConfirmVerificationCode(new ConfirmVerificationCodeDto
            {
                PhoneNumber = dto.PhoneNumber,
                UserIp = dto.UserIp,
                VerificationCode = dto.VerificationCode,
            });

            if (confirmCodeResult.IsSuccess == false)
            {
                await _loginLoggerService.LogLoginAsync(
                    user.UserName, dto.UserIp, dto.UserAgent, LoginStatus.Failed, LoginType.Sms, LoginSource.Web,confirmCodeResult.Message);                
                return confirmCodeResult;
            }

            await _loginLoggerService.LogLoginAsync(
                    user.UserName, dto.UserIp, dto.UserAgent, LoginStatus.Success, LoginType.Sms, LoginSource.Web, $"{user.FullName} خوش آمدید");

            var token = _jwtTokenGenerator.GenerateToken(user);
            return result.Successful($"{user.FullName} خوش آمدید", new { Token = token });
        }

        public async Task<ResponseDto> ConfirmVerificationCode(ConfirmVerificationCodeDto dto)
        {
            var result = ResponseDto.Create();

            var applicationUser = await _userManager.FindByNameAsync(dto.PhoneNumber);
            if(applicationUser == null)
            {
                result.CreateError("کاربر قبلا ثبت نام نکرده است");
                return result;
            }

            var userVerification = await _verificationCodeService.GetUnusedCodFor(dto.PhoneNumber);            
            if (userVerification == null)
            {
                result.CreateError("کد تایید برای این کاربر یافت نشد");
                return result;
            }

            userVerification.SentFromIP = dto.UserIp;
            userVerification.TryCount += 1;
            await _unitOfWork.CompleteAsync();

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

            if (userVerification.VerificationCode != dto.VerificationCode)
            {
                result.CreateError("کد تایید وارد شده صحیح نمی‌باشد");
                return result;
            }

            applicationUser.PhoneNumberConfirmed = true;

            userVerification.IsUsed = true;
            userVerification.UsedAt = DateTime.Now;

            await _unitOfWork.CompleteAsync();

            return result.Successful();
        }

        public async Task<ResponseDto> SendVerificationCode(SendVerificationCodeRequestDto dto)
        {
            var result = ResponseDto.Create();
            var todaySentCount = await _verificationCodeService.GetTodaySendCount(dto.PhoneNumber);
            if (todaySentCount > Setting.MAX_VERIFICATION_CODE_SEND_PER_DAY)
            {
                return result.CreateError(
                    $"حداکثر تعداد ارسال کد در روز {Setting.MAX_VERIFICATION_CODE_SEND_PER_DAY} میباشد");
            }

            var code = string.Empty.RandomInt(6);
            var smsResult = await _smsService.VerifySendAsync(dto.PhoneNumber, new List<SmsParams>() { new("cde", code) });

            await SaveApplicationUserVerificationCode(dto.PhoneNumber, uint.Parse(code), smsResult.Message);

            var isUserRegistered = await _userRepository.IsUserExist(dto.PhoneNumber);

            return result.Successful(new {IsUserRegistered = isUserRegistered});
        }

        private async Task SaveApplicationUserVerificationCode(string phoneNumber,
            uint verificationCode, string result)
        {
            await _verificationCodeService.Add(new IdentityVerificationCode
            {
                SMSResultDesc = result,
                VerificationCode = verificationCode,
                VerificationDate = DateTime.Now,
                PhoneNumber = phoneNumber,
                IsUsed = false,
            });            

            await _unitOfWork.CompleteAsync();
        }        

    }
}
