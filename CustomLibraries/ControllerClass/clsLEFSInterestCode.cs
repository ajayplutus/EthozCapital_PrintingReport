using EthozCapital.CustomLibraries;
using EthozCapital.Data;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace EthozCapital.CustomLibraries
{
	public class clsLEFSInterestCode
	{
        private static ILog glog = log4net.LogManager.GetLogger(typeof(clsLEFSInterestCode));

        public clsLEFSInterestCode()
        {
        }

		public ResultViewModel InsertLEFSInterestCode(LEFSInterestCodeViewModel Model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertLEFSInterestCode: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						Maintenance_LEFSInterestCode InterestCode = new Maintenance_LEFSInterestCode();
						var isInterestCode = db.Maintenance_LEFSInterestCode.Any(x => x.InterestCode == Model.InterestCode && x.Id != Model.Id);
						if (isInterestCode)
						{
							result.Status = 2;
							result.Message = " Interest code is already exist!";
							return result;
						}
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
                        
						InterestCode.InterestCode = Model.InterestCode;
						InterestCode.InterestType = Model.InterestType;
						InterestCode.SubContractType = Model.SubContractType;
						InterestCode.EffectiveDate = Convert.ToDateTime(Model.EffectiveDate);
						InterestCode.Description = Model.Description;
						InterestCode.BankRate = Model.BankRate;
						InterestCode.CoyRate = Model.CoyRate;
						InterestCode.RiskSpring = Model.RiskSpring;
						InterestCode.RiskEthoz = Model.RiskEthoz;
						InterestCode.RepaymentPeriodFrom = Model.RepaymentPeriodFrom;
						InterestCode.RepaymentPeriodTo = Model.RepaymentPeriodTo;
						InterestCode.Remarks = Model.Remarks;
						
						if (Model.Id > 0)
						{
							var data= GetLEFSInterestCodeById(Model.Id);
							InterestCode.UpdatedBy = userMail;
							InterestCode.UpdatedDate = DateTime.Now;
							InterestCode.Status = data.ActiveStatus;
							InterestCode.CreatedBy = data.CreatedBy;
							InterestCode.CreatedDate = !string.IsNullOrEmpty(data.CreatedDate) ? DateTime.ParseExact(data.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
						}
						else
						{
							InterestCode.Status = "O";
							InterestCode.CreatedBy = userMail;
							InterestCode.CreatedDate = DateTime.Now;
						}
						db.Maintenance_LEFSInterestCode.Add(InterestCode);
						if (Model.Id > 0)
						{
							InterestCode.Id = Model.Id;
							db.Entry(InterestCode).State = System.Data.Entity.EntityState.Modified;
						}
						else {
							db.Entry(InterestCode).State = System.Data.Entity.EntityState.Added;

						}
						var id = db.SaveChanges();

						transaction.Commit();                        
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "New LEFS interest code saved successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving new LEFS interest code";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertLEFSInterestCode Exception: " + ex.Message);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertLEFSInterestCode: Exit");
					return result;
				}
			}
		}

		public ResultViewModel DeactivateInterestCode(int Id, string Reason, string UserName, bool Confirm)
		{
			string userMail = "";
			glog.Debug("DeactivateInterestCode: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						Maintenance_LEFSInterestCode objLEFSInterestCode = db.Maintenance_LEFSInterestCode.Find(Id);
						PreContract_Master objPreContract = new PreContract_Master();
						bool isExists = db.PreContract_Master.Any(x => x.LEFSInterestCode.Equals(objLEFSInterestCode.InterestCode) && x.Status.Equals("P"));

						if (isExists)
						{
							result.Status = 2;
							result.Message = " Interest code is selected in Pre-Contract, you are not allowed to deactivate it.";
							return result;
						}
						if (!Confirm)
						{
							result.Status = 2;
							result.Message = "Do you want to deactivate the LEFS Interest Code?";
							return result;
						}
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
                        
						objLEFSInterestCode.Status = "D";
						objLEFSInterestCode.DeactivationRemarks = Reason;
						objLEFSInterestCode.UpdatedBy = userMail;
						objLEFSInterestCode.UpdatedDate = DateTime.Now;

						db.Maintenance_LEFSInterestCode.Add(objLEFSInterestCode);
						db.Entry(objLEFSInterestCode).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();                        
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "LEFS interest code deactivated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving new LEFS interest code";
						}
					}
					catch (Exception ex)
					{
						glog.Error("DeactivateInterestCode Exception: " + ex.Message);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("DeactivateInterestCode: Exit");
					return result;
				}
			}
		}

		public ResultViewModel ActivateInterestCode(int Id, string Reason, string UserName, bool Confirm)
		{
			string userMail = "";
			glog.Debug("ActivateInterestCode: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						Maintenance_LEFSInterestCode objLEFSInterestCode = db.Maintenance_LEFSInterestCode.Find(Id);
						PreContract_Master objPreContract = new PreContract_Master();
						bool isExists = db.PreContract_Master.Any(x => x.LEFSInterestCode.Equals(objLEFSInterestCode.InterestCode) && x.Status.Equals("P"));

						if (isExists)
						{
							result.Status = 2;
							result.Message = " Interest code is selected in Pre-Contract, you are not allowed to Activate it.";
							return result;
						}
						if (!Confirm)
						{
							result.Status = 2;
							result.Message = "Do you want to Activate the LEFS Interest Code?";
							return result;
						}
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						
						objLEFSInterestCode.Status = "O";
						objLEFSInterestCode.DeactivationRemarks = Reason;
						objLEFSInterestCode.UpdatedBy = userMail;
						objLEFSInterestCode.UpdatedDate = DateTime.Now;

						db.Maintenance_LEFSInterestCode.Add(objLEFSInterestCode);
						db.Entry(objLEFSInterestCode).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();						
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "LEFS interest code Activated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving new LEFS interest code";
						}
					}
					catch (Exception ex)
					{
						glog.Error("ActivateInterestCode Exception: " + ex.Message);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("ActivateInterestCode: Exit");
					return result;
				}
			}
		}

		public LEFSInterestCodeViewModel GetLEFSInterestCodeById(int? Id)
		{
			LEFSInterestCodeViewModel obj = new LEFSInterestCodeViewModel();
			using (var db = new MainDbContext())
			{
				try
				{
					Maintenance_LEFSInterestCode objResult = db.Maintenance_LEFSInterestCode.Find(Id);
					if (objResult != null)
					{
						obj.Id = objResult.Id;
						obj.InterestCode = objResult.InterestCode;
						obj.InterestType = objResult.InterestType;
						obj.SubContractType = objResult.SubContractType;
                        obj.EffectiveDate = objResult.EffectiveDate != null ? objResult.EffectiveDate.ToString("dd/MM/yyyy") : "";
						obj.Description = objResult.Description;
						obj.BankRate = objResult.BankRate;
						obj.CoyRate = objResult.CoyRate;
						obj.RiskSpring = objResult.RiskSpring;
						obj.RiskEthoz = objResult.RiskEthoz;
						obj.RepaymentPeriodFrom = objResult.RepaymentPeriodFrom;
						obj.RepaymentPeriodTo = objResult.RepaymentPeriodTo;
						obj.Remarks = objResult.Remarks;
						obj.CreatedBy = objResult.CreatedBy;
						obj.CreatedDate = objResult.CreatedDate != null ? Convert.ToDateTime(objResult.CreatedDate).ToString("dd/MM/yyyy") : "";
						obj.UpdatedBy = objResult.UpdatedBy;
                        obj.UpdatedDate = objResult.UpdatedDate != null ? Convert.ToDateTime(objResult.UpdatedDate).ToString("dd/MM/yyyy") : "";
						obj.ActiveStatus = objResult.Status;
						obj.DeactivationRemarks = objResult.DeactivationRemarks;
					}
				}
				catch (Exception ex)
				{
					var result = new ResultViewModel();
					glog.Error("GetLEFSInterestCodeById Exception: " + ex.Message);
					result.Status = 0;
					result.Message = "Please contact MIS, error: " + ex.Message;
				}
			}
			return obj;

		}

		public List<LEFSInterestCodeList> GetLEFSInterestCodeList(LEFSInterestCodeParam Param)
		{
			using (var db = new MainDbContext())
			{
				return db.Database.SqlQuery<LEFSInterestCodeList>(
					"exec GetLEFSInterestCodeList @Status,@InterestCode,@InterestType,@ContractType",
					new SqlParameter("@Status", string.IsNullOrWhiteSpace(Param.Status) ? "" : Param.Status),
					new SqlParameter("@InterestCode", string.IsNullOrWhiteSpace(Param.InterestCode) ? "" : Param.InterestCode),
					new SqlParameter("@InterestType", string.IsNullOrWhiteSpace(Param.InterestType) ? "" : Param.InterestType),
					new SqlParameter("@ContractType", string.IsNullOrWhiteSpace(Param.ContractType) ? "" : Param.ContractType)
				).ToList();
			}
		}
	}
}