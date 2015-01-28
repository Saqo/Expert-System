using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ES_Problems_With_Car.DAL.Repositories
{
    public class CarNodesRepository : IRepository<Car_System_Nodes>
    {
        Exper_System_dbEntities db = new Exper_System_dbEntities();



        public IEnumerable<Car_System_Nodes> GetAll(int? id)
        {
            return db.Car_System_Nodes.Where(x => x.SystemID == id && x.IsDeleted != true).ToList();
        }


        public void Create(Car_System_Nodes node)
        {
            db.Car_System_Nodes.Add(node);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            ((Car_System_Nodes)db.Car_System_Nodes.Where(x => x.ID == id).First()).IsDeleted = true;
            db.SaveChanges();
        }


        public Car_System_Nodes GetById(int? id)
        {
            List<Car_System_Nodes> Nodes = (from c in db.Car_System_Nodes where c.ID == id select c).ToList();
            return (Nodes.Count != 0) ? Nodes[0] : null;
        }

        public void Update(int id)
        {
            db.Entry((db.Car_System_Nodes.Where(x => x.ID == id).First())).Reload();
            db.SaveChanges();
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}