using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthozCapital.Models
{
    public class GlobalModel
    {
        public string pstrMenu { get; set; }
    }

    public class SysAutoGenerateReturn
    {
        //public int MasterID { get; set; }
        public string MasterCode { get; set; }
        public int ID { get; set; }
        public string LastNumber { get; set; }
        public string YearCheck { get; set; }
        public string MonthCheck { get; set; }
        public string NewId { get; set; }
    }

    //For autocomplete drop down
    public class CommonDropDown
    {
        public string value { get; set; }
        public string label { get; set; }
    }

    public class ResultViewModel
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }

    #region CRM
    public class AddressViewModel
    {
        public string Address { get; set; }
        public string AddressId { get; set; }
        public string am_add_ind { get; set; }
        public string am_bill_ind { get; set; }
    }

    public class DepartmentViewModel
    {
        public string cd_ref_num { get; set; }
        public string cd_dept_desc { get; set; }
    }

    public class ContactPersonModel
    {
        public string Contact { get; set; }
        public string Value { get; set; }
    }
	#endregion

	#region Security
	public class RedirectToPageModel
	{
		public string ControllerName { get; set; }
		public string ActionName { get; set; }
		public string URL { get; set; }
		public int SubMenuId { get; set; }
		public string PageURLCode { get; set; }
	}

	public enum SecurityMortagorTable{
		Security_PropertyMortgagor,
		Security_VesselMortgagor
	}

	public enum SecurityCustomer
	{
		Security_PropertyCustomer,
		Security_VesselCustomer,
		Security_VehicleCustomer,
		Security_ConstructionEquipCustomer,
		Security_IndustrialEquipCustomer,
		Security_InventoryCustomer,
		Security_ReceivableCustomer,
		Security_CashEquivalentIndCustomer,
		Security_CashEquivalentComCustomer,
		Security_SecFinInstrumentsCustomer,
		Security_SecDepositCustomer
	}
	#endregion

	public class EmailViewModel
	{
		public string MailType { get; set; }
		public string EmailTo { get; set; }
		public string EmailFrom { get; set; }
		public string CcEmail { get; set; }
		public string Subject { get; set; }
		public string body { get; set; }
		public string UserId { get; set; }
	}
}