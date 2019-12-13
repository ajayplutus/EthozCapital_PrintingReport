﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.API.Models.Tables
{
	public class Approval_ProcessDetail
	{
		[Key]
		[Column(Order = 1)]
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
		public Nullable<DateTime> UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
	}
}