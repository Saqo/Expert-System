using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ES_Problems_With_Car.DAL.Repositories
{
    public class CorrectionMethodsRepository : IRepository<Reasons_Correction_Methods>
    {
        Exper_System_dbEntities db = new Exper_System_dbEntities();

        public IEnumerable<Reasons_Correction_Methods> GetAll(int? id)
        {
            return db.Reasons_Correction_Methods.ToList();
        }

        public void Create(Reasons_Correction_Methods entity)
        {
            db.Reasons_Correction_Methods.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            ((Reasons_Correction_Methods)db.Reasons_Correction_Methods.Where(x => x.ID == id).First()).IsDeleted = true;
            db.SaveChanges();
        }


        public Reasons_Correction_Methods GetById(int? id)
        {
            return (Reasons_Correction_Methods)db.Reasons_Correction_Methods.Where(x => x.ID == id).First();
        }

        public void Update(int id)
        {
            db.Entry((db.Reasons_Correction_Methods.Where(x => x.ID == id).First())).Reload();
            db.SaveChanges();
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}