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
	public class PaymentController : ApiController
	{
		private Payment _payment;

		public PaymentController()
		{
			_payment = new Payment();
		}

		public IHttpActionResult Get([FromUri]string SpotterRefNum, string strStatus)
		{
			var result = _payment.RetrieveSpotterFeebyRefNum(SpotterRefNum, strStatus);

			HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
			return Ok(result);
		}
	}
}
