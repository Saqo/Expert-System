using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ES_Problems_With_Car.DAL.Repositories
{
    public class CarSystemsRepository : IRepository<Car_Systems>
    {
        Exper_System_dbEntities db = new Exper_System_dbEntities();
        public IEnumerable<Car_Systems> GetAll(int? id)
        {
            return db.Car_Systems.Where(x => x.IsDeleted == false).ToList();
        }

        public IEnumerable<Car_System_Nodes> GetNodesBySystemId(int? id)
        {
            return db.Car_System_Nodes.Where(x => x.SystemID == id).ToList();
        }

        public void Create(Car_Systems system)
        {
            db.Car_Systems.Add(system);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            ((Car_Systems)db.Car_Systems.Where(x => x.ID == id).First()).IsDeleted = true;
            db.SaveChanges();
        }


        public Car_Systems GetById(int? id)
        {
            List<Car_Systems> System = (from c in db.Car_Systems where c.ID == id select c).ToList();
            return (System.Count != 0) ? System[0] : null;
        }

        public void Update(int id)
        {
            db.Entry((db.Car_Systems.Where(x => x.ID == id).First())).Reload();
            db.SaveChanges();
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}