using BlazorInventory.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace BlazorInventory.Data
{
    internal class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly ClientSideDbContext _dbContext;

        public BaseRepository(ClientSideDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T GetById(int id)
        {
            return _dbContext!.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().AsNoTracking().ToList();
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            SaveChanges();
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            SaveChanges();
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            SaveChanges();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
            SaveChanges();
        }

        private void SaveChanges()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
