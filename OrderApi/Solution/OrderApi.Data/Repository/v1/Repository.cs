using Microsoft.EntityFrameworkCore;
using OrderApi.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Data.Repository.v1
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly OrderContext _orderContext;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(OrderContext orderContext)
        {
            _orderContext = orderContext;
            _dbSet = _orderContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            try
            {
                return _dbSet.AsNoTracking();

            }
            catch (Exception)
            {
                throw new Exception("Couldn't retrieve entities");
            }
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity nao pode ser null!");
            }

            try
            {
                await _dbSet.AddAsync(entity);
                int retorno = await _orderContext.SaveChangesAsync();

                return (retorno > 0) ? entity : default;
            }
            catch (Exception)
            {
                throw new Exception($"{nameof(entity)} could not be saved");
            }

        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(UpdateAsync)} entity nao pode ser null!");
            }

            try
            {
                _dbSet.Update(entity);
                int retorno = await _orderContext.SaveChangesAsync();

                return (retorno > 0) ? entity : default;
            }
            catch (Exception)
            {
                throw new Exception($"{nameof(entity)} could not be updated");
            }
        }

        public async Task UpdateRangeAsync(List<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException($"{nameof(entities)} entities must not be null");
            }

            try
            {
                _dbSet.UpdateRange(entities);
                await _orderContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Entidade nao pode ser alterada");
            }

        }
    }
}
