using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using ES_Problems_With_Car.Filters;
using ES_Problems_With_Car.Models;
using ES_Problems_With_Car.DAL.Repositories;
using System.Data.Objects.DataClasses;

namespace ES_Problems_With_Car.Controllers
{
    public class EditorController : Controller
    {

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        //public List<EntityObject> getEntitiesList(IRepository<EntityObject> repo, EntityObject entity)
        //{
        //    List<EntityObject> list = repo.GetAll().ToList();
        //    if (list.Count != 0)
        //        ViewData["" + entity + ""] = list;
        //    else
        //        ViewData["" + entity + ""] = null;
        //    return list;
        //}

        [HttpGet]
        public ActionResult CreateSystem()
        {
            CarSystemsRepository repo = new CarSystemsRepository();
            List<Car_Systems> systems = repo.GetAll(null).ToList();
            ViewData["h1"] = "Редактирование систем";
            ViewData["currentStepURL"] = "System";
            ViewData["nextStepURL"] = "Node";
            if (systems.Count != 0)
                ViewData["systems"] = systems;
            else
                ViewData["systems"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult CreateSystem(Car_Systems system)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CarSystemsRepository repo = new CarSystemsRepository();
                    repo.Create(system);
                    return RedirectToAction("CreateSystem", "Editor");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            return View(system);
        }

        [HttpGet]
        public ActionResult DeleteSystem(int id)
        {
            CarSystemsRepository sr = new CarSystemsRepository();
            sr.Delete(id);
            return RedirectToAction("CreateSystem", "Editor");
        }

        [HttpGet]
        public ActionResult UpdateSystem(int id)
        {
            CarSystemsRepository repo = new CarSystemsRepository();
            Car_Systems system = repo.GetById(id);
            return View(system);
        }

        [HttpPost]
        public ActionResult UpdateSystem(Car_Systems system)
        {
            CarSystemsRepository repo = new CarSystemsRepository();
            repo.GetById(system.ID).Name = system.Name;
            repo.Save();
            return RedirectToAction("CreateSystem");
        }

        public ActionResult CancelUpdateSystem(int id)
        {
            CarSystemsRepository repo = new CarSystemsRepository();
            repo.Update(id);
            return RedirectToAction("CreateSystem");
        }

        [HttpGet]
        public ActionResult CreateNode(int systemID)
        {
            CarSystemsRepository systemrepo = new CarSystemsRepository();
            CarNodesRepository repo = new CarNodesRepository();
            List<Car_System_Nodes> nodes = repo.GetAll(systemID).ToList();
            ViewData["h1"] = "Редактирование узлов системы";
            ViewData["currentStepURL"] = "Node";
            ViewData["nextStepURL"] = "Reasons";
            ViewData["system"] = systemrepo.GetById(systemID);
            if (nodes.Count != 0)
                ViewData["nodes"] = nodes;
            else
                ViewData["nodes"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult CreateNode(Car_System_Nodes node)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CarNodesRepository repo = new CarNodesRepository();
                    repo.Create(node);
                    return RedirectToAction("CreateNode", "Editor", new { systemID = node.SystemID });
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            return View(node);
        }

        [HttpGet]
        public ActionResult DeleteNode(int id)
        {
            CarNodesRepository repo = new CarNodesRepository();
            repo.Delete(id);
            return RedirectToAction("CreateNode", "Editor", new { systemID = repo.GetById(id).SystemID });
        }

        [HttpGet]
        public ActionResult UpdateNode(int id)
        {
            CarNodesRepository repo = new CarNodesRepository();
            Car_System_Nodes node = repo.GetById(id);
            return View(node);
        }

        public ActionResult CancelUpdateNode(int id)
        {
            CarNodesRepository repo = new CarNodesRepository();
            repo.Update(id);
            return RedirectToAction("CreateNode", new { systemID = (repo.GetById(id)).SystemID });
        }

        [HttpPost]
        public ActionResult UpdateNode(Car_System_Nodes node)
        {
            CarNodesRepository repo = new CarNodesRepository();
            repo.GetById(node.ID).Name = node.Name;
            repo.Save();

            return RedirectToAction("CreateNode", new { systemID = node.SystemID });
        }

        [HttpGet]
        public ActionResult CreateReasons(int nodeID, int? PreviousReasonID)
        {
            CarNodesRepository noderepo = new CarNodesRepository();
            ReasonsRepository repo = new ReasonsRepository();
            ViewData["PreviousReasonsID"] = PreviousReasonID == null ? null : PreviousReasonID;
            List<Defect_Reasons> reasons = PreviousReasonID == null ? repo.GetAll(nodeID).ToList() : repo.GetClarifyingQuestion(PreviousReasonID).ToList();
            ViewData["PreviousReasonLabel"] = PreviousReasonID == null ? "" : repo.GetById(PreviousReasonID).Label;
            ViewData["h1"] = "Редактирование признаков неисправности";
            ViewData["nextStepURL"] = "Defect";
            ViewData["currentStepURL"] = "Reason";
            ViewData["node"] = noderepo.GetById(nodeID);
            if (reasons.Count != 0)
                ViewData["reasons"] = reasons;
            else
                ViewData["reasons"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult CreateReasons(Defect_Reasons reason)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ReasonsRepository repo = new ReasonsRepository();
                    reason.ID = repo.SetID();
                    repo.Create(reason);
                    repo.Save();
                    return RedirectToAction("CreateReasons", "Editor", new { nodeID = reason.NodeID, PreviousReasonID = reason.PreviousReasonsID });
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            return View(reason);
        }

        [HttpGet]
        public ActionResult DeleteReason(int id)
        {
            ReasonsRepository repo = new ReasonsRepository();
            repo.Delete(id);
            Defect_Reasons reason = repo.GetById(id);
            return RedirectToAction("CreateReasons", "Editor", new { nodeID = reason.NodeID, PreviousReasonID = reason.PreviousReasonsID });
        }

        [HttpGet]
        public ActionResult UpdateReason(int id)
        {
            ReasonsRepository repo = new ReasonsRepository();
            Defect_Reasons reason = repo.GetById(id);
            return View(reason);
        }

        [HttpPost]
        public ActionResult UpdateReason(Defect_Reasons reason)
        {
            ReasonsRepository repo = new ReasonsRepository();
            repo.GetById(reason.ID).Label = reason.Label;
            repo.Save();
            return RedirectToAction("CreateReasons", new { nodeID = reason.NodeID, PreviousReasonID = reason.PreviousReasonsID });
        }

        public ActionResult CancelUpdateReason(int id)
        {
            ReasonsRepository repo = new ReasonsRepository();
            repo.Update(id);
            Defect_Reasons reason = repo.GetById(id);
            return RedirectToAction("CreateReasons", new { nodeID = reason.NodeID, PreviousReasonID = reason.PreviousReasonsID });
        }

        [HttpGet]
        public ActionResult CreateCorrection_Method(int id)
        {
            CorrectionMethodsRepository repo = new CorrectionMethodsRepository();
            List<Reasons_Correction_Methods> correctionMethods = repo.GetAll(id).ToList();
            ViewData["currentStepURL"] = "Correction_Method";
            if (correctionMethods.Count != 0)
                ViewData["correctionMethods"] = correctionMethods;
            else
                ViewData["correctionMethods"] = correctionMethods;
            return View();
        }

        [HttpPost]
        public ActionResult CreateCorrection_Method(Reasons_Correction_Methods correctionMethod)
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteCorrection_Method(int id)
        {
            CorrectionMethodsRepository repo = new CorrectionMethodsRepository();
            repo.Delete(id);
            return RedirectToAction("CreateCorrection_Method", "Editor");
        }

        [HttpGet]
        public ActionResult UpdateCorrection_Method(int id)
        {
            CorrectionMethodsRepository repo = new CorrectionMethodsRepository();
            Reasons_Correction_Methods method = repo.GetById(id);
            return View(method);
        }

        [HttpPost]
        public ActionResult UpdateCorrection_Method(Reasons_Correction_Methods correctionMethod)
        {
            if (ModelState.IsValid)
            {
                CorrectionMethodsRepository repo = new CorrectionMethodsRepository();
                repo.GetById(correctionMethod.ID).Answer = correctionMethod.Answer;
                repo.Save();
                return RedirectToAction("CreateCorrection_Method");
            }
            return View(correctionMethod);
        }

        public ActionResult CancelUpdateCorrection_Method(int id)
        {
            CorrectionMethodsRepository repo = new CorrectionMethodsRepository();
            repo.Update(id);
            return RedirectToAction("CreateCorrection_Method", new { id = (repo.GetById(id)).DefectID });
        }

        [HttpGet]
        public ActionResult CreateDiagnosis(int id)
        {
            DiagnosisRepository repo = new DiagnosisRepository();
            List<Reasons_Diagnosis> diagnosisMethods = repo.GetAll(id).ToList();
            ViewData["currentStepURL"] = "Diagnosis";
            if (diagnosisMethods.Count != 0)
                ViewData["diagnosisMethods"] = diagnosisMethods;
            else
                ViewData["diagnosisMethods"] = diagnosisMethods;
            return View();
        }

        [HttpPost]
        public ActionResult CreateDiagnosis(Reasons_Diagnosis diagnosisMethod)
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteDiagnosis(int id)
        {
            return RedirectToAction("CreateDiagnosis", "Editor");
        }

        [HttpGet]
        public ActionResult UpdateDiagnosis(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateDiagnosis(Reasons_Diagnosis diagnosis)
        {
            return View();
        }

        public ActionResult CancelUpdateDiagnosis(int id)
        {
            DiagnosisRepository repo = new DiagnosisRepository();
            repo.Update(id);
            return RedirectToAction("CreateDiagnosis", new { id = (repo.GetById(id)).ID });
        }

        [HttpGet]
        public ActionResult CreateDefect(int reasonID)
        {
            ReasonsRepository r = new ReasonsRepository();
            ViewData["Reason"] = r.GetById(reasonID);
            ViewData["h1"] = "Редактирование признаков неисправности";
            ViewData["currentStepURL"] = "Defect";
            CarDefectsRepository repo = new CarDefectsRepository();
            List<Car_Defects> defects = repo.GetAll(reasonID).ToList();
            if (defects.Count != 0)
                ViewData["defects"] = defects;
            else
                ViewData["defects"] = defects;
            return View();
        }

        [HttpPost]
        public ActionResult CreateDefect(Car_Defects defect)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ReasonsRepository r = new ReasonsRepository();
                    CarDefectsRepository repo = new CarDefectsRepository();
                    repo.Create(defect);
                    return RedirectToAction("CreateDefect", "Editor", new { reasonID = (r.GetById(defect.ReasonID)).ID });
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            return View(defect);
        }

        [HttpPost]
        public ActionResult DeleteDefect(int id)
        {
            CarDefectsRepository repo = new CarDefectsRepository();
            repo.Delete(id);
            return RedirectToAction("CreateDefect", "Editor", new { reasonID = repo.GetById(id).ReasonID });
        }

        [HttpGet]
        public ActionResult UpdateDefect(int id)
        {
            CarDefectsRepository repo = new CarDefectsRepository();
            Car_Defects defect = repo.GetById(id);
            return View(defect);
        }

        [HttpPost]
        public ActionResult UpdateDefect(Car_Defects defect)
        {
            CarDefectsRepository repo = new CarDefectsRepository();
            repo.GetById(defect.ID).Answer = defect.Answer;
            repo.Save();
            return RedirectToAction("CreateDefect", "Editor", new { reasonID = defect.ReasonID });
        }

        public ActionResult CancelUpdateDefect(int id)
        {
            CarDefectsRepository repo = new CarDefectsRepository();
            repo.Update(id);
            return RedirectToAction("CreateDefect", new { reasonID = (repo.GetById(id)).ReasonID });
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            ModelState.AddModelError("", "Имя пользователя или пароль указаны неверно.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "User");
        }

        //
        // GET: /Account/Register

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(Car_Systems obj)
        {
            if (ModelState.IsValid)
            {
                // Попытка зарегистрировать пользователя
                try
                {
                    CarSystemsRepository repo = new CarSystemsRepository();

                    repo.Create(obj);
                    //WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    //WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "User");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(obj);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Удалять связь учетной записи, только если текущий пользователь — ее владелец
            if (ownerAccount == User.Identity.Name)
            {
                // Транзакция используется, чтобы помешать пользователю удалить учетные данные последнего входа
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Пароль изменен."
                : message == ManageMessageId.SetPasswordSuccess ? "Пароль задан."
                : message == ManageMessageId.RemoveLoginSuccess ? "Внешняя учетная запись удалена."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // В ряде случаев при сбое ChangePassword породит исключение, а не вернет false.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильный текущий пароль или недопустимый новый пароль.");
                    }
                }
            }
            else
            {
                // У пользователя нет локального пароля, уберите все ошибки проверки, вызванные отсутствующим
                // полем OldPassword
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Не удалось создать локальную учетную запись. Возможно, учетная запись \"{0}\" уже существует.", User.Identity.Name));
                    }
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // Если текущий пользователь вошел в систему, добавляется новая учетная запись
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // Новый пользователь, запрашиваем желаемое имя участника
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Добавление нового пользователя в базу данных
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Проверка наличия пользователя в базе данных
                    if (user == null)
                    {
                        // Добавление имени в таблицу профиля
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "Имя пользователя уже существует. Введите другое имя пользователя.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Вспомогательные методы
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Полный список кодов состояния см. по адресу http://go.microsoft.com/fwlink/?LinkID=177550
            //.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Имя пользователя уже существует. Введите другое имя пользователя.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Имя пользователя для данного адреса электронной почты уже существует. Введите другой адрес электронной почты.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Указан недопустимый пароль. Введите допустимое значение пароля.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Указан недопустимый адрес электронной почты. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Указан недопустимый ответ на вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Указан недопустимый вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Указано недопустимое имя пользователя. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.ProviderError:
                    return "Поставщик проверки подлинности вернул ошибку. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                case MembershipCreateStatus.UserRejected:
                    return "Запрос создания пользователя был отменен. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                default:
                    return "Произошла неизвестная ошибка. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";
            }
        }
        #endregion
    }
}
