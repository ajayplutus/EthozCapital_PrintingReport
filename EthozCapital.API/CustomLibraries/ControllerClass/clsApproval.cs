using EthozCapital.API.Models;
using EthozCapital.API.Models.Tables;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace EthozCapital.API.CustomLibraries.ControllerClass
{
	public class clsApproval
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(Payment));

		public ResultViewModel FnStartNewProcessTier(ApprovalProcess model)
		{
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				glog.Debug("FnStartNewProcessTier: Entry");
				try
				{
					int tier = 0;
					string ApprovingOfficer;

					if (model.ApprovalHeaderID != 0)
					{
						tier = db.Sys_Approval.Where(w => w.ApprovalHeaderID == model.ApprovalHeaderID).Select(s => s.Tier).Max() + 1;
					}
					else
					{
						tier = db.Sys_Approval.Where(w => w.ApprovalName == model.ApprovalName && w.ModuleID == model.ModuleID).Select(s => s.Tier).Max() + 1;
					}

					var PrimaryOfficer = db.Sys_ApprovalDetail.Where(w => w.ApprovalDetailID == model.ApprovalDetailID).Select(s => s.PrimaryApprovingOfficer).FirstOrDefault();
					var SecondoryOfficer = db.Sys_ApprovalDetail.Where(w => w.ApprovalDetailID == model.ApprovalDetailID).Select(s => s.SecondaryApprovingOfficer).FirstOrDefault();

					if (FnGetApprovingOfficerAvailbility(PrimaryOfficer))
					{
						if (FnGetApprovingOfficerAvailbility(SecondoryOfficer))
						{
							ApprovingOfficer = PrimaryOfficer;
						}
						else
						{
							ApprovingOfficer = SecondoryOfficer;
						}
					}
					else
					{
						ApprovingOfficer = PrimaryOfficer;
					}
					var Id = 0;
					if (model.CurrentTier == 1)
					{
						var ApprovalProcess = new Approval_Process()
						{
							ApprovalHeaderID = model.ApprovalHeaderID,
							CompositeKey1 = model.RefNo,
							Status = "P",
							CreatedDate = DateTime.Now,
							CreatedBy = model.UserName,
						};
						db.Approval_Process.Add(ApprovalProcess);
						db.Entry(ApprovalProcess).State = System.Data.Entity.EntityState.Added;
						var ProcId = db.SaveChanges();

						var ApprovalProcessDtl = new Approval_ProcessDetail()
						{
							ApprovalProcessID = Id,
							Tier = tier,
							AssignedTo = ApprovingOfficer,
							CreatedDate = DateTime.Now,
							CreatedBy = model.UserName,
						};
						db.Approval_ProcessDetail.Add(ApprovalProcessDtl);
						db.Entry(ApprovalProcessDtl).State = System.Data.Entity.EntityState.Added;

						Id = db.SaveChanges();

					}
					else
					{
						var approvalProcess = db.Approval_Process.Where(w => w.ApprovalHeaderID == model.ApprovalHeaderID).FirstOrDefault();
						var ApprovalProcessDtl = new Approval_ProcessDetail()
						{
							ApprovalProcessID = approvalProcess.ApprovalProcessID,
							Tier = model.CurrentTier + 1,
							AssignedTo = ApprovingOfficer,
							CreatedDate = DateTime.Now,
							CreatedBy = model.UserName,
							ApprovedDate = DateTime.Now,
							ApprovedBy = model.UserName
						};
						db.Approval_ProcessDetail.Add(ApprovalProcessDtl);
						db.Entry(ApprovalProcessDtl).State = System.Data.Entity.EntityState.Added;
						Id = db.SaveChanges();

					}
					if (Id > 0)
					{
						result.Status = 1;
						result.Message = "Data has been approved!";
					}
					else
					{
						result.Status = 0;
						result.Message = "An error occurred when updating approval process!";
					}
					glog.Debug("FnStartNewProcessTier: Exit");
					return result;
				}
				catch (Exception ex)
				{
					glog.Error("FnStartNewProcessTier Exception: " + ex.Message + ex.InnerException);
					result.Status = 0;
					result.Message = "Please contact MIS, error: " + ex.Message;
					return result;
				}
			}


		}

		public bool FnGetApprovingOfficerAvailbility(string empCode)
		{
			bool IsAvailable = false;
			if (FnCheckLeaveRecord(empCode))
			{
				IsAvailable = true;
			}
			else
			{
				if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")) < Convert.ToDateTime("12:00"))
				{
					if (FnCheckLeaveAM(empCode))
					{
						IsAvailable = true;
					}
					else
					{
						IsAvailable = false;
					}
				}
				else
				{
					if (FnCheckLeavePM(empCode))
					{
						IsAvailable = true;
					}
					else
					{
						IsAvailable = false;
					}
				}
			}
			return IsAvailable;
		}

		public bool FnCheckLeaveRecord(string empCode)
		{
			bool IsLeaveRecord = false;
			glog.Debug("FnCheckLeaveRecord: Entry");
			//using (var db = new ORIX_ESS_DB_DevEntities())
			//{
			//	try
			//	{
			//		DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

			//		var result = from ssLevMas in db.ss_lev_mas
			//					 join ssLevDet in db.ss_lev_det on ssLevMas.lm_bat_num equals ssLevDet.ld_bat_num
			//					 where ssLevMas.lm_emp_cod == empCode && ssLevDet.ld_lev_am == true
			//					 && ssLevDet.ld_lev_pm == true && ssLevDet.ld_app_ind == "A" && ssLevDet.ld_sta_ind != "X"
			//					 && ssLevDet.ld_lev_dat == currentDate
			//					 select new { ssLevMas, ssLevDet };

			//		if (result.Any())
			//		{
			//			IsLeaveRecord = true;
			//		}
			//		else
			//		{
			//			IsLeaveRecord = false;
			//		}
			//		glog.Debug("FnCheckLeaveRecord: Exit");
			//	}
			//	catch (Exception ex)
			//	{
			//		glog.Error("FnCheckLeaveRecord Exception: " + ex.Message);
			//		throw;
			//	}
			//}
			return IsLeaveRecord;
		}

		public bool FnCheckLeaveAM(string empCode)
		{
			bool IsLeaveAM = false;
			glog.Debug("FnCheckLeaveAM: Entry");
			//using (var db = new ORIX_ESS_DB_DevEntities())
			//{
			//	try
			//	{
			//		DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
			//		var result = (from ssLevMas in db.ss_lev_mas
			//					  join ssLevDet in db.ss_lev_det on ssLevMas.lm_bat_num equals ssLevDet.ld_bat_num
			//					  where ssLevMas.lm_emp_cod == empCode && ssLevDet.ld_lev_am == true
			//					  && ssLevDet.ld_app_ind == "A" && ssLevDet.ld_sta_ind != "X"
			//					  && ssLevDet.ld_lev_dat == currentDate
			//					  select new { ssLevMas, ssLevDet }).ToList();

			//		var result1 = (from ssAbsMas in db.ss_abs_mas
			//					   join ssAbsDet in db.ss_abs_det on ssAbsMas.am_bat_num equals ssAbsDet.ad_bat_num
			//					   where ssAbsMas.am_emp_cod == empCode && ssAbsDet.ad_lev_am == true
			//					   && ssAbsDet.ad_mrk_ind == "O" && ssAbsDet.ad_sta_ind != "X"
			//						&& ssAbsDet.ad_lev_dat == currentDate
			//					   select new { ssAbsMas, ssAbsDet }).ToList(); ;

			//		if (result.Any() || result1.Any())
			//		{
			//			IsLeaveAM = true;
			//		}
			//		else
			//		{
			//			IsLeaveAM = false;
			//		}
			//		glog.Debug("FnCheckLeaveAM: Exit");
			//	}
			//	catch (Exception ex)
			//	{
			//		glog.Error("FnCheckLeaveAM Exception: " + ex.Message);
			//		throw;
			//	}
			//}
			return IsLeaveAM;
		}

		public bool FnCheckLeavePM(string empCode)
		{
			bool IsLeavePM = false;
			glog.Debug("FnCheckLeavePM: Entry");
			//using (var db = new ORIX_ESS_DB_DevEntities())
			//{
			//	try
			//	{
			//		DateTime currentDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
			//		var result = from ssLevMas in db.ss_lev_mas
			//					 join ssLevDet in db.ss_lev_det on ssLevMas.lm_bat_num equals ssLevDet.ld_bat_num
			//					 where ssLevMas.lm_emp_cod == empCode && ssLevDet.ld_lev_pm == true
			//					 && ssLevDet.ld_app_ind == "A" && ssLevDet.ld_sta_ind != "X"
			//					 && ssLevDet.ld_lev_dat == currentDate
			//					 select new { ssLevMas, ssLevDet };

			//		var result1 = from ssAbsMas in db.ss_abs_mas
			//					  join ssAbsDet in db.ss_abs_det on ssAbsMas.am_bat_num equals ssAbsDet.ad_bat_num
			//					  where ssAbsMas.am_emp_cod == empCode && ssAbsDet.ad_lev_pm == true
			//					  && ssAbsDet.ad_mrk_ind == "O" && ssAbsDet.ad_sta_ind != "X"
			//					  && ssAbsDet.ad_lev_dat == currentDate
			//					  select new { ssAbsMas, ssAbsDet };

			//		if (result.Any() || result1.Any())
			//		{
			//			IsLeavePM = true;
			//		}
			//		else
			//		{
			//			IsLeavePM = false;
			//		}
			//		glog.Debug("FnCheckLeavePM: Exit");
			//	}
			//	catch (Exception ex)
			//	{
			//		glog.Error("FnCheckLeavePM Exception: " + ex.Message);
			//		throw;
			//	}
			//}
			return IsLeavePM;
		}
	}
}