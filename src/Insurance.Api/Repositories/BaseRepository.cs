using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Insurance.Api.Model;
using System.Linq;

namespace Insurance.Api.Repositories
{
    public class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
    {
        protected readonly DbSet<TEntity> _objs;
        public BaseRepository(DbSet<TEntity> objs)
        {
            this._objs = objs ?? throw new ArgumentNullException(nameof(objs));

        }

        public async Task<TEntity> Add(TEntity obj)
        {
            var ret = await this._objs.AddAsync(obj);
            //await _db.SaveChangesAsync();
            return ret.Entity;
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await this._objs.ToListAsync();
        }

        public async Task<TEntity> Get(int id)
        {
            return await this._objs
                .Where(x => id.Equals(x.Id)).FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> GetByIdsAsync(List<int> ids)
        {
            return await this._objs
                .Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public void Delete(TEntity obj)
        {

            this._objs.Remove(obj);
        }
    }
}
