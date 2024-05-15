using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electric_Scooter.Repositories
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> GetAllData();
        T GetDataById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
