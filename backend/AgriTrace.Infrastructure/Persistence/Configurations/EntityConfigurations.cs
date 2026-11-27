using AgriTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace AgriTrace.nfrastructure.Persistence.Configurations;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("regions");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();
        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Code).HasMaxLength(20).IsRequired();
        builder.HasIndex(r => r.Code).IsUnique();
    }
}

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable("provinces");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Name).HasMaxLength(120).IsRequired();
        builder.Property(p => p.Code).HasMaxLength(20).IsRequired();
        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasIndex(p => p.RegionId);
        builder.HasOne<Region>().WithMany().HasForeignKey(p =>p.RegionId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CooperativeConfiguration : IEntityTypeConfiguration<Cooperative>
{
    public void Configure(EntityTypeBuilder<Cooperative> builder)
    {
        builder.ToTable("cooperatives");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Address).HasMaxLength(300).IsRequired();
        builder.HasIndex(c => c.ProvinceId);
        builder.HasIndex(c => c.OwnerUserId). IsUnique();
        builder.HasOne<Province>().WithMany().HasForeignKey(c =>c.ProvinceId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class AgriProductConfiguration : IEntityTypeConfiguration<AgriProduct>
{
    public void Configure(EntityTypeBuilder<AgriProduct> builder)
    {
        builder.ToTable("agri_products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(2000).IsRequired();
        builder.Property(p => p.Price).HasPrecision(18,2);
        builder.Property(p => p.NutritionInfoJson).IsRequired();
        builder.Property(p => p.SuspensionReason).HasMaxLength(500);

        // Index cho các cột lọc theo checklist
        builder.HasIndex(p => p.ProvinceId);
        builder.HasIndex(p => p.CooperativeId);
        builder.HasIndex(p => p.Status);

        builder.HasOne<Cooperative>().WithMany().HasForeignKey(p => p.CooperativeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Province>().WithMany().HasForeignKey(p => p.ProvinceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.Certifications).WithOne().HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p =>p.Certifications).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class ProductCertificationConfiguration : IEntityTypeConfiguration<ProductCertification>
{
    public void Configure(EntityTypeBuilder<ProductCertification> builder)
    {
        builder.ToTable("product_certifications");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.GradeOrCode).HasMaxLength(100).IsRequired();
        builder.Property(c => c.IssuedBy).HasMaxLength(200).IsRequired();

        // Composite Index (ProductId, Type) theo checklist
        builder.HasIndex(c => new
        {
            c.ProductId,
            c.Type
        });
    }
}

public class HarvestBatchConfiguration : IEntityTypeConfiguration<HarvestBatch>
{
    public void Configure(EntityTypeBuilder<HarvestBatch> builder)
    {
        builder.ToTable("harvest_batches");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.BatchCode).HasMaxLength(60).IsRequired();
        builder.Property(b => b.ExpectedYieldKg).HasPrecision(18, 2);
        builder.Property(b => b.QrSignature).HasMaxLength(128);

        // Unique Index trên BatchCode theo checklist
        builder.HasIndex(b => b.BatchCode).IsUnique();
        builder.HasIndex(b => b.ProductId);

        builder.HasOne<AgriProduct>().WithMany().HasForeignKey(b => b.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(b => b.FarmingLogs).WithOne().HasForeignKey(l => l.HarvestBatchId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(b => b.FarmingLogs).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class FarmingLogConfiguration : IEntityTypeConfiguration<FarmingLog>
{
    public void Configure(EntityTypeBuilder<FarmingLog> builder)
    {
        builder.ToTable("farming_logs");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.ActivityType).HasMaxLength(100).IsRequired();
        builder.Property(l => l.Description).HasMaxLength(1000).IsRequired();
        builder.Property(l => l.InputMaterial).HasMaxLength(200);
        builder.Property(l => l.PhotoUrl).HasMaxLength(500);
        builder.HasIndex(l => l.HarvestBatchId);

        // Bỏ qua thuộc tính tính toán (không tạo cột thừa trong DB)
        builder.Ignore(l => l.EarliestSafeDate);
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.ShippingAddress).HasMaxLength(300).IsRequired();
        builder.Property(o => o.ReceiverPhone).HasMaxLength(20).IsRequired();
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Property(o => o.CancelReason).HasMaxLength(500);
        builder.Property(o => o.IdempotencyKey).HasMaxLength(100);

        // Unique Index trên Order.IdempotencyKey theo checklist
        builder.HasIndex(o =>o.IdempotencyKey).IsUnique();
        builder.HasIndex(o => o.BuyerUserId);
        builder.HasIndex(o => new
        {
            o.Status,
            o.PaymentDeadlineUtc
        });

        builder.HasMany(o =>o.Items).WithOne().HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(o =>o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.HasIndex(i => i.OrderId);
        builder.HasOne<AgriProduct>().WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);

        // Bỏ qua thuộc tính tính toán Subtotal
        builder.Ignore(i => i.Subtotal);
    }
}

public class ViolationReportConfiguration : IEntityTypeConfiguration<ViolationReport>
{
    public void Configure(EntityTypeBuilder<ViolationReport> builder)
    {
        builder.ToTable("violation_reports");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(r => r.EvidenceUrl).HasMaxLength(500);
        builder.Property(r => r.ResolutionNote).HasMaxLength(1000);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => new
        {
            r.TargetType,
            r.TargetId
        });
    }
}