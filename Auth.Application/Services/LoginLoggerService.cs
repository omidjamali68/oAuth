using Auth.Application.Repositories.Contracts;
using Auth.Application.Services.Contracts;
using Auth.Domain.Entities;
using UAParser;

namespace Auth.Application.Services
{
    internal class LoginLoggerService : IloginLoggerService
    {
        private readonly ILoginLoggerRepository _loginLoggerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LoginLoggerService(ILoginLoggerRepository loginLoggerRepository, IUnitOfWork unitOfWork)
        {
            _loginLoggerRepository = loginLoggerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task LogLoginAsync(
            string phoneNumber,
            string ip,
            string userAgent,
            LoginStatus status,
            LoginType type,
            LoginSource source,
            string note)
        {
            var parser = Parser.GetDefault();
            ClientInfo client = parser.Parse(userAgent);

            string deviceInfo = $"{client.Device.Family} / {client.OS.Family} {client.OS.Major}.{client.OS.Minor} / {client.UA.Family} {client.UA.Major}";

            var log = new UserLoginLog
            {
                UserId = phoneNumber,
                IpAddress = ip,
                UserAgent = userAgent,
                DeviceInfo = deviceInfo,
                LoginStatus = status,
                LoginSource = source,
                LoginType = type,
                Note = note,
                LoginTime = DateTime.Now
            };

            await _loginLoggerRepository.Add(log);

            await _unitOfWork.CompleteAsync();
        }
    }
}
