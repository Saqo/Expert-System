using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ES_Problems_With_Car.DAL.Repositories
{
    public interface IRepository<TEntity>
    {
        IEnumerable<TEntity> GetAll(int? id);
        TEntity GetById(int? id);
        //void Remove(int id);
        void Create(TEntity entity);

        void Delete(int id);

        void Update(int id);

        void Save();
    }
}