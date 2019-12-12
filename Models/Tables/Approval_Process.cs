using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
	public class Approval_Process
	{
		[Key]
		[Column(Order = 1)]
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
		public Nullable<DateTime> UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public Nullable<DateTime> ApprovedDate { get; set; }
		public string ApprovedBy { get; set; }
		public Nullable<DateTime> RejectedDate { get; set; }
		public string RejectedBy { get; set; }
	}
}