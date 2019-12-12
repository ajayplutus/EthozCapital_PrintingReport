	using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
	public class Spotter_Master
	{
		[Key]
		[Column(Order = 1)]
		public int SpotterID { get; set; }
		public string SpotterRefNumber { get; set; }
		public DateTime PreparationDate { get; set; }
		public Nullable<decimal> Amount { get; set; }
		public Nullable<decimal> ApprovedAmount { get; set; }
		[StringLength(1)]
		public string Status { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public Nullable<DateTime> UpdatedDate { get; set; }
	}
}