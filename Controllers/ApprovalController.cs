using EthozCapital.CustomLibraries;
using EthozCapital.CustomLibraries.ControllerClass;
using EthozCapital.Models;
using EthozCapital.Models.ViewModels;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace EthozCapital.Controllers
{
	public class ApprovalController : Controller
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(PaymentController));
		private clsGlobal _clsGlobal;
		private clsApproval _clsApproval;


		public ApprovalController()
		{
			glog.Debug("ApprovalController: Entry");
			_clsGlobal = new clsGlobal();
			_clsApproval = new clsApproval();
			glog.Debug("ApprovalController: Exit");
		}

		// GET: Approval
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult ApprovalView(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL

			PendingApprovalViewModel vm = new PendingApprovalViewModel();
			string AssignedTo = _clsApproval.GetCurrentAssignToCode(User.Identity.Name);
			string BatchNo = String.Empty;
			vm = _clsApproval.GetApproval(AssignedTo, BatchNo, "", "", "", "IsPending");
			vm.ApprovalPage = "IsPending";
			return View(vm);
		}

		public ActionResult ReassignView(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL
			PendingApprovalViewModel vm = new PendingApprovalViewModel();
			vm.ApprovalPage = "IsReassign";
			SelectList lstType = new SelectList(_clsApproval.FnRetriveApprovingOfficer(), "Value", "Text");
			ViewBag.ListOfReassignApprovalOfficer = lstType;
			return View(vm);
		}

		public ActionResult ApprovedView(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL
			PendingApprovalViewModel vm = new PendingApprovalViewModel();
			vm.ApprovalPage = "IsHistory";
			SelectList lstType = new SelectList(_clsApproval.GetApprovalName());
			ViewBag.ListOfApprovalName = lstType;
			return View(vm);
		}

		public JsonResult GetBatchNoAutoComplete(string sourcePage, string textFilter)
		{
			var res = _clsApproval.GetBatchNo(sourcePage, textFilter);
			var result = Json(new { data = JsonConvert.SerializeObject(res).ToString() }, JsonRequestBehavior.AllowGet);
			return result;
		}

		public ActionResult OnSearchApprovedView(string AssignTo, string BatchNo, string DateFrom, string DateTo, string Type, string ApprovalPage)
		{
			PendingApprovalViewModel vm = new PendingApprovalViewModel();
			vm = _clsApproval.GetApproval(AssignTo, BatchNo, DateFrom, DateTo, Type, ApprovalPage);
			vm.ApprovalPage = !string.IsNullOrEmpty(DateFrom) ? "IsHistory" : "IsReassign";
			ViewData["NoPendingApproval"] = "";
			if (vm.PendingApproval.Count == 0 && string.IsNullOrEmpty(AssignTo))
			{
				ViewData["NoPendingApproval"] = "No approval found on selected date range. Please try another date!";
			}
			else if (vm.PendingApproval.Count == 0 && !string.IsNullOrEmpty(AssignTo))
			{
				ViewData["NoPendingApproval"] = "No approval found on selected Batch No and Approving Officer. Please try another details!";
			}

			return PartialView("_ApprovalListPartialView", vm);
		}

		public PartialViewResult ApprovalSpotterDetails(string SpotterRefNo, int ApprovalProcessDetailID, int ApprovalHeaderID, int ApprovalProcessID, string strStatus)
		{
			ApprovalSpotterDetails vm = new ApprovalSpotterDetails();
			vm = _clsApproval.GetApprovalSpotterDetails(SpotterRefNo, strStatus);
			vm.ApprovalProcessDetailID = ApprovalProcessDetailID;
			vm.ApprovalHeaderID = ApprovalHeaderID;
			vm.ApprovalProcessID = ApprovalProcessID;
			ViewBag.ViewType = strStatus == "P" ? "IsPending" : "IsHistory";
			return PartialView("_ApprovalDetaisPartialView", vm);
		}

		public PartialViewResult ReassignApprovalDetails(string SpotterRefNo, string PreparationDate, int ApprovalProcessDetailID)
		{
			ApprovalSpotterDetails vm = new ApprovalSpotterDetails();
			vm = _clsApproval.GetApprovalSpotterDetails(SpotterRefNo, "P");
			var EmpList = _clsGlobal.GetEmployeeList();
			vm.ApprovingOfficerList = EmpList.Select(x => new SelectListItem()
			{
				Text = x.em_sht_nam,
				Value = x.em_emp_cod,
			}).ToList();
			return PartialView("_ReassignApprovalPartialView", vm);
		}

		public ActionResult ApproveRejectSpotter(string json)
		{
			var model = new ApprovalSpotterDetails();
			model = JsonConvert.DeserializeObject<ApprovalSpotterDetails>(json);

			var result = _clsApproval.FnApproveRejectEvent(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult GetApprovingOfficerAvailbility(string ReassignTo)
		{
			var result = _clsApproval.FnGetApprovingOfficerAvailbility(ReassignTo);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult ReassignApproval(string json)
		{
			var model = new ApprovalSpotterDetails();
			model = JsonConvert.DeserializeObject<ApprovalSpotterDetails>(json);

			var result = _clsApproval.FnUpdateCurrentAssignedTo(model, User.Identity.Name);
			if (result.Status == 1)
			{
				SelectList lstType = new SelectList(_clsApproval.FnRetriveApprovingOfficer(), "Value", "Text");
				ViewBag.ListOfReassignApprovalOfficer = lstType;
			}
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public JsonResult GetApprovingOfficerList()
		{
			var listOfReassignApprovalOfficer = _clsApproval.FnRetriveApprovingOfficer();
			listOfReassignApprovalOfficer.Insert(0, new SelectListItem { Text = "--Select--", Value = "" });

			return Json(listOfReassignApprovalOfficer);
		}
	}
}