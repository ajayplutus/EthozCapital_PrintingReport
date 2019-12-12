using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.ViewModels
{
	public class SecurityPropertyViewModel
	{
		public SecurityPropertyViewModel()
		{
			PropertyDetails = new PropertyDetailsModel();
			Mortgagor = new List<MortgagorModel>();
			CustomerToAccess = new List<CustomerToAccess>();
			ContractDetails = new List<SecurityContractViewModel>();
		}
		public string Id { get; set; }
		public string PropertyAddress { get; set; }
		public string GroupCode { get; set; }
		public PropertyDetailsModel PropertyDetails { get; set; }
		public List<MortgagorModel> Mortgagor { get; set; }
		public List<CustomerToAccess> CustomerToAccess { get; set; }
		public string CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public string Status { get; set; }
		public List<SecurityContractViewModel> ContractDetails { get; set; }			
		public List<FieldChangeHistoryModel> FieldChangeHistory { get; set; }
	}
}