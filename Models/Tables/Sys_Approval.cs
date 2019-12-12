using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
	public class Sys_Approval
	{
		[Key]
		[Column(Order = 1)]
		public int ApprovalHeaderID { get; set; }
		public string ApprovalName { get; set; }
		public int Tier { get; set; }
		public string ModuleID { get; set; }
		public DateTime EffectiveFrom { get; set; }
		public DateTime EffectiveTo { get; set; }
		public DateTime CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<DateTime>UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
	}
}