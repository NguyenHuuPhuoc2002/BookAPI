using BookAPI.Repositories.Interfaces;
using BookAPI.Repositories.UnitOfWork;
using BookAPI.Services.Interfaces;
using BookAPI.Services;
using EcommerceWeb.Services;
using MailKit;

namespace BookAPI.Repositories
{
    public static class DependencyInjectionRepository
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<ISachRepository, SachRepository>();
            services.AddScoped<ILoaiRepository, LoaiRepository>();
            services.AddScoped<IGioHangRepository, GioHangRepository>();
            services.AddScoped<IGioHangChiTietRepository, GioHangChiTietRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IHoaDonRepository, HoaDonRepository>();
            services.AddScoped<IChiTietHoaDonRepository, ChiTietHoaDonRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IPublisherRepository, PublisherRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
           
        }
    }
}
