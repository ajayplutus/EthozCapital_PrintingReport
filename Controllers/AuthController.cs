using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using System.Security.Claims;
using EthozCapital.CustomLibraries;
using System.Threading.Tasks;
using log4net;
using EthozCapital.Data;

//using Microsoft.Owin.Security;
//using Owin;

namespace EthozCapital.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(AuthController));
        private static clsGlobal _clsGlobal;

        public AuthController()
        {
            _clsGlobal = new clsGlobal();
        }

        // GET: /Auth/       
        [HttpGet]
        public ActionResult Login()
        {
            glog.Debug("HttpGet Login: Entry");
            glog.Debug("HttpGet Login: Exit");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Sys_Users model)
        {
            glog.Debug("HttpPost Login: Entry");
            if (!ModelState.IsValid) //Checks if input fields have the correct format
            {
                glog.Error("Invalid UserName/Password. Please try again.");
                return Json(new
                {
                    NotificationTitle = clsGlobal.SwalTitle_Fail,
                    NotificationContent = "Invalid UserName/Password. Please try again.",
                    NotificationType = clsGlobal.SwalType_Warning
                });
            }
            try
            {
                using (var db = new MainDbContext())
                {
                    ////Store username and password
                    string getEmpCode = "";
                    clsGlobal.LoginID = model.Email;
                    clsGlobal.Password = model.Password;
                    ////Store username and password

                    //glog.Debug(db);
                    //glog.Debug("emailCheck: Entry");
                    //var emailCheck = db.Sys_Users.FirstOrDefault(u => u.Email == clsGlobal.LoginID.ToString().Trim()); // change to clscommon
                    //glog.Debug("emailCheck: Exit");

                    if (EthozCapital.CustomLibraries.clsGlobal.GetConnection() == "")
                    {
                        return Json(new
                        {
                            NotificationTitle = clsGlobal.SwalTitle_Error,
                            NotificationContent = "Please contact MIS, error: Invalid User",
                            NotificationType = clsGlobal.SwalType_Error
                        });
                    };

                    //Retrieve em_emp_cod from CRM OrixDB using em_sybase_id
                    //string getEmpCode = EthozCapital.CustomLibraries.clsUserAccessLogin.GetEmpCode(clsGlobal.LoginID.ToString().Trim());

                    using (var OrixDB = new OrixDBEntities())
                    {
                        //getEmpCode = "S0497";
                        var allowedStatus = new[] { "N", "E" };
                        var EmpCode = OrixDB.hr_emp_mas.FirstOrDefault(u => u.em_sybase_id == clsGlobal.LoginID.ToString().Trim() && allowedStatus.Contains(u.em_staff_status));
                        if (EmpCode == null)
                        {
                            return Json(new
                            {
                                NotificationTitle = clsGlobal.SwalTitle_Error,
                                NotificationContent = "Please contact MIS, error: Invalid User",
                                NotificationType = clsGlobal.SwalType_Error
                            });
                        }
                        else
                            getEmpCode = Convert.ToString(EmpCode.em_emp_cod);
                    }
                    //Pass em_emp_cod to here
                    //Check User exist in Orix DB or user group not assigned
                    //if (!EthozCapital.CustomLibraries.clsUserAccessLogin.ChkUserInfor(getEmpCode.ToString().Trim()))
                    //{
                    //    return Json(new
                    //    {
                    //        NotificationTitle = clsGlobal.SwalTitle_Error,
                    //        NotificationContent = "Please contact MIS, error: Invalid User",
                    //        NotificationType = clsGlobal.SwalType_Error
                    //    });
                    //}

                    //Get group after check user exists in Orix DB
                    var getGroupCode = db.Sys_UserGroupMembers.FirstOrDefault(u => u.EmployeeCode == getEmpCode.ToString().Trim());
                    clsGlobal.UserGroupLogin = getGroupCode.GroupCode.ToString().Trim();

                    //User Exist
                    //if (emailCheck != null)

                    //Check Password Pending
                    if (getEmpCode.ToString().Trim() != null)
                    {
                        //var getPassword = db.Sys_Users.Where(u => u.Email == model.Email).Select(u => u.Password);                        
                        var getPassword = db.Sys_Users.Where(u => u.EmployeeCode == getEmpCode.ToString().Trim()).Select(u => u.Password);
                        var materializePassword = getPassword.ToList();
                        var password = materializePassword[0];
                        var decryptedPassword = clsDecrypt.Decrypt(password);

                        if (clsGlobal.LoginID.ToString().Trim() != null)
                        {
                            var getName = db.Sys_Users.Where(u => u.Email == clsGlobal.LoginID.ToString().Trim()).Select(u => u.Name);
                            var materializeName = getName.ToList();
                            var name = materializeName[0];

                            var getEmail = db.Sys_Users.Where(u => u.EmployeeCode == getEmpCode.ToString().Trim()).Select(u => u.Email);
                            var materializeEmail = getEmail.ToList();
                            var email = materializeEmail[0];

                            var getId = db.Sys_Users.Where(u => u.EmployeeCode == getEmpCode.ToString().Trim()).Select(u => u.Id);
                            var materializeId = getId.ToList();
                            var id = materializeId[0];

                            var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, name),
                        new Claim(ClaimTypes.Email, email),                        
                        new Claim("JobTitle", "ADMIN"),                                    
                        new Claim("UserGroupCode", clsGlobal.UserGroupLogin),
                        new Claim(ClaimTypes.NameIdentifier, id.ToString())
                    }, "ApplicationCookie");

                            var ctx = Request.GetOwinContext();
                            var authManager = ctx.Authentication;
                            authManager.SignIn(identity);

                            return Json(new
                            {
                                NotificationType = clsGlobal.SwalType_Success,
                                redirectUrl = Url.Action("Default", "Home"),
                                isRedirect = true,
                                JsonRequestBehavior.AllowGet
                            });
                        }
                    }
                    //Check Password Pending
                }

                glog.Error("Invalid UserName/Password. Please try again.");
                return Json(new
                {
                    NotificationTitle = clsGlobal.SwalTitle_Fail,
                    NotificationContent = "Invalid UserName/Password. Please try again.",
                    NotificationType = clsGlobal.SwalType_Warning
                });
            }
            catch (Exception ex)
            {
                glog.Error("Please contact MIS, error:" + ex.Message);
                return Json(new
                {
                    NotificationTitle = clsGlobal.SwalTitle_Error,
                    NotificationContent = "Please contact MIS, error:" + ex.Message,
                    NotificationType = clsGlobal.SwalType_Error
                });
            }
            glog.Debug("HttpPost Login: Exit");
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }

        #region Partial Layout
        public ActionResult _MenuUserPartial()
        {
            //GetDefaultValue();
            glog.Debug("_MenuUserPartial: Entry");
            UrlHelper u = new UrlHelper(this.ControllerContext.RequestContext);
            string url = u.Action("Login", "User", null);
            ViewBag.URL = url;

            glog.Debug("url:" + url);

            glog.Debug("_MenuUserPartial: Exit");
            return PartialView();
        }

        public ActionResult _MenuNavigationPartial()
        {
            glog.Debug("_MenuNavigationPartial: Entry");
            glog.Debug("_MenuNavigationPartial: Exit");
            return PartialView(AuthViewModel.ReturnNavigationPartial(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value));
        }
        #endregion
    }
}