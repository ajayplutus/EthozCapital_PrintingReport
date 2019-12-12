using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
	public class Sys_ApprovalDetail
	{
		[Key]
		[Column(Order = 1)]
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
}