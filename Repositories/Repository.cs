using Electric_Scooter.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Electric_Scooter.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly I_ChiEntities _db;
        protected readonly DbSet<T> _dbSet;

        public Repository(I_ChiEntities db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
                _db.SaveChanges();
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public IEnumerable<T> GetAllData()
        {
            return _dbSet.ToList();
        }

        public T GetDataById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Update(T entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}