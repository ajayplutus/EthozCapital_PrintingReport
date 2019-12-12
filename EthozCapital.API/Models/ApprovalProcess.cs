using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthozCapital.API.Models
{
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
}