using BlazorInventory.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Validation;

namespace BlazorInventory.Data.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T GetById(int id)
        {
            return _dbContext.Set<T>().Find(id);
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
                SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                ThrowEnhancedValidationException(e);
            }
        }

        protected virtual void ThrowEnhancedValidationException(DbEntityValidationException e)
        {
            var errorMessages = e.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            var fullErrorMessage = string.Join("; ", errorMessages);
            var exceptionMessage = string.Concat(e.Message, " The validation errors are: ", fullErrorMessage);

            throw new DbEntityValidationException(exceptionMessage, e.EntityValidationErrors);
        }
    }
}
