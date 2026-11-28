using AgriTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AgriTrace.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace AgriTrace.Infrastructure.Persistence;

public class AgriTraceDbContext(DbContextOptions<AgriTraceDbContext> options) : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<Cooperative> Cooperatives => Set<Cooperative>();
    public DbSet<AgriProduct> AgriProducts => Set<AgriProduct>();
    public DbSet<ProductCertification> ProductCertifications => Set<ProductCertification>();
    public DbSet<HarvestBatch> HarvestBatches => Set<HarvestBatch>();
    public DbSet<FarmingLog> FarmingLogs => Set<FarmingLog>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ViolationReport> ViolationReports => Set<ViolationReport>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Tự động nạp toàn bộ class cấu hình IEntityTypeConfiguration<T> trong Infrastructure
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgriTraceDbContext).Assembly);
        // Cấu hình đặc thù PostgreSQL (tách riêng để Phần 4.6 chạy SQLite in-memory test không bị lỗi)
        if (Database.IsNpgsql())
        {
            modelBuilder.Entity<AgriProduct>(entity =>
            {
                entity.Property(p => p.NutritionInfoJson).HasColumnType("jsonb");
                entity.Property<uint>("xmin").IsRowVersion(); // Cấu hình đặc thù PostgreSQL (tách riêng để Phần 4.6 chạy SQLite in-memory test không bị lỗi)
            });
        }
    }
}