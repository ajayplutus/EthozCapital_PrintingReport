using EthozCapital.CustomLibraries;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

namespace EthozCapital.Controllers
{
    public class LEFSInterestCodeController : Controller
    {
        private clsLEFSInterestCode _clsLEFSInterestCode;
        private clsGlobal _clsGlobal;

        public LEFSInterestCodeController()
        {
            _clsLEFSInterestCode = new clsLEFSInterestCode();
            _clsGlobal = new clsGlobal();
        }

        // GET: LEFSInterestCode
        public ActionResult LEFSInterestCodeNew(string SubMenuId, int? InterestCodeId, bool IsEnable = true)
        {
            Session.Clear();
            var db = new MainDbContext();
            ViewBag.IsPrePostContrat = 0;
            LEFSInterestCodeViewModel obj = new LEFSInterestCodeViewModel();

            #region Check Whether LEFS Interest Code in active Pre/Posted contract. If yes, not allowed to modify.
            if (InterestCodeId > 0)
            {
                obj = _clsLEFSInterestCode.GetLEFSInterestCodeById(InterestCodeId);
                if (obj != null)
                {
                    var contractQuery =
                        (from pre in db.PreContract_Master
                         select new { pre.LEFSInterestCode, pre.Status }
                         ).Where(x => x.LEFSInterestCode != null && x.Status != null && x.Status.ToLower() != "x")
                        .Union
                            (from post in db.Contract_Master
                             select new { post.LEFSInterestCode, post.Status }).Where(x => x.LEFSInterestCode != null && x.Status != null && x.Status.ToLower() != "x");

                    int count = contractQuery.Where(x => x.LEFSInterestCode.ToLower().Equals(obj.InterestCode.ToLower())).Count();
                    ViewBag.IsPrePostContrat = count;
					TempData.Keep("SubMenuId");
				}
            }
            else
            {
                #region Check UserGroup when user direct key in URL
                if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
                {                    
                    ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
                }
                #endregion  Check UserGroup when user direct key in URL
            }
            #endregion

            List<SelectListItem> lstInterestType = _clsGlobal.GetListOfValue("LEFS_INTEREST_TYPE", "", "O", "", "");
            ViewData["getType"] = lstInterestType;

            var lstContractType = _clsGlobal.GetListOfValue("SUB_CONTRACT_TYPE", "CT-1004", "O", "", "");
            ViewData["getContract"] = lstContractType;

            List<SelectListItem> lstStatus = _clsGlobal.GetListOfValue("GENERAL_STATUS", "", "", "", "");
            ViewData["status"] = lstStatus;

            List<SelectListItem> lstRepaymentPeriod = new List<SelectListItem>();

            lstRepaymentPeriod.Add(new SelectListItem { Text = "--Select--", Value = "" });
            for (int i = 1; i <= 20; i++)
            {
                lstRepaymentPeriod.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.RepaymentPeriodFrom = new SelectList(lstRepaymentPeriod, "Value", "Text", obj.RepaymentPeriodFrom);
            ViewBag.RepaymentPeriodTo = new SelectList(lstRepaymentPeriod, "Value", "Text", obj.RepaymentPeriodTo);

            ViewBag.IsEnable = IsEnable;

            return View(obj);
        }

        public ActionResult LEFSInterestCodeView(string SubMenuId)
        {
            #region Check UserGroup when user direct key in URL
            if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
            {
                ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
            }
			#endregion  Check UserGroup when user direct key in URL
			TempData["SubMenuId"] = SubMenuId;
			return View();
        }

        public PartialViewResult _LEFSInterestCodeSearchView(string ViewType)
        {
            Session.Clear();

            var db = new MainDbContext();

            List<SelectListItem> lstInterestType = _clsGlobal.GetListOfValue("LEFS_INTEREST_TYPE", "", "O", "", "");
            ViewData["getType"] = lstInterestType;

            var lstContractType = _clsGlobal.GetListOfValue("SUB_CONTRACT_TYPE", "CT-1004", "O", "", "");
            ViewData["getContract"] = lstContractType;

            List<SelectListItem> lstStatus = _clsGlobal.GetListOfValue("GENERAL_STATUS", "", "", "", "");
            if (lstStatus.Where(x => x.Value.Equals("O")).Count() > 0)
            {
                lstStatus.Where(x => x.Value.Equals("O")).FirstOrDefault().Selected = true;
            }
            ViewData["status"] = lstStatus;

            ViewBag.ViewType = ViewType;
            return PartialView("_LEFSInterestCodeSearchView");
        }

        public PartialViewResult LEFSInterestCodeListView(LEFSInterestCodeParam Param, string ViewType)
        {
            var db = new MainDbContext();

            var interestCodeList = _clsLEFSInterestCode.GetLEFSInterestCodeList(Param);

            LEFSInterestCodeListViewModel vm = new LEFSInterestCodeListViewModel();
            vm.Action = ViewType;

            vm.LEFSInterestCodeList.AddRange(interestCodeList);

            return PartialView("_LEFSInterestCodeListView", vm);
        }

        public ActionResult LEFSInterestCodeEdit(string SubMenuId)
        {
            #region Check UserGroup when user direct key in URL
            if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
            {
                ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
            }
			#endregion  Check UserGroup when user direct key in URL
			TempData["SubMenuId"] = SubMenuId;
			return View();
        }

        [HttpPost]
        public ActionResult InsertLEFSInterestCode(string Json)
        {
            if (ModelState.IsValid)
            {
                var model = JsonConvert.DeserializeObject<LEFSInterestCodeViewModel>(Json);
                var result = _clsLEFSInterestCode.InsertLEFSInterestCode(model, User.Identity.Name);
                var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return res;
            }
            else
            {
                return base.Json(new
                {
                    NotificationTitle = clsGlobal.SwalTitle_Fail,
                    NotificationContent = "Invalid Input",
                    NotificationType = clsGlobal.SwalTitle_Error
                });
            }
        }

        [HttpPost]
        public ActionResult DeactivateInterestCode(int Id, string Reason, bool Confirm)
        {
            if (ModelState.IsValid)
            {
                var result = _clsLEFSInterestCode.DeactivateInterestCode(Id, Reason, User.Identity.Name, Confirm);
                var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return res;
            }
            else
            {
                return Json(new
                {
                    NotificationTitle = clsGlobal.SwalTitle_Fail,
                    NotificationContent = "Invalid Input",
                    NotificationType = clsGlobal.SwalTitle_Error
                });
            }
        }

        [HttpPost]
        public ActionResult ActivateInterestCode(int Id, string Reason, bool Confirm)
        {
            if (ModelState.IsValid)
            {
                var result = _clsLEFSInterestCode.ActivateInterestCode(Id, Reason, User.Identity.Name, Confirm);
                var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return res;
            }
            else
            {
                return Json(new
                {
                    NotificationTitle = clsGlobal.SwalTitle_Fail,
                    NotificationContent = "Invalid Input",
                    NotificationType = clsGlobal.SwalTitle_Error
                });
            }
        }
		
	}
}