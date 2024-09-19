using AutoMapper;
using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace BookAPI.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IDbContextTransaction? _transaction = null;
        private readonly IAccountRepository _accountRepository;
        private readonly IChiTietHoaDonRepository _chiTietHoaDonRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGioHangRepository _gioHangRepository;
        private readonly IHoaDonRepository _hoaDonRepository;
        private readonly ILoaiRepository _loaiRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISachRepository _sachRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IGioHangChiTietRepository _gioHangChiTietRepository;

        public UnitOfWork(DataContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
                          IMapper mapper, IUrlHelper urlHelper, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _configuration = configuration;
            _signInManager = signInManager;
            _accountRepository = new AccountRepository(_userManager, _signInManager, _configuration, _roleManager, null, _urlHelper);
            _chiTietHoaDonRepository = new ChiTietHoaDonRepository(_context, null);
            _userRepository = new UserRepository(_userManager);
            _gioHangRepository = new GioHangRepository(_context, mapper, null);
            _loaiRepository = new LoaiRepository(_context, null, mapper);
            _publisherRepository = new PublisherRepository(_context, mapper, null);
            _refreshTokenRepository = new RefreshTokenRepository(_context, null);
            _roleRepository = new RoleRepository(_roleManager);
            _sachRepository = new SachRepository(_context, mapper, null);
            _supplierRepository = new SupplierRepository(_context, null, mapper);
            _userRoleRepository = new UserRoleRepository(_userManager, _roleManager, _context);
            _gioHangChiTietRepository = new GioHangChiTietRepository(_context, mapper, null);
            _hoaDonRepository =  new HoaDonRepository(_context, mapper, null);
        }
        public IAccountRepository Accounts => _accountRepository;

        public IChiTietHoaDonRepository ChiTietHoaDons => _chiTietHoaDonRepository;

        public IGioHangRepository GioHangs => _gioHangRepository;

        public IHoaDonRepository HoaDons => _hoaDonRepository;

        public IGioHangChiTietRepository GioHangChiTiets => _gioHangChiTietRepository;

        public ILoaiRepository Loais => _loaiRepository;

        public IPublisherRepository Publishers => _publisherRepository;

        public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository;

        public IRoleRepository Roles => _roleRepository;

        public ISachRepository Sachs => _sachRepository;

        public ISupplierRepository Suppliers => _supplierRepository;

        public IUserRepository Users => _userRepository;

        public IUserRoleRepository UserRoles => _userRoleRepository;

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
            }
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
