using BookAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories.RepositoryBase
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DataContext _context;
        public DbSet<TEntity> Entities { get; }
        public Repository(DataContext context)
        {
            _context = context;
            Entities = _context.Set<TEntity>();
        }
        public async Task<TEntity> GetById(string id)
        {
            return await Entities.FindAsync(id);
        }
        public async Task<TEntity> GetById(Guid id)
        {
            return await Entities.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await Entities.ToListAsync();
        }

        public async Task Add(TEntity entity)
        {
            await Entities.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(TEntity entity)
        {
            Entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TEntity entity)
        {
            Entities.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
