using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insurance.Api.Repositories
{
    public interface IRepository<TEntity>
    {
        Task<List<TEntity>> GetAll();
        Task<TEntity> Get(int id);
        Task<List<TEntity>> GetByIdsAsync(List<int> ids);
        Task<TEntity> Add(TEntity obj);
        void Delete(TEntity obj);
    }
}