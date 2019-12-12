using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using EthozCapital.CustomLibraries;
using log4net;
using System.ComponentModel.DataAnnotations.Schema;

namespace EthozCapital.Models.Tables
{
	public class Contract_Master
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(Contract_Master));
		//[Key]
		//public int Id { get; set; }
		[Key, Column(Order = 0)]
		public string ContractNumber { get; set; }
		[Key, Column(Order = 1)]
		public int RolloverNumber { get; set; }
		public string CustomerType { get; set; }
		public string Customer { get; set; }
		public string CustomerAddress { get; set; }
		public string CustomerDept { get; set; }
		public string CustomerConPerson { get; set; }
		public string BuyBackInd { get; set; }
		public string RecourseInd { get; set; }
		public string PreContractNumber { get; set; }
		public int PreConRolloverNumber { get; set; }
		public string Status { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string LEFSInterestCode { get; set; }
		public string SubContractType { get; set; }
	}
}