using EthozCapital.CustomLibraries;
using EthozCapital.Data;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EthozCapital.CustomLibraries.ControllerClass
{
	public class clsPayment
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(clsPayment));
		private clsGlobal _clsGlobal;
		private clsCRM _clsCRM;

		public clsPayment()
		{
			_clsGlobal = new clsGlobal();
			_clsCRM = new clsCRM();
		}

		public SpotterFeeViewModel FnPopulateSpotterFee(string spotterRefNum)
		{
			glog.Debug("FnPopulateSpotterFee: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				try
				{
					var SpotterFee = new SpotterFeeViewModel();
					SpotterFee.SpotterSummary = new SpotterSummaryViewModel();
					SpotterFee.SpotterDetails = db.Database.SqlQuery<SpotterDetailsViewModel>(
					"exec GetSpotterDetails"
						).ToList();

					SpotterFee.SpotterDetails.Select(c => { c.SumOfAmount = Math.Round((decimal)c.SumOfAmount, 2); return c; }).ToList();
					glog.Debug("FnPopulateSpotterFee: Exit");
					return SpotterFee;
				}
				catch (Exception ex)
				{
					glog.Error("FnPopulateSpotterFee Exception: " + ex.Message + ex.InnerException);
					result.Status = 0;
					result.Message = "Please contact MIS, error: " + ex.Message;
					return new SpotterFeeViewModel();
				}
			}
		}

		public SpotterSummaryViewModel GetSpotterMaster(string spotterRefNum)
		{
			glog.Debug("GetSpotterMaster: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				try
				{
					var SpotterSummary = new SpotterSummaryViewModel();
					var Summary = db.Spotter_Master.Where(w => w.SpotterRefNumber == spotterRefNum).FirstOrDefault();
					if (Summary != null)
					{
						SpotterSummary.Amount = Math.Round((decimal)Summary.Amount, 2);
						SpotterSummary.PreparationDate = Summary.PreparationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
						SpotterSummary.SpotterID = Summary.SpotterID;
						SpotterSummary.Status = Summary.Status;
						SpotterSummary.CreatedBy = Summary.CreatedBy;
						SpotterSummary.CreatedDate = Summary.CreatedDate.ToString();
					}

					return SpotterSummary;
				}
				catch (Exception ex)
				{
					glog.Error("GetSpotterMaster Exception: " + ex.Message + ex.InnerException);
					result.Status = 0;
					result.Message = "Please contact MIS, error: " + ex.Message;
					return new SpotterSummaryViewModel();
				}
			}
		}

		public List<OutstandingSpotterFeeViewModel> FnRetrieveOutstandingSpotterFee(string dtPreparationDate)
		{
			glog.Debug("FnRetrieveOutstandingSpotterFee: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				try
				{
					var OutstandingSpotterFee = new List<OutstandingSpotterFeeViewModel>();
					
					OutstandingSpotterFee = db.Database.SqlQuery<OutstandingSpotterFeeViewModel>(
					"exec RetrieveOutstandingSpotterFee @OrixDB_Name, @AsatDate",
					new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
					new SqlParameter("@AsatDate", string.IsNullOrWhiteSpace(dtPreparationDate) ? "" : DateTime.ParseExact(dtPreparationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"))
					).ToList();

					OutstandingSpotterFee.Select(c => { c.SpotterAmt = Math.Round((decimal)c.SpotterAmt, 2); return c; }).ToList();

					glog.Debug("FnRetrieveOutstandingSpotterFee: Exit");
					return OutstandingSpotterFee;
				}
				catch (Exception ex)
				{
					glog.Error("FnRetrieveOutstandingSpotterFee Exception: " + ex.Message + ex.InnerException);
					result.Status = 0;
					result.Message = "Please contact MIS, error: " + ex.Message;
					return new List<OutstandingSpotterFeeViewModel>();
				}
			}
		}

		public List<SpotterContractViewModel> FnPopulateContractDetails(string refferId)
		{
			glog.Debug("FnCalculateSummary: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var SpotterContract = new List<SpotterContractViewModel>();

						SpotterContract = db.Database.SqlQuery<SpotterContractViewModel>(
						"exec GetSpotterContractDetails @RefferId",
						new SqlParameter("@RefferId", string.IsNullOrWhiteSpace(refferId) ? "" : refferId)
							).ToList();
						glog.Debug("FnPopulateSpotterFee: Exit");
						return SpotterContract;
					}
					catch (Exception ex)
					{
						glog.Error("FnCalculateSummary Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						return new List<SpotterContractViewModel>();
					}
					finally
					{
						transaction.Dispose();

					}
				}
			}
		}

		public ResultViewModel InsertSpotterData(SpotterViewModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertSpotterData: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("Payment", "SFP", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Spotter_Master spotterMaster = new Spotter_Master()
						{
							SpotterID = model.SpotterId,
							SpotterRefNumber = model.SpotterId == 0 ? NewId.NewId : model.SpotterRefNumber,
							PreparationDate = DateTime.ParseExact(model.PreparationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
							ApprovedAmount = (decimal)0.00,
							Amount = model.Amount,
							Status = "P",
							CreatedBy = model.SpotterId == 0 ? userMail : model.CreatedBy,
							CreatedDate = model.SpotterId == 0 ? DateTime.Now : Convert.ToDateTime(model.CreatedDate),
							UpdatedBy = model.SpotterId == 0 ? null : userMail,
							UpdatedDate = model.SpotterId == 0 ? (DateTime?)null : DateTime.Now,
						};
						db.Spotter_Master.Add(spotterMaster);
						if (model.SpotterId == 0)
						{
							db.Entry(spotterMaster).State = System.Data.Entity.EntityState.Added;
						}
						else
						{
							db.Entry(spotterMaster).State = System.Data.Entity.EntityState.Modified;
						}


						var Id = db.SaveChanges();

						#region Spotter Details
						var spotterId = 0;
						foreach (var data in model.SpotterContractDetails)
						{
							Spotter_Detail spotter = new Spotter_Detail()
							{
								SpotterDetailId = data.SpotterDetailId,
								ApprovedInd = "P",
								SpotterId = model.SpotterId == 0 ? spotterMaster.SpotterID : model.SpotterId,
								ContractNumber = data.ContractNumber,
								RolloverNumber = data.RolloverNumber,
								ItemNumber = data.ItemNumber,
								Valid = data.Valid,
								CreatedBy = model.SpotterId == 0 ? userMail : model.CreatedBy,
								CreatedDate = model.SpotterId == 0 ? DateTime.Now : Convert.ToDateTime(model.CreatedDate),
								UpdatedBy = model.SpotterId == 0 ? null : userMail,
								UpdatedDate = model.SpotterId == 0 ? (DateTime?)null : DateTime.Now,
							};
							db.Spotter_Detail.Add(spotter);
							if (model.SpotterId == 0)
							{
								db.Entry(spotter).State = EntityState.Added;
							}
							else
							{
								db.Entry(spotter).State = EntityState.Modified;
							}

						}
						spotterId = db.SaveChanges();
						#endregion

						#region Contract Spotter
						foreach (var data in model.SpotterContractDetails)
						{
							Contract_Spotter contractSpotter = new Contract_Spotter()
							{
								ContractNumber = data.ContractNumber,
								RolloverNumber = data.RolloverNumber,
								ItemNumber = data.ItemNumber,
								SpotterAmt = data.SpotterAmt,
								Status = data.Status,
								ApprovedInd = data.ApprovedInd,
								Referral = data.ReferralID,
								CreatedBy = data.CreatedBy,
								CreatedDate = data.CreatedDate,
								UpdatedBy = userMail,
								UpdatedDate = DateTime.Now,
							};
							db.Contract_Spotter.Add(contractSpotter);
							db.Entry(contractSpotter).State = System.Data.Entity.EntityState.Modified;
						}
						var contractId = db.SaveChanges();
						#endregion

						if (model.SpotterId == 0)
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}
						if (model.SpotterId != 0)
						{
							_clsGlobal.RemoveLockRecord("Spotter", spotterMaster.SpotterRefNumber, UserName);
						}
						if (spotterId > 0)
						{
							transaction.Commit();
							var isEmailSend = SendEmailNotificationAPI(model, NewId.NewId, UserName, userMail);
							if (model.SpotterId == 0)
							{
								result.Status = 1;
								result.Message = "Data had been submitted for approval!";
							}
							else
							{
								result.Status = 1;
								result.Message = String.Format("{0}  updated successfully!", spotterMaster.SpotterRefNumber);
							}
						}
						else
						{
							transaction.Rollback();
							if (model.SpotterId == 0)
							{
								result.Status = 0;
								result.Message = "Error occurred when submitting for approval.";
							}
							else
							{
								result.Status = 1;
								result.Message = String.Format("Error occurred when updating {0}!", spotterMaster.SpotterRefNumber);

							}
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertSpotterData Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertSpotterData: Exit");
					return result;
				}
			}
		}

		private string GenerateHtmlForEmail(SpotterViewModel model, string SpotterRefNumber, string UserName)
		{
			string html = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplate/Spotter_Email.html"));
			StringBuilder tablerows = new StringBuilder();
			int count = 0;
			decimal sumOfAmount = (decimal)0.00;
			foreach (var item in model.SpotterContractDetails)
			{
				if (item.Valid == "Y")
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
			}
			#region bottom row
			tablerows = tablerows.Append("<TR>");
			tablerows = tablerows.Append("<TD COLSPAN=3 ALIGN=RIGHT WIDTH=58% style='font-family: Verdana, Arial, Helvetica, sans-serif;font-size: 10px;color: #000000;font-weight: bold;'><FONT COLOR=BLUE>TOTAL CLAIMS</FONT></TD>");
			tablerows = tablerows.Append("<TD ALIGN=RIGHT WIDTH=10% style='font-family: Verdana, Arial, Helvetica, sans-serif;font-size: 10px;color: #000000;font-weight: bold;'><FONT COLOR=BLUE>" + sumOfAmount + "</FONT></TD>");
			tablerows = tablerows.Append("</TR>");
			#endregion
			StringBuilder sb = new StringBuilder(html);
			sb = sb.Replace("@preparationDate", model.PreparationDate.ToString());
			sb = sb.Replace("@CreatedBy", UserName);
			sb = sb.Replace("@RefNo", SpotterRefNumber);
			sb = sb.Replace("@ApprovingOfficer", UserName);
			sb = sb.Replace("@TableRows", tablerows.ToString());
			return sb.ToString();
		}

		public bool SendEmailNotificationAPI(SpotterViewModel model, string SpotterRefNumber, string UserName, string UserId)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(ConfigurationManager.AppSettings["SitePathAPI"]);

				string html = GenerateHtmlForEmail(model, SpotterRefNumber, UserName);

				EmailViewModel emailModel = new EmailViewModel()
				{
					MailType = clsVariables.SpotterMailType,
					EmailTo = ConfigurationManager.AppSettings["EmailTo"],
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

		public List<string> GetSpotterRefNumber()
		{
			glog.Debug("GetSpotterRefNumber: Entry");
			var result = new List<string>();
			using (var db = new MainDbContext())
			{
				try
				{
					var SpotterRefNum = new List<string>();
					SpotterRefNum = db.Spotter_Master.Where(w => w.Status == "P").Select(i => i.SpotterRefNumber).ToList();
					glog.Debug("GetSpotterRefNumber: Exit");
					return SpotterRefNum;
				}
				catch (Exception ex)
				{
					glog.Error("GetSpotterRefNumber Exception: " + ex.Message + ex.InnerException);
					return new List<string>();
				}
			}
		}

		public List<OutstandingSpotterFeeViewModel> RetrieveSpotterFeeByRefNumber(string refNumber, string strStatus)
		{
			glog.Debug("RetrieveSpotterFeeByRefNumber: Entry");
			var spotterFee = new List<OutstandingSpotterFeeViewModel>();
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(ConfigurationManager.AppSettings["SitePathAPI"]);
				var responseTask = client.GetAsync("Payment?SpotterRefNum=" + refNumber +"&strStatus="+ strStatus);

				responseTask.Wait();
				var result = responseTask.Result;
				if (result.IsSuccessStatusCode)
				{
					var readTask = result.Content.ReadAsAsync<List<OutstandingSpotterFeeViewModel>>();
					readTask.Wait();

					spotterFee = readTask.Result;

				}
				else //web api sent error response 
				{
					//log response status here..
					glog.Debug("RetrieveSpotterFeeByRefNumber: Exit");
					spotterFee = new List<OutstandingSpotterFeeViewModel>();

				}
				return spotterFee;
			}
		}
	}
}