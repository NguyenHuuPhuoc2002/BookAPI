using BookAPI.Models;
using BookAPI.Repositories.Database;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class LoaiRepository : ILoaiRepository
    {
        private readonly DataContext _context;

        public LoaiRepository(DataContext context) 
        {
            _context = context;
        }
        public Task<IEnumerable<LoaiModel>> GetAllLoaiAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LoaiModel> GetLoaiByIdAsync()
        {
            throw new NotImplementedException();
        }
    }
}
