using System;
using System.Collections.Generic;
using System.Data;
using EthozCapital.CustomLibraries;
using EthozCapital.Data;
using EthozCapital.Models;
using EthozCapital.Models.ViewModels;
using log4net;
using Newtonsoft.Json;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using EthozCapital.AlertListWS;
using System.Data.SqlClient;

namespace EthozCapital.CustomLibraries
{
	public class clsCRM
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(clsCRM));
		public static AlertListWS.AlertListWS ALWS = new AlertListWS.AlertListWS();
		//ECA EN002
		public static string enECA = "EN002";
		//Customer PT001
		public static string ptCustomer = "PT001";
		//Vendor PT002
		public static string ptVendor = "PT002";
		//Guarantor PT003
		public static string ptGuarantor = "PT003";
		//Lawyer PT004
		public static string ptLawyer = "PT004";
		//Spotter PT005
		public static string ptSpotter = "PT005";
		//Insurance Company PT006
		public static string ptInsuranceCompany = "PT006";
		//Security Giver PT007
		public static string ptSecurityGiver = "PT007";

		public enum SupplierStatus : int
		{
			Ok = 1,
			AlertList = 2,
			Exception = 3
		}

		public clsCRM()
		{
		}

		#region Alert List
		public string CheckAlertListFromCRM(string clientCode, ref int status)
		{
			string charSupp = GetCusAlertStatus("OCA", clientCode, out status);
			if (charSupp == "W" || charSupp == "N")
			{
				status = 0;
				string reason = GetCusAlertReason("OCA", clientCode, out status);
				return reason;
			}
			else
			{
				return charSupp;
			}
		}

		public static string GetCusAlertStatus(string transCode, string clientCode, out int status)
		{
			try
			{
				entAlertUtil eAU = new entAlertUtil();
				eAU.TransCode = transCode;
				eAU.ClientCode = clientCode;
				System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)192 | (System.Net.SecurityProtocolType)768 | (System.Net.SecurityProtocolType)3072;
				System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
				//var result = ALWS.CheckCusAlertList(EthozCapital.CustomLibraries.clsGlobal.GetConnectionOrixDB(), eAU);
				var result = ALWS.CheckCusAlertList("Data Source=dbsvrd;Initial Catalog=Orix_DB_Dev;Integrated Security=False;user id=mis_master;password=obivan;", eAU);
				status = (int)SupplierStatus.Ok;
				return result;
			}
			catch (Exception ex)
			{
				status = (int)SupplierStatus.Exception;
				return ex.Message;
			}
		}

		public static string GetCusAlertReason(string transCode, string clientCode, out int status)
		{
			try
			{
				entAlertUtil eAU = new entAlertUtil();
				eAU.TransCode = transCode;
				eAU.ClientCode = clientCode;
				System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)192 | (System.Net.SecurityProtocolType)768 | (System.Net.SecurityProtocolType)3072;
				System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
				//var result = ALWS.GetClientAlertReason(EthozCapital.CustomLibraries.clsGlobal.GetConnectionOrixDB(), eAU);                
				var result = ALWS.GetClientAlertReason("Data Source=dbsvrd;Initial Catalog=Orix_DB_Dev;Integrated Security=False;user id=mis_master;password=obivan;", eAU);
				status = (int)SupplierStatus.AlertList;
				return result;
			}
			catch (Exception ex)
			{
				status = (int)SupplierStatus.Exception;
				return ex.Message;
			}
		}
		#endregion

		#region CRM Client Data (Customer/Vendor/Guarantor/Lawyer/Spotter/InsuranceCompany/SecurityGiver)

		public List<CommonDropDown> GetCRMCustomer(string IndividualCorporate, string textFilter = null)
		{
			var data = GetCRMProfileByTypeEntity(ptCustomer, enECA, IndividualCorporate, textFilter);
			return data;
		}

		public List<CommonDropDown> GetCRMVendor(string IndividualCorporate, string textFilter = null)
		{
			var data = GetCRMProfileByTypeEntity(ptVendor, enECA, IndividualCorporate, textFilter);
			return data;
		}

		public List<CommonDropDown> GetCRMGuarantor(string IndividualCorporate, string textFilter = null)
		{
			var data = GetCRMProfileByTypeEntity(ptGuarantor, enECA, IndividualCorporate, textFilter);
			return data;
		}

		public List<CommonDropDown> GetCRMLawyer(string IndividualCorporate, string textFilter = null)
		{
			var data = GetCRMProfileByTypeEntity(ptLawyer, enECA, IndividualCorporate, textFilter);
			return data;
		}

		public List<CommonDropDown> GetCRMSpotter(string IndividualCorporate, string textFilter = null)
		{
			var data = GetCRMProfileByTypeEntity(ptSpotter, enECA, IndividualCorporate, textFilter);
			return data;
		}

		public List<CommonDropDown> GetCRMInsuranceCompany(string IndividualCorporate, string textFilter = null)
		{
			var data = GetCRMProfileByTypeEntity(ptInsuranceCompany, enECA, IndividualCorporate, textFilter);
			return data;
		}

		public List<CommonDropDown> GetCRMSecurityGiver(string IndividualCorporate, string textFilter = null)
		{
			var data = GetCRMProfileByTypeEntity(ptSecurityGiver, enECA, IndividualCorporate, textFilter);
			return data;
		}

		//Amended By Jason 03/07/2019
        public List<CommonDropDown> GetCRMProfileByTypeEntity(string profileType, string entityType, string IndividualCorporate, string textFilter = null)
        {
            using (var db = new OrixDBEntities())
            {
                var data = from profile in db.crmtb_client_entity_profile
                           join profileC in db.crmtb_client_mas on profile.ep_client_cod equals profileC.cm_client_cod
                           join profileD in db.crmtb_client_id_mas on profileC.cm_client_cod equals profileD.im_client_cod
                           where profile.ep_profiletype_cod == profileType && profile.ep_entity_cod == entityType
                           && profileC.cm_sta_ind == "O" &&
                           (string.IsNullOrEmpty(textFilter) ? true : profileC.cm_client_nam.StartsWith(textFilter)) &&
                           (IndividualCorporate == clsVariables.Individual ? profileC.cm_typ_cod == "C0001" :
                           IndividualCorporate == clsVariables.Corporate ? new string[] { "C0002", "C0003" }.Contains(profileC.cm_typ_cod) :
                           new string[] { "C0001", "C0002", "C0003" }.Contains(profileC.cm_typ_cod)
                   )
                           select new { profileC.cm_client_nam, profileC.cm_client_cod, profileD.im_id_num };

                List<CommonDropDown> result = new List<CommonDropDown>();

                result = data.Select(x => new CommonDropDown()
                {
                    label = x.cm_client_nam.Replace("\"", "&#34").Replace("\'", "&#39").Replace("<", "&#12296").Replace(">", "&#12297") + " (" + x.im_id_num + ")",
                    value = x.cm_client_cod                    
                }).ToList();
                return result;
            }
        }

		public string GetCRMProfileByClientId(string Id)
		{
			using (var db = new OrixDBEntities())
			{
				var data = db.crmtb_client_mas.AsNoTracking().Where(w => w.cm_sta_ind == "O"
				 && w.cm_client_cod == Id 
				 ).OrderBy(o => o.cm_client_nam).ToList();

				var result = string.Empty;

				if (data != null)
				{
					result = data.Select(s=>s.cm_client_nam).FirstOrDefault().ToString();
				}
				
				return result;
			}
		}
		//temp
		public IEnumerable<CommonDropDown> GetSupplierFromCRM()
		{
			using (var db = new OrixDBEntities())
			{
				var supplier = db.crmtb_client_entity_profile.AsNoTracking().Where(f => f.ep_sta_ind == "O" && f.ep_profiletype_cod == "PT002" && f.ep_entity_cod == "EN002"
				 && f.crmtb_client_mas.cm_sta_ind == "O").OrderBy(f => f.crmtb_client_mas.cm_client_nam).Select(f => new CommonDropDown()
				 {
					 value = f.ep_client_cod,
					 label = f.crmtb_client_mas.cm_client_nam.Replace("\"", "&#34").Replace("\'", "&#39").Replace("<", "&#12296").Replace(">", "&#12297")
				 }).ToList();
				return supplier;
			}
		}

		public string getNricFinPassportType(string selected)
		{
			glog.Debug("getNricFinPassportType: Entry");
			string NricType = null;
			using (var db = new OrixDBEntities())
			{
				try
				{
					IEnumerable<crmtb_client_id_mas> model = db.crmtb_client_id_mas.Where(x =>
						 x.crmtb_client_mas.cm_client_cod == selected &&
						 x.im_sta_ind == "O" &&
				  (x.im_id_typ == "NRIC" || x.im_id_typ == "FIN" || x.im_id_typ == "PASSPORT")).ToList();
					if (model != null)
					{
						if (model.Any(x => x.im_id_typ == "NRIC"))
							NricType = model.Where(x => x.im_id_typ == "NRIC").Select(x => x.im_id_num).FirstOrDefault();
						else if (model.Any(x => x.im_id_typ == "FIN" || x.im_id_typ == "PASSPORT"))
							NricType = model.Where(x => x.im_id_typ == "FIN").Select(x => x.im_id_num).FirstOrDefault();
						else
							NricType = model.Where(x => x.im_id_typ == "PASSPORT").Select(x => x.im_id_num).FirstOrDefault();
					}
					else
						NricType = "";
					glog.Debug("getNricFinPassportType: Exit");
					return NricType;
				}
				catch (Exception ex)
				{
					NricType = string.Empty;
					glog.Error("getNricFinPassportType Exception: " + ex.Message);
					return NricType;
				}
			}
		}

		public List<AddressViewModel> getAddress(string selected, string IndividualCorporate)
		{
			glog.Debug("getAddress: Entry");
			using (var db = new OrixDBEntities())
			{
				try
				{
					var address = db.crmtb_client_address_mas.Where(x => x.am_ref_cod == x.crmtb_client_mas.cm_client_cod &&
				  x.crmtb_client_mas.cm_client_cod == selected &&
				  x.am_sta_ind == "O"
				  ).ToList();

                    //var sortAddress = (IndividualCorporate == "Corporate" ? address.OrderBy(o => o.am_add_ind == "R") : address.OrderBy(o => o.am_bill_ind == "Y")).Select(x => new AddressViewModel()
                    var sortAddress = (IndividualCorporate == clsVariables.Corporate ? 
                        address.Where(x => x.am_add_ind == "R").Concat(address.Where(x => x.am_add_ind != "R")).ToList() :
                        address = address.Where(x => x.am_bill_ind == "Y").Concat(address.Where(x => x.am_bill_ind != "Y")).ToList())
                        .Select(x => new AddressViewModel()
					{
						am_add_ind = x.am_add_ind,
						am_bill_ind = x.am_bill_ind,
						AddressId = x.am_add_cod,
						Address = (x.am_blk_typ == "BLK" ? (x.am_blk_typ + " ") : string.Empty) +
											  (!string.IsNullOrEmpty(x.am_blk_num) ? (x.am_blk_num + " ") : string.Empty) +
											  (!string.IsNullOrEmpty(x.am_street_nam) ? (x.am_street_nam + " ") : string.Empty) +
											  (!string.IsNullOrEmpty(x.am_floor_num) ? ("#" + x.am_floor_num + " ") : " ") +
											  (!string.IsNullOrEmpty(x.am_unit_num) ? ("-" + x.am_unit_num + " ") : " ") +
											  (!string.IsNullOrEmpty(x.am_build_nam) ? x.am_build_nam : string.Empty) +
											  (!string.IsNullOrEmpty(x.am_postal_cod) ? ("," + x.am_postal_cod + " ") : string.Empty) +
											  (!string.IsNullOrEmpty(x.am_country_cod) ? (db.gentb_country_mas.Where(c => c.cm_country_cod == x.am_country_cod && c.cm_sta_ind == "O").Select(c => c.cm_country_nam).FirstOrDefault()) : string.Empty)
					}).ToList();
					glog.Debug("getAddress: Exit");
					return sortAddress;
				}
				catch (Exception ex)
				{
					glog.Error("getAddress Exception: " + ex.Message);
					return new List<AddressViewModel>();
				}
			}
		}
		
		public string getRocUenType(string selected)
		{
			glog.Debug("getRocUenType: Entry");
			string rocUenType = string.Empty;
			using (var db = new OrixDBEntities())
			{
				try
				{
					IEnumerable<crmtb_client_id_mas> model = db.crmtb_client_id_mas.Where(x =>
						x.im_client_cod == selected &&
						x.im_sta_ind == "O" && (x.im_id_typ == "ROC" || x.im_id_typ == "UEN")).ToList();
                    if (model.Any())
					{
						if (model.Any(x => x.im_id_typ == "ROC"))
							rocUenType = model.Where(x => x.im_id_typ == "ROC").Select(x => x.im_id_num).FirstOrDefault();
						else
							rocUenType = model.Where(x => x.im_id_typ == "UEN").Select(x => x.im_id_num).FirstOrDefault();
					}
					else
						rocUenType = "";
					glog.Debug("getRocUenType: Exit");
					return rocUenType;
				}
				catch (Exception ex)
				{
					rocUenType = string.Empty;
					glog.Error("getRocUenType Exception: " + ex.Message);
					return rocUenType;
				}
			}
		}

		public List<DepartmentViewModel> getDepartmentList(string selectedAddress)
		{
			glog.Debug("getDepartmentList: Entry");
			using (var db = new OrixDBEntities())
			{
				try
				{
					var departmentList = db.crmtb_client_dept.Where(x =>
					x.cd_add_cod == x.crmtb_client_address_mas.am_add_cod &&
				   x.crmtb_client_address_mas.am_add_cod == selectedAddress &&
				   x.cd_sta_ind == "O").ToList().OrderBy(x => x.cd_ref_num);
					List<DepartmentViewModel> deptModel = new List<DepartmentViewModel>();
					deptModel = departmentList.Select(x => new DepartmentViewModel()
					{
						cd_ref_num = x.cd_ref_num,
						cd_dept_desc = x.cd_dept_desc
					}).ToList();
					glog.Debug("getDepartmentList: Exit");
					return deptModel;
				}
				catch (Exception ex)
				{
					glog.Error("getDepartmentList Exception: " + ex.Message);
					return new List<DepartmentViewModel>();
				}
			}
		}

		public List<ContactPersonModel> getContactPerson(string selectedDepartment)
		{
			glog.Debug("getContactPerson: Entry");
			using (var db = new OrixDBEntities())
			{
				try
				{
					var contactPerson = db.crmtb_client_contact_mas.Where(x => x.cm_ref_cod == x.crmtb_client_dept.cd_ref_num &&
				 x.crmtb_client_dept.cd_ref_num == selectedDepartment &&
				 x.cm_sta_ind == "O").OrderBy(x => x.cm_con_typ == "M").Select(x => new ContactPersonModel()
				 {
					 Contact = (!string.IsNullOrEmpty(x.cm_title) ? (x.cm_title + " ") : string.Empty) + x.cm_full_nam,
					 Value = x.cm_con_cod
				 }).ToList();
					glog.Debug("getContactPerson: Exit");
					return contactPerson;
				}
				catch (Exception ex)
				{
					glog.Error("getContactPerson Exception: " + ex.Message);
					return new List<ContactPersonModel>();
				}
			}
		}

        public string getContactMobile(string selectedContactPerson)
        {
            glog.Debug("getContactMobile: Entry");
            using (var db = new OrixDBEntities())
            {
                try
                {
                    var contactMobileList = db.crmtb_client_contact_det.Where(x => x.cd_con_cod == selectedContactPerson &&
                 x.cd_sta_ind == "O" && x.cd_typ_cod == "MOBILE").Select(x =>
                      x.cd_typ_val
                 );
                    var contactMobile = string.Join(";", contactMobileList);
                    glog.Debug("getContactMobile: Exit");
                    return contactMobile;
                }
                catch (Exception ex)
                {
                    glog.Error("getContactMobile Exception: " + ex.Message);
                    return "";
                }
            }
        }
        public string getContactEmail(string selectedContactPerson)
        {
            glog.Debug("getContactEmail: Entry");
            using (var db = new OrixDBEntities())
            {
                try
                {
                    var contactEmailList = db.crmtb_client_contact_det.Where(x => x.cd_con_cod == selectedContactPerson &&
                 x.cd_sta_ind == "O" && x.cd_typ_cod == "EMAIL").Select(x =>
                      x.cd_typ_val
                 );
                    var contactEmail = string.Join(";", contactEmailList);
                    glog.Debug("getContactEmail: Exit");
                    return contactEmail;
                }
                catch (Exception ex)
                {
                    glog.Error("getContactEmail Exception: " + ex.Message);
                    return "";
                }
            }
        }
        public string getContactOffice(string selectedContactPerson)
        {
            glog.Debug("getContactOffice: Entry");
            using (var db = new OrixDBEntities())
            {
                try
                {
                    var contactOfficeList = db.crmtb_client_contact_det.Where(x => x.cd_con_cod == selectedContactPerson &&
                 x.cd_sta_ind == "O" && x.cd_typ_cod == "OFFICE").Select(x =>
                      x.cd_typ_val
                 );
                    var contactOffice = string.Join(";", contactOfficeList);
                    glog.Debug("getContactOffice: Exit");
                    return contactOffice;
                }
                catch (Exception ex)
                {
                    glog.Error("getContactOffice Exception: " + ex.Message);
                    return "";
                }
            }
        }
        public string getContactHome(string selectedContactPerson)
        {
            glog.Debug("getContactHome: Entry");
            using (var db = new OrixDBEntities())
            {
                try
                {
                    var contactHomeList = db.crmtb_client_contact_det.Where(x => x.cd_con_cod == selectedContactPerson &&
                 x.cd_sta_ind == "O" && x.cd_typ_cod == "HOME").Select(x =>
                      x.cd_typ_val
                 );
                    var contactHome = string.Join(";", contactHomeList);
                    glog.Debug("getContactHome: Exit");
                    return contactHome;
                }
                catch (Exception ex)
                {
                    glog.Error("getContactHome Exception: " + ex.Message);
                    return "";
                }
            }
        }
        public string getContactFax(string selectedContactPerson)
        {
            glog.Debug("getContactFax: Entry");
            using (var db = new OrixDBEntities())
            {
                try
                {
                    var contactFaxList = db.crmtb_client_contact_det.Where(x => x.cd_con_cod == selectedContactPerson &&
                 x.cd_sta_ind == "O" && x.cd_typ_cod == "FAX").Select(x =>
                      x.cd_typ_val
                 );
                    var contactFax = string.Join(";", contactFaxList);
                    glog.Debug("getContactFax: Exit");
                    return contactFax;
                }
                catch (Exception ex)
                {
                    glog.Error("getContactFax Exception: " + ex.Message);
                    return "";
                }
            }
        }
        public string getContactPager(string selectedContactPerson)
        {
            glog.Debug("getContactPager: Entry");
            using (var db = new OrixDBEntities())
            {
                try
                {
                    var contactPagerList = db.crmtb_client_contact_det.Where(x => x.cd_con_cod == selectedContactPerson &&
                 x.cd_sta_ind == "O" && x.cd_typ_cod == "PAGER").Select(x =>
                      x.cd_typ_val
                 );
                    var contactPager = string.Join(";", contactPagerList);
                    glog.Debug("getContactPager: Exit");
                    return contactPager;
                }
                catch (Exception ex)
                {
                    glog.Error("getContactPager Exception: " + ex.Message);
                    return "";
                }
            }
        }
        
        public string getClientNameByCode(string code)
        {
            glog.Debug("getClientNameByCode: Entry");
            using (var db = new OrixDBEntities())
            {
                try
                {
                    var clientname = db.crmtb_client_mas.Where(x => x.cm_sta_ind == "O" &&
                        x.cm_client_cod == code).Select(x => x.cm_client_nam).FirstOrDefault();

                    glog.Debug("getClientNameByCode: Exit");
                    return clientname;
                }
                catch (Exception ex)
                {
                    glog.Error("getClientNameByCode Exception: " + ex.Message);
                    return "";
                }
            }
        }
		#endregion

		#region Printing Report

		#region Get Address By Customer Address
		public AddressViewModel getAddressByCustomerAddress(string strCustomerAddress, string strCustomerType)
		{
			glog.Debug("getAddressByCustomerAddress: Entry");
			using (var db = new OrixDBEntities())
			{
				try
				{
					var address = db.crmtb_client_address_mas.Where(x => x.am_add_cod == strCustomerAddress &&
				  x.am_sta_ind == "O").ToList();

					var sortAddress = (strCustomerType == clsVariables.Corporate ?
						address.Where(x => x.am_add_ind == "R").Concat(address.Where(x => x.am_add_ind != "R")).ToList() :
						address = address.Where(x => x.am_bill_ind == "Y").Concat(address.Where(x => x.am_bill_ind != "Y")).ToList())
						.Select(x => new AddressViewModel()
						{
							am_add_ind = x.am_add_ind,
							am_bill_ind = x.am_bill_ind,
							AddressId = x.am_add_cod,
							Address = (x.am_blk_typ == "BLK" ? (x.am_blk_typ + " ") : string.Empty) +
											  (!string.IsNullOrEmpty(x.am_blk_num) ? (x.am_blk_num + " ") : string.Empty) +
											  (!string.IsNullOrEmpty(x.am_street_nam) ? (x.am_street_nam + " ") : string.Empty) +
											  (!string.IsNullOrEmpty(x.am_floor_num) ? ("#" + x.am_floor_num + " ") : " ") +
											  (!string.IsNullOrEmpty(x.am_unit_num) ? ("-" + x.am_unit_num + " ") : " ") +
											  (!string.IsNullOrEmpty(x.am_build_nam) ? x.am_build_nam : string.Empty) +
											  (!string.IsNullOrEmpty(x.am_postal_cod) ? ("," + x.am_postal_cod + " ") : string.Empty) +
											  (!string.IsNullOrEmpty(x.am_country_cod) ? (db.gentb_country_mas.Where(c => c.cm_country_cod == x.am_country_cod && c.cm_sta_ind == "O").Select(c => c.cm_country_nam).FirstOrDefault()) : string.Empty)
						}).FirstOrDefault();
					glog.Debug("getAddress: Exit");
					return sortAddress;
				}
				catch (Exception ex)
				{
					glog.Error("getAddress Exception: " + ex.Message);
					return new AddressViewModel();
				}
			}

		}
		#endregion

		#region Get Client Contact Mas By CustomerConPerson
		public ClientContactMasViewModel GetClientContactMasByCustomerConPerson(string strCustomerConPerson)
		{
			glog.Debug("GetClientContactMasByCustomerConPerson: Entry");
			using (var db = new OrixDBEntities())
			{
				try
				{
					var result = db.Database.SqlQuery<ClientContactMasViewModel>(
						   "exec GetCrmtbClientContactMasByCustomerConPerson @CustomerConPerson",
						   new SqlParameter("@CustomerConPerson", string.IsNullOrWhiteSpace(strCustomerConPerson) ? "" : strCustomerConPerson)).FirstOrDefault();
					glog.Debug("GetClientContactMasByCustomerConPerson: Exit");
					return result;
				}
				catch (Exception ex)
				{
					glog.Error("GetClientContactMasByCustomerConPerson Exception: " + ex.Message);
					throw;
				}
			}
		}
		#endregion

		#endregion

	}
}