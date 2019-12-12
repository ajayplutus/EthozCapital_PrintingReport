using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthozCapital.API.Models
{
	public class SpotterFeeViewModel
	{
		public string ContractNumber { get; set; }
		public int RolloverNumber { get; set; }
		public int ItemNumber { get; set; }
		public string ReferralID { get; set; }
		public string ReferralName { get; set; }
		public decimal SpotterAmt { get; set; }
		public string ApprovedInd { get; set; }
		public string Status { get; set; }
		public string PostInd { get; set; }
		public int SpotterDetailId { get; set; }
		public string CreatedBy { get; set; }
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
}