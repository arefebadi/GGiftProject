using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GGiftProject.Services
{
   public interface IDataService<T>
    {
        IEnumerable<T> GetAll();
        T GetSingle(Expression<Func<T, bool>> predicate);
        void Update(T entity);
        void Create(T entity);
        IEnumerable<T> Query(Expression<Func<T, bool>> predicate);
        void Remove(T entity);
    }
}
