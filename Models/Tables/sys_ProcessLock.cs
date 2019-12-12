using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthozCapital.Models.Tables
{
	public class sys_ProcessLock
	{
		[Key]
		[Column(Order = 1)]
		public string RefNo { get; set; }
		public string Module { get; set; }
		public string CreatedBy { get; set; }
		public Nullable<DateTime> CreatedDate { get; set; }
	}
}