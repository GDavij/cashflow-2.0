using Cashflow.Core;
using Cashflow.Domain.Abstractions.RequestPipeline;
using Cashflow.Infrastructure.RequestPipeline;
using Microsoft.Extensions.Primitives;

namespace Cashflow.API;

public static class DependencyInjection
{
    public static IServiceCollection AddRequestPipeline(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        

        services.AddScoped<IAuthenticatedUser, AuthenticatedUser>((sp) =>
        {
            var contextAcessor = sp.GetRequiredService<IHttpContextAccessor>();

            string? ipAddress = contextAcessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            string? userAgent = contextAcessor.HttpContext?.Request.Headers.UserAgent.ToString();

            Guid traceIdentifier = Guid.NewGuid();

            var authenticatedUser = new AuthenticatedUser
            {
                Email = "Mock@email.com",
                Role = Roles.User,
                IpAddress = ipAddress,
                UserAgent = userAgent,
            };
            
            authenticatedUser.BindMessageTrace(traceIdentifier, 1);
            return authenticatedUser;
        });

        return services;
    }
}