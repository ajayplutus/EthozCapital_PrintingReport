using EthozCapital.CustomLibraries;
using EthozCapital.CustomLibraries.ControllerClass;
using EthozCapital.Data.OrixEss;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace EthozCapital.Controllers
{
	public class ReportsController : Controller
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(PaymentController));
		private clsGlobal _clsGlobal;
		private clsCRM _clsCRM;
		private clsContractGeneral _clsContractGeneral;
		private clsListOfValue _clsListOfValue;
		private clsPreCon _clsPreCon;
		private clsNumberToWord _clsNumberToWord;

		public ReportsController()
		{
			glog.Debug("ReportsController: Entry");
			_clsGlobal = new clsGlobal();
			_clsCRM = new clsCRM();
			_clsContractGeneral = new clsContractGeneral();
			_clsListOfValue = new clsListOfValue();
			_clsPreCon = new clsPreCon();
			_clsNumberToWord = new clsNumberToWord();
			glog.Debug("ReportsController: Exit");
		}

		// GET: Reports
		public ActionResult Index()
		{
			return View();
		}

		#region Printing
		public ActionResult Printing(string SubMenuId)
		{
			glog.Debug("HttpGet Printing: Entry");
			try
			{
				#region Check UserGroup when user direct key in URL
				if (!(_clsGlobal.CheckUserGroup(((ClaimsIdentity)User.Identity).FindFirst("UserGroupCode").Value, Convert.ToInt16(SubMenuId))))
				{
					ViewData["Message"] = "You Have No Access Rights For This Module!, error: Invalid Access Rights";
				}
				#endregion  Check UserGroup when user direct key in URL

				List<SelectListItem> lstOfValue = _clsGlobal.GetListOfValue("CONTRACT_REPORT_TYPE", "", "O", "", "");
				ViewBag.ReportTypeList = new SelectList(lstOfValue, "Value", "Text");
				glog.Debug("HttpGet Printing: Exit");
				return View();
			}
			catch (Exception ex)
			{
				glog.Error("Please contact MIS, error:" + ex.Message);
				return Json(new
				{
					NotificationTitle = clsGlobal.SwalTitle_Error,
					NotificationContent = "Please contact MIS, error:" + ex.Message,
					NotificationType = clsGlobal.SwalType_Error
				});
			}

		}
		#endregion

		#region Funtion GetIsContractNumberValid
		[HttpGet]
		public ActionResult FnGetIsContractNumberValid(string strContractNumber)
		{
			glog.Debug("HttpGet FnGetIsContractNumberValid: Entry");
			try
			{
				var IsContractValid = _clsContractGeneral.GetIsContractNumberValid(strContractNumber);
				if (IsContractValid == 0)
				{
					glog.Debug("HttpGet FnGetIsContractNumberValid: Exit");
					return Json("Invalid contract number inputted, please try another contract number!", JsonRequestBehavior.AllowGet);
				}
				else
				{
					glog.Debug("HttpGet FnGetIsContractNumberValid: Exit");
					return Json("Sucessfully Checked Valid Contract Number!", JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				glog.Error("Please contact MIS, error:" + ex.Message);
				var strError = "Please contact MIS, error:" + ex.Message;
				return Json(strError, JsonRequestBehavior.AllowGet);
			}
			
		}
		#endregion

		#region FnPrepareContractData
		[HttpGet]
		public ActionResult FnPrepareContractData(string strContractNumber, bool blIsLetterOfOfferChecked,bool fnSubContractTypeByPreContractNumber)
		{
			glog.Debug("HttpGet FnPrepareContractData: Entry");
			string strSubContractType = "";
			string strLogicCode = "";
			try
			{
				if (fnSubContractTypeByPreContractNumber)
				{
					strSubContractType = _clsContractGeneral.FnGetPreContractData(strContractNumber);
				}
				else
				{
					strSubContractType = _clsContractGeneral.FnGetContractData(strContractNumber);
				}

				strLogicCode = _clsGlobal.GetLogicCode("F-10004", strSubContractType, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);

				if (blIsLetterOfOfferChecked)
				{
					if (!(strLogicCode == "L-10008"))
					{
						blIsLetterOfOfferChecked = false;
					}
				}
				
			}
			catch (Exception ex)
			{
				glog.Error("Please contact MIS, error:" + ex.Message);
				var strError = "Please contact MIS, error:" + ex.Message;
				return Json(strError, JsonRequestBehavior.AllowGet);
			}
			try
			{
				GenerateTermLoanLOFViewModel _model = new GenerateTermLoanLOFViewModel();

				#region Get GroupCode For Property
				var strGroupCodeForProperty = _clsContractGeneral.GetGroupCodeByGroupMemberDesc("Property");
				#endregion

				_model.strContractNumber = strContractNumber;
				if (strContractNumber.StartsWith("T"))
				{
					_model.ContractDetails = _clsContractGeneral.GetPreContractDetailsByContractNumber(strContractNumber);
					_model.SecurityItemByProperty = _clsContractGeneral.GetPreContractSecuityItemDetailsBySecurityListLevel2AndContractNumber(strContractNumber, strGroupCodeForProperty);
					if (_model.ContractDetails != null)
					{
						if (_model.ContractDetails.UpfrontPaymentMth > 0)
						{
							_model.ContractScheduleDetails = _clsContractGeneral.GetPreContractScheduleDetailsByContractNumberAndUpfrontPaymentMth(strContractNumber, _model.ContractDetails.UpfrontPaymentMth);
						}
						else
						{
							_model.ContractScheduleDetails = _clsContractGeneral.GetPreContractScheduleDetailsByContractNumber(strContractNumber);
						}

					}
					_model.RepaymetContractScheduleDetails = _clsContractGeneral.GetPreContractRepaymentScheduleDetailsByContractNumber(strContractNumber);
					_model.WithdrawSuitIndDetails = _clsContractGeneral.GetPreContractWithdrawSuitIndByContractNumber(strContractNumber);
					_model.DiscontSuitIndDetails = _clsContractGeneral.GetPreContractDiscontSuitIndByContractNumber(strContractNumber);
					_model.CaveatIndDetails = _clsContractGeneral.GetPreContractCaveatIndDetailsByContractNumber(strContractNumber);
					_model.strMentalCapacityInd = _clsContractGeneral.GetPreContractMentalCapacityIndByContractNumber(strContractNumber);
					_model.strCPFDischargeInd = _clsContractGeneral.GetPreContractCPFDischargeIndByContractNumber(strContractNumber);
					_model.AddOnLoanIndDetails = _clsContractGeneral.GetPreContractAddOnLoanIndByContractNumber(strContractNumber);
					_model.PartialPrepayIndDetails = _clsContractGeneral.GetPreContractPartialPrepayIndByContractNumber(strContractNumber);
					_model.CrossCollaIndDetails = _clsContractGeneral.GetPreContractCrossCollaIndByContractNumber(strContractNumber);
					_model.strDeceasedInd = _clsContractGeneral.GetPreContractDeceasedIndByContractNumber(strContractNumber);
					_model.strChildConsentInd = _clsContractGeneral.GetPreContractChildConsentIndByContractNumber(strContractNumber);
					_model.GuarantorList = _clsContractGeneral.GetPreContractGuarantorListByContractNumber(strContractNumber);
					_model.MortgagorList = _clsContractGeneral.GetPreContractMortgagorListByContractNumber(strContractNumber);
					_model.MortgagorListForAcknowledgement = _clsContractGeneral.GetPreContractMortgagorListForAcknowledgementByContractNumber(strContractNumber);
					_model.DeceasedIndAndCpfDischargeIndDetailsList = _clsContractGeneral.GetPreContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber(strContractNumber);
					_model.ChildConsentIndDetailsList = _clsContractGeneral.GetPreContractChildConsentIndDetailsByContractNumber(strContractNumber);
					_model.SumOfIndicativeValuation = _clsContractGeneral.GetPreContractSumOfIndicativeValuationByContractNumber(strContractNumber);
					_model.SumOfLTVPercentage = _clsContractGeneral.GetPreContractSumOfLTVPercentageByContractNumber(strContractNumber);
					_model.SecurityPropertyModalDetails = _clsContractGeneral.GetPreContractPropertyDetailsByContractNumber(strContractNumber);
				}
				else
				{
					_model.ContractDetails = _clsContractGeneral.GetContractDetailsByContractNumber(strContractNumber);
					_model.SecurityItemByProperty = _clsContractGeneral.GetContractSecuityItemDetailsBySecurityListLevel2AndContractNumber(strContractNumber, "Property");
					if (_model.ContractDetails != null)
					{
						if (_model.ContractDetails.UpfrontPaymentMth > 0)
						{
							_model.ContractScheduleDetails = _clsContractGeneral.GetContractScheduleDetailsByContractNumberAndUpfrontPaymentMth(strContractNumber, _model.ContractDetails.UpfrontPaymentMth);
						}
						else
						{
							_model.ContractScheduleDetails = _clsContractGeneral.GetContractScheduleDetailsByContractNumber(strContractNumber);
						}
					}
					_model.RepaymetContractScheduleDetails = _clsContractGeneral.GetContractRepaymentScheduleDetailsByContractNumber(strContractNumber);
					_model.WithdrawSuitIndDetails = _clsContractGeneral.GetContractWithdrawSuitIndByContractNumber(strContractNumber);
					_model.DiscontSuitIndDetails = _clsContractGeneral.GetContractDiscontSuitIndByContractNumber(strContractNumber);
					_model.CaveatIndDetails = _clsContractGeneral.GetContractCaveatIndDetailsByContractNumber(strContractNumber);
					_model.strMentalCapacityInd = _clsContractGeneral.GetContractMentalCapacityIndByContractNumber(strContractNumber);
					_model.strCPFDischargeInd = _clsContractGeneral.GetContractCPFDischargeIndByContractNumber(strContractNumber);
					_model.AddOnLoanIndDetails = _clsContractGeneral.GetContractAddOnLoanIndByContractNumber(strContractNumber);
					_model.PartialPrepayIndDetails = _clsContractGeneral.GetContractPartialPrepayIndByContractNumber(strContractNumber);
					_model.CrossCollaIndDetails = _clsContractGeneral.GetContractCrossCollaIndByContractNumber(strContractNumber);
					_model.strDeceasedInd = _clsContractGeneral.GetContractDeceasedIndByContractNumber(strContractNumber);
					_model.strChildConsentInd = _clsContractGeneral.GetContractChildConsentIndByContractNumber(strContractNumber);
					_model.GuarantorList = _clsContractGeneral.GetContractGuarantorListByContractNumber(strContractNumber);
					_model.MortgagorList = _clsContractGeneral.GetContractMortgagorListByContractNumber(strContractNumber);
					_model.MortgagorListForAcknowledgement= _clsContractGeneral.GetContractMortgagorListForAcknowledgementByContractNumber(strContractNumber);
					_model.DeceasedIndAndCpfDischargeIndDetailsList = _clsContractGeneral.GetContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber(strContractNumber);
					_model.ChildConsentIndDetailsList = _clsContractGeneral.GetContractChildConsentIndByDetailsContractNumber(strContractNumber);
					_model.SumOfIndicativeValuation = _clsContractGeneral.GetContractSumOfIndicativeValuationByContractNumber(strContractNumber);
					_model.SumOfLTVPercentage = _clsContractGeneral.GetContractSumOfLTVPercentageByContractNumber(strContractNumber);
					_model.SecurityPropertyModalDetails = _clsContractGeneral.GetContractPropertyDetailsByContractNumber(strContractNumber);
				}

				if (_model.ContractDetails != null)
				{
					if (_model.ContractDetails.SalesExec != null)
					{
						_model.EmployeeDetails = _clsGlobal.GetEmployeeDetails(_model.ContractDetails.SalesExec);
					}
					if (_model.ContractDetails.Customer != null)
					{
						_model.strClientName = _clsCRM.getClientNameByCode(_model.ContractDetails.Customer);
						_model.strChildAcknowledegmentClientNameByCustomer = _clsCRM.getClientNameByCode(_model.ContractDetails.Customer);
					}
					if (_model.ContractDetails.CustomerAddress != null && _model.ContractDetails.CustomerType != null)
					{
						_model.CustomerAddresssList = _clsCRM.getAddressByCustomerAddress(_model.ContractDetails.CustomerAddress, _model.ContractDetails.CustomerType);
					}
					if (_model.ContractDetails.CustomerConPerson != null)
					{
						_model.strContactMobile = _clsCRM.getContactMobile(_model.ContractDetails.CustomerConPerson);
						_model.strContactEmail = _clsCRM.getContactEmail(_model.ContractDetails.CustomerConPerson);
						_model.strContactOffice = _clsCRM.getContactOffice(_model.ContractDetails.CustomerConPerson);
						_model.strContactHome = _clsCRM.getContactHome(_model.ContractDetails.CustomerConPerson);
						_model.strContactFax = _clsCRM.getContactFax(_model.ContractDetails.CustomerConPerson);
						_model.strContactPager = _clsCRM.getContactPager(_model.ContractDetails.CustomerConPerson);
						_model.ClientContactMacDetails = _clsCRM.GetClientContactMasByCustomerConPerson(_model.ContractDetails.CustomerConPerson);
					}
					if (_model.ContractDetails.CustomerDept != null)
					{
						_model.ContactPersonList = _clsCRM.getContactPerson(_model.ContractDetails.CustomerDept);
					}
				}

				if (_model.EmployeeDetails != null)
				{
					if (_model.EmployeeDetails.em_design_cod != null)
					{
						_model.strPmPerDescription = _clsGlobal.GetDesignationByEmployeeDesignCode(_model.EmployeeDetails.em_design_cod);
					}
					if (_model.EmployeeDetails.em_dept_cod != null)
					{
						_model.strFaxNumber = _clsGlobal.GetFaxNumberByEmployeeDepartmentCode(_model.EmployeeDetails.em_dept_cod);
					}
				}

				if (_model.SecurityItemByProperty != null)
				{
					if (_model.SecurityItemByProperty.SecurityID != null)
					{
						_model.SecurityPropertyMortgagorDetails = _clsContractGeneral.GetPropertyMortgagorDetailsBySecurityID(_model.SecurityItemByProperty.SecurityID);
					}
				}

				var strCompanyClientCode = _clsGlobal.GetDefaultValue("P-10019", DateTime.UtcNow);
				_model.strCompanyClientName = _clsCRM.getClientNameByCode(strCompanyClientCode);
				_model.strPreparedBy = User.Identity.Name;
				_model.strPreparedDate = String.Format("{0:dd MMMM yyyy}", DateTime.UtcNow);
				_model.IsLetterOfOfferChecked = blIsLetterOfOfferChecked;
				glog.Debug("HttpGet FnPrepareContractData: Exit");
				return FnGenerateTermLoanLOF(_model);
			}
			catch (Exception ex)
			{
				glog.Error("Please contact MIS, error:" + ex.Message);
				var strError = "Please contact MIS, error:" + ex.Message;
				return Json(strError, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion

		#region FnGenerateTermLoanLOF
		[HttpPost]
		public ActionResult FnGenerateTermLoanLOF(GenerateTermLoanLOFViewModel ModelData)
		{
			glog.Debug("HttpPost FnGenerateTermLoanLOF: Entry");
			try
			{
				#region Doc File
				#region Doc logic
				var path = HttpContext.Request.PhysicalApplicationPath + "Content\\DocTemplate\\";
				string fileName = path + Guid.NewGuid().ToString() + ".docx";
				string contentFile = "Printing Report Doc Template.docx";
				var newDoc = DocX.Load(path + contentFile);
				#endregion

				#region Common logic FirstThirdParty,Guarantor List, Acknowledegment Mortgagor list

				#region FirstThirdParty & PropertyList
				var strFirstThirdParty = "";
				var lstPropertyDetails = "";
				if (ModelData.SecurityPropertyModalDetails.Count > 0)
				{
					if (ModelData.SecurityPropertyModalDetails[0].FirstThirdParty == "F")
					{
						strFirstThirdParty = "First Party";
					}
					else if (ModelData.SecurityPropertyModalDetails[0].FirstThirdParty == "T")
					{
						strFirstThirdParty = "Third Party";
					}
					if (ModelData.SecurityPropertyModalDetails.Count == 1)
					{
						lstPropertyDetails = ModelData.SecurityPropertyModalDetails[0].PropertyAddress;
					}
					else if (ModelData.SecurityPropertyModalDetails.Count > 1)
					{
						var totalCount = ModelData.SecurityPropertyModalDetails.Count;
						var Count = 1;
						string saperator = ", ";
						for (int i = 0; i < ModelData.SecurityPropertyModalDetails.Count; i++)
						{
							if (totalCount == 2 && Count == 1)
							{
								saperator = " and ";
							}
							else if (totalCount > 2 && (Count == (totalCount - 1)))
							{
								saperator = " and ";
							}

							lstPropertyDetails += ModelData.SecurityPropertyModalDetails[i].PropertyAddress + saperator;

							Count++;
							if (Count == totalCount)
							{
								saperator = "";
							}
						}
					}
				}
				#endregion

				#region Bind Guarantor List

				#region Section 1
				var lstGuarantorSection1 = "";
				var strGuarantorTitle = "1)Personal Guarantee by ";
				if (ModelData.GuarantorList.Count == 1)
				{
					lstGuarantorSection1 = ModelData.GuarantorList[0].cm_client_nam + " (" + ModelData.GuarantorList[0].im_id_typ + " No. " + ModelData.GuarantorList[0].im_id_num + ")";
				}
				else if (ModelData.GuarantorList.Count > 1)
				{
					var Count = 0;
					strGuarantorTitle = "Joint and Several guarantee by :-" + Environment.NewLine;
					foreach (var item in ModelData.GuarantorList)
					{
						Count++;
						lstGuarantorSection1 += Count.ToString() + ")" + item.cm_client_nam + " (" + item.im_id_typ + " No. " + item.im_id_num + ")" + Environment.NewLine;
					}

				}
				newDoc.ReplaceText("guarantor_list", lstGuarantorSection1.Trim());
				newDoc.ReplaceText("strGuarantorTitle", strGuarantorTitle);
				#endregion

				#region MentalCapacityInd Guarantor List
				var lstMentalCapacityGuarantor = "";
				List<GuarantorOrMortgagorListViewModel> Mentallist = ModelData.GuarantorList.Where(x => x.MentalCapacityInd == "Y").ToList();
				var MentalCount = ModelData.GuarantorList.Where(x => x.MentalCapacityInd == "Y").ToList().Count;
				if (MentalCount == 1)
				{
					lstMentalCapacityGuarantor += Mentallist[0].cm_client_nam + " (" + Mentallist[0].im_id_typ + " No. " + Mentallist[0].im_id_num + ")";
				}
				else if (MentalCount > 1)
				{
					var totalCount = MentalCount;
					var Count = 1;
					string saperator = ", ";
					foreach (var item in Mentallist)
					{

						if (MentalCount == 2 && Count == 1)
						{
							saperator = " and ";
						}
						else if (MentalCount > 2 && (Count == (totalCount - 1)))
						{
							saperator = ", and ";
						}

						lstMentalCapacityGuarantor += item.cm_client_nam + " (" + item.im_id_typ + " No. " + item.im_id_num + ")" + saperator;

						Count++;
						if (Count == totalCount)
						{
							saperator = "";
						}
					}
				}
				#endregion

				#region WithdrawSuitInd Guarantor List
				var lstWithdrawSuitIndGuarantorList = "";
				List<GuarantorOrMortgagorListViewModel> WithdrawSuitIndlist = ModelData.GuarantorList.Where(x => x.WithdrawSuitInd == "Y").ToList();
				var WithdrawSuitIndCount = ModelData.GuarantorList.Where(x => x.WithdrawSuitInd == "Y").ToList().Count;
				if (WithdrawSuitIndCount == 1)
				{
					lstWithdrawSuitIndGuarantorList += WithdrawSuitIndlist[0].cm_client_nam + " (" + WithdrawSuitIndlist[0].im_id_typ + " No. " + WithdrawSuitIndlist[0].im_id_num + ")";
				}
				else if (WithdrawSuitIndCount > 1)
				{
					var totalCount = WithdrawSuitIndCount;
					var Count = 1;
					string saperator = ", ";
					foreach (var item in WithdrawSuitIndlist)
					{

						if (WithdrawSuitIndCount == 2 && Count == 1)
						{
							saperator = " and ";
						}
						else if (WithdrawSuitIndCount > 2 && (Count == (totalCount - 1)))
						{
							saperator = ", and ";
						}

						lstWithdrawSuitIndGuarantorList += item.cm_client_nam + " (" + item.im_id_typ + " No. " + item.im_id_num + ")" + saperator;

						Count++;
						if (Count == totalCount)
						{
							saperator = "";
						}
					}
				}
				#endregion

				#region DiscontSuitInd Guarantor List
				var lstDiscontSuitIndGuarantor = "";
				List<GuarantorOrMortgagorListViewModel> DiscontSuitIndlist = ModelData.GuarantorList.Where(x => x.DiscontSuitInd == "Y").ToList();
				var DiscontSuitIndCount = ModelData.GuarantorList.Where(x => x.DiscontSuitInd == "Y").ToList().Count;
				if (DiscontSuitIndCount == 1)
				{
					lstDiscontSuitIndGuarantor += DiscontSuitIndlist[0].cm_client_nam + " (" + DiscontSuitIndlist[0].im_id_typ + " No. " + DiscontSuitIndlist[0].im_id_num + ")";
				}
				else if (DiscontSuitIndCount > 1)
				{
					var totalCount = DiscontSuitIndCount;
					var Count = 1;
					string saperator = ", ";
					foreach (var item in DiscontSuitIndlist)
					{

						if (DiscontSuitIndCount == 2 && Count == 1)
						{
							saperator = " and ";
						}
						else if (DiscontSuitIndCount > 2 && (Count == (totalCount - 1)))
						{
							saperator = ", and ";
						}

						lstDiscontSuitIndGuarantor += item.cm_client_nam + " (" + item.im_id_typ + " No. " + item.im_id_num + ")" + saperator;

						Count++;
						if (Count == totalCount)
						{
							saperator = "";
						}
					}
				}
				#endregion

				#region DeceasedInd Details List
			    var lstDeceasedIndMortgagorList = "";
				List<DeceasedIndAndCpfDischargeIndViewModel> DeceasedIndMortgagorlist = ModelData.DeceasedIndAndCpfDischargeIndDetailsList.Where(x => x.DeceasedInd == "Y").ToList();
				var DeceasedIndCount = ModelData.DeceasedIndAndCpfDischargeIndDetailsList.Where(x => x.DeceasedInd == "Y").ToList().Count;
				if (DeceasedIndCount == 1)
				{
					lstDeceasedIndMortgagorList += DeceasedIndMortgagorlist[0].cm_client_nam + " (deceased) (" + DeceasedIndMortgagorlist[0].im_id_typ + " No. " + DeceasedIndMortgagorlist[0].im_id_num + ")";
				}
				else if (DeceasedIndCount > 1)
				{
					var totalCount = DeceasedIndCount;
					var Count = 1;
					string saperator = ", ";
					foreach (var item in DeceasedIndMortgagorlist)
					{

						if (DeceasedIndCount == 2 && Count == 1)
						{
							saperator = " and ";
						}
						else if (DeceasedIndCount > 2 && (Count == (totalCount - 1)))
						{
							saperator = ", and ";
						}

						lstDeceasedIndMortgagorList += item.cm_client_nam + " (deceased) (" + item.im_id_typ + " No. " + item.im_id_num + ")" + saperator;

						Count++;
						if (Count == totalCount)
						{
							saperator = "";
						}
					}
				}
				#endregion

				#region CpfDischargeInd Details List
				var lstCpfDischargeIndMortgagorList = "";
				List<DeceasedIndAndCpfDischargeIndViewModel> CpfDischargeIndMortgagorlist = ModelData.DeceasedIndAndCpfDischargeIndDetailsList.Where(x => x.CpfDischargeInd == "Y").ToList();
				var CpfDischargeIndCount = ModelData.DeceasedIndAndCpfDischargeIndDetailsList.Where(x => x.CpfDischargeInd == "Y").ToList().Count;
				if (CpfDischargeIndCount == 1)
				{
					lstCpfDischargeIndMortgagorList += CpfDischargeIndMortgagorlist[0].cm_client_nam + " (" + CpfDischargeIndMortgagorlist[0].im_id_typ + " No. " + CpfDischargeIndMortgagorlist[0].im_id_num + ")";
				}
				else if (CpfDischargeIndCount > 1)
				{
					var totalCount = CpfDischargeIndCount;
					var Count = 1;
					string saperator = ", ";
					foreach (var item in CpfDischargeIndMortgagorlist)
					{

						if (CpfDischargeIndCount == 2 && Count == 1)
						{
							saperator = " and ";
						}
						else if (CpfDischargeIndCount > 2 && (Count == (totalCount - 1)))
						{
							saperator = ", and ";
						}

						lstCpfDischargeIndMortgagorList += item.cm_client_nam + " (" + item.im_id_typ + " No. " + item.im_id_num + ")" + saperator;

						Count++;
						if (Count == totalCount)
						{
							saperator = "";
						}
					}
				}
				#endregion

				#region ChildConsentInd Details List
				var lstChildConsentMortgagorList = "";
				List<ChildConsentIndViewModel> ChildConsentMortgagorlist = ModelData.ChildConsentIndDetailsList.Where(x => x.ChildConsentInd == "Y").ToList();
				var ChildConsentCount = ModelData.ChildConsentIndDetailsList.Where(x => x.ChildConsentInd == "Y").ToList().Count;
				if (ChildConsentCount == 1)
				{
					lstChildConsentMortgagorList += ChildConsentMortgagorlist[0].cm_client_nam + " (" + ChildConsentMortgagorlist[0].im_id_typ + " No. " + ChildConsentMortgagorlist[0].im_id_num + ")";
				}
				else if (ChildConsentCount > 1)
				{
					var totalCount = ChildConsentCount;
					var Count = 1;
					string saperator = ", ";
					foreach (var item in ChildConsentMortgagorlist)
					{

						if (ChildConsentCount == 2 && Count == 1)
						{
							saperator = " and ";
						}
						else if (ChildConsentCount > 2 && (Count == (totalCount - 1)))
						{
							saperator = ", and ";
						}

						lstChildConsentMortgagorList += item.cm_client_nam + " (" + item.im_id_typ + " No. " + item.im_id_num + ")" + saperator;

						Count++;
						if (Count == totalCount)
						{
							saperator = "";
						}
					}
				}

				#endregion

				#region MortgagorList For Acknowledgement
				var lstAckMortgagorList = "";
				var lstSortMortgagorList = ModelData.MortgagorListForAcknowledgement.Select(x => new { cm_client_nam = x.cm_client_nam, im_id_typ = x.im_id_typ, im_id_num = x.im_id_num }).Distinct().ToList();

				if (lstSortMortgagorList.Count == 1)
				{
					lstAckMortgagorList += lstSortMortgagorList[0].cm_client_nam + " (" + lstSortMortgagorList[0].im_id_typ + " No. " + lstSortMortgagorList[0].im_id_num + ")";
				}
				else if (lstSortMortgagorList.Count > 1)
				{
					var totalCount = lstSortMortgagorList.Count;
					var Count = 1;
					string saperator = ", ";
					foreach (var item in lstSortMortgagorList)
					{

						if (lstSortMortgagorList.Count == 2 && Count == 1)
						{
							saperator = " and ";
						}
						else if (lstSortMortgagorList.Count > 2 && (Count == (totalCount - 1)))
						{
							saperator = ", and ";
						}

						lstAckMortgagorList += item.cm_client_nam + " (" + item.im_id_typ + " No. " + item.im_id_num + ")" + saperator;
						Count++;
						if (Count == totalCount)
						{
							saperator = "";
						}
					}

				}
				#endregion

				#region Section 14
				var lstGuarantorSection14 = "";
				
				if (ModelData.GuarantorList.Count == 1)
				{
					lstGuarantorSection14 += ModelData.GuarantorList[0].cm_client_nam + " (" + ModelData.GuarantorList[0].im_id_typ + " No. " + ModelData.GuarantorList[0].im_id_num + ")";
				}
				else if (ModelData.GuarantorList.Count > 1)
				{
					var totalCount = ModelData.GuarantorList.Count;
					var Count = 1;
					string saperator = ", ";
					foreach (var item in ModelData.GuarantorList)
					{

						if (ModelData.GuarantorList.Count == 2 && Count == 1)
						{
							saperator = " and ";
						}
						else if (ModelData.GuarantorList.Count > 2 && (Count == (totalCount - 1)))
						{
							saperator = ", and ";
						}

						lstGuarantorSection14 += item.cm_client_nam + " (" + item.im_id_typ + " No. " + item.im_id_num + ")" + saperator;

						Count++;
						if (Count == totalCount)
						{
							saperator = "";
						}
					}
				}
				newDoc.ReplaceText("section14_guarantorlist", lstGuarantorSection14.Trim());
				#endregion

				#endregion

				#region Section 14 Deceased Mortgagor List & Acknowledegment Mortgagor list 
				var lstMortgagorList = "";
				var lstMortgagorClinetNameList = "";
				if (ModelData.SecurityPropertyMortgagorDetails.Count == 1)
				{
					lstMortgagorList += ModelData.SecurityPropertyMortgagorDetails[0].cm_client_nam + " (" + ModelData.SecurityPropertyMortgagorDetails[0].im_id_typ + " No. " + ModelData.SecurityPropertyMortgagorDetails[0].im_id_num + ")";
					lstMortgagorClinetNameList += ModelData.SecurityPropertyMortgagorDetails[0].cm_client_nam;
				}
				else if (ModelData.SecurityPropertyMortgagorDetails.Count > 1)
				{
					var totalCount = ModelData.SecurityPropertyMortgagorDetails.Count;
					var Count = 1;
					string saperator = ", ";
					foreach (var item in ModelData.SecurityPropertyMortgagorDetails)
					{

						if (ModelData.SecurityPropertyMortgagorDetails.Count == 2 && Count == 1)
						{
							saperator = " and ";
						}
						else if (ModelData.SecurityPropertyMortgagorDetails.Count > 2 && (Count == (totalCount - 1)))
						{
							saperator = ", and ";
						}

						lstMortgagorList += item.cm_client_nam + " (" + item.im_id_typ + " No. " + item.im_id_num + ")" + saperator;
						lstMortgagorClinetNameList += item.cm_client_nam;
						Count++;
						if (Count == totalCount)
						{
							saperator = "";
						}
					}

				}
				#endregion

				#endregion

				#region Header
				newDoc.ReplaceText("str_companyclientname", ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName);
				newDoc.ReplaceText("ss_emp_mas_em_sht_nam", ModelData.EmployeeDetails == null ? "" : ModelData.EmployeeDetails.em_sht_nam == null ? "" : ModelData.EmployeeDetails.em_sht_nam);
				newDoc.ReplaceText("ss_emp_mas_em_did", ModelData.EmployeeDetails == null ? "" : ModelData.EmployeeDetails.em_did == null ? "" : ModelData.EmployeeDetails.em_did);
				newDoc.ReplaceText("fax_Number", ModelData.strFaxNumber == null ? "" : ModelData.strFaxNumber);
				newDoc.ReplaceText("ss_emp_mas_em_smtp_id", ModelData.EmployeeDetails == null ? "" : ModelData.EmployeeDetails.em_smtp_id == null ? "" : ModelData.EmployeeDetails.em_smtp_id);
				newDoc.ReplaceText("letterofofferdate", ModelData.ContractDetails == null ? "" : String.Format("{0:dd MMMM yyyy}", ModelData.ContractDetails.LetterOfferDate));
				newDoc.ReplaceText("crmtb_client_mas_cm_client_nam", ModelData.strClientName == null ? "" : ModelData.strClientName);
				newDoc.ReplaceText("address_details", ModelData.CustomerAddresssList.Address != null ? ModelData.CustomerAddresssList.Address : "");
				if (ModelData.strContactOffice != null)
				{
					newDoc.ReplaceText("crmtb_client_contact_det.cd_typ_val_office", ModelData.strContactOffice);
				}
				else
				{
					newDoc.ReplaceText("crmtb_client_contact_det.cd_typ_val_office", ModelData.strContactMobile == null ? "" : ModelData.strContactMobile);
				}
				newDoc.ReplaceText("crmtb_client_contact_det.cd_typ_val_fax", ModelData.strContactFax == null ? "" : ModelData.strContactFax);
				newDoc.ReplaceText("crmtb_client_contact_mas_cm_title", (ModelData.ClientContactMacDetails.cm_title == null ? "" : ModelData.ClientContactMacDetails.cm_title+" ")+(ModelData.ClientContactMacDetails.cm_full_nam == null ? "" : ModelData.ClientContactMacDetails.cm_full_nam));

				#region Letter Of Offer Checked logic
				if (ModelData.IsLetterOfOfferChecked)
				{
					var strLetterOfOffer = "";
					if (ModelData.AddOnLoanIndDetails != null)
					{
						if (ModelData.AddOnLoanIndDetails.AddOnLoanInd == "Y")
						{
							strLetterOfOffer = "In addition to the facilities offered under " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + "’s Letter of Offer dated " + (ModelData.ContractDetails != null ? String.Format("{0:dd MMMM yyyy}", ModelData.ContractDetails.LetterOfferDate) : "");
						}
					}
					newDoc.ReplaceText("letterofoffer_details", strLetterOfOffer);
					newDoc.ReplaceText("strHeaderTitle", strLetterOfOffer != "" ? " we" : "We");
				}
				else
				{
					newDoc.ReplaceText("letterofoffer_details", "");
					newDoc.ReplaceText("strHeaderTitle", "We");
				}
				#endregion

				#endregion End Header 

				#region Section 1
				var strPartialPerpayPercent = "zero per cent (" + ((ModelData.ContractDetails == null ? "0" : String.Format("{0:#.#}", ModelData.ContractDetails.PrepayPercent)) + "%)");
				#region TotalCashPrice,PropertyTotLTVPer,LeasePeriod,RenewPeriod & TermChargesPercent,CancelFeePercent,PrepayPercent,FacilityFee & CommitFee
				if (ModelData.ContractDetails != null)
				{
					var strTotalWord = _clsNumberToWord.ConvertToWords(String.Format("{0:0.00}", ModelData.ContractDetails.TotalCashPrice));
					newDoc.ReplaceText("total_cash_price", "Singapore Dollars " + strTotalWord.Trim() + " (S$" + String.Format("{0:n}", ModelData.ContractDetails.TotalCashPrice) + ")");
					var strCancelWord = _clsNumberToWord.ConvertToWords(String.Format("{0:0.00}", ModelData.ContractDetails.CancelFeePercent));
					newDoc.ReplaceText("cancelfeepercent", strCancelWord.ToLower().Trim() + " per cent (" + String.Format("{0:#.#}", ModelData.ContractDetails.CancelFeePercent) + "%)");
					var strPrepayPercent = _clsNumberToWord.ConvertToWords(String.Format("{0:#.#}", ModelData.ContractDetails.PrepayPercent));
					strPartialPerpayPercent = strPrepayPercent.ToLower().Trim() + " per cent (" + String.Format("{0:#.#}", ModelData.ContractDetails.PrepayPercent) + "%)";
				}
				else
				{
					newDoc.ReplaceText("total_cash_price", "Singapore Dollars Zero (S$0.00)");
					newDoc.ReplaceText("cancelfeepercent", "zero per cent (" + ((ModelData.ContractDetails == null ? "0" : String.Format("{0:0.00}", ModelData.ContractDetails.CancelFeePercent)) + "%)"));
				}
				newDoc.ReplaceText("prepay_percent", strPartialPerpayPercent);
				newDoc.ReplaceText("intprepaypercent", (ModelData.ContractDetails == null ? "0" : String.Format("{0:#.#}", ModelData.ContractDetails.PrepayPercent)) + "%");
				newDoc.ReplaceText("cancel_fee_percent", (ModelData.ContractDetails != null ? String.Format("{0:#.#}", ModelData.ContractDetails.CancelFeePercent) : "0"));
				newDoc.ReplaceText("totalcashprice", (ModelData.ContractDetails != null ? String.Format("{0:n}", ModelData.ContractDetails.TotalCashPrice) : "0"));
				newDoc.ReplaceText("ltv_percentage", (ModelData.ContractDetails != null ? String.Format("{0:0.00}", ModelData.ContractDetails.PropertyTotLTVPer) : "0"));
				newDoc.ReplaceText("lease_period", (ModelData.ContractDetails != null ? String.Format("{0:#.#}", ModelData.ContractDetails.LeasePeriod) : "0"));
				newDoc.ReplaceText("renew_period", (ModelData.ContractDetails != null ? ModelData.ContractDetails.RenewPeriod == null ? "0" : String.Format("{0:#.#}", ModelData.ContractDetails.RenewPeriod) : "0"));
				newDoc.ReplaceText("term_charges_percent", (ModelData.ContractDetails != null ? String.Format("{0}", ModelData.ContractDetails.TermChargesPercent) : "0"));
				newDoc.ReplaceText("facility_fee", (ModelData.ContractDetails != null ? String.Format("{0:n}", ModelData.ContractDetails.FacilityFee) : "0"));
				newDoc.ReplaceText("commit_fee", (ModelData.ContractDetails != null ? String.Format("{0:n}", ModelData.ContractDetails.CommitFee) : "0"));
				#endregion

				#region Populate Dynamically Clause
				var strpopulatedynamicallyclause = "";
				//WithdrawSuitInd
				var strTitlewithdrawSuitInd = "";
				var listwithdrawSuitInd = "";
				var strAEComma = "";
				var discont_SuitInd = "";
				var caveat_Ind = "";
				//MentalCapacityInd
				var strTitleMentalCapacityInd = "";
				var listMentalCapacityInd = "";
				var strComEnd = "";
				var cPF_DischargeInd = "";
				//AddOnLoanInd
				var strTitleAddOnLoanInd = "";
				var strBoldPro = "";
				var strAMortI = "";
				var strBoldMortNo = "";
				var strADated = "";
				var strBoldMortDate = "";
				var strADeed = "";
				var strBoldDeedDate = "";
				var strAdComEnd = "";
				var strAExDeed = "";
				//Existing Deed
				var strExistingDeed = "";

				#region security_property_first_third_party
				if (ModelData.ContractDetails != null)
				{
					if ((ModelData.ContractDetails.ContractNumber.Contains("LCP") || ModelData.ContractDetails.ContractNumber.Contains("ISS")) && strFirstThirdParty == "First Party")
					{
						strpopulatedynamicallyclause = "Copy of the Mortgagor's Board and Shareholder's resolution, if it is a corporate, duly certified as a true copy by two Directors or a Director and the Company Secretary;";
					}
				}
				#endregion

				#region WithdrawSuitInd,DiscontSuitInd,CaveatInd & AddOnLoanInd
				// WithdrawSuitInd
				if (ModelData.WithdrawSuitIndDetails != null)
				{
					if (ModelData.WithdrawSuitIndDetails.WithdrawSuitInd == "Y")
					{
						strTitlewithdrawSuitInd = "Evidence of withdrawal for bankruptcy suit no. " + (ModelData.WithdrawSuitIndDetails.WSuitNo != null ? ModelData.WithdrawSuitIndDetails.WSuitNo : "") + " of " + (ModelData.WithdrawSuitIndDetails.WSuitYear != null ? ModelData.WithdrawSuitIndDetails.WSuitYear : "") + " on ";
						listwithdrawSuitInd = lstWithdrawSuitIndGuarantorList;
						strAEComma = ";";

					}
				}
				// DiscontSuitInd
				if (ModelData.DiscontSuitIndDetails != null)
				{
					if (ModelData.DiscontSuitIndDetails.DiscontSuitInd == "Y")
					{
						discont_SuitInd = "Notice of discontinuance for Suit No. " + (ModelData.DiscontSuitIndDetails.DSuitNo != null ? ModelData.DiscontSuitIndDetails.DSuitNo : "") + " of " + (ModelData.DiscontSuitIndDetails.DSuitYear != null ? ModelData.DiscontSuitIndDetails.DSuitYear : "") + ";";
					}
				}
				// CaveatInd
				if (ModelData.CaveatIndDetails != null)
				{
					if (ModelData.CaveatIndDetails.CaveatInd == "Y")
					{
						caveat_Ind = "Evidence of removal of the caveat " + (ModelData.CaveatIndDetails.CaveatNo != null ? ModelData.CaveatIndDetails.CaveatNo : "") + " by " + (ModelData.CaveatIndDetails.CaveatCompany != null ? ModelData.CaveatIndDetails.CaveatCompany : "") + ";";
					}
				}
				//AddOnLoanInd
				if (ModelData.AddOnLoanIndDetails != null)
				{
					if (ModelData.AddOnLoanIndDetails.AddOnLoanInd == "Y")
					{
						var lstAddOnLoanIndPropertyDetails = "";

						if (ModelData.SecurityPropertyModalDetails.Count > 0)
						{
							List<Security_PropertyModel_ViewModel> lstAddOnLoanIndProperty = new List<Security_PropertyModel_ViewModel>();
							lstAddOnLoanIndProperty = ModelData.SecurityPropertyModalDetails.Where(x => x.AddOnLoanInd == "Y").ToList();

							if (lstAddOnLoanIndProperty.Count == 1)
							{
								lstAddOnLoanIndPropertyDetails = lstAddOnLoanIndProperty[0].PropertyAddress;
							}
							else if (lstAddOnLoanIndProperty.Count > 1)
							{
								var adtotalCount = lstAddOnLoanIndProperty.Count;
								var adCount = 1;
								string saperator = ", ";
								for (int i = 0; i < lstAddOnLoanIndProperty.Count; i++)
								{
									if (adtotalCount == 2 && adCount == 1)
									{
										saperator = " and ";
									}
									else if (adtotalCount > 2 && (adCount == (adtotalCount - 1)))
									{
										saperator = " and ";
									}

									lstAddOnLoanIndPropertyDetails += lstAddOnLoanIndProperty[i].PropertyAddress + saperator;

									adCount++;
									if (adCount == adtotalCount)
									{
										saperator = "";
									}
								}
							}
						}

						strTitleAddOnLoanInd = "Existing " + strFirstThirdParty + "(ies) All Monies Open First Legal Mortgage in favour of " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + " over the property ";
						strBoldPro = lstAddOnLoanIndPropertyDetails;

						if (ModelData.AddOnLoanIndDetails.MortgInstrumentNo != null)
						{
							strAMortI = " (Mortgage Instrument No. ";
							strBoldMortNo = (ModelData.AddOnLoanIndDetails.MortgInstrumentNo);
							strADated = " dated ";
							strBoldMortDate = (ModelData.AddOnLoanIndDetails.MortgInstrumentDate != null ? String.Format("{0:dd MMMM yyyy}", ModelData.AddOnLoanIndDetails.MortgInstrumentDate) : "");

							if (ModelData.AddOnLoanIndDetails.DeedOfAssignDate != null)
							{
								strADeed = ") and deed of assignment of rental proceeds in respect of the property dated ";
								strBoldDeedDate = (ModelData.AddOnLoanIndDetails.DeedOfAssignDate != null ? String.Format("{0:dd MMMM yyyy}", ModelData.AddOnLoanIndDetails.DeedOfAssignDate) : "");
								strAdComEnd = ";";
							}
						}
						else
						{
							strAdComEnd = ";";
						}
						if (ModelData.AddOnLoanIndDetails.DeedOfSuborDate != null)
						{
							strExistingDeed ="Existing deed of subordination dated " + (ModelData.AddOnLoanIndDetails.DeedOfSuborDate != null ? String.Format("{0:dd MMMM yyyy}", ModelData.AddOnLoanIndDetails.DeedOfSuborDate) : "") + ";";
						}
					}

				}
				#endregion

				#region MentalCapacityInd
				if (ModelData.strMentalCapacityInd == "Y")
				{
					strTitleMentalCapacityInd = "Certificate of Mental Capacity for ";
					listMentalCapacityInd = lstMentalCapacityGuarantor;
					strComEnd = ";";
				}
				#endregion

				#region CPFDischargeInd
				if (ModelData.strCPFDischargeInd == "Y")
				{
					cPF_DischargeInd = strFirstThirdParty + " Mortgagor(s) to provide documentary evidence of CPF Discharge for the property referred to in Clause 6;";
				}
				#endregion

				newDoc.ReplaceText("strpopulatedynamicallyclause", strpopulatedynamicallyclause);
				//WithdrawSuitInd
				newDoc.ReplaceText("strTitlewithdrawSuitInd", strTitlewithdrawSuitInd);
				newDoc.ReplaceText("listwithdrawSuitInd", listwithdrawSuitInd);
				newDoc.ReplaceText("strAEComma", strAEComma);
				newDoc.ReplaceText("discont_SuitInd", discont_SuitInd);
				newDoc.ReplaceText("caveat_Ind", caveat_Ind);
				//MentalCapacityInd
				newDoc.ReplaceText("strTitleMentalCapacityInd", strTitleMentalCapacityInd);
				newDoc.ReplaceText("listMentalCapacityInd", listMentalCapacityInd);
				newDoc.ReplaceText("strComEnd", strComEnd);
				newDoc.ReplaceText("cPF_DischargeInd", cPF_DischargeInd);
				//AddOnLoanInd
				newDoc.ReplaceText("strTitleAddOnLoanInd", strTitleAddOnLoanInd);
				newDoc.ReplaceText("strBoldPro", strBoldPro);
				newDoc.ReplaceText("strAMortI", strAMortI);
				newDoc.ReplaceText("strBoldMortNo", strBoldMortNo);
				newDoc.ReplaceText("strADated", strADated);
				newDoc.ReplaceText("strBoldMortDate", strBoldMortDate);
				newDoc.ReplaceText("strADeed", strADeed);
				newDoc.ReplaceText("strBoldDeedDate", strBoldDeedDate);
				newDoc.ReplaceText("strAdComEnd", strAdComEnd);
				newDoc.ReplaceText("strAExDeed", strAExDeed);
				newDoc.ReplaceText("strExistingDeed", strExistingDeed);
				#endregion

				#region PopulateLCPClause Clause
				var strOnePopulateLCPClause = "";
				var strTwoPopulateLCPClause = "";
				if (ModelData.ContractDetails != null)
				{
					if (ModelData.ContractDetails.ContractNumber.Contains("LCP"))
					{
						strOnePopulateLCPClause = "All title deeds and documents relating to the property referred to in Clause 6 including the duly stamped sale and purchase agreement in respect of the said property and the ad valorem Certificate of Stamp Duty;";

						strTwoPopulateLCPClause = "Documentary evidence that the difference between the purchase price and the amount of the Facility has been paid to the vendor of the property before any disbursement of the Facility or any part thereof;";
					}
				}
				newDoc.ReplaceText("onepopulatelcpclause", strOnePopulateLCPClause);
				newDoc.ReplaceText("twopopulatelcpclause", strTwoPopulateLCPClause);

				#endregion

				#region PopulatePaymentClause logic
				var strFirstInstalment = "The first instalment payment of ";
				var intInsAmount = "S$" + (ModelData.ContractScheduleDetails != null ? (String.Format("{0:n}", ModelData.ContractScheduleDetails.InsAmount) == "" ? "0" : String.Format("{0:n}", ModelData.ContractScheduleDetails.InsAmount)) : "0");
				var strPaymentofcommitemt = ", payment of the commitment fee of ";
				var intcommitfee = "S$" + (ModelData.ContractDetails != null ? String.Format("{0:n}", ModelData.ContractDetails.CommitFee) : "0");
				var strAndPayment = " and payment of the facility fee of ";
				var intfacilityfee = "S$" + (ModelData.ContractScheduleDetails != null ? (String.Format("{0:n}", ModelData.ContractDetails.FacilityFee) == "" ? "0" : String.Format("{0:n}", ModelData.ContractDetails.FacilityFee)) : "0");
				var strSemecolumn = ";";

				if (ModelData.ContractDetails != null)
				{
					if (ModelData.ContractDetails.ContractNumber.Contains("ISS"))
					{
						strFirstInstalment = "The first interest payment of ";
					}
					else if (ModelData.ContractDetails.ContractNumber.Contains("LCP"))
					{
						strSemecolumn = "; and all other fees (including legal fees) and costs and disbursements incurred by " + ModelData.strCompanyClientName + " in connection with this facility letter, the Secured Term Loan Facility Agreement or the Facility have been paid ;";
					}
				}
				newDoc.ReplaceText("strFirstInstalment", strFirstInstalment);
				newDoc.ReplaceText("intInsAmount", intInsAmount);
				newDoc.ReplaceText("strPaymentofcommitemt", strPaymentofcommitemt);
				newDoc.ReplaceText("intcommitfee", intcommitfee);
				newDoc.ReplaceText("strAndPayment", strAndPayment);
				newDoc.ReplaceText("intfacilityfee", intfacilityfee);
				newDoc.ReplaceText("strSemecolumn", strSemecolumn);
				#endregion

				#endregion End Section 1

				#region Section 3
				newDoc.ReplaceText("ReInsamount", (ModelData.RepaymetContractScheduleDetails != null ? (String.Format("{0:n}", ModelData.RepaymetContractScheduleDetails.InsAmount) == "" ? "0.00" : String.Format("{0:n}", ModelData.RepaymetContractScheduleDetails.InsAmount)) : "0.00"));
				#endregion End Section 3

				#region Section 5
				newDoc.ReplaceText("late_payment_interest", (ModelData.ContractDetails != null ? (String.Format("{0}", ModelData.ContractDetails.LPIPercent) == "" ? "0" : String.Format("{0}", ModelData.ContractDetails.LPIPercent)) : "0"));
				newDoc.ReplaceText("min_payment", (ModelData.ContractDetails != null ? (String.Format("{0:n}", ModelData.ContractDetails.MinLPI) == "" ? "0" : String.Format("{0:n}", ModelData.ContractDetails.MinLPI)) : "0"));
				#endregion End Section 5

				#region Section 6
				var str_sixHeader = strFirstThirdParty+"(ies) All Monies Open First Legal Mortgage in favour of ";
				var str_sixBody = ModelData.strCompanyClientName + " over the property ";
				var str_sixStFooter = "";
				var str_sixMiFooter = "";
				var str_sixLaFooter = " free from encumbrances.";
				if (ModelData.SecurityPropertyModalDetails.Count > 1)
				{
					str_sixBody = ModelData.strCompanyClientName + " over the properties ";
					str_sixStFooter = " (collectively the ";
					str_sixMiFooter = "“property”";
					str_sixLaFooter = "), free from encumbrances.";
				}
				newDoc.ReplaceText("str_sixHeader", str_sixHeader);
				newDoc.ReplaceText("str_sixBody", str_sixBody);
				newDoc.ReplaceText("lst_sixPropertyList", lstPropertyDetails);
				newDoc.ReplaceText("str_sixStFooter", str_sixStFooter);
				newDoc.ReplaceText("str_sixMiFooter", str_sixMiFooter);
				newDoc.ReplaceText("str_sixLaFooter", str_sixLaFooter);

				newDoc.ReplaceText("security_property_first_third_party_6", strFirstThirdParty);
				newDoc.ReplaceText("indicative_valuation_amt", (String.Format("{0:n}", ModelData.SumOfIndicativeValuation) == "" ? "0" : String.Format("{0:n}", ModelData.SumOfIndicativeValuation)));
				newDoc.ReplaceText("sumOfltvpercentage", (String.Format("{0:n}", ModelData.SumOfLTVPercentage) == "" ? "0" : String.Format("{0:n}", ModelData.SumOfLTVPercentage)));
				#endregion End Section 6

				#region Section 10
				var strprepay_notice_mth = "0";
				if (ModelData.ContractDetails != null)
				{
					var strPrepay = _clsNumberToWord.ConvertToWords(String.Format("{0:#.#}", ModelData.ContractDetails.PrepayNoticeMth));
					strprepay_notice_mth = strPrepay.ToLower().Trim() + " (" + String.Format("{0:#.#}", ModelData.ContractDetails.PrepayNoticeMth) + ")";
				}
				newDoc.ReplaceText("prepay_notice_mth", (strprepay_notice_mth == "0" ? "zero (0)" : strprepay_notice_mth));

				#region partial_perpayInd_clause
				var strFalsePartialPerpayIndClause = "Partial prepayment of the Facility is not allowed;"+Environment.NewLine;
				var strSectionBreak = "";
				if (ModelData.PartialPrepayIndDetails.Count> 0)
				{
					strFalsePartialPerpayIndClause = "";
					strSectionBreak = Environment.NewLine;
					var totalCount = ModelData.PartialPrepayIndDetails.Count;
					int listCount = 1;
					for (int i = 0; i < ModelData.PartialPrepayIndDetails.Count; i++)
					{
						var strTrueHeader = "In the event the property at " + ModelData.PartialPrepayIndDetails[i].PropertyAddress + " (“";

						var lstTrueList = ModelData.PartialPrepayIndDetails[i].PropertyAddress + " property";

						var strTrueBody = "”) is sold, you may by giving " + (strprepay_notice_mth == "0" ? "zero (0)" : strprepay_notice_mth) + " months’ prior written notice and prepaying " + (ModelData.PartialPrepayIndDetails[i].LTVPercentage != null ? String.Format("{0:0.00}", ModelData.PartialPrepayIndDetails[i].LTVPercentage) : "0") + "% of the outstanding Facility together with a prepayment fee of " + strPartialPerpayPercent + " flat on the outstanding principal amount to be pre-paid, redeem the " + ModelData.PartialPrepayIndDetails[i].PropertyAddress + " property on completion of the sale.";

						if (i != (ModelData.PartialPrepayIndDetails.Count - 1))
						{
							strTrueBody += Environment.NewLine;
						}
						newDoc.ReplaceText("str_" + listCount + "_TrueHeader", strTrueHeader);
						newDoc.ReplaceText("lst_" + listCount + "_TrueList", lstTrueList);
						newDoc.ReplaceText("str_" + listCount + "_TrueBody", strTrueBody);

						listCount++;

					}
					if (totalCount < 12)
					{
						for (int i = 1; i < 12; i++)
						{
							newDoc.ReplaceText("str_" + i + "_TrueHeader", "");
							newDoc.ReplaceText("lst_" + i + "_TrueList", "");
							newDoc.ReplaceText("str_" + i + "_TrueBody", "");
						}
					}
				}
				else
				{
					for (int i = 1; i < 12; i++)
					{
						newDoc.ReplaceText("str_"+i+"_TrueHeader", "");
						newDoc.ReplaceText("lst_" + i + "_TrueList", "");
						newDoc.ReplaceText("str_" + i + "_TrueBody", "");
					}
					
				}
				newDoc.ReplaceText("FalsepartialperpayIndclause", strFalsePartialPerpayIndClause);
				newDoc.ReplaceText("strSectionBreak", strSectionBreak);


				#endregion

				#endregion End Section 10

				#region Section 11
				//@CancelFeePercent
				#endregion End Section 11

				#region Section 13
				#region Cross-Collateralization Clause
				var strcollateralizationclause = "You agree that the security interest granted (if any) in the security documents as stated herein constitute a first lien on the subject matter of the security here under which will be kept free from all other liens, claims, security interests or encumbrances and if there is any other indebtedness outstanding, now existing or hereafter incurred under any other agreement or instrument between yourselves and  " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + ". " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + " shall retain the security interest in the subject matter of the security here under to secure all such indebtedness until all such indebtedness is satisfied in full and " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + " shall have the right to apply, in any order or priority, any payments received from you against any such indebtedness.";
				var strcrosscollateralizationclause = "";
				if (ModelData.CrossCollaIndDetails != null)
				{
					if (ModelData.CrossCollaIndDetails.CrossCollaInd == "Y")
					{
						strcollateralizationclause = "You agree that the security interest (if any) in the security documents as stated herein constitute a first lien on the subject matter of the security here under which will be kept free from all other liens, claims, security interests or encumbrances and if there is any other indebtedness outstanding, now existing or hereafter incurred under any other agreement or instrument between yourself and " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + " including but not limited to the credit facilities which were extended by " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + " pursuant to loan agreement no. " + (ModelData.CrossCollaIndDetails.CrossCollaContractNum != null ? ModelData.CrossCollaIndDetails.CrossCollaContractNum : "") + ". " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + " shall retain the security interest in the subject matter of the security here under to secure all such aforesaid indebtedness until they are satisfied in full and " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + " shall have the right to apply, in any order or priority, any payments received from you against any such indebtedness."+Environment.NewLine;
						strcrosscollateralizationclause = "For the avoidance of doubt, any security interest granted in the security documents shall subsist unless and until all indebtedness outstanding, now existing or hereafter incurred under any other agreement or instrument between yourself and " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + " (including but not limited to the credit facilities which were extended by " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + " pursuant to loan agreement no. " + (ModelData.CrossCollaIndDetails.CrossCollaContractNum != null ? ModelData.CrossCollaIndDetails.CrossCollaContractNum : "") + ") have been satisfied in full.";
						

					}
				}
				newDoc.ReplaceText("strcollateralizationclause", strcollateralizationclause);
				newDoc.ReplaceText("strcrosscollateralizationclause", strcrosscollateralizationclause);
				#endregion

				#endregion End Section 13

				#region Section 14

				#region Bind Guarantor List  // Inside Section 1
				newDoc.ReplaceText("lstguarantorsectionfourteen ", (ModelData.SecurityPropertyMortgagorDetails != null ? lstAckMortgagorList + " " : ""));
				#endregion

				#region Populate Dyncamically Clause For 14

				#region  Define Variable 
				var strBreackLine = "";
				//withdrawalSuitInd
				var strHeaderwithdrawalSuitInd = "";
				var lstwithdrawalSuitInd = "";
				var strEndwithdrawalSuitInd = "";
				//DiscontSuitInd
				var lstDiscontSuitInd = "";
				var strBodyDiscontSuitInd = "";
				//DeceasedInd
				var strHeaderDeceasedInd = "";
				var lstDeceasedInd = "";
				var strEndDeceasedInd = "";
				//MentalCapacityInd
				var strHeaderMentalCapacityInd = "";
				var lstMentalCapacityInd = "";
				var strBodyMentalCapacityInd = "";
				//CPFDischargedInd
				var strHeaderCPFDischargedInd = "";
				var lstCPFDischargedInd = "";
				var strBodyCPFDischargedInd = "";
				//ChildConsentInd
				var strHeaderChildConsentInd = "";
				var lstChildConsentInd = "";
				var strBodyChildConsentInd = "";

				Dictionary<string, string> lst = new Dictionary<string, string>();
				#endregion

				#region WithdrawSuitInd
				if (ModelData.WithdrawSuitIndDetails != null)
				{
					if (ModelData.WithdrawSuitIndDetails.WithdrawSuitInd == "Y")
					{
						strBreackLine += "InFirst";

						strHeaderwithdrawalSuitInd = "You are to provide evidence of withdrawal for the bankruptcy suit no. " + (ModelData.WithdrawSuitIndDetails.WSuitNo != null ? ModelData.WithdrawSuitIndDetails.WSuitNo : "") + " of " + (ModelData.WithdrawSuitIndDetails.WSuitYear != null ? ModelData.WithdrawSuitIndDetails.WSuitYear : "") + " on ";
						lstwithdrawalSuitInd = lstWithdrawSuitIndGuarantorList;
						strEndwithdrawalSuitInd = " by " + (ModelData.WithdrawSuitIndDetails.WSuitFiledBy != null ? ModelData.WithdrawSuitIndDetails.WSuitFiledBy : "") + ".";

						lst.Add("strEndwithdrawalSuitInd", strEndwithdrawalSuitInd);

					}
				}
				#endregion

				#region  DiscontSuitInd
				if (ModelData.DiscontSuitIndDetails != null)
				{
					if (ModelData.DiscontSuitIndDetails.DiscontSuitInd == "Y")
					{
						strBreackLine += "InSecond";

						lstDiscontSuitInd = (ModelData.SecurityPropertyMortgagorDetails != null ? lstDiscontSuitIndGuarantor + " " : "");
						strBodyDiscontSuitInd = "to provide a certified true copy of the notice of discontinuance filed by " + (ModelData.DiscontSuitIndDetails.DSuitFiledBy != null ? ModelData.DiscontSuitIndDetails.DSuitFiledBy : "") + " (" + (ModelData.DiscontSuitIndDetails.GroupMemberDesc != null ? ModelData.DiscontSuitIndDetails.GroupMemberDesc : "") + " " + (ModelData.DiscontSuitIndDetails.DSuitFiledById != null ? ModelData.DiscontSuitIndDetails.DSuitFiledById : "") + ") or its lawyers in respect of Suit No. " + (ModelData.DiscontSuitIndDetails.DSuitNo != null ? ModelData.DiscontSuitIndDetails.DSuitNo : "") + " of " + (ModelData.DiscontSuitIndDetails.DSuitYear != null ? ModelData.DiscontSuitIndDetails.DSuitYear : "") + ".";

						lst.Add("strBodyDiscontSuitInd", strBodyDiscontSuitInd);
					}
				}
				#endregion

				#region DeceasedInd 
				if (ModelData.strDeceasedInd == "Y")
				{
					strBreackLine += "InThird";

					strHeaderDeceasedInd = "Prior to disbursement you are required to remove ";
					lstDeceasedInd = lstDeceasedIndMortgagorList;
					strEndDeceasedInd = " as joint tenant of the Property.";

					lst.Add("strEndDeceasedInd", strEndDeceasedInd);
				}
				
				#endregion

				#region MentalCapacityInd
				if (ModelData.strMentalCapacityInd == "Y")
				{
					strBreackLine += "InFourth";

					strHeaderMentalCapacityInd = strFirstThirdParty + "(ies) Mortgagor(s) / Guarantors ";
					lstMentalCapacityInd = lstMentalCapacityGuarantor;
					strBodyMentalCapacityInd = " are to provide medical certificate(s) in form and substance acceptable to " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + " from a competent and qualified medical doctor attesting to their mental capacity to make sound decisions on their own.";

					lst.Add("strBodyMentalCapacityInd", strBodyMentalCapacityInd);
				}
				#endregion

				#region CPFDischargeInd
				//CPFDischargeInd
				if (ModelData.strCPFDischargeInd == "Y")
				{
					strBreackLine += "InFive";

					strHeaderCPFDischargedInd = "Prior to disbursement you are required to obtained from the " + strFirstThirdParty + "(ies) Mortgagor(s) ";
					lstCPFDischargedInd = lstCpfDischargeIndMortgagorList;
					strBodyCPFDischargedInd = " documentary evidence of CPF Discharge for the Property.";

					lst.Add("strBodyCPFDischargedInd", strBodyCPFDischargedInd);
				}
				#endregion

				#region ChildConsentInd
				//ChildConsentInd
				if (ModelData.strChildConsentInd == "Y")
				{
					strBreackLine += "InSix";

					strHeaderChildConsentInd = strFirstThirdParty + "(ies) Mortgagor(s) ";
					lstChildConsentInd = lstChildConsentMortgagorList;
					strBodyChildConsentInd = "’s children’s consent to her provision of the Security for this facility are to be obtained.";

					lst.Add("strBodyChildConsentInd", strBodyChildConsentInd);
				}
				#endregion

				#region Replace Text
				for (int i = 0; i < lst.Count - 1; i++)
				{
					lst[lst.ElementAt(i).Key] = lst.ElementAt(i).Value + Environment.NewLine;
				}
				newDoc.ReplaceText("strBreackLine", (strBreackLine != "" ? Environment.NewLine + " " : strBreackLine));
				//withdrawalSuitInd
				newDoc.ReplaceText("strHeaderwithdrawalSuitInd", strHeaderwithdrawalSuitInd);
				newDoc.ReplaceText("lstwithdrawalSuitInd", lstwithdrawalSuitInd);
				newDoc.ReplaceText("strEndwithdrawalSuitInd", lst.ContainsKey("strEndwithdrawalSuitInd") ? lst["strEndwithdrawalSuitInd"] : string.Empty);
				//DiscontSuitInd
				newDoc.ReplaceText("lstDiscontSuitInd", lstDiscontSuitInd);
				newDoc.ReplaceText("strBodyDiscontSuitInd", lst.ContainsKey("strBodyDiscontSuitInd") ? lst["strBodyDiscontSuitInd"] : string.Empty);
				//DeceasedInd
				newDoc.ReplaceText("strHeaderDeceasedInd", strHeaderDeceasedInd);
				newDoc.ReplaceText("lstDeceasedInd", lstDeceasedInd);
				newDoc.ReplaceText("strEndDeceasedInd", lst.ContainsKey("strEndDeceasedInd") ? lst["strEndDeceasedInd"] : string.Empty);
				//MentalCapacityInd
				newDoc.ReplaceText("strHeaderMentalCapacityInd", strHeaderMentalCapacityInd);
				newDoc.ReplaceText("lstMentalCapacityInd", lstMentalCapacityInd);
				newDoc.ReplaceText("strBodyMentalCapacityInd", lst.ContainsKey("strBodyMentalCapacityInd") ? lst["strBodyMentalCapacityInd"] : string.Empty);
				//CPFDischargedInd
				newDoc.ReplaceText("strHeaderCPFDischargedInd", strHeaderCPFDischargedInd);
				newDoc.ReplaceText("lstCPFDischargedInd", lstCPFDischargedInd);
				newDoc.ReplaceText("strBodyCPFDischargedInd", lst.ContainsKey("strBodyCPFDischargedInd") ? lst["strBodyCPFDischargedInd"] : string.Empty);
				//ChildConsentInd
				newDoc.ReplaceText("strHeaderChildConsentInd", strHeaderChildConsentInd);
				newDoc.ReplaceText("lstChildConsentInd", lstChildConsentInd);
				newDoc.ReplaceText("strBodyChildConsentInd", lst.ContainsKey("strBodyChildConsentInd") ? lst["strBodyChildConsentInd"] : string.Empty);
				#endregion

				#endregion

				#endregion End Section 14

				#region Footer
				//@ss_emp_mas.em_sht_nam
				newDoc.ReplaceText("footer_designation", (ModelData.strPmPerDescription == null ? "" : ModelData.strPmPerDescription));
				#endregion End Footer

				#region Signature Section
				newDoc.ReplaceText("lstguaramtorlist", lstGuarantorSection14);
				var lstLeftguaramtorlist = "";
				var lstRightguaramtorlist = "";
				if (ModelData.GuarantorList.Count > 0)
				{
					bool isEvenNUmber = ModelData.GuarantorList.Count % 2 == 0 ? true : false;
					for (int i = 0; i < ModelData.GuarantorList.Count; i++)
					{
						bool needSpace = (i == (ModelData.GuarantorList.Count - 1)) ? false : isEvenNUmber ? (i == (ModelData.GuarantorList.Count - 2)) ? false : true : true;

						if (i % 2 == 0)
						{
							lstLeftguaramtorlist += "..............................................................." + Environment.NewLine + ModelData.GuarantorList[i].cm_client_nam + " (" + ModelData.GuarantorList[i].im_id_typ + " No. " + ModelData.GuarantorList[i].im_id_num + ") " + Environment.NewLine + "Date:";
							if (needSpace)
								lstLeftguaramtorlist += Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;

						}
						else
						{
							lstRightguaramtorlist += "............................................................." + Environment.NewLine + ModelData.GuarantorList[i].cm_client_nam + " (" + ModelData.GuarantorList[i].im_id_typ + " No. " + ModelData.GuarantorList[i].im_id_num + ") " + Environment.NewLine + "Date:";
							if (needSpace)
								lstRightguaramtorlist += Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;
						}

					}
				}
				newDoc.ReplaceText("lstLeftguaramtorlist", lstLeftguaramtorlist);
				newDoc.ReplaceText("lstRightguaramtorlist", lstRightguaramtorlist);
				#endregion End Signature Section

				#region Acknowledgement Section
				if (String.IsNullOrEmpty(lstPropertyDetails))
				{
					for (int i = 1; i < 12; i++)
					{
						newDoc.ReplaceText("str_" + i + "_ACMTitle", "");
						newDoc.ReplaceText("str_" + i + "_ProAddress", "");
						newDoc.ReplaceText("str_" + i + "_ToTitle", "");
						newDoc.ReplaceText("str_" + i + "_BodyHeader", "");
						newDoc.ReplaceText("lst_" + i + "_MortgagorList", "");
						newDoc.ReplaceText("str_" + i + "_BodyACM", "");
						newDoc.ReplaceText("Lst_" + i + "_LeftDateACM", "");
						newDoc.ReplaceText("Lst_" + i + "_RightDateACM", "");
						newDoc.ReplaceText("str_" + i + "_lineSpace", "");
					}
					//Child
					newDoc.ReplaceText("strChildAC", "");
					newDoc.ReplaceText("strChildPropertyAddress", "");
					newDoc.ReplaceText("strChildToTitle", "");
					newDoc.ReplaceText("strChHeader", "");
					newDoc.ReplaceText("lstChMortgagorList", "");
					newDoc.ReplaceText("strHearBy", "");
					newDoc.ReplaceText("lstclientMor", "");
					newDoc.ReplaceText("strAsSec", "");
					newDoc.ReplaceText("strCCName", "");
					newDoc.ReplaceText("strToAdd", "");
					newDoc.ReplaceText("strCNameByC", "");
					newDoc.ReplaceText("strChildBody", "");
					newDoc.ReplaceText("lstLeftChildDate", "");
					newDoc.ReplaceText("lstRightChildDate", "");
				}
				else
				{
					#region Acknowledgement 

					if (ModelData.SecurityPropertyModalDetails.Count > 0)
					{
						var totalCount = ModelData.SecurityPropertyModalDetails.Count;
						int listCount = 1;
						for (int i = 0; i < ModelData.SecurityPropertyModalDetails.Count; i++)
						{
							var strACMTitle = "ACKNOWLEDGEMENT AND CONSENT BY MORTGAGOR";
							var strProAddress = "Property Address: " + ModelData.SecurityPropertyModalDetails[i].PropertyAddress+ Environment.NewLine;
							var strToTitle = "To : " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + Environment.NewLine;

							var strBodyHeader = "I/We, ";
							var strBodyACM = " hereby confirm my/our consent to the terms and conditions set out in foregoing facility letter and consent to the provision of the property as security for the Facility granted to the Borrower.The Mortgage and Security Documents to be executed in your favour shall not be prejudiced, diminished or affected or discharged or impaired by any restructure, amendment, modification or variation to the terms and conditions or the pricing under the facility letter or the Deed of Covenants/Secured Term Loan Facility Agreement from time to time, with or without notice to me/us."+ Environment.NewLine + Environment.NewLine + Environment.NewLine;
							

							var lstLeftDateACM = "";
							var lstRightDateACM = "";
							var strlineSpace = "";
							var lstClientList = "";
							List<GuarantorOrMortgagorListViewModel> lstproperty = new List<GuarantorOrMortgagorListViewModel>();
							if (ModelData.MortgagorListForAcknowledgement.Count > 0)
							{
								lstproperty = ModelData.MortgagorListForAcknowledgement.Where(x => x.SecurityID == ModelData.SecurityPropertyModalDetails[i].ID).ToList();

								bool isEvenNUmber = lstproperty.Count % 2 == 0 ? true : false;
								for (int j = 0; j < lstproperty.Count; j++)
								{
									bool needSpace = (j == (lstproperty.Count - 1)) ? false : isEvenNUmber ? (j == (lstproperty.Count - 2)) ? false : true : true;

									if (j % 2 == 0)
									{

										lstLeftDateACM += "............................................................." + Environment.NewLine + lstproperty[j].cm_client_nam + " (" + lstproperty[j].im_id_typ + " No. " + lstproperty[j].im_id_num + ")" + Environment.NewLine + "Date:";
										if (needSpace)
											lstLeftDateACM += Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;
									}
									else
									{
										lstRightDateACM += "..............................................................." + Environment.NewLine + lstproperty[j].cm_client_nam + " (" + lstproperty[j].im_id_typ + " No. " + lstproperty[j].im_id_num + ")" + Environment.NewLine + "Date:";
										if (needSpace)
											lstRightDateACM += Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;
									}

								}

								if (lstproperty.Count == 1)
								{
									lstClientList += lstproperty[0].cm_client_nam + " (" + lstproperty[0].im_id_typ + " No. " + lstproperty[0].im_id_num + ")";
								}
								else if (lstproperty.Count > 1)
								{
									var aktotalCount = lstproperty.Count;
									var iCount = 1;
									string aksaperator = ", ";
									foreach (var item in lstproperty)
									{

										if (lstproperty.Count == 2 && iCount == 1)
										{
											aksaperator = " and ";
										}
										else if (lstproperty.Count > 2 && (iCount == (aktotalCount - 1)))
										{
											aksaperator = ", and ";
										}

										lstClientList += item.cm_client_nam + " (" + item.im_id_typ + " No. " + item.im_id_num + ")" + aksaperator;
										iCount++;
										if (iCount == aktotalCount)
										{
											aksaperator = "";
										}
									}

								}
							}

							if (i != (totalCount - 1))
							{
								strlineSpace += Environment.NewLine+ Environment.NewLine+ Environment.NewLine;
							}

							newDoc.ReplaceText("str_" + listCount + "_ACMTitle", strACMTitle);
							newDoc.ReplaceText("str_" + listCount + "_ProAddress", strProAddress);
							newDoc.ReplaceText("str_" + listCount + "_ToTitle", strToTitle);
							newDoc.ReplaceText("str_" + listCount + "_BodyHeader", strBodyHeader);
							newDoc.ReplaceText("lst_" + listCount + "_MortgagorList", lstClientList);
							newDoc.ReplaceText("str_" + listCount + "_BodyACM", strBodyACM);
							newDoc.ReplaceText("Lst_" + listCount + "_LeftDateACM", lstLeftDateACM);
							newDoc.ReplaceText("Lst_" + listCount + "_RightDateACM", lstRightDateACM);
							newDoc.ReplaceText("str_" + listCount + "_lineSpace", strlineSpace);


							listCount++;

						}
						if (totalCount < 12)
						{
							for (int i = 1; i < 12; i++)
							{
								newDoc.ReplaceText("str_" + i + "_ACMTitle", "");
								newDoc.ReplaceText("str_" + i + "_ProAddress", "");
								newDoc.ReplaceText("str_" + i + "_ToTitle", "");
								newDoc.ReplaceText("str_" + i + "_BodyHeader", "");
								newDoc.ReplaceText("lst_" + i + "_MortgagorList", "");
								newDoc.ReplaceText("str_" + i + "_BodyACM", "");
								newDoc.ReplaceText("Lst_" + i + "_LeftDateACM", "");
								newDoc.ReplaceText("Lst_" + i + "_RightDateACM", "");
								newDoc.ReplaceText("str_" + i + "_lineSpace", "");
							}
						}
					}
					else
					{
						for (int i = 1; i < 12; i++)
						{
							newDoc.ReplaceText("str_"+ i + "_ACMTitle", "");
							newDoc.ReplaceText("str_" + i + "_ProAddress", "");
							newDoc.ReplaceText("str_" + i + "_ToTitle", "");
							newDoc.ReplaceText("str_" + i + "_BodyHeader", "");
							newDoc.ReplaceText("lst_" + i + "_MortgagorList", "");
							newDoc.ReplaceText("str_" + i + "_BodyACM", "");
							newDoc.ReplaceText("Lst_" + i + "_LeftDateACM", "");
							newDoc.ReplaceText("Lst_" + i + "_RightDateACM", "");
							newDoc.ReplaceText("str_" + i + "_lineSpace", "");
						}

					}
					#endregion

					#region Child Section

					if (ModelData.strChildConsentInd == "N" || ModelData.strChildConsentInd == null)
					{
						newDoc.ReplaceText("strChildAC", "");
						newDoc.ReplaceText("strChildPropertyAddress", "");
						newDoc.ReplaceText("strChildToTitle", "");
						newDoc.ReplaceText("strChHeader", "");
						newDoc.ReplaceText("lstChMortgagorList", "");
						newDoc.ReplaceText("strHearBy", "");
						newDoc.ReplaceText("lstclientMor", "");
						newDoc.ReplaceText("strAsSec", "");
						newDoc.ReplaceText("strCCName", "");
						newDoc.ReplaceText("strToAdd", "");
						newDoc.ReplaceText("strCNameByC", "");
						newDoc.ReplaceText("strChildBody", "");
						newDoc.ReplaceText("strChildDate", "");
						newDoc.ReplaceText("lstLeftChildDate", "");
						newDoc.ReplaceText("lstRightChildDate", "");

					}
					else
					{
						newDoc.ReplaceText("strChildAC", "ACKNOWLEDGEMENT AND CONSENT");
						newDoc.ReplaceText("strChildPropertyAddress", "Property Address: " + ModelData.SecurityPropertyModalDetails[0].PropertyAddress + Environment.NewLine);
						newDoc.ReplaceText("strChildToTitle", "To : " + (ModelData.strCompanyClientName == null ? "" : ModelData.strCompanyClientName) + Environment.NewLine);

						var strChHeader = "I/We, ";
						var strHearBy = " hereby confirm my/our consent to the provision of the property by my " + (ChildConsentMortgagorlist.Count > 0 ? ChildConsentMortgagorlist[0].GroupMemberDesc : " ")+" ";
						var strAsSec = ", as security for the Secured Term Loan Facility granted by ";
						var strChildBody = " The Mortgage and Security Documents to be executed in your favour shall not be prejudiced, diminished or affected or discharged or impaired by any restructure, amendment, modification or variation to the terms and conditions or the pricing under the Facility letter or the Deed of Covenants/Secured Term Loan Facility Agreement from time to time, with or without notice to me/us." + Environment.NewLine + Environment.NewLine + Environment.NewLine;
						var lstChildHeaderList = "";
						var lstChildLeftDateACM = "";
						var lstChildRightDateACM = "";

						if (ModelData.ChildConsentIndDetailsList.Count > 0)
						{
							var totNumber = ModelData.ChildConsentIndDetailsList[0].ChildNumber;
							var intCount = 1;
							string strsaperator = ", ";

							if (totNumber == 1)
								{
									lstChildHeaderList += "___________________________ (NRIC No. _________________)";
									lstChildLeftDateACM += "............................................................." + Environment.NewLine + "Name: " + Environment.NewLine + "NRIC No. " + Environment.NewLine + "Date: ";
								}
								else if (totNumber > 1)
								{

									bool isEvenNUmber = totNumber % 2 == 0 ? false : true;
									for (int j = 0; j < totNumber; j++)
									{
										bool needSpace = (j == (totNumber - 1)) ? false : isEvenNUmber ? (j == (totNumber - 2)) ? false : true : true;

										if (totNumber == 2 && intCount == 1)
										{
											strsaperator = " and ";
										    needSpace = false;
										}
										else if (totNumber > 2 && intCount == (totNumber - 1))
										{
											strsaperator = ", and ";
										}

										lstChildHeaderList += "___________________________ (NRIC No. _________________)" + strsaperator;
										intCount++;
										if (intCount == totNumber)
										{
											strsaperator = "";
										}

										if (j % 2 == 0)
										{
											lstChildLeftDateACM += "...................................." + Environment.NewLine + "Name: " + Environment.NewLine + "NRIC No. " + Environment.NewLine + "Date: ";
											if (needSpace)
											{
												lstChildLeftDateACM += Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;
											}
										}
										else
										{
											lstChildRightDateACM += "............................................................." + Environment.NewLine + "Name: " + Environment.NewLine + "NRIC No. " + Environment.NewLine + "Date: ";
											if (needSpace)
											{
												lstChildRightDateACM += Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;
											}
										}
									}
							}

						}
						newDoc.ReplaceText("strChHeader", strChHeader);
						newDoc.ReplaceText("lstChMortgagorList", lstChildHeaderList);
						newDoc.ReplaceText("strHearBy", strHearBy);
						newDoc.ReplaceText("lstclientMor", lstChildConsentMortgagorList);
						newDoc.ReplaceText("strAsSec", strAsSec);
						newDoc.ReplaceText("strCCName", ModelData.strCompanyClientName);
						newDoc.ReplaceText("strToAdd", " to ");
						newDoc.ReplaceText("strCNameByC", (ModelData.strChildAcknowledegmentClientNameByCustomer != null ? ModelData.strChildAcknowledegmentClientNameByCustomer : ""));
						newDoc.ReplaceText("strChildBody", strChildBody);
						newDoc.ReplaceText("lstLeftChildDate", lstChildLeftDateACM);
						newDoc.ReplaceText("lstRightChildDate", lstChildRightDateACM);

					}
					#endregion
				}


				#endregion End Acknowledgement

				#region Last Page Bottom
				newDoc.ReplaceText("str_PreparedBy", ModelData.strPreparedBy == null ? "" : ModelData.strPreparedBy);
				newDoc.ReplaceText("str_PreparedDate", ModelData.strPreparedDate == null ? "" : ModelData.strPreparedDate);
				newDoc.ReplaceText("strReferenceContractNumber", ModelData.strContractNumber);
				#endregion End Last Page Bottom

				#region Doc File Save And Download Logic
				newDoc.AddPasswordProtection(Xceed.Document.NET.EditRestrictions.readOnly, "ETHOZ");
				newDoc.SaveAs(fileName);

				if (System.IO.File.Exists(fileName))
				{
					byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
					string fileName1 = "LetterOffer_" + ModelData.strContractNumber + "-" + (ModelData.ContractDetails == null ? "" : ModelData.ContractDetails.RolloverNumber.ToString())+"(v"+ (ModelData.ContractDetails == null ? "" : ModelData.ContractDetails.VersionNumber.ToString()) + ")_"+ DateTime.Now.ToString("yyMMddHHmmss") + ".docx";
					System.IO.File.Delete(fileName);
					glog.Debug("HttpPost FnGenerateTermLoanLOF: Exit");
					return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName1);
				}
				else
				{
					return Json(null, JsonRequestBehavior.AllowGet);
				}
				#endregion
				#endregion

			}
			catch (Exception ex)
			{
				glog.Error("Please contact MIS, error:" + ex.Message);
				var error = "Please contact MIS, error:" + ex.Message;
				return Json(error, JsonRequestBehavior.AllowGet);
			}

		}
		#endregion
	}
}