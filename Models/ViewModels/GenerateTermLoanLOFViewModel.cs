using EthozCapital.Data;
using EthozCapital.Data.OrixEss;
using EthozCapital.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.ViewModels
{
	#region Generate Term Loan LOF View Model
	public class GenerateTermLoanLOFViewModel
	{
		public GenerateTermLoanLOFViewModel()
		{
			ContractDetails = new ContractDetailsViewModel();
			SecurityItemByProperty = new SecurityItem_ByType_ViewModel();
			ContractScheduleDetails = new ContractScheduleViewModel();
			RepaymetContractScheduleDetails = new ContractScheduleViewModel();
			ClientContactMacDetails = new ClientContactMasViewModel();
			SecurityPropertyModalDetails = new List<Security_PropertyModel_ViewModel>();
			SecurityPropertyMortgagorDetails = new List<SecurityPropertyMortgagorDetailsViewModel>();
			CustomerAddresssList = new AddressViewModel();
			ContactPersonList = new List<ContactPersonModel>();
			EmployeeDetails = new ss_emp_mas();
			CaveatIndDetails = new CaveatIndDetailsViewModel();
			WithdrawSuitIndDetails = new WithdrawSuitIndDetailsViewModel();
			DiscontSuitIndDetails = new DiscontSuitIndDetailsViewModel();
			AddOnLoanIndDetails = new AddOnLoanIndDetailsViewModel();
			CrossCollaIndDetails = new CrossCollaIndDetailsViewModel();
			PartialPrepayIndDetails = new List<PartialPrepayIndDetailsViewModel>();
			CrmtbClientIdMasDetails = new List<CrmtbClientIdMasDetails>();
			GuarantorList = new List<GuarantorOrMortgagorListViewModel>();
			MortgagorList = new List<GuarantorOrMortgagorListViewModel>();
			MortgagorListForAcknowledgement = new List<GuarantorOrMortgagorListViewModel>();
			DeceasedIndAndCpfDischargeIndDetailsList = new List<DeceasedIndAndCpfDischargeIndViewModel>();
			ChildConsentIndDetailsList = new List<ChildConsentIndViewModel>();
		}
		public bool IsLetterOfOfferChecked { get; set; }
		public ContractDetailsViewModel ContractDetails { get; set; }
		public SecurityItem_ByType_ViewModel SecurityItemByProperty { get; set; }
		public ContractScheduleViewModel ContractScheduleDetails { get; set; }
		public ContractScheduleViewModel RepaymetContractScheduleDetails { get; set; }
		public ClientContactMasViewModel ClientContactMacDetails { get; set; }
		public List<Security_PropertyModel_ViewModel> SecurityPropertyModalDetails { get; set; }
		public List<SecurityPropertyMortgagorDetailsViewModel> SecurityPropertyMortgagorDetails { get; set; }
		public AddressViewModel CustomerAddresssList { get; set; }
		public List<ContactPersonModel> ContactPersonList { get; set; }
		public List<GuarantorOrMortgagorListViewModel> GuarantorList { get; set; }
		public List<GuarantorOrMortgagorListViewModel> MortgagorList { get; set; }
		public List<GuarantorOrMortgagorListViewModel> MortgagorListForAcknowledgement { get; set; }
		public ss_emp_mas EmployeeDetails { get; set; }
		public List<CrmtbClientIdMasDetails> CrmtbClientIdMasDetails { get; set; }
		public CaveatIndDetailsViewModel CaveatIndDetails { get; set; }
		public WithdrawSuitIndDetailsViewModel WithdrawSuitIndDetails { get; set; }
		public DiscontSuitIndDetailsViewModel DiscontSuitIndDetails { get; set; }
		public AddOnLoanIndDetailsViewModel AddOnLoanIndDetails { get; set; }
		public CrossCollaIndDetailsViewModel CrossCollaIndDetails { get; set; }
		public List<PartialPrepayIndDetailsViewModel> PartialPrepayIndDetails { get; set; }
		public List<DeceasedIndAndCpfDischargeIndViewModel> DeceasedIndAndCpfDischargeIndDetailsList { get; set; }
		public List<ChildConsentIndViewModel> ChildConsentIndDetailsList { get; set; }
		public decimal SumOfIndicativeValuation { get; set; }
		public decimal SumOfLTVPercentage { get; set; }
		public string strMentalCapacityInd { get; set; }
		public string strCPFDischargeInd { get; set; }
		public string strDeceasedInd { get; set; }
		public string strChildConsentInd { get; set; }
		public string strCompanyClientName { get; set; }
		public string strPmPerDescription { get; set; }
		public string strFaxNumber { get; set; }
		public string strClientName { get; set; }
		public string strContactMobile { get; set; }
		public string strContactEmail { get; set; }
		public string strContactOffice { get; set; }
		public string strContactHome { get; set; }
		public string strContactFax { get; set; }
		public string strContactPager { get; set; }
		public string strPreparedDate { get; set; }
		public string strPreparedBy { get; set; }
		public string strContractNumber { get; set; }
		public string Error { get; set; }
		public string strChildAcknowledegmentClientNameByCustomer { get; set; }

	}
	#endregion

	#region Crmtb ClientId Mas Details View Model
	public class CrmtbClientIdMasDetails
	{
		public CrmtbClientIdMasDetails()
		{

		}
		public string cm_client_nam { get; set; }
		public string im_id_num { get; set; }
		public string im_id_typ { get; set; }
	}
	#endregion

	#region Guarantor Or Mortgagor List View Model
	public class GuarantorOrMortgagorListViewModel
	{
		public GuarantorOrMortgagorListViewModel()
		{

		}
		public string GroupCode { get; set; }
		public string SecurityID { get; set; }
		public string ContractNumber { get; set; }
		public string Mortgagor { get; set; }
		public string DiscontSuitInd { get; set; }
		public string WithdrawSuitInd { get; set; }
		public string MentalCapacityInd { get; set; }
		public string cm_client_nam { get; set; }
		public string im_id_num { get; set; }
		public string im_id_typ { get; set; }
	}
	#endregion

	#region Contract/PreContract Details View Model
	public class ContractDetailsViewModel
	{
		public ContractDetailsViewModel()
		{

		}
		public string ContractNumber { get; set; }
		public string SalesExec { get; set; }
		public string CustomerType { get; set; }
		public string Customer { get; set; }
		public string CustomerAddress { get; set; }
		public string CustomerDept { get; set; }
		public string CustomerConPerson { get; set; }
		public string AgreementDate { get; set; }
		public decimal? TotalCashPrice { get; set; }
		public decimal? PropertyTotLTVPer { get; set; }
		public int? LeasePeriod { get; set; }
		public int? RenewPeriod { get; set; }
		public decimal? PrepayPercent { get; set; }
		public decimal? CancelFeePercent { get; set; }
		public decimal? FacilityFee { get; set; }
		public decimal? CommitFee { get; set; }
		public decimal? LPIPercent { get; set; }
		public decimal? MinLPI { get; set; }
		public int? PrepayNoticeMth { get; set; }
		public decimal? TermChargesPercent { get; set; }
		public Nullable<DateTime> LetterOfferDate { get; set; }
		public int? PrepayBlackoutMth { get; set; }
		public int? UpfrontPaymentMth { get; set; }

		public int? VersionNumber { get; set; }
		public int? RolloverNumber { get; set; }

	}
	#endregion

	#region Client Contact Mas View Model
	public class ClientContactMasViewModel
	{
		public ClientContactMasViewModel()
		{

		}

		public string cm_con_cod { get; set; }
		public string cm_title { get; set; }
		public string cm_full_nam { get; set; }
	}
	#endregion

	#region SecurityItem By Type  View Model
	public class SecurityItem_ByType_ViewModel
	{
		public string ContractNumber { get; set; }
		public string SecurityListLevel2 { get; set; }
		public string SecurityID { get; set; }
		public string CaveatInd { get; set; }
		public string AddOnLoanInd { get; set; }
		public string WithdrawSuitInd { get; set; }
		public string DiscontSuitInd { get; set; }
		public string WSuitNo { get; set; }
		public string WSuitYear { get; set; }
		public string DSuitNo { get; set; }
		public string DSuitYear { get; set; }
		public string CaveatNo { get; set; }
		public string CaveatCompany { get; set; }
		public string MortgInstrumentNo { get; set; }
		public Nullable<DateTime> MortgInstrumentDate { get; set; }
		public Nullable<DateTime> DeedOfAssignDate { get; set; }
		public Nullable<DateTime> DeedOfSuborDate { get; set; }
		public string CrossCollaContractNum { get; set; }
		public string CrossCollaInd { get; set; }
		public string WSuitFiledBy { get; set; }
		public string DSuitFiledBy { get; set; }
		public string DSuitFiledByIdType { get; set; }
		public string DSuitFiledById { get; set; }
		public string PartialPrepayInd { get; set; }
		public decimal? LTVPercentage { get; set; }
		public string GroupMemberDesc { get; set; }
	}
	#endregion

	#region Contract Schedule View Model
	public class ContractScheduleViewModel
	{
		public ContractScheduleViewModel()
		{

		}
		public int InsNumber { get; set; }
		public decimal? InsAmount { get; set; }

	}
	#endregion

	#region Security Property Mortgagor Details ViewModel
	public class SecurityPropertyMortgagorDetailsViewModel
	{
		public SecurityPropertyMortgagorDetailsViewModel()
		{

		}

		public string MasterID { get; set; }
		public string Mortgagor { get; set; }
		public string Relationship { get; set; }
		public int ChildNumber { get; set; }
		public string cm_client_nam { get; set; }
		public string im_id_num { get; set; }
		public string im_id_typ { get; set; }

	}
	#endregion

	#region Security_PropertyModel_ViewModel

	public class Security_PropertyModel_ViewModel
	{
		public Security_PropertyModel_ViewModel()
		{
				
		}

		public string ID { get; set; }
		public string PropertyAddress { get; set; }
		public string FirstThirdParty { get; set; }
		public decimal? IndicativeValuation { get; set; }
		public string MortgageNumber { get; set; }
		public string AddOnLoanInd { get; set; }
	}



	#endregion

	#region CaveatInd Details View Model
	public class CaveatIndDetailsViewModel
	{
		public CaveatIndDetailsViewModel()
		{
				
		}

		public string CaveatInd { get; set; }
		public string CaveatNo { get; set; }
		public string CaveatCompany { get; set; }
	}
	#endregion

	#region WithdrawSuitInd Details View Model
	public class WithdrawSuitIndDetailsViewModel
	{
		public WithdrawSuitIndDetailsViewModel()
		{

		}

		public string WithdrawSuitInd { get; set; }
		public string WSuitNo { get; set; }
		public string WSuitYear { get; set; }
		public string WSuitFiledBy { get; set; }
	}
	#endregion

	#region DiscontSuitInd Details View Model
	public class DiscontSuitIndDetailsViewModel
	{
		public DiscontSuitIndDetailsViewModel()
		{

		}

		public string DiscontSuitInd { get; set; }
		public string DSuitNo { get; set; }
		public string DSuitYear { get; set; }
		public string DSuitFiledBy { get; set; }
		public string DSuitFiledById { get; set; }
		public string GroupMemberDesc { get; set; }
	}
	#endregion

	#region AddOnLoanInd Details View Model
	public class AddOnLoanIndDetailsViewModel
	{
		public AddOnLoanIndDetailsViewModel()
		{

		}

		public string AddOnLoanInd { get; set; }
		public string MortgInstrumentNo { get; set; }
		public Nullable<DateTime> MortgInstrumentDate { get; set; }
		public Nullable<DateTime> DeedOfAssignDate { get; set; }
		public Nullable<DateTime> DeedOfSuborDate { get; set; }
	}
	#endregion

	#region CrossCollaInd Details View Model
	public class CrossCollaIndDetailsViewModel
	{
		public CrossCollaIndDetailsViewModel()
		{

		}

		public string CrossCollaInd { get; set; }
		public string CrossCollaContractNum { get; set; }
		public Nullable<DateTime> DeedOfAssignDate { get; set; }
		public Nullable<DateTime> DeedOfSuborDate { get; set; }
	}
	#endregion

	#region PartialPrepayInd Details View Model
	public class PartialPrepayIndDetailsViewModel
	{
		public PartialPrepayIndDetailsViewModel()
		{

		}
		public string ContractNumber { get; set; }
		public string PartialPrepayInd { get; set; }
		public decimal? LTVPercentage { get; set; }
		public string PropertyAddress { get; set; }
	}
	#endregion

	#region DeceasedInd And CpfDischargeInd ViewModel
	public class DeceasedIndAndCpfDischargeIndViewModel
	{
		public DeceasedIndAndCpfDischargeIndViewModel()
		{
				
		}
		public string ContractNumber { get; set; }
		public string CpfDischargeInd { get; set; }
		public string DeceasedInd { get; set; }
		public string cm_client_nam { get; set; }
		public string im_id_num { get; set; }
		public string im_id_typ { get; set; }
	}

	#endregion

	#region ChildConsentInd View Model

	public class ChildConsentIndViewModel
	{
		public ChildConsentIndViewModel()
		{
				
		}

		public string ContractNumber { get; set; }
		public string Relationship { get; set; }
		public string GroupMemberDesc { get; set; }
		public int ChildNumber { get; set; }
		public string ChildConsentInd { get; set; }
		public string PropertyAddress { get; set; }
		public string cm_client_nam { get; set; }
		public string im_id_num { get; set; }
		public string im_id_typ { get; set; }

	}
	#endregion
}