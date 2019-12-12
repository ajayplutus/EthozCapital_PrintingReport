using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
	public class Approval_ProcessEvent
	{
		[Key]
		[Column(Order = 1)]
		public int ApprovalProcessEventID { get; set; }
		public int ApprovalProcessDetailID { get; set; }
		public Nullable<DateTime> ApprovedDate { get; set; }
		public string ApprovedBy { get; set; }
		public Nullable<DateTime> RejectedDate { get; set; }
		public string RejectedBy { get; set; }
		public string RejectReason { get; set; }
		public DateTime CreatedDate { get; set; }
		public string CreatedBy { get; set; }
	}
}