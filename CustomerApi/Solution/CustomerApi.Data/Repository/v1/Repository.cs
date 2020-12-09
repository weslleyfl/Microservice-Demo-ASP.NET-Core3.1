using CustomerApi.Data.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Data.Repository.v1
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly CustomerContext _customerContext;

        public Repository(CustomerContext customerContext)
        {
            _customerContext = customerContext;
        }

        public IQueryable<TEntity> GetAll()
        {
            try
            {
                
                return _customerContext.Set<TEntity>().AsNoTracking();
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
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                await _customerContext.Set<TEntity>().AddAsync(entity);
                var retorno = await _customerContext.SaveChangesAsync();

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
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                _customerContext.Update(entity);
                await _customerContext.SaveChangesAsync();

                return entity;
            }
            catch (Exception)
            {
                throw new Exception($"{nameof(entity)} could not be updated");
            }
        }
    }
}
