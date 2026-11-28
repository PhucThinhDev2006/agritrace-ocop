using System.Runtime.CompilerServices;
using AgriTrace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AgriTrace.Domain.Interfaces;
using AgriTrace.Infrastructure.Persistence.Repositories;
namespace AgriTrace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Chưa cấu hình chuỗi kết nối 'DefaultConnection'.");
        services.AddDbContext<AgriTraceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IRegionRepository, EfRegionRepository>();
        services.AddScoped<ICooperativeRepository, EfCooperativeRepository>();
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<IHarvestBatchRepository, EfHarvestBatchRepository>();
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<IViolationReportRepository, EfViolationReportRepository>();
        return services;
    }
}