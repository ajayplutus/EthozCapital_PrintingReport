using EthozCapital.API.CustomLibraries.ControllerClass;
using EthozCapital.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EthozCapital.API.Controllers
{
	public class ApprovalController : ApiController
	{
		private clsApproval _clsApproval;

		public ApprovalController()
		{
			_clsApproval = new clsApproval();
		}

		[HttpGet]
		public IHttpActionResult GetApprovingOfficerAvailbility([FromUri]string AssignTo)
		{
			var result = _clsApproval.FnGetApprovingOfficerAvailbility(AssignTo);

			HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
			return Ok(result);
		}

		[HttpGet]
		public IHttpActionResult FnStartNewProcessTier(ApprovalProcess approvalProcessModel)
		{
			var result = _clsApproval.FnStartNewProcessTier(approvalProcessModel);
			if (result != null)
			{
				HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
				return Ok(result);
			}
			else
			{
				HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
				return Ok();
			}
		}
	}
}
