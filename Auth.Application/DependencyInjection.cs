using Auth.Application.Services;
using Auth.Application.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServiceDependencyInjection(this IServiceCollection services)
        {            
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISmsService, SmsServiceIr>();
            services.AddScoped<IloginLoggerService, LoginLoggerService>();
            services.AddScoped<IVerificationCodeService, VerificationCodeService>();
            services.AddScoped<IRegisterUserService, RegisterUserService>();
            services.AddScoped<IApplicationUserService, ApplicationUserService>();

            return services;
        }
    }
}
