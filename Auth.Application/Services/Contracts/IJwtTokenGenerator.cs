using Auth.Domain.Entities;

namespace Auth.Application.Services.Contracts
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateToken(ApplicationUser applicationUser);
    }
}
