using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Cinema_Ticket_Bocking.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<EntityEntry<T>> AddAsync(T entity);
     

        Task<IEnumerable<T>> GetAsync(
           Expression<Func<T, bool>>? filter = null,
           Expression<Func<T, object>>[]? icludes = null,
           bool tracked = true
           );
    
        Task<T?> GetoneAsync(
          Expression<Func<T, bool>>? filter = null,
          Expression<Func<T, object>>[]? icludes = null,
          bool tracked = true
          );

         void Update(T entity);
      
        void Delete(T entity);
      
        Task<int> CommittAsync();
    

    }
}
