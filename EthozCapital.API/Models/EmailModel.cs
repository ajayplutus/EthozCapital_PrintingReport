using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthozCapital.API.Models
{
	public class EmailViewModel
	{
		public string MailType { get; set; }
		public string EmailTo { get; set; }
		public string EmailFrom { get; set; }
		public string CcEmail { get; set; }
		public string Subject { get; set; }
		public string body { get; set; }
		public string UserId { get; set; }
	}
}