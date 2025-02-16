using Cashflow.Domain.Abstractions.DataAccess;
using Cashflow.Infrastructure.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cashflow.Infrastructure.DataAccess;

public static class DependencyInjection
{
    
 public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<ICashflowDbContext, CashflowDbContext>(cfg =>
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                cfg.EnableSensitiveDataLogging();
            }
            
            cfg.UseAzureSql(configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }}