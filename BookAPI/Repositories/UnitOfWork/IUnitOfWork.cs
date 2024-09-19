using BookAPI.Repositories.Interfaces;

namespace BookAPI.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Accounts { get; }
        IChiTietHoaDonRepository ChiTietHoaDons { get; }
        IGioHangRepository GioHangs { get; }
        IHoaDonRepository HoaDons { get; }
        IGioHangChiTietRepository GioHangChiTiets { get; }
        ILoaiRepository Loais { get; }
        IPublisherRepository Publishers { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IRoleRepository Roles { get; }
        ISachRepository Sachs { get; }
        ISupplierRepository Suppliers { get; }
        IUserRepository Users { get; }
        IUserRoleRepository UserRoles { get; }
        void CommitTransaction();
        void RollbackTransaction();
        void BeginTransaction();
        Task<int> SaveChangesAsync();
    }
}
