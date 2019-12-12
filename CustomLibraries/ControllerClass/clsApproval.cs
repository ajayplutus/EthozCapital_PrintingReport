using EthozCapital.Data.OrixEss;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EthozCapital.CustomLibraries.ControllerClass
{
	public class clsApproval
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(clsSecurity));
		private clsPayment _clsPayment;

		public clsApproval()
		{
			_clsPayment = new clsPayment();
		}

		public string GetCurrentAssignToCode(string UserName)
		{
			string assignTo = "";
			glog.Debug("GetCurrentAssignToCode: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				try
				{
					assignTo = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).EmployeeCode;

				}
				catch (Exception ex)
				{
					glog.Error("GetCurrentAssignToCode Exception: " + ex.Message + ex.InnerException);
				}
				glog.Debug("GetCurrentAssignToCode: Exit");
				return assignTo;
			}
		}

		public PendingApprovalViewModel GetApproval(string AssignedTo, string BatchNo, string DateFrom, string DateTo, string Type, string ApprovalPage)
		{
			PendingApprovalViewModel obj = new PendingApprovalViewModel();
			using (var db = new MainDbContext())
			{
				try
				{
					glog.Debug("GetApproval: Entry");
					var pendingApproval = db.Database.SqlQuery<PendingApproval>(
					"exec RetrievePendingApproval @AssignedTo,@BatchNo,@DateFrom,@DateTo,@Type,@ApprovalPage",
					new SqlParameter("@AssignedTo", string.IsNullOrWhiteSpace(AssignedTo) ? "" : AssignedTo),
					new SqlParameter("@BatchNo", string.IsNullOrWhiteSpace(BatchNo) ? "" : BatchNo),
					new SqlParameter("@Type", string.IsNullOrWhiteSpace(Type) ? "" : Type),
					new SqlParameter("@DateFrom", string.IsNullOrWhiteSpace(DateFrom) ? "" : DateTime.ParseExact(DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")),
					new SqlParameter("@DateTo", string.IsNullOrWhiteSpace(DateTo) ? "" : DateTime.ParseExact(DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")),
					new SqlParameter("@ApprovalPage", string.IsNullOrWhiteSpace(ApprovalPage) ? "" : ApprovalPage)
					).ToList();

					obj.PendingApproval = pendingApproval;
					obj.PendingCount = pendingApproval.Count;
				}
				catch (Exception ex)
				{
					var result = new ResultViewModel();
					glog.Error("GetApproval Exception: " + ex.Message);
					result.Status = 0;
					result.Message = "Please contact MIS, error: " + ex.Message;
					glog.Debug("GetApproval: Exit");
				}
			}
			glog.Debug("GetApproval: Exit");
			return obj;
		}

		public ApprovalSpotterDetails GetApprovalSpotterDetails(string SpotterRefNo, string strStatus)
		{
			ApprovalSpotterDetails obj = new ApprovalSpotterDetails();
			using (var db = new MainDbContext())
			{
				try
				{
					glog.Debug("GetApprovalSpotterDetails: Entry");

					var SpotterSummary = new SpotterSummaryViewModel();
					var Summary = db.Spotter_Master.Where(w => w.SpotterRefNumber == SpotterRefNo).FirstOrDefault();
					var Reason = db.Spotter_Detail.Where(w => w.SpotterId == Summary.SpotterID).FirstOrDefault().RejectionReason;
					if (Summary != null)
					{
						obj.SpotterSummary.SpotterRefNumber = Summary.SpotterRefNumber;
						obj.SpotterSummary.Amount = Math.Round((decimal)Summary.Amount, 2);
						obj.SpotterSummary.ApprovedAmount = Math.Round((decimal)Summary.ApprovedAmount, 2);
						obj.SpotterSummary.PreparationDate = Summary.PreparationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
						obj.SpotterSummary.SpotterID = Summary.SpotterID;
						obj.SpotterSummary.Status = Summary.Status;
						obj.SpotterSummary.CreatedBy = Summary.CreatedBy;
						obj.SpotterSummary.CreatedDate = Summary.CreatedDate.ToString();
						obj.SpotterSummary.RejectReason = Reason;
					}

					var outstandingSpotterFee = new List<OutstandingSpotterFeeViewModel>();
					outstandingSpotterFee = _clsPayment.RetrieveSpotterFeeByRefNumber(SpotterRefNo, strStatus);

					obj.OutstandingFee = outstandingSpotterFee;
				}
				catch (Exception ex)
				{
					var result = new ResultViewModel();
					glog.Error("GetApprovalSpotterDetails Exception: " + ex.Message);
					result.Status = 0;
					result.Message = "Please contact MIS, error: " + ex.Message;
					glog.Debug("GetApprovalSpotterDetails: Exit");
				}
			}
			glog.Debug("GetApprovalSpotterDetails: Exit");
			return obj;
		}
		
		public ResultViewModel FnApproveRejectEvent(ApprovalSpotterDetails model, string UserName)
		{
			string userMail = "";
			int maxTier = 0, currentTier = 0;
			glog.Debug("InsertSpotterData: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						bool IsSave = false;

						if (!string.IsNullOrEmpty(model.RejectReason))
						{
							var RejectedAmount = model.OutstandingFee.Select(s => s.SpotterAmt).Sum();
							var ApprovedAmount = model.SpotterSummary.Amount - RejectedAmount;
							if (FnUpdateCurrentApproval(db, model.IsAllCheck, model.ApprovalProcessID, model.RejectReason, userMail))
							{
								if (FnUpdateSpotterMaster(db, model.IsAllCheck, model.SpotterSummary.SpotterRefNumber, ApprovedAmount, model.RejectReason, userMail))
								{
									if (FnUpdateSpotterDetail(db, model.OutstandingFee, model.ApprovalProcessDetailID, model.RejectReason, userMail))
									{
										if (FnUpdateContractSpotter(db, model.OutstandingFee, model.ApprovalProcessDetailID, model.RejectReason, userMail))
										{
											IsSave = true;
										}
										else
										{
											IsSave = false;
										}
									}
									else
									{
										IsSave = false;
									}
								}
								else
								{
									IsSave = false;
								}
							}
							else
							{
								IsSave = false;
							}
						}
						else
						{
							maxTier = db.Sys_Approval.Where(w => w.ApprovalHeaderID == model.ApprovalHeaderID).Select(s => s.Tier).FirstOrDefault();
							currentTier = db.Approval_ProcessDetail.Where(w => w.ApprovalProcessID == model.ApprovalProcessID).Select(s => s.Tier).Max();
							var ApprovedAmount = model.OutstandingFee.Select(s => s.SpotterAmt).Sum();
							if (currentTier == maxTier)
							{
								if (FnUpdateCurrentApproval(db, model.IsAllCheck, model.ApprovalProcessID, model.RejectReason, userMail))
								{
									if (FnUpdateSpotterMaster(db, model.IsAllCheck, model.SpotterSummary.SpotterRefNumber, ApprovedAmount, model.RejectReason, userMail))
									{
										if (FnUpdateSpotterDetail(db, model.OutstandingFee, model.ApprovalProcessDetailID, model.RejectReason, userMail))
										{
											if (FnUpdateContractSpotter(db, model.OutstandingFee, model.ApprovalProcessDetailID, model.RejectReason, userMail))
											{
												IsSave = true;
											}
											else
											{
												IsSave = false;
											}
										}
										else
										{
											IsSave = false;
										}
									}
									else
									{
										IsSave = false;
									}
								}
								else
								{
									IsSave = false;
								}
							}
							else
							{
								var sysApproval = db.Sys_Approval.Where(w => w.ApprovalHeaderID == model.ApprovalHeaderID).FirstOrDefault();
								ApprovalProcess approvalProcessModel = new ApprovalProcess()
								{
									ApprovalDetailID = model.ApprovalProcessID,
									CurrentTier = currentTier,
									ApprovalHeaderID = model.ApprovalHeaderID,
									ModuleID = sysApproval.ModuleID,
									ApprovalName = sysApproval.ApprovalName,
									RefNo = model.SpotterSummary.SpotterRefNumber,
									UserName = userMail
								};
								result = FnStartNewProcessTier(approvalProcessModel);
								if (result.Status == 1)
								{
									IsSave = true;
								}
								else
								{
									IsSave = false;
								}
							}
						}

						if (IsSave)
						{
							transaction.Commit();
							string EmpEmailId = string.Empty;
							var employeeCode = "";
							if (currentTier == maxTier)
							{
								employeeCode = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).EmployeeCode;
								EmpEmailId = GetEmailByEmpCode(employeeCode);
							}
							else
							{
								employeeCode = db.Approval_ProcessDetail.FirstOrDefault(x => x.ApprovalProcessID == model.ApprovalProcessID).AssignedTo;
								EmpEmailId = GetEmailByEmpCode(employeeCode);
							}
							var isEmailSend = SendEmailNotificationAPI(model, EmpEmailId, UserName, userMail, currentTier, maxTier, employeeCode);
							result.Status = 1;
							if (string.IsNullOrEmpty(model.RejectReason))
							{
								result.Message = "Data has been approved!";
							}
							else
							{
								result.Message = "Data has been rejected!";
							}
						}
						else
						{
							transaction.Rollback();
							result.Status = 0;
							result.Message = "An error occurred when updating approval process";
						}
					}
					catch (Exception ex)
					{
						glog.Error("FnApproveRejectEvent Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("FnApproveRejectEvent: Exit");
					return result;
				}
			}
		}

		public bool FnUpdateCurrentApproval(MainDbContext db, bool IsAllCheck, int ApprovalProcessID, string RejectReason, string userMail)
		{
			try
			{
				var approvalProcess = db.Approval_Process.Where(w => w.ApprovalProcessID == ApprovalProcessID).FirstOrDefault();
				var approvalProcessDtl = db.Approval_ProcessDetail.Where(w => w.ApprovalProcessID == approvalProcess.ApprovalProcessID && w.ApprovedDate == null && w.RejectedDate == null).FirstOrDefault();
				var Id = 0;
				if (string.IsNullOrEmpty(RejectReason))
				{

					approvalProcess.Status = IsAllCheck ? "A" : "W";
					approvalProcess.UpdatedDate = DateTime.Now;
					approvalProcess.UpdatedBy = userMail;
					approvalProcess.ApprovedDate = DateTime.Now;
					approvalProcess.ApprovedBy = userMail;

					db.Approval_Process.Add(approvalProcess);
					db.Entry(approvalProcess).State = System.Data.Entity.EntityState.Modified;
					Id = db.SaveChanges();

					if (approvalProcessDtl != null && Id > 0)
					{
						approvalProcessDtl.ApprovedDate = DateTime.Now;
						approvalProcessDtl.ApprovedBy = userMail;

						db.Approval_ProcessDetail.Add(approvalProcessDtl);
						db.Entry(approvalProcessDtl).State = System.Data.Entity.EntityState.Modified;
						var dtlId = db.SaveChanges();
						if (dtlId < 1)
						{
							Id = 0;
						}
					}
				}
				else
				{
					approvalProcess.Status = IsAllCheck ? "R" : "W";
					approvalProcess.UpdatedDate = DateTime.Now;
					approvalProcess.UpdatedBy = userMail;
					approvalProcess.RejectedDate = DateTime.Now;
					approvalProcess.RejectedBy = userMail;

					db.Approval_Process.Add(approvalProcess);
					db.Entry(approvalProcess).State = System.Data.Entity.EntityState.Modified;
					Id = db.SaveChanges();

					if (approvalProcessDtl != null && Id > 0)
					{
						approvalProcessDtl.RejectedDate = DateTime.Now;
						approvalProcessDtl.RejectedBy = userMail;

						db.Approval_ProcessDetail.Add(approvalProcessDtl);
						db.Entry(approvalProcessDtl).State = System.Data.Entity.EntityState.Modified;
						var dtlId = db.SaveChanges();
						if (dtlId != 0)
						{
							Id = 0;
						}
					}
				}

				if (Id > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				glog.Error("FnUpdateCurrentApproval Exception: " + ex.Message + ex.InnerException);
				return false;
				throw;
			}

		}

		public bool FnUpdateSpotterMaster(MainDbContext db, bool IsAllCheck, string SpotterRefNumber, decimal ApprovedAmount, string RejectReason, string userMail)
		{
			try
			{
				var spotterMaster = db.Spotter_Master.Where(w => w.SpotterRefNumber == SpotterRefNumber).FirstOrDefault();

				spotterMaster.Status = IsAllCheck ? "D" : "W";
				spotterMaster.ApprovedAmount = ApprovedAmount;
				spotterMaster.UpdatedDate = DateTime.Now;
				spotterMaster.UpdatedBy = userMail;

				db.Spotter_Master.Add(spotterMaster);
				db.Entry(spotterMaster).State = EntityState.Modified;

				var Id = db.SaveChanges();
				if (Id > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				glog.Error("FnUpdateCurrentApproval Exception: " + ex.Message + ex.InnerException);
				return false;
				throw;
			}

		}

		public bool FnUpdateSpotterDetail(MainDbContext db, List<OutstandingSpotterFeeViewModel> SpotterContractDetails, int ApprovalProcessDetailID, string RejectReason, string userMail)
		{
			try
			{
				if (string.IsNullOrEmpty(RejectReason))
				{
					foreach (var data in SpotterContractDetails)
					{
						var spotterDtl = db.Spotter_Detail.Where(w => w.SpotterDetailId == data.SpotterDetailId).FirstOrDefault();

						spotterDtl.ApprovedInd = "A";
						spotterDtl.ApprovedBy = userMail;
						spotterDtl.ApprovalDate = DateTime.Now;
						spotterDtl.UpdatedBy = userMail;
						spotterDtl.UpdatedDate = DateTime.Now;

						db.Spotter_Detail.Add(spotterDtl);
						db.Entry(spotterDtl).State = EntityState.Modified;
					}
				}
				else
				{
					foreach (var data in SpotterContractDetails)
					{
						var spotterDtl = db.Spotter_Detail.Where(w => w.SpotterDetailId == data.SpotterDetailId).FirstOrDefault();

						spotterDtl.ApprovedInd = "R";
						spotterDtl.RejectedBy = userMail;
						spotterDtl.RejctedDate = DateTime.Now;
						spotterDtl.RejectionReason = RejectReason;
						spotterDtl.UpdatedBy = userMail;
						spotterDtl.UpdatedDate = DateTime.Now;

						db.Spotter_Detail.Add(spotterDtl);
						db.Entry(spotterDtl).State = EntityState.Modified;
					}
				}
				var Id = db.SaveChanges();
				if (Id > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				glog.Error("FnUpdateSpotterDetail Exception: " + ex.Message + ex.InnerException);
				return false;
				throw;
			}
		}

		public bool FnUpdateContractSpotter(MainDbContext db, List<OutstandingSpotterFeeViewModel> SpotterContractDetails, int ApprovalProcessDetailID, string RejectReason, string userMail)
		{
			try
			{
				if (string.IsNullOrEmpty(RejectReason))
				{
					foreach (var data in SpotterContractDetails)
					{
						var contractSpotter = db.Contract_Spotter.Where(w => w.ContractNumber == data.ContractNumber.Trim() && w.RolloverNumber == data.RolloverNumber && w.ItemNumber == data.ItemNumber).FirstOrDefault();
						if (contractSpotter != null)
						{
							contractSpotter.ApprovedInd = "A";
							contractSpotter.UpdatedBy = userMail;
							contractSpotter.UpdatedDate = DateTime.Now;

							db.Contract_Spotter.Add(contractSpotter);
							db.Entry(contractSpotter).State = EntityState.Modified;
						}
					}
				}
				else
				{
					foreach (var data in SpotterContractDetails)
					{
						var contractSpotter = db.Contract_Spotter.Where(w => w.ContractNumber == data.ContractNumber.Trim() && w.RolloverNumber == data.RolloverNumber && w.ItemNumber == data.ItemNumber).FirstOrDefault();
						if (contractSpotter != null)
						{
							contractSpotter.ApprovedInd = "P";
							contractSpotter.UpdatedBy = userMail;
							contractSpotter.UpdatedDate = DateTime.Now;

							db.Contract_Spotter.Add(contractSpotter);
							db.Entry(contractSpotter).State = EntityState.Modified;
						}
					}
				}
				var Id = db.SaveChanges();
				if (Id > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				glog.Error("FnUpdateContractSpotter Exception: " + ex.Message + ex.InnerException);
				return false;
				throw;
			}
		}

		public ResultViewModel FnStartNewProcessTier(ApprovalProcess model)
		{
			var result = new ResultViewModel();
			try
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(ConfigurationManager.AppSettings["SitePathAPI"]);
					var responseTask = client.PostAsJsonAsync<ApprovalProcess>("FnStartNewProcessTier", model);

					responseTask.Wait();
					var responseResult = responseTask.Result;
					if (responseResult.IsSuccessStatusCode)
					{
						var readTask = responseResult.Content.ReadAsAsync<ResultViewModel>();
						readTask.Wait();
						result = readTask.Result;
					}
					else
					{
						glog.Debug("FnStartNewProcessTier: Exit");
						result.Status = 0;
						result.Message = "An error occurred when updating approval process!";
					}
				}
				return result;
			}
			catch (Exception ex)
			{
				glog.Error("FnStartNewProcessTier Exception: " + ex.Message + ex.InnerException);
				return result;
				throw;
			}
		}

		public ResultViewModel FnUpdateCurrentAssignedTo(ApprovalSpotterDetails model, string UserName)
		{
			string userMail = "";
			glog.Debug("FnUpdateCurrentAssignedTo: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						if (FnGetApprovingOfficerAvailbility(model.AssignedTo))
						{
							userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;

							var approvalProcessDtl = db.Approval_ProcessDetail.Where(w => w.ApprovalProcessID == model.ApprovalProcessDetailID).FirstOrDefault();
							approvalProcessDtl.AssignedTo = model.AssignedTo;
							approvalProcessDtl.ReassignDate = DateTime.Now;
							approvalProcessDtl.ReassignBy = userMail;
							approvalProcessDtl.ReassignReason = model.ReassignReason;

							db.Approval_ProcessDetail.Add(approvalProcessDtl);
							db.Entry(approvalProcessDtl).State = System.Data.Entity.EntityState.Modified;
							var Id = db.SaveChanges();
							if (Id > 0)
							{
								transaction.Commit();
								string EmpEmailId = GetEmailByEmpCode(model.AssignedTo);
								var isEmailSend = SendReassignEmailNotificationAPI(model, EmpEmailId, UserName, userMail, model.AssignedTo);
								result.Status = 1;
								result.Message = String.Format("Approval reassign to {0}", GetApprovingOfficerNameByCode(model.AssignedTo));
							}
							else
							{
								transaction.Rollback();
								result.Status = 0;
								result.Message = "An error occurred while reassign task!";
							}
						}
						else
						{
							result.Status = 2;
							result.Message = "Selected approving officer is currently unavailable. Please select another approving officer.Thanks";
						}
					}
					catch (Exception ex)
					{
						glog.Error("FnUpdateCurrentAssignedTo Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("FnUpdateCurrentAssignedTo: Exit");
					return result;
				}
			}
		}

		public List<string> GetApprovalName()
		{
			glog.Debug("GetSpotterRefNumber: Entry");
			var result = new List<string>();
			using (var db = new MainDbContext())
			{
				try
				{
					var SpotterRefNum = new List<string>();
					SpotterRefNum = db.Sys_Approval.Select(i => i.ApprovalName).ToList();
					glog.Debug("GetApprovalName: Exit");
					return SpotterRefNum;
				}
				catch (Exception ex)
				{
					glog.Error("GetApprovalName Exception: " + ex.Message + ex.InnerException);
					return new List<string>();
				}
			}
		}

		public List<SelectListItem> FnRetriveApprovingOfficer()
		{
			glog.Debug("FnRetriveApprovingOfficer: Entry");
			List<SelectListItem> result = new List<SelectListItem>();
			using (var db = new MainDbContext())
			{
				try
				{
					var approvingOfficer = new List<string>();
					//approvingOfficer = db.Approval_ProcessDetail.Select(i => i.AssignedTo).Distinct().ToList();
					approvingOfficer = db.Database.SqlQuery<string>(
					"exec GetApprovingOfficer"
					).ToList();
					if (approvingOfficer != null)
					{
						result = approvingOfficer.Select(x => new SelectListItem()
						{
							Text = GetApprovingOfficerNameByCode(x),
							Value = x,
						}).ToList();

					}
					glog.Debug("FnRetriveApprovingOfficer: Exit");
					return result;
				}
				catch (Exception ex)
				{
					glog.Error("FnRetriveApprovingOfficer Exception: " + ex.Message + ex.InnerException);
					return result;
				}
			}
		}

		public List<CommonDropDown> GetBatchNo(string sourcePage, string textFilter = null)
		{
			using (var db = new MainDbContext())
			{
				glog.Debug("GetBatchNo: Exit");
				List<CommonDropDown> result = new List<CommonDropDown>();
				try
				{
					if (sourcePage == "IsHistory")
					{
						result = db.Approval_Process.Where(w => w.Status != "P" && w.CompositeKey1.StartsWith(textFilter)).Select(x => new CommonDropDown()
						{
							label = x.CompositeKey1,
							value = x.CompositeKey1
						}).ToList();
					}
					else if (sourcePage == "IsReassign")
					{

						result = db.Approval_Process.Where(w => w.Status == "P" && w.CompositeKey1.StartsWith(textFilter)).Select(x => new CommonDropDown()
						{
							label = x.CompositeKey1,
							value = x.CompositeKey1
						}).ToList();
					}
				}
				catch (Exception ex)
				{
					glog.Error("GetBatchNo Exception: " + ex.Message + ex.InnerException);
					throw;
				}
				glog.Debug("GetBatchNo: Exit");
				return result;
			}
		}

		public bool FnGetApprovingOfficerAvailbility(string ReassignTo)
		{

			glog.Debug("FnGetApprovingOfficerAvailbility: Entry");
			var result = false;
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(ConfigurationManager.AppSettings["SitePathAPI"]);
				var responseTask = client.GetAsync("Approval/GetApprovingOfficerAvailbility?AssignTo=" + ReassignTo);

				responseTask.Wait();
				var responseResult = responseTask.Result;

				if (responseResult.IsSuccessStatusCode)
				{
					var readTask = responseResult.Content.ReadAsAsync<bool>();
					readTask.Wait();
					if (readTask.Result)
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
				else //web api sent error response 
				{
					glog.Error("FnGetApprovingOfficerAvailbility Response status: " + responseResult.StatusCode);
					glog.Debug("FnGetApprovingOfficerAvailbility: Exit");
				}
				return result;
			}
		}

		public bool SendEmailNotificationAPI(ApprovalSpotterDetails model, string empEmailId, string UserName, string UserId, int currentTier, int maxTier, string employeeCode)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(ConfigurationManager.AppSettings["SitePathAPI"]);

				var emailModel = new EmailViewModel();
				if (!String.IsNullOrEmpty(model.RejectReason))
				{
					string html = GenerateHtmlForEmailApprovedReject(model, clsVariables.Reject, UserName, employeeCode);
					emailModel = new EmailViewModel()
					{
						MailType = clsVariables.ApprovalMailType,
						EmailTo = empEmailId,
						EmailFrom = UserId,
						CcEmail = "",
						Subject = clsVariables.SpotterMailSubject,
						body = html,
						UserId = UserId,
					};
				}
				else
				{
					if (currentTier == maxTier)
					{
						string html = GenerateHtmlForEmailApprovedReject(model, clsVariables.Approved, UserName, employeeCode);
						emailModel = new EmailViewModel()
						{
							MailType = clsVariables.ApprovalMailType,
							EmailTo = empEmailId,
							EmailFrom = UserId,
							CcEmail = "",
							Subject = String.Format("Spotter Fee Approved {0}", model.SpotterSummary.SpotterRefNumber),
							body = html,
							UserId = UserId,
						};
					}
					else
					{
						string html = GenerateHtmlForEmailSpotterApproval(model, "", UserName, employeeCode);
						emailModel = new EmailViewModel()
						{
							MailType = clsVariables.ApprovalMailType,
							EmailTo = empEmailId,
							EmailFrom = UserId,
							CcEmail = "",
							Subject = clsVariables.SpotterMailSubject,
							body = html,
							UserId = UserId,
						};
					}
				}

				var responseTask = client.PostAsJsonAsync<EmailViewModel>("Email", emailModel);

				responseTask.Wait();

				var result = responseTask.Result;
				if (result.IsSuccessStatusCode)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public bool SendReassignEmailNotificationAPI(ApprovalSpotterDetails model, string empEmailId, string UserName, string UserId, string employeeCode)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(ConfigurationManager.AppSettings["SitePathAPI"]);
				var emailModel = new EmailViewModel();
				string html = GenerateHtmlForEmailSpotterApproval(model, "", UserName, employeeCode);
				emailModel = new EmailViewModel()
				{
					MailType = clsVariables.ApprovalMailType,
					EmailTo = empEmailId,
					EmailFrom = UserId,
					CcEmail = "",
					Subject = clsVariables.SpotterMailSubject,
					body = html,
					UserId = UserId,
				};

				var responseTask = client.PostAsJsonAsync<EmailViewModel>("Email", emailModel);

				responseTask.Wait();

				var result = responseTask.Result;
				if (result.IsSuccessStatusCode)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		private string GenerateHtmlForEmailApprovedReject(ApprovalSpotterDetails model, string Status, string UserName, string employeeCode)
		{
			string html = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplate/Approved_Rejected_HTML.html"));
			StringBuilder tablerows = new StringBuilder();
			int count = 0;
			decimal sumOfAmount = (decimal)0.00;
			foreach (var item in model.OutstandingFee)
			{
				count++;
				tablerows = tablerows.Append("<tr>");
				tablerows = tablerows.Append("<td align='CENTER' width='4%' style='font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 10px; color: #000000;font-weight: bold;'>" + count + "</td>");
				tablerows = tablerows.Append("<td align='LEFT' width='20%' style='font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 10px; color: #000000;font-weight: bold;'>" + item.ReferralName + "</td>");
				tablerows = tablerows.Append("<td align='LEFT' width='6%' style='font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 10px; color: #000000;font-weight: bold;'>" + item.ContractNumber + "</td>");
				tablerows = tablerows.Append("<td align='RIGHT' width='8%' style='font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 10px; color: #000000;font-weight: bold;'>" + item.SpotterAmt + "</td>");
				tablerows = tablerows.Append("</tr>");
				sumOfAmount = sumOfAmount + item.SpotterAmt;
			}
			#region bottom row
			tablerows = tablerows.Append("<tr>");
			tablerows = tablerows.Append("<td colspan='3' align='RIGHT' width='58%' style='font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 10px; color: #000000;font-weight: bold;'><font color='BLUE'>TOTAL CLAIMS</font></td>");
			tablerows = tablerows.Append("<td align='RIGHT' width='10%' style='font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 10px; color: #000000;font-weight: bold;'><font color='BLUE'>" + sumOfAmount + "</font></td>");
			tablerows = tablerows.Append("</tr>");
			#endregion
			StringBuilder sb = new StringBuilder(html);
			sb = sb.Replace("@preparationDate", model.SpotterSummary.PreparationDate);
			sb = sb.Replace("@RefNo", model.SpotterSummary.PreparationDate);
			sb = sb.Replace("@ApprovingOfficer", GetApprovingOfficerNameByCode(employeeCode));
			sb = sb.Replace("@Status", Status);
			sb = sb.Replace("@CreatedBy", UserName);
			sb = sb.Replace("@TableRows", tablerows.ToString());
			return sb.ToString();
		}

		private string GenerateHtmlForEmailSpotterApproval(ApprovalSpotterDetails model, string SpotterRefNumber, string UserName, string employeeCode)
		{
			string html = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplate/Spotter_Email.html"));
			StringBuilder tablerows = new StringBuilder();
			int count = 0;
			decimal sumOfAmount = (decimal)0.00;
			foreach (var item in model.OutstandingFee)
			{
				count++;
				tablerows = tablerows.Append("<TR>");
				tablerows = tablerows.Append("<TD ALIGN=CENTER WIDTH= 4% style='font-family: Verdana, Arial, Helvetica, sans-serif;font-size: 10px;color: #000000;font-weight: bold;'>" + count + "</TD>");
				tablerows = tablerows.Append("<TD ALIGN=LEFT WIDTH= 20% style='font-family: Verdana, Arial, Helvetica, sans-serif;font-size: 10px;color: #000000;font-weight: bold;'>" + item.ReferralName + "</TD>");
				tablerows = tablerows.Append("<TD ALIGN=LEFT WIDTH= 6% style='font-family: Verdana, Arial, Helvetica, sans-serif;font-size: 10px;color: #000000;font-weight: bold;'>" + item.ContractNumber + "</TD>");
				tablerows = tablerows.Append("<TD ALIGN=RIGHT WIDTH= 8%  style='font-family: Verdana, Arial, Helvetica, sans-serif;font-size: 10px;color: #000000;font-weight: bold;'>" + item.SpotterAmt + "</TD>");
				tablerows = tablerows.Append("</TR>");
				sumOfAmount = sumOfAmount + item.SpotterAmt;
			}
			#region bottom row
			tablerows = tablerows.Append("<TR>");
			tablerows = tablerows.Append("<TD COLSPAN=3 ALIGN=RIGHT WIDTH=58% style='font-family: Verdana, Arial, Helvetica, sans-serif;font-size: 10px;color: #000000;font-weight: bold;'><FONT COLOR=BLUE>TOTAL CLAIMS</FONT></TD>");
			tablerows = tablerows.Append("<TD ALIGN=RIGHT WIDTH=10% style='font-family: Verdana, Arial, Helvetica, sans-serif;font-size: 10px;color: #000000;font-weight: bold;'><FONT COLOR=BLUE>" + sumOfAmount + "</FONT></TD>");
			tablerows = tablerows.Append("</TR>");
			#endregion
			StringBuilder sb = new StringBuilder(html);
			sb = sb.Replace("@preparationDate", model.SpotterSummary.PreparationDate);
			sb = sb.Replace("@CreatedBy", UserName);
			sb = sb.Replace("@RefNo", model.SpotterSummary.SpotterRefNumber);
			sb = sb.Replace("@ApprovingOfficer", GetApprovingOfficerNameByCode(employeeCode));
			sb = sb.Replace("@TableRows", tablerows.ToString());
			return sb.ToString();
		}

		private string GetApprovingOfficerNameByCode(string code)
		{
			glog.Debug("getApprovingOfficerNameByCode: Entry");
			using (var db = new ORIX_ESS_DB_DevEntities())
			{
				try
				{
					var clientname = db.ss_emp_mas.Where(x => x.em_emp_cod == code).Select(x => x.em_sht_nam).FirstOrDefault();

					glog.Debug("getApprovingOfficerNameByCode: Exit");
					return clientname;
				}
				catch (Exception ex)
				{
					glog.Error("getApprovingOfficerNameByCode Exception: " + ex.Message);
					return "";
				}
			}
		}

		private string GetEmailByEmpCode(string code)
		{
			glog.Debug("GetEmailByEmpCode: Entry");
			using (var db = new ORIX_ESS_DB_DevEntities())
			{
				try
				{
					var clientname = db.ss_emp_mas.Where(x => x.em_emp_cod == code).Select(x => x.em_smtp_id).FirstOrDefault();

					glog.Debug("GetEmailByEmpCode: Exit");
					return clientname;
				}
				catch (Exception ex)
				{
					glog.Error("GetEmailByEmpCode Exception: " + ex.Message);
					return "";
				}
			}
		}
	}
}