using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
	public class Maintenance_LEFSInterestCode
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(Maintenance_LEFSInterestCode));

		[Key]
		public int Id { get; set; }

		[Required]
		public string InterestCode { get; set; }

		public string InterestType { get; set; }

		public string SubContractType { get; set; }

		public DateTime EffectiveDate { get; set; }
		
		public string Description { get; set; }

		public decimal BankRate { get; set; }

		public decimal CoyRate { get; set; }

		public decimal RiskSpring { get; set; }

		public decimal RiskEthoz { get; set; }

		public int RepaymentPeriodFrom { get; set; }

		public int? RepaymentPeriodTo { get; set; }

		public string Remarks { get; set; }

		public string Status { get; set; }
		public string DeactivationRemarks { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime? UpdatedDate { get; set; }
	}
}