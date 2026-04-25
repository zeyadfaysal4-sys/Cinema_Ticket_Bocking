using Cinema_Ticket_Bocking.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Cinema_Ticket_Bocking.Repository
{
    public class Repositories<T> : IRepository<T> where T : class
    {
        ApplicationDbContext _context;
        DbSet<T> _dbSet;
        public Repositories(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<EntityEntry<T>> AddAsync(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? icludes = null,
            bool tracked = true
            )
        {
            var entities = _dbSet.AsQueryable();

            if (filter != null)
            {
                entities = entities.Where(filter);
            }
            
            if (icludes != null)
            {
                foreach(var iclude in icludes)
                entities = entities.Include(iclude);
            }

            if (!tracked)
            {
                entities = entities.AsNoTracking();
            }
            return await entities.ToListAsync();
        }

        public async Task<T?> GetoneAsync(
          Expression<Func<T, bool>>? filter = null,
          Expression<Func<T, object>>[]? icludes = null,
          bool tracked = true
          )
        {
            var entities = _dbSet.AsQueryable();

            if (filter != null)
            {
                entities = entities.Where(filter);
            }

            if (icludes != null)
            {
                foreach (var iclude in icludes)
                    entities = entities.Include(iclude);
            }

            if (!tracked)
            {
                entities = entities.AsNoTracking();
            }
            return await entities.FirstOrDefaultAsync();
        }

        public void Update(T entity)
        {  
            _dbSet.Update(entity); 
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> CommittAsync()
        {
            try 
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1; 
            }
        }

       
    }
}
