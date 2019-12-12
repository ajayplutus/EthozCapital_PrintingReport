using EthozCapital.API.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EthozCapital.API.CustomLibraries.ControllerClass
{
	public class Payment
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(Payment));

		public List<SpotterFeeViewModel> RetrieveSpotterFeebyRefNum(string SpotterRefNum, string strStatus)
		{
			using (var db = new MainDbContext())
			{
				glog.Debug("RetrieveSpotterFeebyRefNum: Entry");
				try
				{
					var SpotterFee = new List<SpotterFeeViewModel>();

					SpotterFee = db.Database.SqlQuery<SpotterFeeViewModel>(
					"exec RetrieveSpotterFeeByRefNum @SpotterRefNo,@strStatus,@OrixDB_Name",
					new SqlParameter("@SpotterRefNo", string.IsNullOrWhiteSpace(SpotterRefNum) ? "" : SpotterRefNum),
					new SqlParameter("@strStatus", string.IsNullOrWhiteSpace(strStatus) ? "" : strStatus),
					new SqlParameter("@OrixDB_Name", System.Configuration.ConfigurationManager.AppSettings["OrixDatabase"])
					).ToList();

					SpotterFee.Select(c => { c.SpotterAmt = Math.Round((decimal)c.SpotterAmt, 2); return c; }).ToList();

					glog.Debug("RetrieveSpotterFeebyRefNum: Exit");
					return SpotterFee;
				}
				catch (Exception ex)
				{
					glog.Error("RetrieveSpotterFeebyRefNum Exception: " + ex.Message + ex.InnerException);
					return new List<SpotterFeeViewModel>();
				}
			}
		}
	}
}