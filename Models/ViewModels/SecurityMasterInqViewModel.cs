using EthozCapital.CustomLibraries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.ViewModels
{

	public class SecurityMasterInqViewModel
	{
		public string SecurityTypeLevel1 { get; set; }
		public string SecurityTypeLevel2 { get; set; }
		public string SecuritySystemId { get; set; }
		public string SecurityItemStatus { get; set; }
		public string ContractNumber { get; set; }
		public string ContractRolloverNo { get; set; }
		public string ContractsStatus { get; set; }
	}

	public class SecurityMasterInqListViewModel
	{
		public SecurityMasterInqListViewModel()
		{
			SecurityMasterInqList = new List<SecurityMasterInqList>();
			ContractDetails = new List<SecurityContractViewModel>();
		}
		[Display(Name = "SecurityMasterInqList")]
		public List<SecurityMasterInqList> SecurityMasterInqList { get; set; }

		[Display(Name = "SecurityContractList")]
		public List<SecurityContractViewModel> ContractDetails { get; set; }


		public string Action { get; set; }
	}

	public class SecurityMasterInqList
	{
		public string Id { get; set; }
		public string SecurityListLevel1Code { get; set; }
		public string SecurityListLevel1 { get; set; }
		public string SecurityListLevel2Code { get; set; }
		public string SecurityListLevel2 { get; set; }
		public string SecurityID { get; set; }
		public string SecurityStatusCode { get; set; }
		public string SecurityStatus { get; set; }
		public DateTime? SecurityCreatedDate { get; set; }
		public DateTime? SecurityChargeDate { get; set; }
		public string ContractCustomerType { get; set; }
		public string ContractCustomer { get; set; }
		public string ContractCustomerName { get; set; }
		public string ContractStatusCode { get; set; }
		public string ContractStatus { get; set; }
		public string ContractNumber { get; set; }
		public int RolloverNumber { get; set; }
		public bool IsDisableLevel2 { get; set; }
		public string ControllerName { get; set; }
		public string ActionName { get; set; }
	}

	public class SecurityMasterInqParam
	{
		public string SecurityTypeLevel1 { get; set; }
		public string SecurityTypeLevel2 { get; set; }
		public string SecuritySystemId { get; set; }
		public string SecurityItemStatus { get; set; }
		public string CreatedDateFrom { get; set; }
		public string CreatedDateTo { get; set; }
		public string ChargeDateFrom { get; set; }
		public string ChargeDateTo { get; set; }
		public string SecurityItemsIndividual { get; set; }
		public string SecurityItemsCustomer { get; set; }
		public string ContractsIndividual { get; set; }
		public string ContractsCustomer { get; set; }
		public string ContractNumber { get; set; }
		public string ContractRolloverNo { get; set; }
		public string ContractsStatus { get; set; }
	}

	public class SecurityContractViewModel
	{
		public string SysSequrityID { get; set; }
		public string ContractNumber { get; set; }
		public int ContractRolloverNo { get; set; }
		public string ContractsCustomer { get; set; }
		public string ContractsStatus { get; set; }
		public string ContractsCustomerName { get; set; }
	}
}
