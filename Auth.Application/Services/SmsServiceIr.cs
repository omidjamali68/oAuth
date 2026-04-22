using Auth.Application.Dto;
using Auth.Application.Services.Contracts;
using Auth.Application.Settings;
using IPE.SmsIrClient;
using IPE.SmsIrClient.Models.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auth.Application.Services
{
    public class SmsServiceIr : ISmsService
    {
        private readonly IrSmsSetting _smsSetting;
        private readonly ILogger<SmsServiceIr> _logger;

        public SmsServiceIr(IOptions<IrSmsSetting> smsSetting, ILogger<SmsServiceIr> logger)
        {
            _smsSetting = smsSetting.Value;
            _logger = logger;
        }

        public async Task<ResponseDto> VerifySendAsync(string mobile, List<SmsParams> parameters)
        {
            var result = ResponseDto.Create();
            try
            {
                var normalizedMobile = (mobile ?? string.Empty).Trim();
                var developerNumber = (_smsSetting.SkipNumber ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(developerNumber) &&
                    string.Equals(normalizedMobile, developerNumber, StringComparison.OrdinalIgnoreCase))
                {
                    return result.Successful("ارسال پیامک برای شماره توسعه‌دهنده غیرفعال است و فقط در دیتابیس ثبت شد");
                }

                string apiKey = _smsSetting.ApiKey;
                string number = _smsSetting.Number;
                int templateId = int.Parse(_smsSetting.LoginPaternCode);
                SmsIr smsIr = new SmsIr(apiKey);
                List<VerifySendParameter> pms = new List<VerifySendParameter>();
                parameters.ForEach(x => pms.Add(new(x.Key, x.Value)));
                var verificationSendResult = await smsIr.VerifySendAsync(normalizedMobile, templateId, pms.ToArray());
                if (verificationSendResult.Status == 1)
                {
                    result.Successful("پیامک ارسال شد");
                }
                else
                {                    
                    result.CreateError($"خطا در ارسال پیامک : {verificationSendResult.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($" خطا در سرویس ارسال پیامک ایران sms : {ex.Message}" );
                result.CreateError(ex.Message);
            }

            return result;
        }
    }
}
