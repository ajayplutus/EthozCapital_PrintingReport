using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthozCapital.Models.ViewModels
{
	public class ApprovalProcessViewModel
	{
		public int ApprovalProcessID { get; set; }
		public int ApprovalHeaderID { get; set; }
		public string CompositeKey1 { get; set; }
		public string CompositeKey2 { get; set; }
		public string CompositeKey3 { get; set; }
		public string CompositeKey4 { get; set; }
		public string CompositeKey5 { get; set; }
		public string Status { get; set; }
		public string RejectReason { get; set; }
		public DateTime CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
	}

	public class ApprovalProcessDetailViewModel
	{
		public int ApprovalProcessDetailID { get; set; }
		public int ApprovalProcessID { get; set; }
		public int Tier { get; set; }
		public string AssignedTo { get; set; }
		public DateTime ApprovedDate { get; set; }
		public string ApprovedBy { get; set; }
		public DateTime RejectedDate { get; set; }
		public string RejectedBy { get; set; }
		public string RejectionReason { get; set; }
		public DateTime CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
	}

	public class SysApprovalViewModel
	{
		public int ApprovalHeaderID { get; set; }
		public string ApprovalName { get; set; }
		public int Tier { get; set; }
		public int ModuleID { get; set; }
		public DateTime EffectiveFrom { get; set; }
		public DateTime EffectiveTo { get; set; }
		public DateTime CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
	}

	public class SysApprovalDetailViewModel
	{
		public int ApprovalDetailID { get; set; }
		public int ApprovalHeaderID { get; set; }
		public int Tier { get; set; }
		public string PrimaryApprovingOfficer { get; set; }
		public string SecondaryApprovingOfficer { get; set; }
		public DateTime EffectiveFrom { get; set; }
		public DateTime EffectiveTo { get; set; }
		public DateTime CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
	}

	public class PendingApprovalViewModel
	{
		public PendingApprovalViewModel()
		{
			PendingApproval = new List<PendingApproval>();
		}
		
		public int PendingCount { get; set; }
		public string ApprovalPage { get; set; }
		public List<PendingApproval> PendingApproval { get; set; }
	}

	public class PendingApproval
	{
		public int SrNo { get; set; }
		public string Type { get; set; }
		public string BatchNo { get; set; }
		public decimal Amount { get; set; }
		public decimal ApprovedAmount { get; set; }
		public string Status { get; set; }
		public string SubmittedDate { get; set; }
		public string SubmittedBy { get; set; }
		public string CompositeKey1 { get; set; }
		public string CompositeKey2 { get; set; }
		public int ApprovalProcessID { get; set; }
		public int ApprovalProcessDetailID { get; set; }
		public int ApprovalHeaderID { get; set; }
		public string ApprovedDate { get; set; }
		public string ApprovedBy { get; set; }
		public string RejectedDate { get; set; }
		public string RejectedBy { get; set; }
		public string PreparationDate { get; set; }
	}

	public class ApprovalSpotterDetails
	{
		public ApprovalSpotterDetails()
		{
			SpotterSummary = new SpotterSummaryViewModel();
			OutstandingFee = new List<OutstandingSpotterFeeViewModel>();
		}
		public int ApprovalProcessID { get; set; }
		public int ApprovalHeaderID { get; set; }
		public int ApprovalProcessDetailID { get; set; }
		public string RejectReason { get; set; }
		public bool IsAllCheck { get; set; }
		public string AssignedTo { get; set; }
		public string ReassignReason { get; set; }
		public string ApprovingOfficer { get; set; }
		public List<SelectListItem> ApprovingOfficerList { get; set; }
		public SpotterSummaryViewModel SpotterSummary { get; set; }
		public List<OutstandingSpotterFeeViewModel> OutstandingFee { get; set; }
	}

	public class ApprovalProcess
	{
		public int ApprovalDetailID { get; set; }
		public int CurrentTier { get; set; }
		public int ApprovalHeaderID { get; set; }
		public string ModuleID { get; set; }
		public string ApprovalName { get; set; }
		public string RefNo { get; set; }
		public string UserName { get; set; }
	}

	public class ReassignApprovalDetails
	{
		public string SpotterRefNumber { get; set; }
		public string PreparationDate { get; set; }
		public int ApprovalProcessDetailID { get; set; }
		public string AssignedTo { get; set; }
		public string ReassignReason { get; set; }
		public string ApprovingOfficer { get; set; }
		public List<SelectListItem> ApprovingOfficerList { get; set; }
		public SpotterSummaryViewModel SpotterSummary { get; set; }
		public List<OutstandingSpotterFeeViewModel> OutstandingFee { get; set; }
	}
}