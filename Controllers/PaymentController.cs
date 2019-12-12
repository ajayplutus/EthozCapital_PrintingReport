using EthozCapital.CustomLibraries;
using EthozCapital.CustomLibraries.ControllerClass;
using EthozCapital.Models.ViewModels;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace EthozCapital.Controllers
{
	public class PaymentController : Controller
	{

		private static ILog glog = log4net.LogManager.GetLogger(typeof(PaymentController));
		private clsGlobal _clsGlobal;
		private clsPayment _clsPayment;

		public PaymentController()
		{
			glog.Debug("PaymentController: Entry");
			_clsGlobal = new clsGlobal();
			_clsPayment = new clsPayment();
			glog.Debug("PaymentController: Exit");
		}

		// GET: Payment
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult SpotterPaymentNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL
			var vm = new SpotterFeeViewModel();
			var outstandingSpotterFee = new List<OutstandingSpotterFeeViewModel>();
			outstandingSpotterFee = _clsPayment.FnRetrieveOutstandingSpotterFee(DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
			System.Web.HttpContext.Current.Session.Add("OutstandingFee", outstandingSpotterFee);
			return View(vm);
		}

		public ActionResult SpotterPaymentEdit(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL
			var vm = new SpotterFeeViewModel();
			SelectList lstType = new SelectList(_clsPayment.GetSpotterRefNumber());
			ViewBag.ListOfRefNumber = lstType;
			return View(vm);
		}

		public ActionResult GetSpotterDetails(string spotterRefNum)
		{
			var vm = _clsPayment.FnPopulateSpotterFee(spotterRefNum);
			var outstandingSpotterFee = (List<OutstandingSpotterFeeViewModel>)HttpContext.Session["OutstandingFee"];
			List<SpotterDetailsViewModel> SpotterDetails = new List<SpotterDetailsViewModel>();

			SpotterDetails = outstandingSpotterFee.GroupBy(i => i.ReferralID)
						.Select(g => new SpotterDetailsViewModel()
						{
							NoOfContract = g.Count(),
							SumOfAmount = g.Sum(i => i.SpotterAmt),
							PostInd = g.Where(w=>w.PostInd=="Y").Select(i => i.PostInd).FirstOrDefault(),
							Referral = g.Select(i => i.ReferralID).FirstOrDefault(),
							ReferralName = g.Select(i => i.ReferralName).FirstOrDefault(),
						}).ToList();

			return PartialView("_SpotterDetails", SpotterDetails);
		}

		public ActionResult GetSpotterMaster(string spotterRefNum)
		{
			var spotterSummary = _clsPayment.GetSpotterMaster(spotterRefNum);
			var res = new JsonResult() { Data = spotterSummary, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public PartialViewResult PopulateContractDetails(string refferId)
		{
			var vm = _clsPayment.FnPopulateContractDetails(refferId);
			var outstandingSpotterFee = new List<OutstandingSpotterFeeViewModel>();

			if (HttpContext.Session["OutstandingFee"] != null)
			{
				outstandingSpotterFee = (List<OutstandingSpotterFeeViewModel>)HttpContext.Session["OutstandingFee"];
				outstandingSpotterFee = outstandingSpotterFee.Where(w => w.ReferralID == refferId).ToList();
			}
			return PartialView("_ContractDetails", outstandingSpotterFee);
		}

		public ActionResult RetrieveOutstandingSpotterFee(string dtPreparationDate)
		{
			var outstandingSpotterFee = new List<OutstandingSpotterFeeViewModel>();
			outstandingSpotterFee = _clsPayment.FnRetrieveOutstandingSpotterFee(dtPreparationDate);
			System.Web.HttpContext.Current.Session.Add("OutstandingFee", outstandingSpotterFee);
			var res = new JsonResult() { Data = outstandingSpotterFee, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		[HttpPost]
		public ActionResult InsertSpotterData(string json)
		{
			var spotterContract = new List<OutstandingSpotterFeeViewModel>();
			spotterContract = (List<OutstandingSpotterFeeViewModel>)HttpContext.Session["OutstandingFee"];

			var model = JsonConvert.DeserializeObject<SpotterViewModel>(json);
			model.SpotterContractDetails = PrepareSpotterData(spotterContract, model.SpotterId);
			var result = _clsPayment.InsertSpotterData(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		private List<SpotterContractDetailsViewModel> PrepareSpotterData(List<OutstandingSpotterFeeViewModel> SpoterContract, int SpotterId)
		{
			List<SpotterContractDetailsViewModel> SpotterContractDetails = new List<SpotterContractDetailsViewModel>();
			if (SpotterId == 0)
			{
				SpotterContractDetails = SpoterContract.Where(w => w.PostInd == "Y").Select(x => new SpotterContractDetailsViewModel()
				{
					SpotterDetailId = x.SpotterDetailId,
					ContractNumber = x.ContractNumber,
					RolloverNumber = x.RolloverNumber,
					ItemNumber = x.ItemNumber,
					Status = x.Status,
					Valid = x.PostInd,
					ReferralID = x.ReferralID,
					ReferralName = x.ReferralName,
					ApprovedInd = "S",
					SpotterAmt = x.SpotterAmt,
					CreatedBy = x.CreatedBy,
					CreatedDate = x.CreatedDate,
				}).ToList();
			}
			else
			{
				SpotterContractDetails = SpoterContract.Select(x => new SpotterContractDetailsViewModel()
				{
					SpotterDetailId = x.SpotterDetailId,
					ContractNumber = x.ContractNumber,
					RolloverNumber = x.RolloverNumber,
					ItemNumber = x.ItemNumber,
					Status = x.Status,
					Valid = x.PostInd,
					ReferralID = x.ReferralID,
					ReferralName = x.ReferralName,
					ApprovedInd = x.PostInd == "N" ? "P" : "S",
					SpotterAmt = x.SpotterAmt,
					CreatedBy = x.CreatedBy,
					CreatedDate = x.CreatedDate,
				}).ToList();
			}
			return SpotterContractDetails;
		}

		public ActionResult IncludeTransaction(string PostInd, string Refereal, string CotractNum)
		{
			var outstandingSpotterFee = new List<OutstandingSpotterFeeViewModel>();
			outstandingSpotterFee = (List<OutstandingSpotterFeeViewModel>)HttpContext.Session["OutstandingFee"];
			if (string.IsNullOrEmpty(CotractNum))
			{
				outstandingSpotterFee.Where(w => w.ReferralID == Refereal).Select(c => { c.PostInd = PostInd; return c; }).ToList();
			}
			else
			{
				outstandingSpotterFee.Where(w => w.ReferralID == Refereal && w.ContractNumber == CotractNum).Select(c => { c.PostInd = PostInd; return c; }).ToList();
			}
			HttpContext.Session["OutstandingFee"] = outstandingSpotterFee;
			var res = new JsonResult() { Data = outstandingSpotterFee, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult RetrieveSpotterFee(string refNumber, string strStatus)
		{
			var outstandingSpotterFee = new List<OutstandingSpotterFeeViewModel>();
			outstandingSpotterFee = _clsPayment.RetrieveSpotterFeeByRefNumber(refNumber,strStatus);
			System.Web.HttpContext.Current.Session.Add("OutstandingFee", outstandingSpotterFee);
			var res = new JsonResult() { Data = outstandingSpotterFee, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult FnCheckRecordLockStatus(string refNumber)
		{
			var result = _clsGlobal.CheckRecordForEditing("Spotter", refNumber, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult LockRecord(string refNumber)
		{
			var result = _clsGlobal.LockRecord("Spotter", refNumber, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult RemoveLockRecord(string refNumber)
		{
			var result = _clsGlobal.RemoveLockRecord("Spotter", refNumber, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}
	}
}