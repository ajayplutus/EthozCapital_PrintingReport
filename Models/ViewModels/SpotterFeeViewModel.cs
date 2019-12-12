using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.ViewModels
{
	public class SpotterFeeViewModel
	{
		public SpotterFeeViewModel()
		{
			SpotterContract = new List<SpotterContractViewModel>();
			SpotterSummary = new SpotterSummaryViewModel();
			SpotterDetails = new List<SpotterDetailsViewModel>();
			OutstandingFee = new List<OutstandingSpotterFeeViewModel>();

		}
		public SpotterSummaryViewModel SpotterSummary { get; set; }
		public List<SpotterDetailsViewModel> SpotterDetails { get; set; }
		public List<SpotterContractViewModel> SpotterContract { get; set; }
		public List<OutstandingSpotterFeeViewModel> OutstandingFee {get;set;}
	}

	public class SpotterViewModel
	{
		public SpotterViewModel()
		{
			SpotterContract = new List<SpotterContractViewModel>();
			SpotterContractDetails = new List<SpotterContractDetailsViewModel>();
		}

		public int SpotterId { get; set; }
		public string SpotterRefNumber { get; set; }
		public string PreparationDate { get; set; }
		public decimal Amount { get; set; }
		public string Status { get; set; }
		public string CreatedBy { get; set; }
		public string CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public Nullable<DateTime> UpdatedDate { get; set; }

		public List<SpotterDetailsViewModel> SpotterDetails { get; set; }
		public List<SpotterContractDetailsViewModel> SpotterContractDetails { get; set; }
		public List<SpotterContractViewModel> SpotterContract { get; set; }
	}

	public class SpotterSummaryViewModel
	{
		public int SpotterID { get; set; }
		public int Count { get; set; }
		public string SpotterRefNumber { get; set; }
		public string PreparationDate { get; set; }
		public string RejectReason { get; set; }
		public decimal Amount { get; set; }
		public decimal ApprovedAmount { get; set; }
		public string Status { get; set; }
		public Nullable<decimal> SumOfAmount { get; set; }
		public string CreatedBy { get; set; }
		public string CreatedDate { get; set; }
	}

	public class SpotterDetailsViewModel
	{
		public int SpotterId { get; set; }
		public string SpotterRefNumber { get; set; }
		public string Referral { get; set; }
		public string ReferralName { get; set; }
		public int NoOfContract { get; set; }
		public decimal SumOfAmount { get; set; }
		public string PostInd { get; set; }
	}

	public class SpotterContractDetailsViewModel
	{
		public string ContractNumber { get; set; }
		public int RolloverNumber { get; set; }
		public int ItemNumber { get; set; }
		public string ReferralID { get; set; }
		public string ReferralName { get; set; }
		public decimal SpotterAmt { get; set; }
		public string ApprovedInd { get; set; }
		public string Status { get; set; }
		public string CreatedBy { get; set; }
		public int SpotterDetailId { get; set; }
		public string Valid { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public Nullable<DateTime> UpdatedDate { get; set; }
		public string SpotterName { get; set; }
		public string ApprovalStatus { get; set; }
		public string ApprovealDate { get; set; }
		public string ApprovedBy { get; set; }
		public string RejectedDate { get; set; }
		public string RejectedBy { get; set; }
		public string RejectionReason { get; set; }
	}

	public class SpotterContractViewModel
	{
		public string ContractNumber { get; set; }
		public string ReferralName { get; set; }
		public Nullable<decimal> ContractAmount { get; set; }
		public string ApprovedInd { get; set; }
	}

	public class OutstandingSpotterFeeViewModel : SpotterContractDetailsViewModel
	{
		public string PostInd { get; set; }
	}
}