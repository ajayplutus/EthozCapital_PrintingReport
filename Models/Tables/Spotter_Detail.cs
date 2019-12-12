using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
	public class Spotter_Detail
	{
		[Key]
		[Column(Order = 1)]
		public int SpotterDetailId { get; set; }
		public int SpotterId { get; set; }
		public string ContractNumber { get; set; }
		public int RolloverNumber { get; set; }
		public int ItemNumber { get; set; }
		[StringLength(1)]
		public string Valid { get; set; }
		public Nullable<DateTime> ApprovalDate { get; set; }
		public string ApprovedBy { get; set; }
		public Nullable<DateTime> RejctedDate { get; set; }
		public string RejectedBy { get; set; }
		public string RejectionReason { get; set; }
		[StringLength(1)]
		public string ApprovedInd { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public Nullable<DateTime> UpdatedDate { get; set; }
	}
}