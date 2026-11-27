namespace AgriTrace.Domain.Interfaces;
/// <summary> Quản lý thay đổi và giao dịch (Transaction) nguyên tử trên cơ sở dữ liệu</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}