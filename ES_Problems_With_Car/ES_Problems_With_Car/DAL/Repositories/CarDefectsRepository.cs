using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ES_Problems_With_Car.DAL.Repositories
{
    public class CarDefectsRepository : IRepository<Car_Defects>
    {
        Exper_System_dbEntities db = new Exper_System_dbEntities();
        public IEnumerable<Car_Defects> GetAll(int? id)
        {
            return db.Car_Defects.Where(x => x.ReasonID == id && x.IsDeleted != true).ToList();
        }

        public void Create(Car_Defects entity)
        {
            db.Car_Defects.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            ((Car_Defects)db.Car_Defects.Where(x => x.ID == id).First()).IsDeleted = true;
            db.SaveChanges();
        }


        public Car_Defects GetById(int? id)
        {
            return (Car_Defects)db.Car_Defects.Where(x => x.ID == id).First();
        }

        public void Update(int id)
        {
            db.Entry((db.Car_Defects.Where(x => x.ID == id).First())).Reload();
            db.SaveChanges();
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}