using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using Newtonsoft.Json;
using EthozCapital.Models;
using EthozCapital.Models.ViewModels;
using EthozCapital.CustomLibraries;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EthozCapital.Controllers
{
	public class SecurityController : Controller
	{
		private clsGlobal _clsGlobal;
		private clsCRM _clsCRM;
		private clsAsset _clsAsset;

		private clsSecurity _clsSecurity;

		public SecurityController()
		{
			_clsSecurity = new clsSecurity();
			_clsGlobal = new clsGlobal();
			_clsCRM = new clsCRM();
			_clsAsset = new clsAsset();
		}
		public ActionResult Index()
		{
			return View();
		}

		#region CRM
		public JsonResult GetCustomerAutoComplete(string textFilter, string IndividualCorporate)
		{
			var res = _clsCRM.GetCRMCustomer(IndividualCorporate, textFilter);
			var result = Json(new { data = JsonConvert.SerializeObject(res).ToString() }, JsonRequestBehavior.AllowGet);
			return result;
		}

		public JsonResult GetMortgagorAutoComplete(string textFilter, string IndividualCorporate)
		{
			var res = _clsCRM.GetCRMSecurityGiver(IndividualCorporate, textFilter);
			return Json(new { data = res }, JsonRequestBehavior.AllowGet);
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
		#endregion

		#region Property
		public ActionResult PropertyNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL

			var model = new SecurityPropertyViewModel();

			SelectList lstType = new SelectList(_clsGlobal.GetListOfValue("PROPERTY_TYPE_LEVEL1", "", "O", "", ""), "Value", "Text");
			ViewBag.PropertyTypeLevel1 = lstType;

			lstType = new SelectList(_clsGlobal.GetListOfValue("PROPERTY_TYPE_LEVEL2", "", "O", "", ""), "Value", "Text");
			ViewBag.PropertyTypeLevel2 = lstType;
			ViewData["IsNew"] = true;
			return View(model);
		}

		[HttpPost]
		public ActionResult InsertPropertyDetails(string json)
		{
			var model = JsonConvert.DeserializeObject<PropertyModel>(json);
			var result = _clsSecurity.InsertPropertyDetails(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult PropertyView()
		{
			return View();
		}

		public ActionResult PropertyEdit()
		{
			return View();
		}

		public PartialViewResult GetPropertyDetails(bool IsView, string SecurityId)
		{
			var model = new SecurityPropertyViewModel();

			SelectList lstType = new SelectList(_clsGlobal.GetListOfValue("PROPERTY_TYPE_LEVEL1", "", "O", "", ""), "Value", "Text");
			ViewBag.PropertyTypeLevel1 = lstType;

			lstType = new SelectList(_clsGlobal.GetListOfValue("PROPERTY_TYPE_LEVEL2", "", "O", "", ""), "Value", "Text");
			ViewBag.PropertyTypeLevel2 = lstType;

			lstType = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstType;

			if (!string.IsNullOrEmpty(SecurityId))
			{
				model = _clsSecurity.GetSecurityPropertyDetails(SecurityId, "Security_PropertyMortgagor");
			}

			ViewBag.Viewable = IsView;
			return PartialView("_PropertyPartialView", model);
		}

		[HttpPost]
		public ActionResult UpdatePropertyStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdatePropertyStatus(Status, Id, User.Identity.Name);
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

		#endregion

		#region Vessel
		[HttpGet]
		public ActionResult GetInsuranceType()
		{
			var iType = _clsGlobal.GetListOfValue("VESSEL_INSURANCE_TYPE", "", "O", "", "");
			var res = new JsonResult() { Data = iType, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult VesselNew(string SubMenuId)
		{
			var model = new VesselModel();
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL

			SelectList countryList = new SelectList(_clsGlobal.GetCountry(), "Value", "Text");
			ViewData["IsNew"] = true;
			ViewBag.Country = countryList;
			return View(model);
		}

		[HttpPost]
		public ActionResult InsertVesselDetails(string json)
		{
			var model = JsonConvert.DeserializeObject<VesselModel>(json);
			var result = _clsSecurity.InsertVesselDetails(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult VesselView()
		{
			return View();
		}

		public ActionResult VesselEdit()
		{
			return View();
		}

		public PartialViewResult GetVesselDetails(bool IsView, string SecurityId)
		{
			var model = new VesselModel();
			SelectList countryList = new SelectList(_clsGlobal.GetCountry(), "Value", "Text");
			ViewBag.Country = countryList;
			SelectList vehicleModel = new SelectList(new List<CommonDropDown>(), "value", "label");
			ViewBag.VehicleModel = vehicleModel;

			SelectList lstType = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstType;

			if (!string.IsNullOrEmpty(SecurityId))
			{
				model = _clsSecurity.GetSecurityVesselDetails(SecurityId, "Security_VesselMortgagor");
			}
			ViewBag.Viewable = IsView;
			return PartialView("_VesselPartialView", model);
		}

		[HttpPost]
		public ActionResult UpdateVesselStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdateVesselStatus(Status, Id, User.Identity.Name);
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
		#endregion

		#region Vehicle
		public ActionResult VehicleNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL
			var model = new VehicleModel();
			SelectList vehicleMake = new SelectList(_clsAsset.GetVehicleMake(false), "value", "label");
			ViewBag.VehicleMake = vehicleMake;
			SelectList vehicleModel = new SelectList(new List<CommonDropDown>(), "value", "label");
			ViewBag.VehicleModel = vehicleModel;
			ViewData["IsNew"] = true;
			return View(model);
		}

		public JsonResult GetVehicleModelByMake(string code)
		{
			return Json(_clsAsset.GetVehicleModelByVehicleMake(code, false), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult InsertVehicleDetails(string json)
		{
			var model = JsonConvert.DeserializeObject<VehicleModel>(json);
			var result = _clsSecurity.InsertVehicleDetails(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult VehicleView()
		{
			return View();
		}

		public ActionResult VehicleEdit()
		{
			return View();
		}

		public PartialViewResult GetSecurityVehicle(bool IsView, string SecurityID)
		{
			var model = new VehicleModel();

			SelectList vehicleMake = new SelectList(_clsAsset.GetVehicleMake(false), "value", "label");
			ViewBag.VehicleMake = vehicleMake;
			SelectList vehicleModel = new SelectList(new List<CommonDropDown>(), "value", "label");

			SelectList lstType = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstType;

			if (!string.IsNullOrEmpty(SecurityID))
			{
				model = _clsSecurity.GetSecurityVehicleDetails(SecurityID);
				if (model != null)
				{
					vehicleModel = new SelectList(_clsAsset.GetVehicleModelByVehicleMake(model.VehicleDetails.VehicleMakeId, false), "value", "label");
				}
			}

			ViewBag.Viewable = IsView;
			ViewBag.VehicleModel = vehicleModel;
			return PartialView("_SecurityVehiclePartialView", model);
		}

		[HttpPost]
		public ActionResult UpdateVehicleStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdateVehicleStatus(Status, Id, User.Identity.Name);
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
		#endregion

		#region Construction Equipment & Industrial Equipment

		public JsonResult GetModelByBrand(string code)
		{
			return Json(_clsAsset.GetModelByBrand(code, false), JsonRequestBehavior.AllowGet);
		}

		public ActionResult ConstructionEquipmentNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL

			var model = new EquipmentModel();

			SelectList brandList = new SelectList(_clsAsset.GetBrand(false), "value", "label");
			ViewBag.EquipmentBrand = brandList;
			SelectList modelList = new SelectList(new List<CommonDropDown>(), "value", "label");
			ViewBag.EquipmentModel = modelList;
			SelectList year = new SelectList(new List<SelectListItem>(), "Value", "Text");
			ViewBag.Years = year;
			ViewBag.Check = "C";
			ViewData["IsNew"] = true;
			return View("EquipmentNew", model);
		}

		[HttpPost]
		public ActionResult InsertConstructionEquipDetails(string json)
		{
			var model = JsonConvert.DeserializeObject<EquipmentModel>(json);
			var result = _clsSecurity.InsertConstructionEquipDetails(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult IndustrialEquipmentNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL

			var model = new EquipmentModel();

			SelectList brandList = new SelectList(_clsAsset.GetBrand(false), "value", "label");
			ViewBag.EquipmentBrand = brandList;
			SelectList modelList = new SelectList(new List<CommonDropDown>(), "value", "label");
			ViewBag.EquipmentModel = modelList;
			SelectList year = new SelectList(new List<SelectListItem>(), "Value", "Text");
			ViewBag.Years = year;
			ViewBag.Check = "I";
			ViewData["IsNew"] = true;
			return View("EquipmentNew", model);
		}

		[HttpPost]
		public ActionResult InsertIndustrialEquipDetails(string json)
		{
			var model = JsonConvert.DeserializeObject<EquipmentModel>(json);
			var result = _clsSecurity.InsertIndustrialEquipDetails(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult ConstructionEquipmentView(string SecurityID)
		{
			var model = new EquipmentModel();

			SelectList brandList = new SelectList(_clsAsset.GetBrand(false), "value", "label");
			ViewBag.EquipmentBrand = brandList;
			SelectList modelList = new SelectList(new List<CommonDropDown>(), "value", "label");
			SelectList year = new SelectList(new List<SelectListItem>(), "Value", "Text");
			ViewBag.Years = year;
			SelectList lstType = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstType;

			if (!string.IsNullOrEmpty(SecurityID))
			{
				model = _clsSecurity.GetEquipmentDetails(SecurityID, true);
				modelList = new SelectList(_clsAsset.GetModelByBrand(model.Brand, false), "value", "label");
			}

			ViewBag.Years = year;
			ViewBag.Check = "C";
			ViewBag.EquipmentModel = modelList;
			ViewBag.Viewable = true;
			return View("Equipmentview", model);
		}

		public ActionResult IndustrialEquipmentView(string SecurityID)
		{
			var model = new EquipmentModel();

			SelectList brandList = new SelectList(_clsAsset.GetBrand(false), "value", "label");
			ViewBag.EquipmentBrand = brandList;
			SelectList modelList = new SelectList(new List<CommonDropDown>(), "value", "label");
			SelectList year = new SelectList(new List<SelectListItem>(), "Value", "Text");
			ViewBag.Years = year;
			SelectList lstType = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstType;

			if (!string.IsNullOrEmpty(SecurityID))
			{
				model = _clsSecurity.GetEquipmentDetails(SecurityID, false);
				modelList = new SelectList(_clsAsset.GetModelByBrand(model.Brand, false), "value", "label");
			}

			ViewBag.EquipmentModel = modelList;
			ViewBag.Check = "I";
			ViewBag.Viewable = true;
			return View("Equipmentview", model);
		}

		public ActionResult ConstructionEquipmentEdit(string SecurityId)
		{
			var model = new EquipmentModel();

			SelectList brandList = new SelectList(_clsAsset.GetBrand(false), "value", "label");
			ViewBag.EquipmentBrand = brandList;
			SelectList modelList = new SelectList(new List<CommonDropDown>(), "value", "label");

			SelectList year = new SelectList(new List<SelectListItem>(), "Value", "Text");
			ViewBag.Years = year;
			SelectList lstType = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstType;

			if (!string.IsNullOrEmpty(SecurityId))
			{
				model = _clsSecurity.GetEquipmentDetails(SecurityId, true);
				modelList = new SelectList(_clsAsset.GetModelByBrand(model.Brand, false), "value", "label");
			}

			ViewBag.Check = "C";
			ViewBag.EquipmentModel = modelList;
			ViewBag.Viewable = false;
			return View("Equipmentview", model);
		}

		public ActionResult IndustrialEquipmentEdit(string SecurityId)
		{
			var model = new EquipmentModel();

			SelectList brandList = new SelectList(_clsAsset.GetBrand(false), "value", "label");
			ViewBag.EquipmentBrand = brandList;
			SelectList modelList = new SelectList(new List<CommonDropDown>(), "value", "label");
			SelectList year = new SelectList(new List<SelectListItem>(), "Value", "Text");
			ViewBag.Years = year;
			SelectList lstType = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstType;

			if (!string.IsNullOrEmpty(SecurityId))
			{
				model = _clsSecurity.GetEquipmentDetails(SecurityId, false);
				modelList = new SelectList(_clsAsset.GetModelByBrand(model.Brand, false), "value", "label");
			}

			ViewBag.EquipmentModel = modelList;
			ViewBag.Check = "I";
			ViewBag.Viewable = false;
			return View("Equipmentview", model);
		}

		[HttpPost]
		public ActionResult UpdateIndustrialEquStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdateIndustrialStatus(Status, Id, User.Identity.Name);
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
		public ActionResult UpdateConstructionEquStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdateConstructionStatus(Status, Id, User.Identity.Name);
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
		#endregion

		#region Inventories
		public ActionResult InventoriesNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL

			var model = new InventoryModel();
			ViewData["IsNew"] = true;
			return View(model);
		}

		[HttpPost]
		public ActionResult InsertInventoryDetails(string json)
		{
			var model = JsonConvert.DeserializeObject<InventoryModel>(json);
			var result = _clsSecurity.InsertInventoryDetails(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult InventoriesView()
		{
			return View();
		}

		public ActionResult InventoriesEdit()
		{
			return View();
		}

		public PartialViewResult GetInventoryDetails(bool IsView, string SecurityId)
		{
			var model = new InventoryModel();

			SelectList lstStatus = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstStatus;

			if (!string.IsNullOrEmpty(SecurityId))
			{
				model = _clsSecurity.GetSecurityInventoryDetails(SecurityId);
			}

			ViewBag.Viewable = IsView;
			return PartialView("_InventoriesPartialView", model);
		}

		[HttpPost]
		public ActionResult UpdateInventoryStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdateInventoryStatus(Status, Id, User.Identity.Name);
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
		#endregion

		#region Receivables
		public ActionResult ReceivablesNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL
			ReceivableModel model = new ReceivableModel();
			ViewData["IsNew"] = true;
			return View(model);
		}

		[HttpPost]
		public ActionResult InsertReceivableDetails(string json)
		{
			var model = JsonConvert.DeserializeObject<ReceivableModel>(json);
			var result = _clsSecurity.InsertReceivableDetails(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult ReceivablesView()
		{
			return View();
		}

		public ActionResult ReceivablesEdit()
		{
			return View();
		}

		public PartialViewResult GetReceivableView(bool IsView, string SecurityID)
		{
			ReceivableModel model = new ReceivableModel();
			SelectList lstStatus = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstStatus;

			if (!string.IsNullOrEmpty(SecurityID))
			{
				model = _clsSecurity.GetDebentureReceivablesDetails(SecurityID);
			}
			ViewBag.Viewable = IsView;

			return PartialView("_ReceivablesPartialView", model);
		}

		[HttpPost]
		public ActionResult UpdateReceivableStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdateReceivableStatus(Status, Id, User.Identity.Name);
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
		#endregion

		#region CashAndEquivalent
		public ActionResult CashAndEquivalentIndividualNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL

			CashEquivalentModel model = new CashEquivalentModel();

			SelectList guaranteeBondsType = new SelectList(_clsGlobal.GetListOfValue("CASH_EQUIVALENT_GUARANTEE_BONDS_TYPE", "", "O", "", ""), "Value", "Text");
			ViewBag.GuaranteeBondsType = guaranteeBondsType;
			SelectList address = new SelectList(new List<AddressViewModel>(), "AddressId", "Address");
			ViewBag.Address = address;
			SelectList departmentList = new SelectList(new List<DepartmentViewModel>(), "cd_ref_num", "cd_dept_desc");
			ViewBag.DepartmentList = departmentList;
			SelectList contact = new SelectList(new List<ContactPersonModel>(), "Value", "Contact");
			ViewBag.Contact = contact;
			ViewData["IsNew"] = true;
			return View(model);
		}

		[HttpPost]
		public ActionResult InsertCashEquivalentIndividualDetail(string json)
		{
			var model = JsonConvert.DeserializeObject<CashEquivalentModel>(json);
			var result = _clsSecurity.InsertCashEquivalentIndividualDetail(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult CashAndEquivalentCompanyNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL

			CashEquivalentModel model = new CashEquivalentModel();

			SelectList guaranteeBondsType = new SelectList(_clsGlobal.GetListOfValue("CASH_EQUIVALENT_GUARANTEE_BONDS_TYPE", "", "O", "", ""), "Value", "Text");
			ViewBag.GuaranteeBondsType = guaranteeBondsType;
			SelectList address = new SelectList(new List<AddressViewModel>(), "AddressId", "Address");
			ViewBag.Address = address;
			SelectList departmentList = new SelectList(new List<DepartmentViewModel>(), "cd_ref_num", "cd_dept_desc");
			ViewBag.DepartmentList = departmentList;
			SelectList contact = new SelectList(new List<ContactPersonModel>(), "Value", "Contact");
			ViewBag.Contact = contact;
			ViewData["IsNew"] = true;
			return View(model);
		}

		[HttpPost]
		public ActionResult InsertCashEquivalentCompanyDetail(string json)
		{
			var model = JsonConvert.DeserializeObject<CashEquivalentModel>(json);
			var result = _clsSecurity.InsertCashEquivalentCompanyDetail(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult CashAndEquivalentIndividualView()
		{
			return View();
		}

		public ActionResult CashAndEquivalentCompanyView()
		{
			return View();
		}

		public ActionResult CashAndEquivalentIndividualEdit()
		{
			return View();
		}

		public ActionResult CashAndEquivalentCompanyEdit()
		{
			return View();
		}

		public PartialViewResult GetCashAndEquivalentIndividual(bool IsView, string SecurityId)
		{
			CashEquivalentModel model = new CashEquivalentModel();

			SelectList guaranteeBondsType = new SelectList(_clsGlobal.GetListOfValue("CASH_EQUIVALENT_GUARANTEE_BONDS_TYPE", "", "O", "", ""), "Value", "Text");
			ViewBag.GuaranteeBondsTypeList = guaranteeBondsType;
			SelectList address = new SelectList(new List<AddressViewModel>(), "AddressId", "Address");
			SelectList departmentList = new SelectList(new List<DepartmentViewModel>(), "cd_ref_num", "cd_dept_desc");
			SelectList contact = new SelectList(new List<ContactPersonModel>(), "Value", "Contact");
			SelectList lstStatus = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstStatus;

			if (!string.IsNullOrEmpty(SecurityId))
			{
				model = _clsSecurity.GetCashAndEquivalentIndividualDetails(SecurityId);
				if (model != null)
				{
					if (model.BillToModel != null)
					{
						address = new SelectList(_clsCRM.getAddress(model.BillToModel.Customer, clsVariables.Individual), "AddressId", "Address");
						departmentList = new SelectList(_clsCRM.getDepartmentList(model.BillToModel.Address), "cd_ref_num", "cd_dept_desc");
						contact = new SelectList(_clsCRM.getContactPerson(model.BillToModel.Department), "Value", "Contact");
						model.BillToModel.NricFinPassport = _clsCRM.getNricFinPassportType(model.BillToModel.Customer);
					}
				}
			}

			ViewBag.AddressList = address;
			ViewBag.DepartmentList = departmentList;
			ViewBag.Contact = contact;
			ViewBag.Viewable = IsView;

			return PartialView("_CashAndEquivalentIndividualPartial", model);
		}

		public PartialViewResult GetCashAndEquivalentCompany(bool IsView, string SecurityId)
		{
			CashEquivalentModel model = new CashEquivalentModel();

			SelectList guaranteeBondsType = new SelectList(_clsGlobal.GetListOfValue("CASH_EQUIVALENT_GUARANTEE_BONDS_TYPE", "", "O", "", ""), "Value", "Text");
			ViewBag.GuaranteeBondsTypeList = guaranteeBondsType;
			SelectList address = new SelectList(new List<AddressViewModel>(), "AddressId", "Address");
			SelectList departmentList = new SelectList(new List<DepartmentViewModel>(), "cd_ref_num", "cd_dept_desc");
			SelectList contact = new SelectList(new List<ContactPersonModel>(), "Value", "Contact");
			SelectList lstStatus = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstStatus;

			if (!string.IsNullOrEmpty(SecurityId))
			{
				model = _clsSecurity.GetCashAndEquivalentCompanyDetails(SecurityId);
				if (model != null)
				{
					if (model.BillToModel != null)
					{
						address = new SelectList(_clsCRM.getAddress(model.BillToModel.Customer, "Customer"), "AddressId", "Address");
						departmentList = new SelectList(_clsCRM.getDepartmentList(model.BillToModel.Address), "cd_ref_num", "cd_dept_desc");
						contact = new SelectList(_clsCRM.getContactPerson(model.BillToModel.Department), "Value", "Contact");
						model.BillToModel.ROCUEN = _clsCRM.getRocUenType(model.BillToModel.Customer);
					}
				}
			}
			ViewBag.AddressList = address;
			ViewBag.DepartmentList = departmentList;
			ViewBag.Contact = contact;
			ViewBag.Viewable = IsView;
			return PartialView("_CashAndEquivalentCompanyPartial", model);
		}

		[HttpPost]
		public ActionResult UpdateCashAndEquivalentIndStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdateCashAndEquivalentIndStatus(Status, Id, User.Identity.Name);
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
		public ActionResult UpdateCashAndEquivalentComStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdateCashAndEquivalentComStatus(Status, Id, User.Identity.Name);
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

		#endregion

		#region Securities/ Financial Instrument
		public ActionResult SecuritiesOrFinancialInstrumentsNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL
			var model = new SecFinInstrumentModel();

            SelectList financialInstrumentType = new SelectList(_clsGlobal.GetListOfValue("SECURITIES_FINANCIAL_INSTRUMENT_TYPE", "", "O", "", ""), "Value", "Text");
			ViewBag.FinancialInstrumentType = financialInstrumentType;
			ViewData["IsNew"] = true;
			return View(model);
		}

		[HttpPost]
		public ActionResult InsertSecFinInstruments(string json)
		{
			var model = JsonConvert.DeserializeObject<SecFinInstrumentModel>(json);
			var result = _clsSecurity.InsertSecFinInstruments(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult SecuritiesOrFinancialInstrumentsView()
		{
			return View();
		}

		public ActionResult SecuritiesOrFinancialInstrumentsEdit()
		{
			return View();
		}

		public PartialViewResult GetSecuritiesOrFinancialInstruments(bool IsView, string SecurityId)
		{
			var model = new SecFinInstrumentModel();

            SelectList financialInstrumentType = new SelectList(_clsGlobal.GetListOfValue("SECURITIES_FINANCIAL_INSTRUMENT_TYPE", "", "O", "", ""), "Value", "Text");
			ViewBag.FinancialInstruments = financialInstrumentType;
			SelectList lstType = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstType;

			if (!string.IsNullOrEmpty(SecurityId))
			{
				model = _clsSecurity.GetSecuritiesOrFinancialInstruments(SecurityId);
			}

			ViewBag.Viewable = IsView;
			return PartialView("_SecuritiesOrFinancialInstrumentsPartial", model);
		}

		[HttpPost]
		public ActionResult UpdateSecFinInstrumentsStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdateSecFinInstrumentsStatus(Status, Id, User.Identity.Name);
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
		#endregion

		#region Security Deposit
		public ActionResult SecurityDepositNew(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL
			var model = new SecurityDepositModel();

			SelectList address = new SelectList(new List<AddressViewModel>(), "AddressId", "Address");
			ViewBag.Address = address;
			SelectList departmentList = new SelectList(new List<DepartmentViewModel>(), "cd_ref_num", "cd_dept_desc");
			ViewBag.DepartmentList = departmentList;
			SelectList contact = new SelectList(new List<ContactPersonModel>(), "Value", "Contact");
			ViewBag.Contact = contact;
			ViewData["IsNew"] = true;
			return View(model);
		}

		[HttpPost]
		public ActionResult InsertSecurityDepositDetail(string json)
		{
			var model = JsonConvert.DeserializeObject<SecurityDepositModel>(json);
			var result = _clsSecurity.InsertSecurityDepositDetail(model, User.Identity.Name);
			var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public ActionResult SecurityDepositView()
		{
			return View();
		}

		public ActionResult SecurityDepositEdit()
		{
			return View();
		}

		public PartialViewResult GetSecurityDepositView(bool IsView, string SecurityID)
		{
			var model = new SecurityDepositModel();
			SelectList address = new SelectList(new List<AddressViewModel>(), "AddressId", "Address");
			SelectList departmentList = new SelectList(new List<DepartmentViewModel>(), "cd_ref_num", "cd_dept_desc");
			SelectList contact = new SelectList(new List<ContactPersonModel>(), "Value", "Contact");
			SelectList lstType = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewBag.StatusList = lstType;

			if (!string.IsNullOrEmpty(SecurityID))
			{
				model = _clsSecurity.GetSecurityDepositDetails(SecurityID);
			}
			if (model != null)
			{
				address = new SelectList(_clsCRM.getAddress(model.BillToDetailModel.Customer, model.BillToDetailModel.IndividualCorporate), "AddressId", "Address");
				departmentList = new SelectList(_clsCRM.getDepartmentList(model.BillToDetailModel.Address), "cd_ref_num", "cd_dept_desc");
				contact = new SelectList(_clsCRM.getContactPerson(model.BillToDetailModel.Department), "Value", "Contact");
				if (model.BillToDetailModel.IndividualCorporate == clsVariables.Individual)
				{
					model.BillToDetailModel.NRICFINPASSPORT = _clsCRM.getNricFinPassportType(model.BillToDetailModel.Customer);
				}
				else
				{
					model.BillToDetailModel.ROCUEN = _clsCRM.getRocUenType(model.BillToDetailModel.Customer);
				}

			}
			ViewBag.Address = address;
			ViewBag.DepartmentList = departmentList;
			ViewBag.Contact = contact;
			ViewBag.Viewable = IsView;
			return PartialView("_SecurityDepositPartialView", model);
		}

		[HttpPost]
		public ActionResult UpdateSecurityDepositStatus(string Status, string Id)
		{
			if (ModelState.IsValid)
			{
				var result = _clsSecurity.UpdateSecurityDepositStatus(Status, Id, User.Identity.Name);
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

		#endregion

		#region Security Master Enquiry
		public ActionResult SecurityMasterEnquiry(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL
			System.Web.HttpContext.Current.Session.Add("SubMenuId", SubMenuId);
			return View();
		}

		public PartialViewResult SecurityMasterEnquirySearchView(string ViewType)
		{
			List<SelectListItem> lstSecurityTypeLevel1 = _clsGlobal.GetListOfValue("SECURITY_LIST_LEVEL_1", "", "O", "", "");
			ViewData["SecurityTypeLevel1"] = lstSecurityTypeLevel1;

			var lstSecurityTypeLevel2 = _clsGlobal.GetListOfValue("SECURITY_LIST_LEVEL_2", "", "O", "", "");
			ViewData["SecurityTypeLevel2"] = lstSecurityTypeLevel2;

			var lstSecurityItemStatus = new SelectList(_clsGlobal.GetListOfValue("SECURITY_ITEM_STATUS", "", "O", "", ""), "Value", "Text");
			ViewData["SecurityItemStatus"] = lstSecurityItemStatus;

			var lstContractsStatus = new SelectList(_clsGlobal.GetListOfValue("CONTRACT_STATUS", "", "O", "", ""), "Value", "Text");
			ViewData["ContractsStatus"] = lstContractsStatus;

			ViewBag.ViewType = ViewType;
			return PartialView("_SecurityMasterEnquirySearchView");
		}

		public PartialViewResult SecurityMasterEnquiryListView(SecurityMasterInqParam Param, string ViewType)
		{
			string groupType = ViewType == "ViewAndUpdate" ? "SECURITY_MASTER_EDIT" : "SECURITY_MASTER_VIEW";

			var securityInquiryList = _clsSecurity.GetSecurityEnquiryList(Param, groupType);

			SecurityMasterInqListViewModel vm = new SecurityMasterInqListViewModel();
			vm.Action = ViewType;

			vm.SecurityMasterInqList.AddRange(securityInquiryList);

			return PartialView("_SecurityMasterEnquiryListView", vm);
		}

		public PartialViewResult SecurityContractsListView(string securityID, string securityListLevel2)
		{
			var securityContractList = _clsSecurity.GetContractOfSecurityItem(securityID, securityListLevel2);

			List<SecurityContractViewModel> vm = new List<SecurityContractViewModel>();

			vm.AddRange(securityContractList);

			return PartialView("_SecurityContractsListView", vm);
		}

		public List<CommonDropDown> GetCustomerIndividual(string textFilter, string IndividualCorporate)
		{
			var res = _clsCRM.GetCRMCustomer(IndividualCorporate, textFilter);
			return res.ToList();
		}

		public JsonResult GetSecurityTypeLevel2ByParentId(string parentId)
		{
			var lstSecurityTypeLevel2 = _clsGlobal.GetListOfValue("SECURITY_LIST_LEVEL_2", parentId, "O", "", "");
			lstSecurityTypeLevel2.Insert(0, new SelectListItem { Text = "--Select--", Value = "" });

			return Json(lstSecurityTypeLevel2);
		}

		public ActionResult GetPageUrlByLevel2Code(string level2Code, string ViewOrUpdate)
		{
			var redirectPageModel = _clsSecurity.GetPageURlBySecurityListLevel2Code(level2Code, ViewOrUpdate, "O");
			return Json(redirectPageModel);
		}
		#endregion

		#region Security Master Modification
		public ActionResult SecurityMasterModification(string SubMenuId)
		{
			#region Check UserGroup when user direct key in URL
			if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
			{
				ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
			}
			#endregion  Check UserGroup when user direct key in URL
			System.Web.HttpContext.Current.Session.Add("SubMenuIdEdit", SubMenuId);
			return View();
		}
		#endregion
	}
}