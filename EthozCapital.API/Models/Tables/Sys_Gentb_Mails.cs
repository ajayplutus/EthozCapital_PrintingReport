using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthozCapital.API.Models.Tables
{
	public class Sys_Gentb_Mails
	{
		[Key]
		public string gm_mail_num { get; set; }
		public string gm_mail_typ { get; set; }
		public string gm_mail_to { get; set; }
		public string gm_mail_cc { get; set; }
		public string gm_mail_subject { get; set; }
		public string gm_mail_body { get; set; }
		public string gm_mail_bodycc { get; set; }
		public string gm_sta_ind { get; set; }
		public string gm_sta_who { get; set; }
		public Nullable<DateTime> gm_sta_dat { get; set; }
	}
}