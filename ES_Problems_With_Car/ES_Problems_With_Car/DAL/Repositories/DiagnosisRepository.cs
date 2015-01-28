using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ES_Problems_With_Car.DAL.Repositories
{
    public class DiagnosisRepository : IRepository<Reasons_Diagnosis>
    {
        Exper_System_dbEntities db = new Exper_System_dbEntities();

        public IEnumerable<Reasons_Diagnosis> GetAll(int? id)
        {
            return db.Reasons_Diagnosis.ToList();
        }

        public void Create(Reasons_Diagnosis entity)
        {
            db.Reasons_Diagnosis.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            ((Reasons_Diagnosis)db.Reasons_Diagnosis.Where(x => x.ID == id).First()).IsDeleted = true;
            db.SaveChanges();
        }


        public Reasons_Diagnosis GetById(int? id)
        {
            return (Reasons_Diagnosis)db.Reasons_Diagnosis.Where(x => x.ID == id).First();
        }

        public void Update(int id)
        {
            db.Entry((db.Reasons_Diagnosis.Where(x => x.ID == id).First())).Reload();
            db.SaveChanges();
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}