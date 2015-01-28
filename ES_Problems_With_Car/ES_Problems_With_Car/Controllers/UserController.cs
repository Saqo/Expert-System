using ES_Problems_With_Car.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;



/*Доделать:
 1. При выборе конкретного узла проходить только признакам этого узла. Если всегда отвечать не знаю, то в конце вывести сообщение, что не хватает данных.
 2. Заблокировать кнопки до того, пока radiobutton не выбран.
 3. Переименовать все надписи и подписи в соответствии с логическим предназначением.
 Справка:
 SelectedNodInPreviousStep   и  NodeWasSelected - true, если узел был выбран ( НЕ ответ "не знаю")*/
namespace ES_Problems_With_Car.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Init();
            return View();
        }

        public void Init()
        {
            CarSystemsRepository srepo = new CarSystemsRepository();
            CarDefectsRepository drepo = new CarDefectsRepository();
            CarNodesRepository nrepo = new CarNodesRepository();
            ReasonsRepository repo = new ReasonsRepository();
            List<Car_Systems> systems = srepo.GetAll(null).ToList();
            foreach (Car_Systems s in systems)
            {
                s.WasViewed = false;
                s.IsSelected = false;
                srepo.Save();

                List<Car_System_Nodes> nodes = nrepo.GetAll(s.ID).ToList();
                foreach (Car_System_Nodes n in nodes)
                {
                    n.WasViewed = false;
                    n.IsSelected = false;
                    nrepo.Save();

                    List<Defect_Reasons> reasons = repo.GetAll(n.ID).ToList();
                    foreach (Defect_Reasons r in reasons)
                    {
                        r.WasViewed = false;
                        r.IsSelected = false;
                        repo.Save();
                        InitClarifyingQuestions(r.ID);

                        //List<Car_Defects> defects = drepo.GetAll(r.ID).ToList();
                        //foreach (Car_Defects d in defects)
                        //{
                        //    d.WasViewed = false;
                        //    d.IsSelected = false;
                        //    drepo.Save();

                        //}
                    }
                }
            }
        }

        public void InitClarifyingQuestions(int previousReasonsID)
        {
            ReasonsRepository repo = new ReasonsRepository();
            List<Defect_Reasons> clarifyingQuestions = repo.GetClarifyingQuestion(previousReasonsID).ToList();
            foreach (Defect_Reasons r in clarifyingQuestions)
            {
                r.WasViewed = false;
                r.IsSelected = false;
                repo.Save();
                InitClarifyingQuestions(r.ID);
            }

        }

        [HttpGet]
        public ActionResult Systems(bool? NodeWasSelected, bool? SystemWasSelected, bool? SearchDefectInThisNode)
        {
            if (SearchDefectInThisNode.HasValue && SearchDefectInThisNode == false || !NodeWasSelected.HasValue && !SystemWasSelected.HasValue && !SearchDefectInThisNode.HasValue || SystemWasSelected == false && SearchDefectInThisNode == false)
            {
                CarSystemsRepository repo = new CarSystemsRepository();
                List<Car_Systems> systems = repo.GetAll(null).Where(x => x.WasViewed == false).ToList();
                ViewData["h1"] = "Выберите систему, в которой обнаружен признак неисправности";
                if (systems.Count != 0)
                    return View(systems);
                else
                    return RedirectToAction("DefectNotFounded");
            }
            else
                return RedirectToAction("DefectNotFounded");
        }

        [HttpPost]
        public ActionResult Systems(FormCollection form)
        {
            int id;
            string SelectedSystemID = form["Systems"];
            bool parseResult = Int32.TryParse(SelectedSystemID, out id);
            if (parseResult)
            {
                //TempData["id"] = id;
                return RedirectToAction("Nodes", "User", new { id = id, SystemWasSelected = parseResult });
            }
            else
            {
                CarSystemsRepository repo = new CarSystemsRepository();
                List<Car_Systems> systems = repo.GetAll(null).Where(x => x.WasViewed == false).ToList();

                if (systems.Count != 0)
                {
                    systems[0].WasViewed = true;
                    repo.Save();
                    return RedirectToAction("Nodes", "User", new { id = systems[0].ID, SystemWasSelected = parseResult });
                }
                else
                {
                    return RedirectToAction("Nodes", "User", new { id = id, SystemWasSelected = parseResult });
                }
            }
        }

        [HttpGet]
        public ActionResult Nodes(int id, bool? SelectedNodInPreviousStep, bool? SystemWasSelected)
        {
            CarSystemsRepository srepo = new CarSystemsRepository();
            srepo.GetById(id).IsSelected = true;
            CarNodesRepository repo = new CarNodesRepository();
            List<Car_System_Nodes> nodes = repo.GetAll(id).ToList();
            ViewData["h1"] = "Выберите узел системы, в которой обнаружен признак неисправности";
            ViewData["SystemWasSelected"] = SystemWasSelected;
            ViewData["systemID"] = id;
            if (nodes.Count != 0)
            {
                if (!SelectedNodInPreviousStep.HasValue && SystemWasSelected.HasValue || SystemWasSelected == true)
                    return View(nodes);
                else
                {
                    nodes[0].WasViewed = true;
                    repo.Save();
                    return RedirectToAction("Reasons", "User", new { id = nodes[0].ID, SelectedNodInPreviousStep = SelectedNodInPreviousStep, SystemWasSelected = SystemWasSelected });
                }
            }
            else
            {
                List<Car_Systems> NotViewedSystems = srepo.GetAll(null).Where(x => x.WasViewed == false).ToList();
                if (NotViewedSystems.Count != 0 && SystemWasSelected != true)
                {
                    NotViewedSystems[0].WasViewed = true;
                    srepo.Save();
                    return RedirectToAction("Nodes", new { id = NotViewedSystems[0].ID, SelectedNodInPreviousStep = SelectedNodInPreviousStep, SystemWasSelected = SystemWasSelected });
                }
                else
                    return RedirectToAction("DefectNotFounded");
            }
        }


        [HttpPost]
        public ActionResult Nodes(FormCollection form)
        {
            int id;
            string SelectedNodeID = form["SelectedNode"];
            bool SystemWasSelected = bool.Parse(form["ViewData[SystemWasSelected]"]);
            int SelectedSystemID = int.Parse(getSystemID(form["item.SystemID"]));

            bool parseResult = Int32.TryParse(SelectedNodeID, out id);
            if (parseResult)
            {
                return RedirectToAction("Reasons", "User", new { id = id, SelectedNodInPreviousStep = parseResult, SystemWasSelected = SystemWasSelected });
            }
            else
            {
                CarNodesRepository repo = new CarNodesRepository();
                List<Car_System_Nodes> nodes = repo.GetAll(SelectedSystemID).Where(x => x.WasViewed == false).ToList();

                if (nodes.Count != 0)
                {
                    nodes[0].WasViewed = true;
                    repo.Save();
                    return RedirectToAction("Reasons", "User", new { id = nodes[0].ID, SelectedNodInPreviousStep = parseResult, SystemWasSelected = SystemWasSelected });
                }
                else
                {
                    CarSystemsRepository srepo = new CarSystemsRepository();
                    srepo.GetById(SelectedSystemID).WasViewed = true;
                    srepo.Save();
                    return RedirectToAction("Systems", "User", new { NodeWasSelected = parseResult, SystemWasSelected = SystemWasSelected });
                }
            }
        }


        public string getSystemID(string uncorrectID)
        {
            return (uncorrectID.Split(','))[0];
        }

        [HttpGet]
        public ActionResult Reasons(int id, int? previousReasonID, bool? dontKnow, bool? SelectedNodInPreviousStep, bool? SystemWasSelected, bool? SearchDefectInThisNode)
        {
            ReasonsRepository repo = new ReasonsRepository();
            CarNodesRepository nrepo = new CarNodesRepository();
            CarSystemsRepository srepo = new CarSystemsRepository();

            Car_System_Nodes node = nrepo.GetById(id);
            ViewData["CurrentNode"] = node.Name;
            ViewData["NodeWasSelected"] = SelectedNodInPreviousStep;
            ViewData["SystemWasSelected"] = SystemWasSelected;
            ViewData["CurrentSystem"] = srepo.GetById(node.SystemID).Name;


            if (!dontKnow.HasValue)
            {

                List<Defect_Reasons> reasons = (previousReasonID == null) ? repo.GetAll(id).Where(x => x.WasViewed == false).ToList() : repo.GetClarifyingQuestion(previousReasonID).Where(x => x.WasViewed == false).ToList();

                if (reasons.Count != 0)
                {
                    foreach (Defect_Reasons reason in reasons)
                    {
                        Defect_Reasons r = repo.GetById(reason.ID);
                        r.WasViewed = true;
                        repo.Save();
                        return View(r);
                    }
                }
                else
                    return RedirectToAction("Systems", new { NodeWasSelected = SelectedNodInPreviousStep, SystemWasSelected = SystemWasSelected, SearchDefectInThisNode = SearchDefectInThisNode });
            }
            else
                if (previousReasonID == null)
                {

                    List<Defect_Reasons> NotViewedReasonsInNode = repo.GetAll(id).Where(x => x.WasViewed == false).ToList();
                    if (NotViewedReasonsInNode.Count != 0)
                    {
                        NotViewedReasonsInNode[0].WasViewed = true;
                        repo.Save();
                        return View(NotViewedReasonsInNode[0]);
                    }

                    if (SelectedNodInPreviousStep.HasValue && SelectedNodInPreviousStep == false)
                    {
                        List<Car_System_Nodes> NotViewedNodesInSystem = nrepo.GetAll(nrepo.GetById(id).SystemID).Where(x => x.WasViewed == false).ToList();
                        if (NotViewedNodesInSystem.Count != 0)
                        {
                            NotViewedNodesInSystem[0].WasViewed = true;
                            nrepo.Save();
                            return RedirectToAction("Reasons", new { id = NotViewedNodesInSystem[0].ID, SelectedNodInPreviousStep = SelectedNodInPreviousStep, SystemWasSelected = SystemWasSelected });
                        }

                        List<Car_Systems> NotViewedSystems = srepo.GetAll(null).Where(x => x.WasViewed == false).ToList();
                        if (NotViewedSystems.Count != 0)
                        {
                            NotViewedSystems[0].WasViewed = true;
                            srepo.Save();
                            return RedirectToAction("Nodes", new { id = NotViewedSystems[0].ID, SelectedNodInPreviousStep = SelectedNodInPreviousStep, SystemWasSelected = SystemWasSelected });
                        }
                    }
                    return RedirectToAction("DefectNotFounded");

                }
                else
                {
                    repo.GetById(previousReasonID).IsSelected = true;
                    List<Defect_Reasons> reasons = (repo.GetClarifyingQuestion(previousReasonID).Where(x => x.WasViewed == false)).ToList();
                    if (reasons.Count != 0)
                    {
                        reasons[0].WasViewed = true;
                        repo.Save();
                        return View(reasons[0]);
                    }
                    else
                    {
                        //if (SelectedNodInPreviousStep.HasValue && SelectedNodInPreviousStep == false)
                        return RedirectToAction("Reasons", new { id = id, previousReasonID = repo.GetById(previousReasonID).PreviousReasonsID, dontKnow = dontKnow, SelectedNodInPreviousStep = SelectedNodInPreviousStep, SystemWasSelected = SystemWasSelected });
                        //else
                        //  return RedirectToAction("Reasons", new { id = id, previousReasonID = previousReasonID, SelectedNodInPreviousStep = SelectedNodInPreviousStep });
                    }
                }
            return View();
        }


        [HttpPost]
        public ActionResult Reasons(FormCollection form)
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetAnswer(int id)
        {
            CarDefectsRepository drepo = new CarDefectsRepository();
            ReasonsRepository repo = new ReasonsRepository();
            List<Car_Defects> defects = drepo.GetAll(id).ToList();
            if (defects.Count != 0)
                return View(defects[0]);
            else
            {
                Defect_Reasons reason = repo.GetById(id);
                if (reason != null)
                {
                    if (reason.PreviousReasonsID == null)
                    {
                        return RedirectToAction("Reasons", new { id = reason.NodeID, previousReasonID = reason.ID });
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult DefectNotFounded()
        {
            return View();
        }
    }
}
