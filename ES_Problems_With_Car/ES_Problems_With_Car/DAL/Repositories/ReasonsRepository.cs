using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ES_Problems_With_Car.DAL.Repositories
{
    public class ReasonsRepository : IRepository<Defect_Reasons>
    {
        Exper_System_dbEntities db = new Exper_System_dbEntities();
        public IEnumerable<Defect_Reasons> GetAll(int? id)
        {
            return db.Defect_Reasons.Where(x => x.NodeID == id && x.PreviousReasonsID == null && x.IsDeleted == false).ToList();
        }

        public void Create(Defect_Reasons reason)
        {
            db.Defect_Reasons.Add(reason);
            //db.SaveChanges();
        }

        public Defect_Reasons GetById(int? id)
        {
            return db.Defect_Reasons.Where(x => x.ID == id).First();
        }

        public IEnumerable<Defect_Reasons> GetClarifyingQuestion(int? reasonID)
        {
            return db.Defect_Reasons.Where(x => x.PreviousReasonsID == reasonID && x.PreviousReasonsID != null && x.IsDeleted == false).ToList();
        }

        //public int GetReasonIdByAnswerId(int id)
        //{
        //    return (db.Defect_Reasons.Where(x => x.DefectID == id).First()).ID;
        //}

        public void Delete(int id)
        {
            ((Defect_Reasons)db.Defect_Reasons.Where(x => x.ID == id).First()).IsDeleted = true;
            db.SaveChanges();
        }

        public void Update(int id)
        {
            db.Entry((db.Defect_Reasons.Where(x => x.ID == id).First())).Reload();
            db.SaveChanges();
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public int SetID()
        {
            return db.Defect_Reasons.Count();
        }
    }
}