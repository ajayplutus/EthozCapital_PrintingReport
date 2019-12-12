using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthozCapital.Models.ViewModels
{
	public class SecurityViewModel
	{
		public SecurityViewModel()
		{
		}
		public string Id { get; set; }
		public string GroupCode { get; set; }
		public string GroupType { get; set; }
		public string ParentId { get; set; }
		public string Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
	}

	#region Property
	public class PropertyModel
	{
		public PropertyModel()
		{
			PropertyDetails = new PropertyDetailsModel();
			Mortgagor = new List<MortgagorModel>();
			CustomerToAccess = new List<CustomerToAccess>();
			ContractDetails = new List<SecurityContractViewModel>();
		}
		public string Id { get; set; }
		public string PropertyAddress { get; set; }
		public string GroupCode { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public string Status { get; set; }
		public PropertyDetailsModel PropertyDetails { get; set; }
		public List<MortgagorModel> Mortgagor { get; set; }
		public List<CustomerToAccess> CustomerToAccess { get; set; }
		public List<SecurityContractViewModel> ContractDetails { get; set; }
		public List<FieldChangeHistoryModel> FieldChangeHistory { get; set; }
	}

	public class PropertyDetailsModel
	{
		public string PropertyTypeLevel1 { get; set; }
		public string PropertyTypeLevel2 { get; set; }
		public string PartyType { get; set; }
		public decimal FormalValuation { get; set; }
		public decimal CreditLimit { get; set; }
		public decimal IndicativeValuation { get; set; }
		public string TitleNumber { get; set; }
		public string MortgagorNumber { get; set; }
		public string ChangeNumber { get; set; }
		public string ChargeDate { get; set; }
		public string SecurityListLevel2 { get; set; }
		public decimal FormalValuationOld { get; set; }
		public decimal CreditLimitOld { get; set; }
		public decimal IndicativeValuationOld { get; set; }
	}

	public class CreatedInfoModel
	{
		public Nullable<DateTime> CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<DateTime> UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public string Status { get; set; }
	}

	#endregion

	#region Vessel
	public class VesselModel
	{
		public VesselModel()
		{
			VesselDetails = new VesselDetailModel();
			Mortgagor = new List<MortgagorModel>();
			InsuranceDetail = new List<InsuranceDetail>();
			CustomerToAccess = new List<CustomerToAccess>();
			ContractDetails = new List<SecurityContractViewModel>();
		}
		public string Id { get; set; }
		public string HullNumber { get; set; }
		public string VesselName { get; set; }
		public VesselDetailModel VesselDetails { get; set; }
		public List<MortgagorModel> Mortgagor { get; set; }
		public List<CustomerToAccess> CustomerToAccess { get; set; }
		public List<InsuranceDetail> InsuranceDetail { get; set; }
		public string Status { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public List<SecurityContractViewModel> ContractDetails { get; set; }
		public List<FieldChangeHistoryModel> FieldChangeHistory { get; set; }
	}
	public class VesselDetailModel
	{
		public string CountryofRegistration { get; set; }
		public string MortgageNumber { get; set; }
		public string ChargeNumber { get; set; }
		public decimal FormalValuation { get; set; }
		public string ChargeDate { get; set; }
		public decimal CreditLimit { get; set; }
		public decimal IndicativeValuation { get; set; }
		public decimal FormalValuationOld { get; set; }
		public decimal CreditLimitOld { get; set; }
		public decimal IndicativeValuationOld { get; set; }
	}
	public class InsuranceDetail
	{
		public string InsuranceType { get; set; }
		public string InsuranceTypeDisplay { get; set; }
		public string ExpiryDate { get; set; }
		public List<SelectListItem> Insurance { get; set; }
		public int ItemNumber { get; set; }
	}
	#endregion

	#region Vehicle
	public class VehicleModel
	{
		public VehicleModel()
		{
			VehicleDetails = new VehicleDetailModel();
			CustomerToAccess = new List<CustomerToAccess>();
			ContractDetails = new List<SecurityContractViewModel>();
		}

		public string Id { get; set; }
		public string ChassisNumber { get; set; }
		public string VehicleRegNumber { get; set; }
		public string Status { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public VehicleDetailModel VehicleDetails { get; set; }
		public List<CustomerToAccess> CustomerToAccess { get; set; }
		public List<SecurityContractViewModel> ContractDetails { get; set; }
	}
	public class VehicleDetailModel
	{
		public string VehicleMakeId { get; set; }
		public string VehicleModelId { get; set; }
		public string VehicleType { get; set; }
		public string EngineNumber { get; set; }
		public string ChargeNumber { get; set; }
		public decimal? Value { get; set; }
		public string ChargeDate { get; set; }
		public string COEExpiryDate { get; set; }
	}
	#endregion

	#region Construction Equipment & Industrial Equipment
	public class EquipmentModel
	{
		public EquipmentModel()
		{
			CustomerToAccess = new List<CustomerToAccess>();
		}

		public string Id { get; set; }
		public string Brand { get; set; }
		public string Model { get; set; }
		public string EquipmentDescription { get; set; }
		public string Status { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public EquipmentDetailModel EquipmentDetails { get; set; }
		public List<CustomerToAccess> CustomerToAccess { get; set; }
		public List<SecurityContractViewModel> ContractDetails { get; set; }
	}

	public class EquipmentDetailModel
	{
		public string SerialNumber { get; set; }
		public decimal SecuredAmount { get; set; }
		public string ManufactureYear { get; set; }
		public string EngineNumber { get; set; }
		public string ChargeNumber { get; set; }
		public string ChargeDate { get; set; }
	}
	#endregion

	#region Inventory
	public class InventoryModel
	{
		public InventoryModel()
		{
			CustomerToAccess = new List<CustomerToAccess>();
			ContractDetails = new List<SecurityContractViewModel>();
		}
		public string Id { get; set; }
		public string Type { get; set; }
		public decimal Value { get; set; }
		public string ChargeNumber { get; set; }
		public string ChargeDate { get; set; }
		public string Status { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public List<CustomerToAccess> CustomerToAccess { get; set; }
		public List<SecurityContractViewModel> ContractDetails { get; set; }
	}
	#endregion

	#region Receivable
	public class ReceivableModel
	{
		public ReceivableModel()
		{
			CustomerToAccess = new List<CustomerToAccess>();
			ContractDetails = new List<SecurityContractViewModel>();
		}

		public string Id { get; set; }
		public decimal Amount { get; set; }
		public string ChargeNumber { get; set; }
		public string ChargeDate { get; set; }
		public string Status { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public List<CustomerToAccess> CustomerToAccess { get; set; }
		public List<SecurityContractViewModel> ContractDetails { get; set; }
	}
	#endregion

	#region Cash Equivalent
	public class CashEquivalentModel
	{
		public CashEquivalentModel()
		{
			CustomerToAccess = new List<CustomerToAccess>();
			ContractDetails = new List<SecurityContractViewModel>();
		}
		public string Id { get; set; }
		public string Refundable { get; set; }
		public string GuaranteeBondsType { get; set; }
		public decimal Amount { get; set; }
		public BillToModel BillToModel { get; set; }
		public string Status { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public List<CustomerToAccess> CustomerToAccess { get; set; }
		public List<SecurityContractViewModel> ContractDetails { get; set; }
	}
	public class BillToModel
	{
		public string Customer { get; set; }
		public string CustomerName { get; set; }
		public string NricFinPassport { get; set; }
		public string ROCUEN { get; set; }
		public string Address { get; set; }
		public string Department { get; set; }
		public string ContactPerson { get; set; }
	}
	#endregion

	#region Securities/ Financial Instrument
	public class SecFinInstrumentModel
	{
		public SecFinInstrumentModel()
		{
			SecurityorFinancialInstrumentDetails = new SecurityorFinancialInstrumentDetails();
			CustomerToAccess = new List<CustomerToAccess>();
			ContractDetails = new List<SecurityContractViewModel>();
		}

		public string Id { get; set; }
		public string FinancialInstrumentType { get; set; }
		public decimal Amount { get; set; }
		public string Status { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public SecurityorFinancialInstrumentDetails SecurityorFinancialInstrumentDetails { get; set; }
		public List<CustomerToAccess> CustomerToAccess { get; set; }
		public List<SecurityContractViewModel> ContractDetails { get; set; }
	}

	public class SecurityorFinancialInstrumentDetails
	{
		public string DocumentNumber { get; set; }
		public string BankNameorFinancialCompany { get; set; }
		public string ChargeDate { get; set; }
	}
	#endregion

	#region Security Deposit
	public class SecurityDepositModel
	{
		public SecurityDepositModel()
		{
			BillToDetailModel = new BillToDetailModel();
			CustomerToAccess = new List<CustomerToAccess>();
			ContractDetails = new List<SecurityContractViewModel>();
		}
		public string Id { get; set; }
		public string Refundable { get; set; }
		public decimal Amount { get; set; }
		public string Status { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public BillToDetailModel BillToDetailModel { get; set; }
		public List<CustomerToAccess> CustomerToAccess { get; set; }
		public List<SecurityContractViewModel> ContractDetails { get; set; }
	}
	public class BillToDetailModel
	{
		public string IndividualCorporate { get; set; }
		public string Customer { get; set; }
		public string CustomerName { get; set; }
		public string NRICFINPASSPORT { get; set; }
		public string ROCUEN { get; set; }
		public string Address { get; set; }
		public string Department { get; set; }
		public string ContactPerson { get; set; }
	}
	#endregion

	public class MortgagorModel
	{
		public string IndividualCorporate { get; set; }
		public string Mortgagor { get; set; }
		public string MortgagorDisplay { get; set; }
		public string MainType { get; set; }
		public string MainDisplay { get; set; }
		public string NRICType { get; set; }
		public string ROCType { get; set; }
		public string Address { get; set; }
		public string AddressDisplay { get; set; }
		public string Department { get; set; }
		public string DepartmentDisplay { get; set; }
		public string ContactPerson { get; set; }
		public string ContactPersonDisplay { get; set; }
		public int ItemNumber { get; set; }
	}

	public class CustomerToAccess
	{
		[Newtonsoft.Json.JsonProperty("IndividualCorporate")]
		public string IndividualCorporate { get; set; }
		public string Customer { get; set; }
		public string CustomerName { get; set; }
		public int ItemNumber { get; set; }
		public string Status { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
	}
	public class SecurityPropertyMortgagorDetailsModel
	{
		public string ClientName { get; set; }
		public string MainMortgagor { get; set; }
		public string NRICFINPASSPORT { get; set; }
		public string ROCUEN { get; set; }
		public string MortgagorAddress { get; set; }
		public string Department { get; set; }
		public string ContactPerson { get; set; }
		public string MobileNumber { get; set; }
		public string Email { get; set; }
		public string OfficeNumber { get; set; }
		public string HomeNumber { get; set; }
		public string FaxNumber { get; set; }
		public string PagerNumber { get; set; }

	}
	public class SecurityVesselMortgagorDetailsModel
	{
		public string MortgagorName { get; set; }
		public string Main_SecondaryMortgagor { get; set; }
		public string NRIC_FIN_PASSPORT { get; set; }
		public string ROCUEN { get; set; }
		public string Address { get; set; }
		public string Department { get; set; }
		public string ContactPerson { get; set; }
		public string MobileNumber { get; set; }
		public string Email { get; set; }
		public string OfficeNumber { get; set; }
		public string HomeNumber { get; set; }
		public string FaxNumber { get; set; }
		public string PagerNumber { get; set; }
	}
	public class FieldChangeHistoryModel
	{
		public string FieldName { get; set; }
		public string FieldValue { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
	}
}