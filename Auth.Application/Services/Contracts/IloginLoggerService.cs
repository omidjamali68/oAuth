using Auth.Domain.Entities;

namespace Auth.Application.Services.Contracts
{
    public interface IloginLoggerService
    {
        Task LogLoginAsync(
            string phoneNumber,
            string ip,
            string userAgent,
            LoginStatus status,
            LoginType type,
            LoginSource source,
            string note);
    }
}
