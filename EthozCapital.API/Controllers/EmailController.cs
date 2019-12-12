using EthozCapital.API.CustomLibraries.ControllerClass;
using EthozCapital.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace EthozCapital.API.Controllers
{
    public class EmailController : ApiController
	{
		private EmailSender _emailSender;

		public EmailController()
		{
			_emailSender = new EmailSender();
		}

		
		public IHttpActionResult SendEmail(EmailViewModel emailModel)
		{
			var result = _emailSender.FnEmailNotification(emailModel.EmailTo, emailModel.CcEmail, emailModel.EmailFrom, emailModel.Subject, emailModel.body);
			if (result)
			{
				_emailSender.InsertEmailNotification(emailModel.MailType, emailModel.EmailTo, emailModel.CcEmail, emailModel.EmailFrom, emailModel.Subject, emailModel.body, emailModel.UserId);
				HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
				return Ok();
			}
			else
			{
				HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
				return Ok();
			}
		}

    }
}