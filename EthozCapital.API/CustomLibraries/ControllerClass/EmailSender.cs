using EthozCapital.API.Models.Tables;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace EthozCapital.API.CustomLibraries.ControllerClass
{
	public class EmailSender
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(EmailSender));

		public bool FnEmailNotification(string EmailTo, string ccEmail, string EmailFrom, string Subject, string body)
		{
			glog.Debug("FnEmailNotification: Entry");
			MailAddress toAddress = new MailAddress(EmailTo); //Jason.teoty@ethoz.com
			MailAddress FromAddress = new MailAddress(ConfigurationManager.AppSettings["EmailUsername"]); //Login user Email
			try
			{
				using (SmtpClient smtp = new SmtpClient())
				{
					smtp.Host = ConfigurationManager.AppSettings["EmailHost"];
					smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPort"]);
					smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EmailUseSSL"]);
					smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailUsername"], ConfigurationManager.AppSettings["EmailPassword"]);

					// MAIL MESSAGE CONFIGURATION
					MailMessage message = new MailMessage();
					message.To.Add(toAddress);
					// message.Bcc.Add(ccEmail);
					message.Subject = Subject;
					message.Body = body;
					message.IsBodyHtml = true;
					message.From = FromAddress;

					smtp.Send(message);
					glog.Debug("FnEmailNotification: Exit");
					return true;
				}
			}
			catch (SmtpFailedRecipientsException ex)
			{
				glog.Error("FnEmailNotification Exception: " + ex.Message + ex.InnerException);
				return false;
			}
		}

		public int InsertEmailNotification(string MailType, string EmailTo, string ccEmail, string EmailFrom, string Subject, string body, string userId)
		{
			glog.Debug("InsertEmailNotification: Entry");
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var NewId = db.Sys_Gentb_Mails.Select(s => s.gm_mail_num).Max();
						int gmailnum = Convert.ToInt32(NewId) + 1;
						NewId = (Convert.ToString(gmailnum)).PadLeft(15, '0');
						Sys_Gentb_Mails sysMails = new Sys_Gentb_Mails()
						{
							gm_mail_num = NewId,
							gm_mail_typ = MailType,
							gm_mail_to = EmailTo,
							gm_mail_cc = ccEmail,
							gm_mail_subject = Subject,
							gm_mail_body = body,
							gm_sta_who = userId,
							gm_sta_dat = DateTime.UtcNow,
						};
						db.Sys_Gentb_Mails.Add(sysMails);
						db.Entry(sysMails).State = System.Data.Entity.EntityState.Added;

						var Id = db.SaveChanges();
						if (Id > 0)
						{
							transaction.Commit();
						}
						glog.Debug("InsertEmailNotification: Exit");
						return Id;
					}
					catch (Exception ex)
					{
						glog.Error("InsertEmailNotification Exception: " + ex.Message + ex.InnerException);
						transaction.Rollback();
						return 0;
					}
					finally
					{
						transaction.Dispose();
					}
				}
			}
		}
	}

}
