using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EthozCapital.Models.ViewModels
{
	public class PreConViewModel
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(PreConViewModel));

		[DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]

		#region Main
		public string strCtrType { get; set; }
		public string strSubCtrType { get; set; }
		[Required]
		public string strProd { get; set; }
		[Required]
		public string strSubProd { get; set; }
		public string strSalesEx { get; set; }
		public string InsOption { get; set; }
		public string PaymentOption { get; set; }
		public string RateOption { get; set; }
		public string CreationDate { get; set; }
		public string LODate { get; set; }
		public string OfferTillDate { get; set; }
		public string AgreementDate { get; set; }
		public bool GSTCheck { get; set; }
		public string GSTPercentage { get; set; }
		public decimal GSTPer { get; set; }
		public string Comm { get; set; }
		[MinLength(1, ErrorMessage = "Value must be greater than 0")]
		public int FontSize { get; set; }
		#endregion

		#region Instalment
		public int PeriodofLease { get; set; }
		public int FreqofInst { get; set; }
		public DateTime ISDate { get; set; }
		public DateTime BeginDate { get; set; }
		public int UpfrontPaymentMth { get; set; }
		public string LEFSIntCode { get; set; }
		#endregion

		#region Additional Info
		public string RefundableDep { get; set; }
		public string NonRefundableDep { get; set; }
		public string InsPdtoOffset { get; set; }
		public string ResidualValue { get; set; }
		public string FRMonths { get; set; }
		public string FRAmount { get; set; }
		public string Months { get; set; }
		public string Percentage { get; set; }
		public string IAAmount { get; set; }
		public string ModeofPayt { get; set; }
		public string Bank { get; set; }
		public string TechRefMths { get; set; }
		public int CreditTerm { get; set; }
		public int IntCreditTerm { get; set; }
		public decimal LatePaytIntPer { get; set; }
		public decimal MinLatePaytAmt { get; set; }
		public string FinanceQuantum { get; set; }
		public string AdminFee { get; set; }
		public decimal ProCommFee { get; set; }
		public string FacilityFee { get; set; }
		public bool GIRO { get; set; }
		public string AccountNo { get; set; }
		public string CustRef { get; set; }
		public bool OpttoRenew { get; set; }
		public string RenewAmt { get; set; }
		public string FairMktVal { get; set; }
		#endregion

		#region Additional Info 2
		public decimal PrepaymentPer { get; set; }
		public int PrepBlackoutPeriod { get; set; }
		public int PrepNoticePeriod { get; set; }
		public decimal CancelationFee { get; set; }
		public DateTime MaturityDate { get; set; }
		public string RenewMths { get; set; }
		#endregion

		#region Referral
		public string SpotterAmt { get; set; }
		public string AdditionalCost { get; set; }
		public string AttentionTo { get; set; }
		#endregion

		#region LEFS Info
		public string CPFirstExpDate { get; set; }
		public string CPSecondExpDate { get; set; }
		public string ContLapseDate { get; set; }
		public string ContCancelationDate { get; set; }
		public string ContPostingDate { get; set; }
		public string AppealDate { get; set; }
		public string SpringRecDate { get; set; }
		public string SpringAppDate { get; set; }
		public string FirstSubDate { get; set; }
		public string SecondSubDate { get; set; }
		public string ContRejDate { get; set; }
		public string ContConDate { get; set; }
		#endregion

		#region setup code
		public string setup_InsOpt_Straight { get; set; }
		public string setup_RateOpt_Flat { get; set; }
		public string setup_PaymentMode_Giro { get; set; }
		public string logic_RunSche_ISS { get; set; }
		public string logic_RunSche_NonSpringOthers { get; set; }
		public string logic_RunSche_Spring { get; set; }
		public string logic_RunSche_NonSpringLeasing { get; set; }
		public string logic_MaturityDate_ISS { get; set; }
		public string logic_RenewalMths_ISS { get; set; }
		#endregion
	}


	public class ContactPersonDetailsModel
	{
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string OfficeNumber { get; set; }
		public string HomeNumber { get; set; }
		public string FaxNumber { get; set; }
		public string PagerNumber { get; set; }
	}
	public class FieldPropertiesModel
	{
		public int ColumnIndex { get; set; }
		public string ColumnName { get; set; }
		public bool IsEnable { get; set; }
		public bool IsVisible { get; set; }
		public bool IsMandatory { get; set; }
		public bool IsChecked { get; set; }
	}

	public class LogicModel
	{
		public string logic_CtrSchCalc { get; set; }
		public string logic_CtrMaturityDate { get; set; }
		public string logic_CtrRenewalMths { get; set; }
	}

	public class DropDownModel
	{
		public List<SelectListItem> lstCtrType { get; set; }
		public List<SelectListItem> lstSubCtrType { get; set; }
		public List<SelectListItem> lstProdType { get; set; }
		public List<SelectListItem> lstInsOption { get; set; }
		public List<SelectListItem> lstRateOption { get; set; }
		public List<SelectListItem> lstFOIO { get; set; }
		public List<LEFSIntrestCode> lstLEFSIntCode { get; set; }
		public List<SelectListItem> lstModeofPayt { get; set; }
		public List<SelectListItem> lstBank { get; set; }
		public List<SelectListItem> lstSalesExecutive { get; set; }
		public List<SelectListItem> lstPaymentOption { get; set; }
		public List<SelectListItem> lstSecurityLevel1 { get; set; }
		public List<SelectListItem> lstBuybackPercentageType { get; set; }
	}

	public class CommonDropDownModel
	{
		public IEnumerable<CommonDropDown> VehicleMake { get; set; }
		public IEnumerable<CommonDropDown> SupplierList { get; set; }
		public IEnumerable<CommonDropDown> ModelList { get; set; }
		public IEnumerable<CommonDropDown> BrandList { get; set; }
		public IEnumerable<CommonDropDown> CustomerList { get; set; }
	}

	public class InstalmentOption
	{
		public string Begin;
		public string End;
		public string InstalmentAmount;
	}

	public class CollectionFeeOption
	{
		public string Begin;
		public string End;
		public string Amount;
		public string NoofCopies;
	}

	public class InsCollFeeOpt
	{
		public List<InstalmentOption> IO;
		public List<CollectionFeeOption> CFO;
	}
	public class LEFSIntrestCode
	{
		public string InterestCode { get; set; }
		public string InterestType { get; set; }
		public string Description { get; set; }
		public decimal CoyRate { get; set; }
	}
	public class InsertPreConModel
	{
		public string json { get; set; }
		public	string securityList { get; set; }
		public	string individualGuarantorList { get; set; }
		public	string corporateGuarantorList { get; set; }
		public	string mortgageList { get; set; }
		public	string debentureList { get; set; }
		public	string buybackList { get; set; }
		public	string recourseList { get; set; }	
	}
	public class PreConSaveModel
	{
		public string LEFSInterestCode { get; set; }
		public string SubConGroupCode { get; set; }
        public string RefNumber;
		public int RolloverNumber;
		public string CustomerType;
		public string Customer;
		public string CustomerAddress;
		public string CustomerDept;
		public string CustomerConPerson;
		public string ContractType;
		public string SubContractType;
		public string ProductType;
		public string SubProductType;
		public string Status;
		public string BuybackInd { get; set; }
		public string RecourseInd { get; set; }
		public string CreatedBy;
		public DateTime CreatedDate;
		public string UpdatedBy;
		public DateTime? UpdatedDate;
		public decimal PropertyTotLTVPer { get; set; }
		public decimal VesselTotLTVPer { get; set; }
		public List<PreConSecurityModel> SecurityList { get; set; }
		public List<GuarantorModel> IndividualGuarantorList { get; set; }
		public List<GuarantorModel> CorparateGuarantorList { get; set; }
		public List<Contract_SecurityItemModel> MortgagorPropertyAndVesselList { get; set; }
		public List<DebentureModel> DebentureList { get; set; }
		public List<BuyBackModel> BuyBackList { get; set; }
		public List<RecourseModel> RecourseList { get; set; }		
	}
	public class PreConSecurityModel
	{
		public int ItemNumber { get; set; }
		public string SecurityLevel1 { get; set; }
		public string SecurityLevel2 { get; set; }
	}
	public class GuarantorModel
	{
		//public string SecurityListLevel2 { get; set; }
		public string GuarantorType { get; set; }
		public string SecurityID { get; set; }
		public string GuarantorAddress { get; set; }
		public string GuarantorDept { get; set; }
		public string GuarantorConPerson { get; set; }
		public string LetterType { get; set; }
		public string Status { get; set; }
	}
	public class DebentureModel
	{
		public string SecurityListLevel2 { get; set; }
		public string CustomerType { get; set; }
		public string Customer { get; set; }
		public string SecurityID { get; set; }
		public string Status { get; set; }
	}
	public class InsuranceModel
	{
		public string ExpiryDate { get; set; }
		public string InsuranceType { get; set; }
	}
	public class Contract_MasterModel
	{
		public string ContractNumber { get; set; }
		public int RolloverNumber { get; set; }
		public string CustomerType { get; set; }
		public string Customer { get; set; }
		public string CustomerAddress { get; set; }
		public string CustomerDept { get; set; }
		public string CustomerConPerson { get; set; }
		public string BuyBackInd { get; set; }
		public string RecourseInd { get; set; }
		public string Status { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public Nullable<DateTime> UpdatedDate { get; set; }
		public string LEFSInterestCode { get; set; }
		public string ContractType { get; set; }
		public string SubContractType { get; set; }
		public string ProductType { get; set; }
		public string SubProductType { get; set; }
	}

	public class Contract_SecurityItemModel
	{
		public string ContractNumber { get; set; }
		public int RolloverNumber { get; set; }
		public string SecurityListLevel2 { get; set; }
		public int ItemNumber { get; set; }
		public string SecurityID { get; set; }
		public string GuarantorType { get; set; }
		public string GuarantorAddress { get; set; }
		public string GuarantorDept { get; set; }
		public string GuarantorConPerson { get; set; }
		public string LetterType { get; set; }
		public string CustomerType { get; set; }
		public string Customer { get; set; }
		public string IndicativeValuationAmt { get; set; }
		public string LoanAmtProportion { get; set; }
		public string LTVPercentage { get; set; }
		public string Status { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public Nullable<DateTime> UpdatedDate { get; set; }
	}

	public class Security_ConstructionEquipModel
	{
		public string ID { get; set; }
		public string EquipBrand { get; set; }
		public string EquipModel { get; set; }
		public string EquipDesc { get; set; }
		public string SerialNumber { get; set; }
		public string SecuredAmount { get; set; }
		public string YearOfManufacture { get; set; }
		public string EngineNumber { get; set; }
		public string ChargeNumber { get; set; }
		public string ChargeDate { get; set; }
	}
	public class Security_PropertyModel
	{
		public string ID { get; set; }
		public string PropertyAddress { get; set; }
		public string PropertyTypeLevel1 { get; set; }
		public string PropertyTypeLevel2 { get; set; }
		public string FirstThirdParty { get; set; }
		public string FormalValuation { get; set; }
		public string CreditLimit { get; set; }
		public string IndicativeValuation { get; set; }
		public string TitleNumber { get; set; }
		public string MortgageNumber { get; set; }
		public string ChargeNumber { get; set; }
		public string ChargeDate { get; set; }
		public string SecurityListLevel2 { get; set; }
		public string Status { get; set; }
		public string CreatedBy { get; set; }
		public string CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public string UpdatedDate { get; set; }
	}
	public class Security_VesselModel
	{
		public string ID { get; set; }
		public string HullNumber { get; set; }
		public string VesselName { get; set; }
		public string CountryOfReg { get; set; }
		public string MortgageNumber { get; set; }
		public string FormalValuation { get; set; }
		public string ChargeNumber { get; set; }
		public string ChargeDate { get; set; }
		public string CreditLimit { get; set; }
		public string IndicativeValuation { get; set; }
		public string SecurityListLevel2 { get; set; }
		public string Status { get; set; }
		public string CreatedBy { get; set; }
		public string CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public string UpdatedDate { get; set; }
	}
	public class Security_InventoryModel
	{
		public string ID { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }
		public string ChargeNumber { get; set; }
		public string ChargeDate { get; set; }
	}
	public class Security_ReceivableModel
	{
		public string ID { get; set; }
		public string Amount { get; set; }
		public string ChargeNumber { get; set; }
		public string ChargeDate { get; set; }
	}
	public class Security_CashEquivalentIndModel
	{
		public string ID { get; set; }
		public string Refundable { get; set; }
		public string GuaranteeBondsType { get; set; }
		public string Amount { get; set; }
		public string BillToCustomer { get; set; }
		public string BillToAddress { get; set; }
		public string BillToDept { get; set; }
		public string BillToConPerson { get; set; }
		//public string SecurityListLevel2 { get; set; }	
		public string BillToNRIC_FIN_PASSPORT { get; set; }
		public string BillToROCUEN { get; set; }
		public string BillToMobileNumber { get; set; }
		public string BillToEmail { get; set; }
		public string BillToOfficeNumber { get; set; }
		public string BillToHomeNumber { get; set; }
		public string BillToFaxNumber { get; set; }
		public string BillToPagerNumber { get; set; }
	}
	public class Security_SecFinInstrumentsModel
	{
		public string ID { get; set; }
		public string Type { get; set; }
		public string Amount { get; set; }
		public string DocumentNumber { get; set; }
		public string BankFinancialCom { get; set; }
		public string ChargeDate { get; set; }
	}
	public class BuyBackModel
	{
		public string Guarantor { get; set; }
		public string GuarantorAddress { get; set; }
		public string GuarantorDept { get; set; }
		public string GuarantorConPerson { get; set; }
		public string LetterType { get; set; }
		public string GuarantorCode { get; set; }
		public string Status { get; set; }
		public int PeriodFrom { get; set; }
		public int PeriodTo { get; set; }
		public string BuyBackType { get; set; }
		public decimal BuyBackPercentage { get; set; }
		public string BuyBackAmount { get; set; }
	}
	public class RecourseModel
	{
		public string Guarantor { get; set; }
		public string GuarantorAddress { get; set; }
		public string GuarantorDept { get; set; }
		public string GuarantorConPerson { get; set; }
		public string LetterType { get; set; }
		public string GuarantorCode { get; set; }
		public string Status { get; set; }
		public int PeriodFrom { get; set; }
		public int PeriodTo { get; set; }
		public string RecourseType { get; set; }
		public decimal RecoursePercentage { get; set; }
		public string RecourseAmount { get; set; }
	}
	public class Security_VehicleModel
	{
		public string ID { get; set; }
		public string ChassisNumber { get; set; }
		public string RegNumber { get; set; }
		public string VehicleMake { get; set; }
		public string VehicleModel { get; set; }
		public string VehicleType { get; set; }
		public string COE_ExpiryDate { get; set; }
		public string EngineNumber { get; set; }
		public string ChargeNumber { get; set; }
		public string ChargeDate { get; set; }
		public string SecurityListLevel2 { get; set; }
		public string Value { get; set; }
	}
}
