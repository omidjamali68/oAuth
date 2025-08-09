using Auth.Domain.Entities;

namespace Auth.Application.Services.Contracts
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser);
    }
}
