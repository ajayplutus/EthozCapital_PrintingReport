using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EthozCapital.Models.Tables
{
	public class Contract_Spotter
	{
		[Key]
		[Column(Order = 1)]
		public string ContractNumber { get; set; }
		[Key]
		[Column(Order = 2)]
		public int ItemNumber { get; set; }
		public Nullable<int> RolloverNumber { get; set; }
		public string Referral { get; set; }
		public Nullable<decimal> SpotterAmt { get; set; }
		[StringLength(1)]
		public string ApprovedInd { get; set; }
		[StringLength(1)]
		public string Status { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public Nullable<DateTime> UpdatedDate { get; set; }
	}
}