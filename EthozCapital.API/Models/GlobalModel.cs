using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthozCapital.API.Models
{
	public class GlobalModel
	{
	}
	public class ResultViewModel
	{
		public int Status { get; set; }
		public string Message { get; set; }
		public string Data { get; set; }
	}
}