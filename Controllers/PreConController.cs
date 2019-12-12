using EthozCapital.CustomLibraries;
using EthozCapital.Data;
using EthozCapital.Models;
using EthozCapital.Models.ViewModels;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace EthozCapital.Controllers
{
	public class PreConController : Controller
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(PreConController));
		private clsSecurity _clsSecurity;
		private clsPreCon _clsPreCon;
		private clsGlobal _clsGlobal;
		private clsCRM _clsCRM;
		private clsAsset _clsAsset;
		private clsContractGeneral _clsContractGeneral;        
		private static LogicModel modelLogic = new LogicModel();
		DropDownModel modelDropDown = new DropDownModel();

		#region declare variable for Schedule
		double s = 0;
		double e = 0;
		double er = 0;
		public static DateTime AW_StartDate;
		public static DateTime AD_BeginDate;
		public static DateTime AD_BeginDate1;
		public static DateTime LD_NextBegDate;
		public static string LW_Day;
		public static string EO_Day;
		public static string ScheCalcLogicID = string.Empty;

		public static double PrevOutstandingAmt = 0;
		public static double sumofPrevSubsidy = 0;
		public static double sumofPrevFinChrg = 0;
		public static double sumofgetIA = 0;

		public DataTable DTSchedule { get; set; }
		public DataTable DTScheduleMain { get; set; }
		public DataTable TempScheduleMain { get; set; }
		#endregion

		public PreConController()
		{
            glog.Debug("PreConController: Entry");
			_clsSecurity = new clsSecurity();
			_clsPreCon = new clsPreCon();
			_clsGlobal = new clsGlobal();
			_clsCRM = new clsCRM();
			_clsAsset = new clsAsset();
			_clsContractGeneral = new clsContractGeneral();
            glog.Debug("PreConController: Exit");
		}

		public ActionResult PreConNew(string CTGroupCode, string SubConGroupCode, string SubMenuId)
		{
			try
			{
				PreConViewModel Main = new PreConViewModel();
				Session.Clear();

				#region Check UserGroup when user direct key in URL
				if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
				{
					ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
				}
				#endregion  Check UserGroup when user direct key in URL

				var db = new MainDbContext();

				#region Set Default Values
				//Creation Date Default Value
				Main.CreationDate = System.DateTime.Now.ToString("dd'/'MM'/'yyyy");
				ViewBag.DefCreationDate = Main.CreationDate;
				ViewBag.DefISDate = Main.CreationDate;
				ViewBag.DefBeginDate = Main.CreationDate;

				//Letter of Offer Default Value
				Main.LODate = System.DateTime.Today.ToString("dd'/'MM'/'yyyy");
				ViewBag.DefLODate = Main.LODate;

				//Check Option To Renew Value.
				//Main.OpttoRenew = false;
				ViewBag.DefOpttoRenew = false;
				#endregion Set Default Values - END

				#region standard functions
				#region Load Field Properties by Sub Ctr Type
				List<FieldPropertiesModel> modelFP = new List<FieldPropertiesModel>();
				_clsPreCon.LoadFieldProperties(modelFP, SubConGroupCode);
				ViewBag.FieldProperties = modelFP;
				//ViewBag.gstChecked = _clsGlobal.GetFieldDefaultCheck("PF-104018", SubConGroupCode,clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
				//Equipment List Table
				List<FieldPropertiesModel> modelFP_Equip = new List<FieldPropertiesModel>();
				_clsPreCon.LoadFieldProperties_EquipmentTbl(modelFP_Equip, SubConGroupCode);
				ViewBag.FieldProperties_EquipmentTbl = modelFP_Equip;
				ViewBag.FieldProperties_EquipmentTbl_Json = Json(modelFP_Equip, JsonRequestBehavior.AllowGet);
				#endregion

				#region Load Logic ID by Sub Ctr Type
				modelLogic = _clsPreCon.LoadLogicID(SubConGroupCode);
				ViewBag.LogicID = modelLogic;
				#endregion

				#region Load Field Default Value from Parameter table OR by Sub Ctr Type
				ViewBag.DefaultValue = _clsPreCon.LoadDefaultValue(SubConGroupCode);
				#endregion

				#region Load Data for Drop Down List
				modelDropDown = _clsPreCon.LoadDropDownData(CTGroupCode, SubConGroupCode);
				ViewBag.DropDownData = modelDropDown;

				ViewBag.EquipmentDropDownData = GetEquipmentDropDownData();

				SelectList address = new SelectList(new List<AddressViewModel>(), "AddressId", "Address");
				ViewBag.Address = address;
				SelectList departmentList = new SelectList(new List<DepartmentViewModel>(), "cd_ref_num", "cd_dept_desc");
				ViewBag.DepartmentList = departmentList;
				SelectList contact = new SelectList(new List<ContactPersonModel>(), "Value", "Contact");
				ViewBag.Contact = contact;

				var CRMIndividualGuarantorName = _clsCRM.GetCRMGuarantor(clsVariables.Individual);
				ViewBag.CRMIndividualGuarantorName = CRMIndividualGuarantorName;

                var CRMCorporateGuarantorName = _clsCRM.GetCRMGuarantor(clsVariables.Corporate);
				ViewBag.CRMCorporateGuarantorName = CRMCorporateGuarantorName;
				#endregion

				#endregion

				#region setup code
                ViewBag.setup_InsOpt_Straight = clsVariables.setup_InsOpt_Straight;
                ViewBag.setup_RateOpt_Flat = clsVariables.setup_RateOpt_Flat;
                ViewBag.setup_PaymentMode_Giro = clsVariables.setup_PaymentMode_Giro;
                ViewBag.logic_RunSche_ISS = clsVariables.logic_RunSche_ISS;
                ViewBag.logic_RunSche_NonSpringOthers = clsVariables.logic_RunSche_NonSpringOthers;
                ViewBag.logic_RunSche_Spring = clsVariables.logic_RunSche_Spring;
                ViewBag.logic_RunSche_NonSpringLeasing = clsVariables.logic_RunSche_NonSpringLeasing;
                ViewBag.logic_MaturityDate_ISS = clsVariables.logic_MaturityDate_ISS;
                ViewBag.logic_RenewalMths_ISS = clsVariables.logic_RenewalMths_ISS;
                ViewBag.setup_IntrestType_Code = clsVariables.setup_IntrestType_Code;
                ViewBag.Individual = clsVariables.Individual;
                ViewBag.Corporate = clsVariables.Corporate;
				#endregion
			}
			catch (Exception ex)
			{
				glog.Error(ex.Message);
			}
			return View();
		}

        public ActionResult PreConEdit(string CTGroupCode, string SubConGroupCode, string SubMenuId)
        {
            try
            {
                Session.Clear();

                #region Check UserGroup when user direct key in URL
                if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
                {
                    ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
                }
                #endregion  Check UserGroup when user direct key in URL
            }
            catch (Exception ex)
            {
                glog.Error(ex.Message);
            }
            return View();
        }

        public ActionResult PreConPost(string CTGroupCode, string SubConGroupCode, string SubMenuId)
        {
            try
            {
                Session.Clear();

                #region Check UserGroup when user direct key in URL
                if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
                {
                    ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
                }
                #endregion  Check UserGroup when user direct key in URL
            }
            catch (Exception ex)
            {
                glog.Error(ex.Message);
            }
            return View();
        }

		[HttpPost]
		public JsonResult InsertPreContract_Master(string json, string securityList, string individualGuarantorList, string corporateGuarantorList, string mortgageList, string debentureList, string buybackList, string recourseList)
		{
            SysAutoGenerateReturn refNumber = new SysAutoGenerateReturn();
            SysAutoGenerateReturn contractNumber = new SysAutoGenerateReturn();
			var model = JsonConvert.DeserializeObject<PreConSaveModel>(json);
			model.SecurityList = JsonConvert.DeserializeObject<List<PreConSecurityModel>>(securityList);
			model.IndividualGuarantorList = JsonConvert.DeserializeObject<List<GuarantorModel>>(individualGuarantorList);
			model.CorparateGuarantorList = JsonConvert.DeserializeObject<List<GuarantorModel>>(corporateGuarantorList);
			model.MortgagorPropertyAndVesselList = JsonConvert.DeserializeObject<List<Contract_SecurityItemModel>>(mortgageList);
			model.DebentureList = JsonConvert.DeserializeObject<List<DebentureModel>>(debentureList);
			model.BuyBackList = JsonConvert.DeserializeObject<List<BuyBackModel>>(buybackList);
            model.RecourseList = JsonConvert.DeserializeObject<List<RecourseModel>>(recourseList);
			//model.ContractNumber = _clsGlobal.GetNewContractNumber("P", model.SubConGroupCode, model.SubProductType, model.LEFSInterestCode).NewId;
            refNumber = clsGlobal.GetSystemID("PreContract", "PRE", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
            contractNumber = _clsGlobal.GetNewContractNumber("P", model.SubConGroupCode, model.SubProductType, model.LEFSInterestCode);
			model.CreatedBy = User.Identity.Name;
			model.CreatedDate = DateTime.Now;
			var res = _clsPreCon.insertPreContract_Master(model, refNumber, contractNumber);
			return Json(res);
		}
		//[HttpPost]
		//public JsonResult InsertPreContract_SecurityList(string json)
		//{
		//    var model = JsonConvert.DeserializeObject<PreConSaveModel>(json);

		//    model.ContractNumber = _clsGlobal.GetNewContractNumber("P", model.SubConGroupCode, model.SubProductType, model.LEFSInterestCode).NewId;
		//    model.CreatedBy = User.Identity.Name;
		//    model.CreatedDate = DateTime.Now;
		//    var res = _clsPreCon.insertPreContract_Master(model);
		//    return Json(true);
		//}

		public JsonResult GetEquipmentDropDownData()
		{
			CommonDropDownModel model = new CommonDropDownModel();
			_clsPreCon.LoadEquipmentDropDownData(model);
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		#region function called from View
		public JsonResult CheckSupplierAlertList(string code)
		{
			int status = 0;
			string msg = _clsCRM.CheckAlertListFromCRM(code, ref status);

			return Json(new { Status = status, Message = msg }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult CheckCtrNoForAddendum(string number)
		{
			string Message;
			bool IsNumExist = _clsPreCon.checkContractExistByCtrNo(number);
			if (IsNumExist)
			{
				bool serialNoExist = _clsPreCon.checkSerialNumberExistByCtrNo(number);
				Message = serialNoExist ? "" : "There is no serial number";
				return Json(new { Successfull = serialNoExist, Message = Message }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				Message = "Wrong contract number";
				return Json(new { Successfull = IsNumExist, Message = Message }, JsonRequestBehavior.AllowGet);
			}
		}

		public JsonResult GetVehicleModelByMake(string code)
		{
			return Json(_clsAsset.GetVehicleModelByVehicleMake(code, true), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetModelByBrand(string code)
		{
			return Json(_clsAsset.GetModelByBrand(code, true), JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetSecurityLevel2BySecurityLevel1(string parentId)
		{
			return Json(_clsGlobal.GetListOfValue("SECURITY_LIST_LEVEL_2", parentId, "O", "", ""), JsonRequestBehavior.AllowGet);
		}
		public JsonResult CheckUnitPriceVisibility(string subProdCode)
		{
			bool unitPrice = _clsGlobal.GetFieldVisible("PF-104016", subProdCode, clsGlobal.MatrixSubProdTypeCode, DateTime.UtcNow);
			return Json(unitPrice, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetSubCon(string id)
		{
			try
			{
				List<SelectListItem> SubCon = _clsContractGeneral.GetSubCtrByCtrNo(id);
				return Json(new SelectList(SubCon, "Value", "Text"));
			}
			catch (Exception)
			{
				throw;
			}
		}

		public JsonResult GetSubProd(string id)
		{
			try
			{
				List<SelectListItem> SubProd = _clsContractGeneral.GetSubProdByProdCode(id);
				return Json(new SelectList(SubProd, "Value", "Text"));
			}
			catch (Exception)
			{
				throw;
			}
		}

		[HttpGet]
		public JsonResult GetSerialNumbers(string item)
		{
			var data = JsonConvert.DeserializeObject<string[]>(item);
			using (var db = new MainDbContext())
			{
				var list = db.Cfstb_serial_num.Where(x => data.Contains(x.cs_ctr_num)).ToList();
				return Json(list, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public JsonResult GetVendorFromSerial(string ctrNum, int conTrl)
		{
			string vendorCode;
			string vendorName;
			using (var db = new MainDbContext())
			{
				vendorCode = db.cfstb_ctr_chd.Where(x => x.cc_ctr_num == ctrNum && x.cc_con_trl == conTrl).Select(x => x.cc_ven_cod).FirstOrDefault();
			}
			//Note: currently vendor code in chd not match with CRM customer id, need mapping during data migration
			using (var db = new OrixDBEntities())
			{
				vendorName = db.crmtb_client_mas.Where(x => x.cm_client_cod == vendorCode).Select(x => x.cm_client_nam).FirstOrDefault();
			}
			return Json(new { vendorCode = vendorCode, vendorName = vendorName }, JsonRequestBehavior.AllowGet);

		}

		public string GetCommDefaultValue(string ProductTypeCode)
		{
			try
			{
				PreConViewModel Main = new PreConViewModel();
				Main.Comm = _clsGlobal.GetDefaultValueMatrix("PM-20001", ProductTypeCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
				ViewBag.DefComm = Main.Comm;
				return (ViewBag.DefComm);
			}
			catch (Exception)
			{
				throw;
			}
		}
		public string CheckSecurityLevel2IsValid(string value, string SubConGroupCode)
		{
			try
			{
				var retValue = "";
				var errMsg = "";
				if (value == clsVariables.CashEquivalentIndividual)
				{
					retValue = _clsGlobal.GetDefaultValueMatrix("PM-20012", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
					if (retValue == "N")
						errMsg = "Security Type – Cash & Equivalent (Individual) is not applicable to current contract type!";
				}
				if (value == clsVariables.CashEquivalentCompany)
				{
					retValue = _clsGlobal.GetDefaultValueMatrix("PM-20013", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
					if (retValue == "N")
						errMsg = "Security Type – Cash & Equivalent (Company) is not applicable to current contract type!";

				}
				if (value == clsVariables.SecurityDeposit)
				{
					retValue = _clsGlobal.GetDefaultValueMatrix("PM-20014", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
					if (retValue == "N")
						errMsg = "Security Type – Security Deposit is not applicable to current contract type!";
				}
				return errMsg;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public JsonResult GetLetterType()
		{
			try
			{
				var letterType = _clsGlobal.GetListOfValue("LETTER_TYPE", "", "O", "", "");
				return Json(new { data = letterType }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion

		#region CRM
		//shivam
		public JsonResult GetSecurityVesselInsurance(string HullNumber)
		{
			var list = _clsContractGeneral.GetSecurityVesselInsurance(HullNumber);
			return Json(list, JsonRequestBehavior.AllowGet);
		}
		public JsonResult GetGuarantorNameAutoComplete(string textFilter, string IndividualorCorporate)
		{
			var CRMIndividualorCorporateGuarantorName = _clsCRM.GetCRMGuarantor(IndividualorCorporate, textFilter);
			var result = Json(new { data = JsonConvert.SerializeObject(CRMIndividualorCorporateGuarantorName).ToString() }, JsonRequestBehavior.AllowGet);
			return result;
		}
		public JsonResult GetCustomerAutoComplete(string textFilter, string IndividualCorporate)
		{
			var res = _clsCRM.GetCRMCustomer(IndividualCorporate, textFilter);
			var result = Json(new { data = JsonConvert.SerializeObject(res).ToString() }, JsonRequestBehavior.AllowGet);
			return result;
		}

		public JsonResult GetSupplierAutoComplete(string textFilter)
		{
			var res = _clsCRM.GetCRMVendor("", textFilter);
			var result = Json(new { data = JsonConvert.SerializeObject(res).ToString() }, JsonRequestBehavior.AllowGet);
			return result;
		}
		public JsonResult GetReferalNameAutoComplete(string textFilter)
		{
			var res = _clsCRM.GetCRMSpotter(null, textFilter);
			var result = Json(new { data = JsonConvert.SerializeObject(res).ToString() }, JsonRequestBehavior.AllowGet);
			return result;
		}
		[HttpGet]
		public JsonResult getNricFinPassportType(string selected)
		{
			string res;
			res = _clsCRM.getNricFinPassportType(selected);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getRocUenType(string selected)
		{

			string res;
			res = _clsCRM.getRocUenType(selected);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getAddress(string selected, string IndividualCorporate)
		{
			var res = _clsCRM.getAddress(selected, IndividualCorporate);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getDepartmentList(string selectedAddress)
		{
			List<DepartmentViewModel> deptModel = _clsCRM.getDepartmentList(selectedAddress);
			return Json(new { data = deptModel }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getContactPerson(string selectedDepartment)
		{
			var res = _clsCRM.getContactPerson(selectedDepartment);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getContactMobile(string selectedContactPerson)
		{
			var res = _clsCRM.getContactMobile(selectedContactPerson);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getContactEmail(string selectedContactPerson)
		{
			var res = _clsCRM.getContactEmail(selectedContactPerson);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getContactOffice(string selectedContactPerson)
		{
			var res = _clsCRM.getContactOffice(selectedContactPerson);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getContactHome(string selectedContactPerson)
		{
			var res = _clsCRM.getContactHome(selectedContactPerson);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getContactFax(string selectedContactPerson)
		{
			var res = _clsCRM.getContactFax(selectedContactPerson);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getContactPager(string selectedContactPerson)
		{
			var res = _clsCRM.getContactPager(selectedContactPerson);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}


		[HttpGet]
		public JsonResult GetContactPersonDetails(string selectedContactPerson)
		{
			var model = new ContactPersonDetailsModel();
			model.Mobile = _clsCRM.getContactMobile(selectedContactPerson);
			model.OfficeNumber = _clsCRM.getContactOffice(selectedContactPerson);
			model.FaxNumber = _clsCRM.getContactFax(selectedContactPerson);
			model.HomeNumber = _clsCRM.getContactHome(selectedContactPerson);
			model.Email = _clsCRM.getContactEmail(selectedContactPerson);
			model.PagerNumber = _clsCRM.getContactPager(selectedContactPerson);
			return Json(new { data = model }, JsonRequestBehavior.AllowGet);
		}
		
		[HttpGet]
		public JsonResult getPropertyAddress(string cm_client_cod)
		{
			var res = _clsContractGeneral.GetPropertyAddress(cm_client_cod);
			foreach (var item in res)
			{
				item.PropertyTypeLevel1 = _clsGlobal.GetListOfValue("PROPERTY_TYPE_LEVEL1", "", "O", "", "").Where(x => x.Value == item.PropertyTypeLevel1).Select(x => x.Text).FirstOrDefault();
				item.PropertyTypeLevel2 = _clsGlobal.GetListOfValue("PROPERTY_TYPE_LEVEL2", "", "O", "", "").Where(x => x.Value == item.PropertyTypeLevel2).Select(x => x.Text).FirstOrDefault();
			}
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult getSecurityPropertyMortgagorDetails(string propertyAddressID)
		{
			var securityPropertyMortgagor = _clsContractGeneral.GetSecurityPropertyMortgagor(propertyAddressID);
			var list = new List<SecurityPropertyMortgagorDetailsModel>();
			foreach (var item in securityPropertyMortgagor)
			{
				var model = new SecurityPropertyMortgagorDetailsModel();

				model.ClientName = _clsCRM.getClientNameByCode(item.Mortgagor);
				model.MainMortgagor = item.MainMortgagor == "Y" ? "Main" : "Secondary";
				model.NRICFINPASSPORT = _clsCRM.getNricFinPassportType(item.Mortgagor);
				model.ROCUEN = _clsCRM.getRocUenType(item.Mortgagor);
				var MortgagorAddress = _clsCRM.getAddress(item.Mortgagor, item.MortgagorType).Where(x => x.AddressId == item.MortgagorAddress).FirstOrDefault();
				model.MortgagorAddress = MortgagorAddress.Address;
				var Department = _clsCRM.getDepartmentList(item.MortgagorAddress).Where(x => x.cd_ref_num == item.MortgagorDept).FirstOrDefault();
				model.Department = Department.cd_dept_desc;
				var ContactPerson = _clsCRM.getContactPerson(item.MortgagorDept).Where(x => x.Value == item.MortgagorConPerson).FirstOrDefault();
				model.ContactPerson = ContactPerson.Contact;
				model.MobileNumber = _clsCRM.getContactMobile(ContactPerson.Value);
				model.Email = _clsCRM.getContactEmail(ContactPerson.Value);
				model.OfficeNumber = _clsCRM.getContactOffice(ContactPerson.Value);
				model.HomeNumber = _clsCRM.getContactHome(ContactPerson.Value);
				model.FaxNumber = _clsCRM.getContactFax(ContactPerson.Value);
				model.PagerNumber = _clsCRM.getContactPager(ContactPerson.Value);
				list.Add(model);
			}
			return Json(new { data = list.OrderBy(x => x.ClientName).ToList() });
		}
		[HttpPost]
		public JsonResult getSecurityPropertyVesselDetails(string selectedHullVessel)
		{
			var securityPropertyMortgagor = _clsContractGeneral.GetSecurityVesselMortgagor(selectedHullVessel);
			var list = new List<SecurityVesselMortgagorDetailsModel>();
			foreach (var item in securityPropertyMortgagor)
			{
				var model = new SecurityVesselMortgagorDetailsModel();

				model.MortgagorName = _clsCRM.getClientNameByCode(item.Mortgagor);
				model.Main_SecondaryMortgagor = item.MainMortgagor == "Y" ? "Main" : "Secondary";
				model.NRIC_FIN_PASSPORT = _clsCRM.getNricFinPassportType(item.Mortgagor);
				model.ROCUEN = _clsCRM.getRocUenType(item.Mortgagor);
				var MortgagorAddress = _clsCRM.getAddress(item.Mortgagor, item.MortgagorType).Where(x => x.AddressId == item.MortgagorAddress).FirstOrDefault();
				model.Address = MortgagorAddress.Address;
				var Department = _clsCRM.getDepartmentList(item.MortgagorAddress).Where(x => x.cd_ref_num == item.MortgagorDept).FirstOrDefault();
				model.Department = Department.cd_dept_desc;
				var ContactPerson = _clsCRM.getContactPerson(item.MortgagorDept).Where(x => x.Value == item.MortgagorConPerson).FirstOrDefault();
				model.ContactPerson = ContactPerson.Contact;
				model.MobileNumber = _clsCRM.getContactMobile(ContactPerson.Value);
				model.Email = _clsCRM.getContactEmail(ContactPerson.Value);
				model.OfficeNumber = _clsCRM.getContactOffice(ContactPerson.Value);
				model.HomeNumber = _clsCRM.getContactHome(ContactPerson.Value);
				model.FaxNumber = _clsCRM.getContactFax(ContactPerson.Value);
				model.PagerNumber = _clsCRM.getContactPager(ContactPerson.Value);
				list.Add(model);
			}
			return Json(new { data = list.OrderBy(x=>x.MortgagorName).ToList() });
		}		

		[HttpGet]
		public JsonResult getHullNumberAndVesselName(string cm_client_cod)
		{
			var res = _clsContractGeneral.GetHullNumberAndVesselName(cm_client_cod);
			res.ForEach(x => x.CountryOfReg = _clsGlobal.GetCountry().Where(y => y.Value == x.CountryOfReg).Select(e => e.Text).FirstOrDefault());

			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}


		[HttpGet]
		public JsonResult getCrossCollateralizationVehicle(string SubConGroupCode, string crossCollateralizationContactNumber, int crossCollateralizationRolloverNumber)
		{
			var valueMatrix = _clsGlobal.GetDefaultValueMatrix("PM-20002", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
			var subContractList = string.IsNullOrEmpty(valueMatrix) ? new string[0] : valueMatrix.Replace("\'", "").Split(',');

			var res = _clsContractGeneral.getSecVehicleByContract(subContractList, crossCollateralizationContactNumber, crossCollateralizationRolloverNumber);
			res.ForEach(x =>
			{
				x.VehicleModel = _clsAsset.GetVehicleModelByVehicleMake(x.VehicleMake, true).Select(r => r.label).FirstOrDefault();
				x.VehicleMake = _clsAsset.GetVehicleMake(true).Where(y => y.value == x.VehicleMake).Select(e => e.label).FirstOrDefault();
			});
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}
		[HttpGet]
		public JsonResult getVehicleChassisAndRegNumber(string cm_client_cod)
		{
			var res = _clsContractGeneral.GetVehicleChassisAndRegNumber(cm_client_cod);
			var vehicleMake = _clsAsset.GetVehicleMake(true);
			res.ForEach(x =>
			{
				x.VehicleModel = _clsAsset.GetVehicleModelByVehicleMake(x.VehicleMake, true).Select(r => r.label).FirstOrDefault();
				x.VehicleMake = _clsAsset.GetVehicleMake(true).Where(y => y.value == x.VehicleMake).Select(e => e.label).FirstOrDefault();
			});

			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getConstructionEquip(string cm_client_cod)
		{
			var res = _clsAsset.GetEquipment(cm_client_cod);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getIndustrialEquip(string cm_client_cod)
		{
			var res = _clsAsset.GetIndustrial(cm_client_cod);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}		

		[HttpGet]
		public JsonResult getInventoryTypeDescription(string cm_client_cod)
		{
			var res = _clsContractGeneral.GetInventoryTypeDescription(cm_client_cod);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getReceivableAmount(string cm_client_cod)
		{
			var res = _clsContractGeneral.GetReceivableAmount(cm_client_cod);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getCashEquivalentInd(string cm_client_cod)
		{
			var res = _clsContractGeneral.GetCashEquivalentInd(cm_client_cod);
			res = getSecurityDetails(res);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}


		[HttpGet]
		public List<Security_CashEquivalentIndModel> getSecurityDetails(List<Security_CashEquivalentIndModel> res)
		{
			foreach (var item in res)
			{
				item.GuaranteeBondsType = _clsGlobal.GetListOfValue("CASH_EQUIVALENT_GUARANTEE_BONDS_TYPE", "", "O", "", "").Where(x => x.Value == item.GuaranteeBondsType).Select(x => x.Text).FirstOrDefault()??"";
				item.BillToNRIC_FIN_PASSPORT = _clsCRM.getNricFinPassportType(item.BillToCustomer)??"";
				item.BillToROCUEN = _clsCRM.getRocUenType(item.BillToCustomer) ?? "";
				var BillToAddress = _clsCRM.getAddress(item.BillToCustomer, clsVariables.Individual).Where(x => x.AddressId == item.BillToAddress).FirstOrDefault();
				item.BillToAddress = BillToAddress.Address;
				var Department = _clsCRM.getDepartmentList(BillToAddress.AddressId).Where(x => x.cd_ref_num == item.BillToDept).FirstOrDefault();
				item.BillToDept = Department.cd_dept_desc;
				var ContactPerson = _clsCRM.getContactPerson(Department.cd_ref_num).Where(x => x.Value == item.BillToConPerson).FirstOrDefault();
				if (ContactPerson != null)
				{
					item.BillToConPerson = ContactPerson.Contact;
					item.BillToMobileNumber = _clsCRM.getContactMobile(ContactPerson.Value);
					item.BillToEmail = _clsCRM.getContactEmail(ContactPerson.Value);
					item.BillToOfficeNumber = _clsCRM.getContactOffice(ContactPerson.Value);
					item.BillToHomeNumber = _clsCRM.getContactHome(ContactPerson.Value);
					item.BillToFaxNumber = _clsCRM.getContactFax(ContactPerson.Value);
					item.BillToPagerNumber = _clsCRM.getContactPager(ContactPerson.Value);
				}
				else
				{
					item.BillToConPerson = string.Empty;
					item.BillToMobileNumber = string.Empty;
					item.BillToEmail = string.Empty;
					item.BillToOfficeNumber = string.Empty;
					item.BillToHomeNumber = string.Empty;
					item.BillToFaxNumber = string.Empty;
					item.BillToPagerNumber = string.Empty;
				}
				item.BillToCustomer = _clsCRM.getClientNameByCode(item.BillToCustomer);
			}
			return res;
		}

		[HttpGet]
		public JsonResult getCashEquivalentCom(string cm_client_cod)
		{
			var res = _clsContractGeneral.GetCashEquivalentCom(cm_client_cod);
			res = getSecurityDetails(res);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getAmountAndDocumentNumber(string cm_client_cod)
		{
			var res = _clsContractGeneral.GetAmountAndDocumentNumber(cm_client_cod);
			res.ForEach(x =>
			x.Type = _clsGlobal.GetListOfValue("SECURITIES_FINANCIAL_INSTRUMENT_TYPE", "", "O", "", "").Where(y => y.Value == x.Type).Select(t => t.Text).FirstOrDefault()??""
			);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult getSecurityDepositBillToAmount(string cm_client_cod)
		{
			var res = _clsContractGeneral.GetSecurityDepositBillToAmount(cm_client_cod);
			res = getSecurityDetails(res);
			return Json(new { data = res.OrderBy(x=>x.BillToCustomer).ThenBy(x=>x.Amount).ToList() }, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Schedule
		public JsonResult RunSchedule(DateTime getBD, string getNOI, string getPOL,
		  string getTermChrg, string getTLA, string getFP, string getTermChrgPer,
		  string getTotCshPrice, string getGSTAmt, string getInsCharges,
		  string getDownpayment, string getTotLeaseAmt, string getIAAmount,
		  string getCollFee, string getInsOpt, string getDefMonths, string insoptjson, string colfeeoptjson)
		{
            glog.Debug("RunSchedule: Entry");
            glog.Debug("getBD: " + getBD.ToString() + ", getNOI: " + getNOI + ", getPOL: " + getPOL +
                ", getTermChrg: " + getTermChrg + ", getTLA: " + getTLA + ", getFP: " + getFP +
                ", getTermChrgPer: " + getTermChrgPer + ", getTotCshPrice: " + getTotCshPrice +
                ", getGSTAmt: " + getGSTAmt + ", getInsCharges: " + getInsCharges +
                ", getDownpayment: " + getDownpayment + ", getTotLeaseAmt: " + getTotLeaseAmt +
                ", getIAAmount: " + getIAAmount + ", getCollFee: " + getCollFee + 
                ", getInsOpt: " + getInsOpt + ", getDefMonths: " + getDefMonths +
                ", insoptjson: " + insoptjson + ", colfeeoptjson: " + colfeeoptjson);

			string JSONString = string.Empty;
			string Error = "";
			bool Success = true;
			var POL = getPOL;
			try
			{
				//CP 2019.04.12 Total Instalment Period = Instalment Period + Deferred Months
				if (Convert.ToInt16(getDefMonths) > 0)
				{
					getPOL = (Convert.ToInt16(getPOL) + Convert.ToInt16(getDefMonths)).ToString();
				}

				//Reset prevoutstanding amount 
				PrevOutstandingAmt = 0;
				sumofPrevSubsidy = 0;
				sumofPrevFinChrg = 0;
				sumofgetIA = 0;
				//Reset prevoutstanding amount
				insoptjson = insoptjson.Replace("Instalment Amount ($)", "InstalmentAmount");
				colfeeoptjson = colfeeoptjson.Replace("No. of Copies", "NoofCopies").Replace("Amount ($)", "Amount");

				insoptjson = "{\"IO\":" + insoptjson + "}";
				colfeeoptjson = "{\"CFO\":" + colfeeoptjson + "}";

				InsCollFeeOpt obj = JsonConvert.DeserializeObject<InsCollFeeOpt>(insoptjson);
				InsCollFeeOpt obj2 = JsonConvert.DeserializeObject<InsCollFeeOpt>(colfeeoptjson);

				//Instalment Option Row Loops
				for (int i = 0; i < obj.IO.Count; i++)
				{
					var getBegin = obj.IO[i].Begin.ToString().Trim();
					var getEnd = obj.IO[i].End.ToString().Trim();
					var getIA = obj.IO[i].InstalmentAmount.ToString().Trim();

					DTSchedule = GetScheduleTable(getBegin, getEnd, getBD, getIA, getNOI, getPOL, getTermChrg, getTLA, getFP, getTermChrgPer, getTotCshPrice, getGSTAmt, getInsCharges, getDownpayment, getTotLeaseAmt, getIAAmount, getCollFee, getInsOpt);

					//Merge Schedule table 
					if (i == 0) //Initiate main table
					{
						DTScheduleMain = DTSchedule;
					}
					else //Amend subsequent loops
					{
						DTScheduleMain.Merge(DTSchedule);
					}
				}
				//StepUp/StepDown/Deffered use Goal Seek to calculate Financial Charges
                if (getInsOpt != clsVariables.setup_InsOpt_Straight || (getInsOpt == clsVariables.setup_InsOpt_Straight && getDefMonths != "0"))
				//if (getDefMonths != "0")
				{
					TempScheduleMain = DTScheduleMain;

					//Generate Start/End for Effective Rate %
					GenerateSE(sumofPrevFinChrg, Convert.ToDouble(getTotCshPrice.Replace(",", "")), Convert.ToDouble(getFP.Replace(",", "")), getPOL);

					//Generate Effective Rate %
					GenerateER(sumofPrevFinChrg, Convert.ToDouble(getTotCshPrice.Replace(",", "")), Convert.ToDouble(getFP.Replace(",", "")), getPOL);
				}
				//If fulfilled then for each loop and update actual schedule table.

				//Add New Row
				var periodoflease = Convert.ToInt32(POL);
				var collfeeTable = obj2.CFO.LastOrDefault();
				var lastEnd = Convert.ToInt32(obj2.CFO.Select(x => x.End).LastOrDefault());

				if ((periodoflease + 1) == lastEnd)
				{

					var lastrow = DTScheduleMain.Rows[DTScheduleMain.Rows.Count - 1]; //DTScheduleMain.Rows(DTScheduleMain.Rows.Count - 1);
					var d = Convert.ToInt32(lastrow.ItemArray[0]);

					if (Convert.ToInt32(lastrow.ItemArray[0]) == (periodoflease + 1) && Convert.ToDecimal(getFP.Replace(",", "")) > 0)
					{
						lastrow["CollFee"] = (collfeeTable != null ? collfeeTable.Amount : "0.00");

					}
					else
					{
						DataRow row = DTScheduleMain.NewRow();
						row["Ins"] = collfeeTable != null ? Convert.ToInt32(collfeeTable.End) : 0;
						row["BeginDate"] = Convert.ToDateTime(lastrow.ItemArray[2]).AddDays(1).ToString("dd'/'MM'/'yyyy");
						row["EndDate"] = Convert.ToDateTime(lastrow.ItemArray[2]).AddDays(1).ToString("dd'/'MM'/'yyyy");
						row["InstalmentAmt"] = "0.00";
						row["InvNo"] = "";
						row["FinCharges"] = "0.00";
						row["CapComponent"] = "0.00";
						row["OutstandingAmt"] = "0.00";
						row["Subsidy"] = "0.00";
						row["CollFee"] = collfeeTable != null ? collfeeTable.Amount : "0.00";
						row["EarnInt"] = "0.00";
						row["EarnPrin"] = "0.00";
						DTScheduleMain.Rows.Add(row);
					}
				}

				if (obj2.CFO[0].Amount != null)
				{
					//Update Collection Fee values into Schedule table
					for (int i = 0; i < obj2.CFO.Count; i++)
					{
						for (int j = Convert.ToInt32(obj2.CFO[i].Begin); j <= Convert.ToDecimal(obj2.CFO[i].End); j++)
						{
							//if (j != lastEnd)
							{
								DTScheduleMain.Rows[j - 1]["CollFee"] = obj2.CFO[i].Amount.ToString().Trim();
							}
						}
					}
				}

				JSONString = JsonConvert.SerializeObject(DTScheduleMain);
			}
			catch (Exception ex)
			{
				Success = false;
				Error = ex.Message;
				//throw;
			}
            glog.Debug("RunSchedule: Exit");
			return Json(new { data = JSONString, success = Success, error = Error }, JsonRequestBehavior.AllowGet);
		}

		public void GenerateSE(double targetedresult = 0, double getTotCshPrice = 0, double getFP = 0, string getPOL = "")
		{
			//For Step Up/Down and Deferred calculations, refer to ECAReference > Goal Seek.xlsx
			//Set Goal Seek target effective rate starts from 15%
			//int p = 15;
			int p = Convert.ToInt16(_clsGlobal.GetDefaultValue("P-10012", DateTime.UtcNow));
			try
			{
				//for (int i = p; 0 <= p; i--)
				for (int i = p; p >= 0; i--)
				{
					// return;      
					if (GetScheduleTableSimplified(i, getTotCshPrice, getFP, getPOL) < targetedresult)
					{
						s = i;
						e = i + 1;
						return;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void GenerateER(double targetedresult, double getTotCshPrice, double getFP, string getPOL)
		{
			double tempresult = 0;
			double vardiff = 0;
			//For Step Up/Down and Deferred calculations, refer to ECAReference > Goal Seek.xlsx
			//Default Goal Seek minimal difference of targeted total financial charges = 0.01 to get target effective rate%
			//double RoundingValue = 0.01;
			double RoundingValue = Convert.ToDouble(_clsGlobal.GetDefaultValue("P-10013", DateTime.UtcNow));
			try
			{
				for (int c = 0; c < 100; c++)
				{
					er = (s + e) / 2;
					glog.Debug("Goal Seek Effective Rate %:" + er.ToString());
					tempresult = Math.Round(GetScheduleTableSimplified(er, getTotCshPrice, getFP, getPOL), 2);
					if (Math.Round(tempresult, 2) < Math.Round(targetedresult, 2))
					{
						vardiff = Math.Round(Math.Abs(tempresult - targetedresult), 2);
						//vardiff = Math.Round(52336.00 - 52336.01,2);
						if (vardiff <= RoundingValue)
						{
							//function here
							glog.Debug("Total Difference < 0.01:" + vardiff.ToString());
							reGenScheduleTable(getTotCshPrice, (vardiff * -1), getFP, getPOL);
							return;
						}
						s = er;
					}
					else if (Math.Round(tempresult, 2) > Math.Round(targetedresult, 2))
					{
						vardiff = Math.Round(Math.Abs(tempresult - targetedresult), 2);
						if (vardiff <= RoundingValue)
						{
							//function here
							glog.Debug("Total Difference < 0.01:" + vardiff.ToString());
							reGenScheduleTable(getTotCshPrice, vardiff, getFP, getPOL);
							return;
						}
						e = er;
					}
					if (Math.Round(tempresult, 2) == Math.Round(targetedresult, 2))
					{
						//Finally
						return;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public double GetScheduleTableSimplified(double i, double getTotCshPrice, double getFP, string getPOL)
		{
			double CapCom = 0;
			double returnresult = 0;
			double CapComponent = 0;
			int Count = 1;
			var EI_Day = 0;
			var LI_EarnDays = 0;
			try
			{
				foreach (DataRow dr in TempScheduleMain.Rows)
				{
					AD_BeginDate = Convert.ToDateTime(dr["BeginDate"]);
					dr["FinCharges"] = Math.Round(Convert.ToDouble(Math.Round(getTotCshPrice, 2) * Math.Round(i, 6) / 100) / 12, 2).ToString("N");

					//CP 2019.04.10 if final payment, last row's financial charges = 0
					if ((Count == Convert.ToInt16(getPOL) + 1) && (getFP > 0)) //Final Payment
					{
						dr["FinCharges"] = 0;
					}

					CapCom = Math.Round(Convert.ToDouble(dr["InstalmentAmt"]), 2) - Math.Round(Convert.ToDouble(dr["FinCharges"]), 2);
					getTotCshPrice = Math.Round(getTotCshPrice, 2) - Math.Round(CapCom, 2);
					returnresult += Math.Round(Convert.ToDouble(dr["FinCharges"]), 2);
					CapComponent = Math.Round(Convert.ToDouble(dr["InstalmentAmt"]), 2) - Math.Round(Convert.ToDouble(dr["FinCharges"]), 2);
					dr["CapComponent"] = CapComponent.ToString("N");
					dr["OutstandingAmt"] = Math.Round(getTotCshPrice, 2).ToString("N");

					//CP 2019.04.10 if final payment, last row's outstanding amount = 0
					if ((Count == Convert.ToInt16(getPOL) + 1) && (getFP > 0)) //Final Payment
					{
						dr["OutstandingAmt"] = 0;
					}

					#region Servicing Loan
					//if (modelLogic.logic_CtrSchCalc == logic_RunSche_ISS) //Run Schedule Calculation - Servicing Loan
					//{
					//    if (Count == TempScheduleMain.Rows.Count + 1) //last new line
					//    {
					//        dr["EarnInt"] = dr["FinCharges"];
					//    }
					//    else
					//    {
					//        if (Convert.ToInt32(LW_Day) > 30)
					//        {
					//            EI_Day = 30;
					//        }
					//        else
					//        {
					//            EI_Day = Convert.ToInt32(LW_Day);
					//        }
					//        LI_EarnDays = 30 - EI_Day + 1;
					//        dr["EarnInt"] = Math.Round((LI_EarnDays * Convert.ToDouble(dr["FinCharges"]) / 30), 2);
					//    }
					//}
					#endregion Servicing Loan

					//CP 2019.04.10
					//if (TempScheduleMain.Rows.Count == Count) //Last Instalment
					if (Count == Convert.ToInt16(getPOL)) //Last Instalment
					{
						dr["EarnInt"] = dr["FinCharges"]; //OK
					}
					else//rest of the days
					{
						if (Convert.ToInt32(LW_Day) > 30)
						{
							EI_Day = 30;
						}
						else
						{
							EI_Day = Convert.ToInt32(LW_Day);
						}
						LI_EarnDays = 30 - EI_Day + 1;
						dr["EarnInt"] = Math.Round((LI_EarnDays * Convert.ToDouble(dr["FinCharges"]) / 30), 2).ToString("N");//OK
					}

					#region Servicing Loan
					//if (modelLogic.logic_CtrSchCalc == logic_RunSche_ISS) //Run Schedule Calculation - Servicing Loan
					//{
					//    if (TempScheduleMain.Rows.Count == Count) //Last Instalment
					//    {
					//        dr["EarnPrin"] = dr["CapComponent"]; 
					//    }
					//    else if (Count == TempScheduleMain.Rows.Count + 1) //last new line
					//    {
					//        dr["EarnPrin"] = dr["CapComponent"];
					//    }
					//    else
					//    {
					//        if (Convert.ToInt32(LW_Day) > 30)
					//        {
					//            EI_Day = 30;
					//        }
					//        else
					//        {
					//            EI_Day = Convert.ToInt32(LW_Day);
					//        }
					//        LI_EarnDays = 30 - EI_Day + 1;
					//        dr["EarnPrin"] = Math.Round((LI_EarnDays * Convert.ToDouble(dr["CapComponent"]) / 30), 2);
					//    }
					//}
					#endregion Servicing Loan

					//CP 2019.04.10
					//if (TempScheduleMain.Rows.Count == Count) //Last Instalment
					if (Count == Convert.ToInt16(getPOL)) //Last Instalment
					{
						dr["EarnPrin"] = dr["CapComponent"]; //OK
					}
					else if (Count == Convert.ToInt16(getPOL) + 1) //Final Payment
					{
						dr["EarnPrin"] = dr["CapComponent"]; //OK
					}
					else//rest of the days
					{
						if (Convert.ToInt32(LW_Day) > 30)
						{
							EI_Day = 30;
						}
						else
						{
							EI_Day = Convert.ToInt32(LW_Day);
						}
						LI_EarnDays = 30 - EI_Day + 1;
						dr["EarnPrin"] = Math.Round((LI_EarnDays * Convert.ToDouble(dr["CapComponent"]) / 30), 2).ToString("N");//OK
					}

					Count++;
				}
			}
			catch (Exception)
			{
				throw;
			}
			return returnresult;
		}

		public void reGenScheduleTable(double getTotCshPrice, double vardiff, double getFP, string getPOL)
		{
			int iLoop = 1;
			double PrevOutstandingAmt = 0;

			try
			{
				foreach (DataRow dr in TempScheduleMain.Rows)
				{
					//CP 2019.04.10
					//if (TempScheduleMain.Rows.Count == iLoop)
					if (iLoop == Convert.ToInt16(getPOL))
					{
						dr["FinCharges"] = Convert.ToDouble(dr["FinCharges"]) - vardiff;
						dr["CapComponent"] = Convert.ToDouble(Convert.ToDouble(dr["InstalmentAmt"]) - Convert.ToDouble(dr["FinCharges"])).ToString("N");
						dr["OutstandingAmt"] = Convert.ToDouble(PrevOutstandingAmt - Convert.ToDouble(dr["CapComponent"])).ToString("N");
						dr["EarnInt"] = dr["FinCharges"];
						dr["EarnPrin"] = dr["CapComponent"];
					}
					PrevOutstandingAmt = Convert.ToDouble(dr["OutstandingAmt"]);
					iLoop++;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public DateTime Get_AD_EndDate(DateTime AD_EndDate, string LI_Day)
		{
			//AD_BeginDate = AW_StartDate;

			var AW_StartDateDays = AW_StartDate.Day.ToString();
			AD_EndDate = AD_BeginDate.AddDays(Convert.ToInt32(LI_Day) - 1); //AD_EndDate
			AD_BeginDate1 = AD_BeginDate.AddMonths(2); //AD_BeginDate
			var firstDayOfMonth = new DateTime(AD_BeginDate1.Year, AD_BeginDate1.Month, 1);
			var LD_MaxDate = firstDayOfMonth.AddDays(-1); //LD_MaxDate

			AD_EndDate = LD_MaxDate;

			LW_Day = AD_BeginDate.Day.ToString();

			//Split AD_EndDate
			var LW_Year = AD_EndDate.Year.ToString();
			var LW_Month = AD_EndDate.Month.ToString();
			var LW_Day1 = AD_EndDate.Day.ToString();

			//Combine fields to get next begin date
			string CombineDate = AW_StartDateDays + "/" + LW_Month + "/" + LW_Year;

			//Check next begin date validity
			DateTime temp;
			if (DateTime.TryParse(CombineDate, out temp))
			{
				LD_NextBegDate = Convert.ToDateTime(CombineDate);
			}
			else
			{
				LD_NextBegDate = LD_MaxDate;
			}

			//Generate end date
			AD_EndDate = Convert.ToDateTime(LD_NextBegDate).AddDays(-1);

			return AD_EndDate;
		}

		public DataTable GetScheduleTable(string getBegin, string getEnd, DateTime getBD, string getIA, string getNOI, string getPOL, string getTermChrg, string getTLA, string getFP, string getTermChrgPer, string getTotCshPrice, string getGSTAmt, string getInsCharges, string getDownpayment, string getTotLeaseAmt, string getIAAmount, string getCollFee, string getInsOpt)
		{
			PreConViewModel Main = new PreConViewModel();
			getBD.ToString(clsGlobal.datetimeFormat); // convert to dd/MM/YYYY
			DataTable table = new DataTable();
			table.Columns.Add("Ins", typeof(int));
			table.Columns.Add("BeginDate", typeof(string));
			table.Columns.Add("EndDate", typeof(string));
			table.Columns.Add("InstalmentAmt", typeof(string));
			table.Columns.Add("InvNo", typeof(string));
			table.Columns.Add("FinCharges", typeof(string));
			table.Columns.Add("CapComponent", typeof(string));
			table.Columns.Add("OutstandingAmt", typeof(string));
			table.Columns.Add("Subsidy", typeof(string));
			table.Columns.Add("CollFee", typeof(string));
			table.Columns.Add("EarnInt", typeof(string));
			table.Columns.Add("EarnPrin", typeof(string));

			double LR_SumDigit = 0;
			double LC_Principle = 0;
			double FinChrg = 0;
			double CapComponent = 0;
			double OutstandingAmt = 0;
			double Subsidy = 0;
			double CollFee = 0;
			double EarnInt = 0;
			double EarnPrin = 0;
			LW_Day = string.Empty;
			Int32 EI_Day = 0;
			Int32 LI_EarnDays = 0;

			AW_StartDate = getBD; //First beginning of the date. Store in global, once and for all.
			AD_BeginDate = getBD; //Store in Global to pass to loop.            

			DateTime AD_EndDate = DateTime.Now;
			var LI_Day = AD_BeginDate.Day.ToString(); //LI_Day
			int loopAdd = 1;

            if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_ISS) //Servicing Loan 
			{
				loopAdd = 2;
			}
			//(Non-Spring Non Leasing or Non-Spring Leasing) and FP more than 0
            else if ((modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_NonSpringOthers || modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_NonSpringLeasing) && Convert.ToDouble(getFP) > 0)
			{
				loopAdd = 2;

				//CP 2019.04.08
				//If StepUp/StepDown with Final Payment, should add one more row during looping for last row in Instalment Option
                if (getInsOpt != clsVariables.setup_InsOpt_Straight) //StepUp/StepDown
				{
					//Check whether it's last row in Instalment Option
					if (Convert.ToInt16(getEnd) < Convert.ToInt16(getPOL))
						loopAdd = 1;
				}
			}

			for (int i = Convert.ToInt16(getBegin); i < Convert.ToInt16(getEnd) + loopAdd; i++)
			{

				int m = i - 1; //Get current begin date instead of next month
							   //If AW_StartDate <> 1
							   //CP 2019.04.05
				EO_Day = DateTime.DaysInMonth(AD_BeginDate.Year, AD_BeginDate.Month).ToString(); //Get last day of begin date.
				if (Convert.ToInt32(LI_Day) != 1)
				{
					//FirstDay
					if (i == 1)
					{
						EO_Day = DateTime.DaysInMonth(AD_BeginDate.Year, AD_BeginDate.Month).ToString(); //Get last day of begin date.
						AD_BeginDate = AW_StartDate;
						AD_EndDate = Get_AD_EndDate(AD_EndDate, LI_Day);

						#region Original Code
						//var AW_StartDateDays = AW_StartDate.Day.ToString();
						//AD_EndDate = AD_BeginDate.AddDays(Convert.ToInt32(LI_Day) - 1); //AD_EndDate
						//AD_BeginDate1 = AD_BeginDate.AddMonths(2); //AD_BeginDate
						//var firstDayOfMonth = new DateTime(AD_BeginDate1.Year, AD_BeginDate1.Month, 1);
						//var LD_MaxDate = firstDayOfMonth.AddDays(-1); //LD_MaxDate
						////if (AD_EndDate > LD_MaxDate)
						////if (AD_EndDate < LD_MaxDate)
						////{
						//    AD_EndDate = LD_MaxDate;
						////};
						////AD_EndDate = LD_MaxDate;
						//LW_Day = AD_BeginDate.Day.ToString();

						////Split AD_EndDate
						//var LW_Year = AD_EndDate.Year.ToString();
						//var LW_Month = AD_EndDate.Month.ToString();
						//var LW_Day1 = AD_EndDate.Day.ToString();

						////Combine fields to get next begin date
						//string CombineDate = AW_StartDateDays + "/" + LW_Month + "/" + LW_Year;

						////Check next begin date validity
						//DateTime temp;
						//if (DateTime.TryParse(CombineDate, out temp))
						//{ 
						//    LD_NextBegDate = Convert.ToDateTime(CombineDate); 
						//}
						//else
						//{ 
						//    LD_NextBegDate = LD_MaxDate; 
						//}

						////Generate end date
						//AD_EndDate = Convert.ToDateTime(LD_NextBegDate).AddDays(-1);
						#endregion Original Code
					}
					else if (i >= Convert.ToInt16(getPOL) + (loopAdd - 1)) //Last line and last new line
					{
						#region End Date for last line.
						if (loopAdd == 1)
						{
							AD_BeginDate = LD_NextBegDate;

							if (getBD.AddMonths(Convert.ToInt32(getNOI)).AddDays(-1) < AD_EndDate)
							{
								AD_EndDate = getBD.AddMonths(Convert.ToInt32(getNOI)).AddDays(-1);
							}
							else
							{
								AD_EndDate = Get_AD_EndDate(AD_EndDate, LI_Day);
							}
						}
						else
						{
							//CP 2019.04.10
							//START
							//If Not Intereset Servicing (e.g. Final Payment), begin and end date = prev end date + 1                            
							AD_BeginDate = LD_NextBegDate;

							//If Interest Servicing, begin and end date = prev end date
                            if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_ISS) //Run Schedule Calculation - Servicing Loan
							{
								AD_BeginDate = LD_NextBegDate.AddDays(-1);
							}
							AD_EndDate = AD_BeginDate;
							//END
						}
						#endregion
					}

					//SecondDay onwards
					else
					{
						AD_BeginDate = LD_NextBegDate;
						EO_Day = DateTime.DaysInMonth(AD_BeginDate.Year, AD_BeginDate.Month).ToString(); //Get last day of begin date.
						AD_EndDate = Get_AD_EndDate(AD_EndDate, LI_Day);
					}
				}
				//If AW_StartDate == 1
				else
				{
					#region if is 1
					if (i == 1)
					{
						AD_BeginDate = getBD.AddMonths(m);
						AD_EndDate = getBD.AddMonths(1).AddDays(-1); //AD_EndDate
						LW_Day = AD_BeginDate.Day.ToString();
					}
					else
					{
						AD_BeginDate = AD_BeginDate.AddMonths(1);
						AD_EndDate = AD_BeginDate.AddMonths(1).AddDays(-1); //AD_EndDate
						LW_Day = AD_BeginDate.Day.ToString();
					}
					#endregion
				}
				//Initiate Calculation
				#region Financial Charges (OK)
				#region Servicing Loan OK
                if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_ISS) //Run Schedule Calculation - Servicing Loan
				{
					if (i == Convert.ToInt16(getPOL)) //Last Instalment Before New Line Logic Here
					{
						FinChrg = Math.Round(Convert.ToDouble(getTermChrg) - sumofPrevFinChrg, 2); //OK
					}
					else if (i == Convert.ToInt16(getPOL) + 1) //New last line
					{
						FinChrg = 0;//OK
					}
					else // rest of the days
					{
						FinChrg = Math.Round((Convert.ToDouble(getTLA) - Convert.ToDouble(getFP) - Convert.ToDouble(getTermChrg)) * Convert.ToDouble(getTermChrgPer) / 100 / 12, 2); //OK
					}
				}
				#endregion Servicing Loan
				#region Non-Spring (OK)
				//Non-Spring Non Leasing or Non-Spring Leasing
                else if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_NonSpringOthers || modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_NonSpringLeasing) //Run Schedule Calculation - Non-Spring
				{
					if (i == Convert.ToInt16(getPOL)) //Last Instalment
					{
						FinChrg = Math.Round(Convert.ToDouble(getTermChrg) - sumofPrevFinChrg, 2); //OK - Not tally with actual data.
																								   //FinChrg = Math.Round(Convert.ToDouble(getTermChrg) - sumofPrevFinChrg); //Round to nearest whole number
					}
					else if ((i == Convert.ToInt16(getPOL) + 1) && (Convert.ToDouble(getFP) > 0)) //New last line - Final Payment
					{
						FinChrg = 0;//OK
					}
					else
					{
						//Normal Straight
						LR_SumDigit = (Convert.ToDouble(getNOI) + 1 - Convert.ToDouble(i)) / ((Convert.ToDouble(getNOI) * (Convert.ToDouble(getNOI) + 1)) / 2);
						FinChrg = Math.Round(LR_SumDigit * Convert.ToDouble(getTermChrg)); //OK Round to nearest whole number
																						   //Normal Straight
					}
				}
				#endregion Non-Spring
				#region Spring (OK)
                else if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_Spring) //Run Schedule Calculation - Spring
				{
					//string A = "100.00";
					//string B = "30.00";
					//string C = "360.00";
					//LC_Principle = Convert.ToDouble(getTotCshPrice) + Convert.ToDouble(getGSTAmt) + Convert.ToDouble(getInsCharges) - Convert.ToDouble(getDownpayment);//OK
					if (i == 1)
					{//First Instalment
						LC_Principle = Convert.ToDouble(getTotCshPrice) + Convert.ToDouble(getGSTAmt) + Convert.ToDouble(getInsCharges) - Convert.ToDouble(getDownpayment);//OK
						FinChrg = Math.Round((Convert.ToDouble(getTermChrgPer) / 100 * 30 / 360) * LC_Principle, 2); //OK
																													 //FinChrg = Convert.ToDouble(getTermChrgPer) / Convert.ToDouble(A) * Convert.ToDouble(B) / Convert.ToDouble(C); 
																													 //FinChrg = Math.Round(FinChrg * Convert.ToDouble(LC_Principle),3);//OK
																													 //FinChrg = Math.Round(FinChrg, 2);
					}
					else
					{//Second Instalment
						LC_Principle = PrevOutstandingAmt;
						FinChrg = Math.Round((Convert.ToDouble(getTermChrgPer) / 100 * 30 / 360) * LC_Principle, 2); //OK
					}
				}
				#endregion Spring
				sumofPrevFinChrg += Math.Round(FinChrg, 2);
				#endregion Financial Charges

				#region Instalment Amount

                if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_ISS) //Run Schedule Calculation - Servicing Loan
				{
					if (i == Convert.ToInt16(getPOL) + 1) //Last new line
					{
						//Refer to Capital Component
						//getIA = Convert.ToString(Math.Round(CapComponent, 2));
					}
					else if (i == Convert.ToInt16(getPOL)) //Last Instalment
					{
						getIA = Convert.ToString(Math.Round(FinChrg, 2)); //OK 
					}
					else
					{
						getIA = Convert.ToString(Math.Round(Convert.ToDouble(getIA), 2));
					}
				}
				//Non-Spring Non Leasing or Non-Spring Leasing
                else if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_NonSpringOthers || modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_NonSpringLeasing) //Run Schedule Calculation - Non- Spring
				{
					if (i == Convert.ToInt16(getPOL) + 1 && Convert.ToDouble(getFP) > 0) //Last new line
					{
						getIA = Convert.ToString(Math.Round(Convert.ToDouble(getFP), 2));
					}
					else if (i == Convert.ToInt16(getPOL)) //Last Instalment
					{
						//KIV
						//LR_TotInstall = sumofgetIA;
						getIA = Convert.ToString(Convert.ToDouble(getTotLeaseAmt) - Convert.ToDouble(getFP) - sumofgetIA);
					}
					else // rest of the days
					{
						getIA = Convert.ToString(Math.Round(Convert.ToDouble(getIA), 2));
						sumofgetIA += Convert.ToDouble(getIA); //sum of IA
					}
				}
                else if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_Spring) //Spring
				{
					if (i == Convert.ToInt16(getPOL)) //Last Instalment
					{
						getIA = Convert.ToString(PrevOutstandingAmt + FinChrg); //OK 
					}
					else
					{
						getIA = Convert.ToString(Math.Round(Convert.ToDouble(getIA), 2));
					}
				}

				#endregion Instalment Amount
				//Check
				#region Capital Component (OK)

                if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_ISS) //Run Schedule Calculation - Servicing Loan
				{
					if (i == Convert.ToInt16(getPOL) + 1) //New last line
					{
						CapComponent = Convert.ToDouble(getTotLeaseAmt) - Convert.ToDouble(getTermChrg);//OK
						getIA = Convert.ToString(Math.Round(CapComponent, 2));
					}
					else // rest of the days
					{
						CapComponent = 0;
					}
				}
				else
				{
					CapComponent = Math.Round(Convert.ToDouble(getIA) - FinChrg, 2);//OK
				}
				#endregion Capital Component

				//IA wuz here

				#region Outstanding Amount (Check)
                if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_Spring) //Run Schedule Calculation - Spring
				{
					if (i == 1)
					{//First Instalment
						OutstandingAmt = Math.Round(Convert.ToDouble(getTotCshPrice) + Convert.ToDouble(getGSTAmt) + Convert.ToDouble(getInsCharges) - Convert.ToDouble(getDownpayment) - CapComponent, 2);//OK
					}
					else
					{//Second Instalment
						OutstandingAmt = Math.Round(PrevOutstandingAmt - CapComponent, 2);//OK
					}
				}
				//Interest Servicing or Non-Spring Non Leasing or Non-Spring Leasing
                else if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_ISS || modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_NonSpringOthers || modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_NonSpringLeasing)
				{
					if (i == 1)
					{//First Instalment
						OutstandingAmt = Math.Round(Convert.ToDouble(getTotLeaseAmt) - Convert.ToDouble(getTermChrg) - CapComponent, 2);//OK
					}

					else if (i == Convert.ToInt16(getPOL)) //Last Instalment
					{
                        if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_ISS)  // Interest Servicing sub of non-spring
						{
							//OutstandingAmt = CapComponent;
							OutstandingAmt = Math.Round(PrevOutstandingAmt - CapComponent, 2);//Check Required
						}
						else
						{
							OutstandingAmt = Math.Round(Convert.ToDouble(getFP), 2);//OK
						}
					}

					else if (i == Convert.ToInt16(getPOL) + 1) //New Last Line, supposed for servicing loan only. MARK HERE If this OK Earn Int should be OK
					{
						OutstandingAmt = 0; // CHECK
					}
					else
					{//rest of the days
						OutstandingAmt = Math.Round(PrevOutstandingAmt - CapComponent, 2);//OK
					}
				}

				PrevOutstandingAmt = OutstandingAmt;
				#endregion Outstanding Amount
				//Check

				#region Subsidy (OK)
                if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_NonSpringOthers || modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_NonSpringLeasing) //Non-Spring Non Leasing or Non-Spring Leasing
				{
					if (i == Convert.ToInt16(getPOL)) //Last Instalment
					{
						Subsidy = Math.Round(Convert.ToDouble(getIAAmount) - sumofPrevSubsidy, 2);//OK
					}
					else
					{
						LR_SumDigit = (Convert.ToDouble(getNOI) + 1 - Convert.ToDouble(i)) / ((Convert.ToDouble(getNOI) * (Convert.ToDouble(getNOI) + 1)) / 2);

						//LR_SumDigit = (Convert.ToDouble(getNOI) + 1 - Convert.ToDouble(getPOL)) / ((Convert.ToDouble(getNOI) * (Convert.ToDouble(getNOI) + 1)) / 2);
						Subsidy = Math.Round(LR_SumDigit * Convert.ToDouble(getIAAmount)); //OK
					}
					sumofPrevSubsidy += Subsidy;
				}
				#endregion Subsidy

				#region Collection Fee (Check - Collection fee table required)
				if (!string.IsNullOrEmpty(getCollFee))
				{
					CollFee = Convert.ToDouble(getCollFee); //Check against collection fee option table.
				}
				#endregion

				#region Earn Int.
				#region Servicing Loan
                if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_ISS) //Run Schedule Calculation - Servicing Loan
				{
					if (i == Convert.ToInt16(getPOL) + 1) //last new line
					{
						EarnInt = FinChrg;
						//EarnInt = 0; //3)	For last instalment, “Earn Int.” = “Financial Charges”.
					}
					else
					{ // rest of the days
						if (Convert.ToInt32(LW_Day) > 30)
						{
							EI_Day = 30;
						}
						else
						{
							EI_Day = Convert.ToInt32(LW_Day);
						}
						LI_EarnDays = Convert.ToInt32(EO_Day) - EI_Day + 1;
						EarnInt = Math.Round((LI_EarnDays * FinChrg / Convert.ToInt32(EO_Day)), 2);
					}
				}
				#endregion  Servicing Loan
				else
				{
					if (i == Convert.ToInt16(getPOL)) //Last Instalment
					{
						//Common law last instalment  to get earned and unearned. 
						EarnInt = FinChrg; //OK
					}
					else//rest of the days
					{
						//CP 2019.04.05 Calc earn days using no. of days of the begin date, no need set to "30" if 31 days
						//if (Convert.ToInt32(LW_Day) > 30)
						//{
						//    EI_Day = 30;
						//}
						//else
						//{
						EI_Day = Convert.ToInt32(LW_Day);
						//}
						LI_EarnDays = Convert.ToInt32(EO_Day) - EI_Day + 1;
						EarnInt = Math.Round((LI_EarnDays * FinChrg / Convert.ToInt32(EO_Day)), 2);//OK
					}
				}
				#endregion

				#region Earn Prin.
				#region Servicing Loan
                if (modelLogic.logic_CtrSchCalc == clsVariables.logic_RunSche_ISS) //Run Schedule Calculation - Servicing Loan
				{
                    //Amednded by Jason 04/07/2019

                    if (i == Convert.ToInt16(getPOL) && (Convert.ToDouble(getFP) > 0)) //Last Instalment with final payment
                    {
                        EarnPrin = CapComponent;
                    }
                    else if (i == Convert.ToInt16(getPOL)) //Last Instalment
					{
						EarnPrin = CapComponent;
					}
					else if (i == Convert.ToInt16(getPOL) + 1) //last new line
					{
						EarnPrin = CapComponent;
					}
					else
					{
						if (Convert.ToInt32(LW_Day) > 30)
						{
							EI_Day = 30;
						}
						else
						{
							EI_Day = Convert.ToInt32(LW_Day);
						}
						LI_EarnDays = Convert.ToInt32(EO_Day) - EI_Day + 1;
						EarnPrin = Math.Round((LI_EarnDays * CapComponent / Convert.ToInt32(EO_Day)), 2);
					}
				}
				#endregion Servicing Loan
				else
				{
                    //Amednded by Jason 04/07/2019

                    if (i == Convert.ToInt16(getPOL) + 1 && (Convert.ToDouble(getFP) > 0)) //Last Instalment with final payment
					{
                        EarnPrin = CapComponent;
                    }
                    else if (i == Convert.ToInt16(getPOL)) //Last Instalment
					{
						EarnPrin = CapComponent; //OK 
					}
					else
					{
						//CP 2019.04.05 Calc earn days using no. of days of the begin date, no need set to "30" if 31 days
						//if (Convert.ToInt32(LW_Day) > 30)
						//{
						//    EI_Day = 30;
						//}
						//else
						//{
						EI_Day = Convert.ToInt32(LW_Day);
						//}
						LI_EarnDays = Convert.ToInt32(EO_Day) - EI_Day + 1;
						EarnPrin = Math.Round((LI_EarnDays * CapComponent / Convert.ToInt32(EO_Day)), 2);//OK
					}
				}
				#endregion

				//Initiate Calculation

				//Presented to numbers
				if (getIA != "0")
				{
					getIA = String.Format("{0:0,0.00}", Convert.ToDouble(getIA));
				}
				else { getIA = "0.00"; }
				string FChrg = FinChrg.ToString("N");
				string CapCom = CapComponent.ToString("N");
				string OutsAmt = OutstandingAmt.ToString("N");
				string Subs = Subsidy.ToString("N");
				string CFee = CollFee.ToString("N");
				string EInt = EarnInt.ToString("N");
				string EPrin = EarnPrin.ToString("N");

				//FinChrg = Convert.ToDouble(String.Format("{0:0,0.00}", FinChrg));
				//OutstandingAmt = Convert.ToDouble(String.Format("{0:0,0.00}", Convert.ToDouble(OutstandingAmt)));
				//Subsidy = Convert.ToDouble(String.Format("{0:0,0.00}", Convert.ToDouble(Subsidy)));
				//CollFee = Convert.ToDouble(String.Format("{0:0,0.00}", Convert.ToDouble(CollFee)));
				//EarnInt = Convert.ToDouble(String.Format("{0:0,0.00}", Convert.ToDouble(EarnInt)));
				//EarnPrin = Convert.ToDouble(String.Format("{0:0,0.00}", Convert.ToDouble(EarnPrin)));
				//Presented to numbers

				//Initiate rows add 
				table.Rows.Add(
						i,
						AD_BeginDate.ToString("dd'/'MM'/'yyyy"),
						AD_EndDate.ToString("dd'/'MM'/'yyyy"),
						getIA,   //Instalment Amt
						"",
						FChrg,
						CapCom,
						OutsAmt,
						Subs,
						CFee,
						EInt,
						EPrin
						);
				//table.Rows.Add(
				//    i,
				//    AD_BeginDate,
				//    AD_EndDate,
				//    getIA,   //Instalment Amt
				//    "",
				//    FinChrg,
				//    CapComponent,
				//    OutstandingAmt,
				//    Subsidy,
				//    CollFee,
				//    EarnInt,
				//    EarnPrin
				//    );
			}

			return table;
		}
		#endregion
	}
}