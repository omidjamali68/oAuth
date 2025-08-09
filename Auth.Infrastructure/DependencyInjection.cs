using Auth.Application.Repositories.Contracts;
using Auth.Application.Services.Contracts;
using Auth.Infrastructure.Repositories;
using Auth.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDependencyInjection(this IServiceCollection services)
        {            
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<ILoginLoggerRepository, LoginLoggerRepository>();
            services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
