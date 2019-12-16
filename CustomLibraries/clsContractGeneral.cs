using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace EthozCapital.CustomLibraries
{
	public class clsContractGeneral
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(clsContractGeneral));

		public clsContractGeneral()
		{
		}
		public List<SelectListItem> GetSubCtrByCtrNo(string id)
		{
			try
			{
				List<SelectListItem> SubCon = new List<SelectListItem>();

				var db = new MainDbContext();
				var li = db.Sys_ListOfValue.Where(u => u.GroupCode.ToString().Trim() == id);
				List<Sys_ListOfValue> a = li.ToList();

				var GroupCode = a[0].GroupCode.ToString().Trim();
				var SubConList = db.Sys_ListOfValue.Where(u => u.ParentID.ToString().Trim() == GroupCode);
				List<Sys_ListOfValue> b = SubConList.ToList();

				for (int i = 0; i < b.Count; i++)
				{
					SubCon.Add(new SelectListItem { Text = b[i].GroupMemberDesc.ToString().Trim(), Value = b[i].ParentID.ToString().Trim() });
				}

				return SubCon;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public List<SelectListItem> GetSubProdByProdCode(string id)
		{
			try
			{
				List<SelectListItem> SubProd = new List<SelectListItem>();

				if (id != "")
				{
					//Get SubProd List
					var db = new MainDbContext();
					var li = db.Sys_ListOfValue.Where(u => u.GroupCode.ToString().Trim() == id);
					List<Sys_ListOfValue> a = li.ToList();

					var GroupCode = a[0].GroupCode.ToString().Trim();
					var GroupName = a[0].GroupMemberDesc.ToString().Trim();
					var SubProdList = db.Sys_ListOfValue.Where(u => u.ParentID.ToString().Trim() == GroupCode);
					SubProdList = SubProdList.OrderBy(w => w.GroupMemberDesc);
					List<Sys_ListOfValue> b = SubProdList.ToList();

					//SubProd.Add(new SelectListItem { Text = "", Value = "" }); //First Value to display empty after Product Type is clicked.
					for (int i = 0; i < b.Count; i++)
					{
						//Value = b[i].GroupCode to get selected value of group code in the dataset.
						SubProd.Add(new SelectListItem { Text = b[i].GroupMemberDesc.ToString().Trim(), Value = b[i].GroupCode.ToString().Trim() });
					}
				}

				return SubProd;
			}
			catch (Exception)
			{
				throw;
			}
		}
		public List<LEFSIntrestCode> GetLEFSInterestCode(string subConType)
		{
			glog.Debug("GetLEFSInterestCode: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = db.Maintenance_LEFSInterestCode.Where(x => x.Status == "O"
											&& x.SubContractType == subConType
											&& x.EffectiveDate <= DateTime.Now)
											.OrderBy(x => x.InterestCode).Select(x => new LEFSIntrestCode()
											{
												CoyRate = x.CoyRate,
												InterestCode = x.InterestCode + " - " + x.Description,
												InterestType = x.InterestType,
												Description = x.Description
											}).ToList();
					glog.Debug("GetLEFSInterestCode: Exit");
					return lst;
				}
				catch (Exception ex)
				{
					glog.Error("GetLEFSInterestCode Exception: " + ex.Message);
					return new List<LEFSIntrestCode>();
				}
			}
		}
		
		public List<Security_PropertyModel> GetPropertyAddress(string cm_client_cod)
		{
			glog.Debug("GetPropertyAddress: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = from s in db.Security_Property // outer sequence
							  join st in db.Security_PropertyCustomer //inner sequence 
							  on s.ID equals st.MasterID // key selector 
							  where s.Status == "O" && st.Status == "O"
							  && st.Customer == cm_client_cod
							  orderby s.PropertyAddress
							  select s;

					var res = lst.AsEnumerable().Select(x => new Security_PropertyModel()
					{
						ChargeDate = x.ChargeDate != null ? x.ChargeDate.Value.ToShortDateString() : "",
						FormalValuation = x.FormalValuation != null ? x.FormalValuation.Value.ToString("0.00") : "",
						ChargeNumber = x.ChargeNumber,
						CreatedBy = x.CreatedBy,
						CreatedDate = x.CreatedDate != null ? x.CreatedDate.Value.ToShortDateString() : "",
						CreditLimit = x.CreditLimit != null ? x.CreditLimit.Value.ToString("0.00") : "",
						FirstThirdParty = x.FirstThirdParty,
						ID = x.ID,
						IndicativeValuation = x.IndicativeValuation != null ? x.IndicativeValuation.Value.ToString("0.00") : "",
						MortgageNumber = x.MortgageNumber,
						PropertyAddress = x.PropertyAddress,
						PropertyTypeLevel1 = x.PropertyTypeLevel1,
						PropertyTypeLevel2 = x.PropertyTypeLevel2,
						SecurityListLevel2 = x.SecurityListLevel2,
						Status = x.Status,
						TitleNumber = x.TitleNumber,
						UpdatedBy = x.UpdatedBy,
						UpdatedDate = x.UpdatedDate != null ? x.UpdatedDate.Value.ToShortDateString() : "",
					}).ToList();
					glog.Debug("GetPropertyAddress: Exit");
					return res;
				}
				catch (Exception ex)
				{
					glog.Error("GetPropertyAddress Exception: " + ex.Message);
					return new List<Security_PropertyModel>();
				}
			}
		}
		public List<Security_PropertyMortgagor> GetSecurityPropertyMortgagor(string propertyAddressID)
		{
			glog.Debug("GetSecurityPropertyMortgagor: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = db.Security_PropertyMortgagor.Where(x => x.MasterID == propertyAddressID
					&& x.Status == "O").OrderBy(x => x.Mortgagor).ToList();
					glog.Debug("GetSecurityPropertyMortgagor: Exit");
					return lst;
				}
				catch (Exception ex)
				{
					glog.Error("GetSecurityPropertyMortgagor Exception: " + ex.Message);
					return new List<Security_PropertyMortgagor>();
				}
			}
		}
		public List<Security_VesselMortgagor> GetSecurityVesselMortgagor(string selectedHullVessel)
		{
			glog.Debug("GetSecurityVesselMortgagor: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = db.Security_VesselMortgagor.Where(x => x.MasterID == selectedHullVessel
					&& x.Status == "O").OrderBy(x => x.Mortgagor).ToList();
					glog.Debug("GetSecurityVesselMortgagor: Exit");
					return lst;
				}
				catch (Exception ex)
				{
					glog.Error("GetSecurityVesselMortgagor Exception: " + ex.Message);
					return new List<Security_VesselMortgagor>();
				}
			}
		}		
		public List<Security_VesselModel> GetHullNumberAndVesselName(string cm_client_cod)
		{
			glog.Debug("GetHullNumberAndVesselName: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = from s in db.Security_Vessel // outer sequence
							  join st in db.Security_VesselCustomer //inner sequence 
							  on s.ID equals st.MasterID // key selector 
							  where s.Status == "O" && st.Status == "O"
							  && st.Customer == cm_client_cod
							  orderby s.HullNumber
							  select s;
					glog.Debug("GetHullNumberAndVesselName: Exit");
					return lst.AsEnumerable().Select(x => new Security_VesselModel()
					{
						ChargeDate = x.ChargeDate != null ? x.ChargeDate.Value.ToShortDateString() : "",
						FormalValuation = x.FormalValuation != null ? x.FormalValuation.Value.ToString("0.00") : "",
						ChargeNumber = x.ChargeNumber,
						CreatedBy = x.CreatedBy,
						CreatedDate = x.CreatedDate != null ? x.CreatedDate.Value.ToShortDateString() : "",
						CreditLimit = x.CreditLimit != null ? x.CreditLimit.Value.ToString("0.00") : "",
						ID = x.ID,
						IndicativeValuation = x.IndicativeValuation != null ? x.IndicativeValuation.Value.ToString("0.00") : "",
						MortgageNumber = x.MortgageNumber,
						SecurityListLevel2 = x.SecurityListLevel2,
						CountryOfReg = x.CountryOfReg,
						HullNumber = x.HullNumber,
						VesselName = x.VesselName,
						Status = x.Status,
						UpdatedBy = x.UpdatedBy,
						UpdatedDate = x.UpdatedDate != null ? x.UpdatedDate.Value.ToShortDateString() : ""
					}).ToList();
				}
				catch (Exception ex)
				{
					glog.Error("GetHullNumberAndVesselName Exception: " + ex.Message);
					return new List<Security_VesselModel>();
				}
			}
		}
		public List<InsuranceModel> GetSecurityVesselInsurance(string HullNumber)
		{
			glog.Debug("GetSecurityVesselInsurance: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = db.Security_VesselInsurance.Where(x => x.MasterID == HullNumber
					&& x.Status == "O").ToList().Select(x => new InsuranceModel()
					{
						InsuranceType = db.Sys_ListOfValue.Where(v => v.GroupCode == x.InsuranceType && v.Status == "O" &&
						v.GroupType == "VESSEL_INSURANCE_TYPE").Select(a => a.GroupMemberDesc).FirstOrDefault() ?? "",
						ExpiryDate = x.ExpiryDate.Value.ToShortDateString(),
					}).OrderBy(x => x.InsuranceType).ToList();
					glog.Debug("GetSecurityVesselInsurance: Exit");
					return lst;
				}
				catch (Exception ex)
				{
					glog.Error("GetSecurityVesselInsurance Exception: " + ex.Message);
					return new List<InsuranceModel>();
				}
			}
		}

		public List<Security_VehicleModel> getSecVehicleByContract(string[] valueMatrix, string crossCollateralizationContactNumber, int crossCollateralizationRolloverNumber)
		{
			glog.Debug("getSecVehicleByContract : Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var status = new List<string>() { "P", "E", "O" };
					var A = db.PreContract_Master.Where(x => status.Contains(x.Status) &&
					(valueMatrix.Any() ? valueMatrix.Contains(x.SubContractType) : true) &&
					x.ContractNumber == crossCollateralizationContactNumber &&
					x.RolloverNumber == crossCollateralizationRolloverNumber).Select(x => new { x.ContractNumber, x.RolloverNumber }).ToList();

					var B = db.Contract_Master.Where(x => status.Contains(x.Status) &&
					x.ContractNumber == crossCollateralizationContactNumber &&
					x.RolloverNumber == crossCollateralizationRolloverNumber).Select(x => new { x.ContractNumber, x.RolloverNumber }).ToList();

					var C = db.PreContract_SecurityItem.Select(x => new { x.ContractNumber, x.RolloverNumber, x.SecurityListLevel2, x.SecurityID }).ToList();
					var D = db.Contract_SecurityItem.Select(x => new { x.ContractNumber, x.RolloverNumber, x.SecurityListLevel2, x.SecurityID }).ToList();

					var tableA = A.Union(B);
					var tableB = C.Union(D).Where(x => x.SecurityListLevel2 == clsVariables.ConstDebentureVehicle);// "SLL2-1003"
					var tableC = db.Security_Vehicle.Where(x => x.Status == "O").ToList();

					var mergetblAwithtblB = tableB.Where(x => tableA.Select(e => e.ContractNumber).Contains(x.ContractNumber)
					&& tableA.Select(e => e.RolloverNumber).Contains(x.RolloverNumber)).ToList();

					var finalResult = tableC.Where(x => mergetblAwithtblB.Select(e => e.SecurityID).Contains(x.ID))
					   .Select(x=>new Security_VehicleModel() {
							 ChargeDate=x.ChargeDate != null ? x.ChargeDate.Value.ToShortDateString() : "",
							 ChargeNumber=x.ChargeNumber,
							 ChassisNumber=x.ChassisNumber,
							 COE_ExpiryDate=x.COE_ExpiryDate != null ? x.COE_ExpiryDate.Value.ToShortDateString() : "",
							 EngineNumber =x.EngineNumber,
							 ID=x.ID,
							 RegNumber=x.RegNumber,
							 SecurityListLevel2=x.SecurityListLevel2,
							 Value= x.Value != null ? x.Value.Value.ToString("0.00") : "0.00",
							 VehicleMake =x.VehicleMake,
							 VehicleType=x.VehicleType,
							 VehicleModel=x.VehicleModel							 
						 })
						.ToList();

					glog.Debug("getSecVehicleByContract : Exit");
					return finalResult;
				}
				catch (Exception ex)
				{
					glog.Error("getSecVehicleByContract  Exception: " + ex.Message);
					return new List<Security_VehicleModel>();
				}
			}
		}
		public List<Security_VehicleModel> GetVehicleChassisAndRegNumber(string cm_client_cod)
		{
			glog.Debug("GetVehicleChassisAndRegNumber: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = (from s in db.Security_Vehicle// outer sequence
							   join st in db.Security_VehicleCustomer //inner sequence 
							   on s.ID equals st.MasterID // key selector 
							   where s.Status == "O" && st.Status == "O"
							   && st.Customer == cm_client_cod
							   orderby s.ChassisNumber
							   select s).AsEnumerable().Select(x => new Security_VehicleModel()
								 {
									 ChargeDate = x.ChargeDate != null ? x.ChargeDate.Value.ToShortDateString() : "",
									 ChargeNumber = x.ChargeNumber,
									 ChassisNumber = x.ChassisNumber,
									 COE_ExpiryDate = x.COE_ExpiryDate != null ? x.COE_ExpiryDate.Value.ToShortDateString() : "",
									 EngineNumber = x.EngineNumber,
									 ID = x.ID,
									 RegNumber = x.RegNumber,
									 SecurityListLevel2 = x.SecurityListLevel2,
									 Value = x.Value!=null?x.Value.Value.ToString("0.00"):"0.00",
									 VehicleMake = x.VehicleMake,
									 VehicleType = x.VehicleType,
									 VehicleModel = x.VehicleModel
								 }).ToList();
					glog.Debug("GetVehicleChassisAndRegNumber: Exit");
					return lst;
				}
				catch (Exception ex)
				{
					glog.Error("GetVehicleChassisAndRegNumber Exception: " + ex.Message);
					return new List<Security_VehicleModel>();
				}
			}
		}		
		public List<Security_InventoryModel> GetInventoryTypeDescription(string cm_client_cod)
		{
			glog.Debug("GetInventoryTypeDescription: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = (from s in db.Security_Inventory// outer sequence
							   join st in db.Security_InventoryCustomer //inner sequence 
							   on s.ID equals st.MasterID // key selector 
							   where s.Status == "O" && st.Status == "O"
							   && st.Customer == cm_client_cod
							   orderby s.Type
							   select s).ToList();
					var res = lst.AsEnumerable().Select(x => new Security_InventoryModel()
					{
						ChargeDate = x.ChargeDate != null ? x.ChargeDate.Value.ToShortDateString() : "",
						ChargeNumber = x.ChargeNumber,
						ID = x.ID,
						Type = x.Type,
						Value = x.Value.ToString("0.00")
					}).ToList();
					glog.Debug("GetInventoryTypeDescription: Exit");
					return res;
				}
				catch (Exception ex)
				{
					glog.Error("GetInventoryTypeDescription Exception: " + ex.Message);
					return new List<Security_InventoryModel>();
				}
			}
		}
		
		public List<Security_ReceivableModel> GetReceivableAmount(string cm_client_cod)
		{
			glog.Debug("GetReceivableAmount: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = (from s in db.Security_Receivable// outer sequence
							   join st in db.Security_ReceivableCustomer //inner sequence 
							   on s.ID equals st.MasterID // key selector 
							   where s.Status == "O" && st.Status == "O"
							   && st.Customer == cm_client_cod
							   orderby s.Amount
							   select s).ToList();
					var res = lst.AsEnumerable().Select(x => new Security_ReceivableModel()
					{
						ChargeDate = x.ChargeDate != null ? x.ChargeDate.Value.ToShortDateString() : "",
						ChargeNumber = x.ChargeNumber,
						ID = x.ID,
						Amount = x.Amount.ToString("0.00")
					}).ToList();
					glog.Debug("GetReceivableAmount: Exit");
					return res;
				}
				catch (Exception ex)
				{
					glog.Error("GetReceivableAmount Exception: " + ex.Message);
					return new List<Security_ReceivableModel>();
				}
			}
		}		
		public List<Security_CashEquivalentIndModel> GetCashEquivalentInd(string cm_client_cod)
		{
			glog.Debug("GetCashEquivalentInd: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = (from s in db.Security_CashEquivalentInd// outer sequence
							   join st in db.Security_CashEquivalentIndCustomer //inner sequence 
							   on s.ID equals st.MasterID // key selector 
							   where s.Status == "O" && st.Status == "O"
							   && st.Customer == cm_client_cod
							   orderby s.Amount
							   select s).ToList();
					var res = lst.AsEnumerable().Select(x => new Security_CashEquivalentIndModel()
					{
						ID = x.ID,
						BillToCustomer = x.BillToCustomer,
						Amount = x.Amount.ToString("0.00"),
						GuaranteeBondsType = x.GuaranteeBondsType,
						Refundable = x.Refundable == "Y" ? "Yes" : "No",
						BillToAddress = x.BillToAddress,
						BillToConPerson = x.BillToConPerson,
						BillToDept = x.BillToDept
					}).ToList();
					glog.Debug("GetCashEquivalentInd: Exit");
					return res;
				}
				catch (Exception ex)
				{
					glog.Error("GetCashEquivalentInd Exception: " + ex.Message);
					return new List<Security_CashEquivalentIndModel>();
				}
			}
		}
		
		public List<Security_CashEquivalentIndModel> GetCashEquivalentCom(string cm_client_cod)
		{
			glog.Debug("GetCashEquivalentInd: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = (from s in db.Security_CashEquivalentCom// outer sequence
							   join st in db.Security_CashEquivalentComCustomer //inner sequence 
							   on s.ID equals st.MasterID // key selector 
							   where s.Status == "O" && st.Status == "O"
							   && st.Customer == cm_client_cod
							   orderby s.Amount
							   select s).ToList();
					var res = lst.AsEnumerable().Select(x => new Security_CashEquivalentIndModel()
					{
						ID = x.ID,
						BillToCustomer = x.BillToCustomer,
						Amount = x.Amount.ToString("0.00"),
						GuaranteeBondsType = x.GuaranteeBondsType,
						Refundable = x.Refundable == "Y" ? "Yes" : "No",
						BillToAddress = x.BillToAddress,
						BillToConPerson = x.BillToConPerson,
						BillToDept = x.BillToDept
					}).ToList();
					glog.Debug("GetCashEquivalentInd: Exit");
					return res;
				}
				catch (Exception ex)
				{
					glog.Error("GetCashEquivalentInd Exception: " + ex.Message);
					return new List<Security_CashEquivalentIndModel>();
				}
			}
		}
		
		public List<Security_SecFinInstrumentsModel> GetAmountAndDocumentNumber(string cm_client_cod)
		{
			glog.Debug("GetAmountAndDocumentNumber: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = (from s in db.Security_SecFinInstruments// outer sequence
							   join st in db.Security_SecFinInstrumentsCustomer //inner sequence 
							   on s.ID equals st.MasterID // key selector 
							   where s.Status == "O" && st.Status == "O"
							   && st.Customer == cm_client_cod
							   orderby s.Amount,s.DocumentNumber
							   select s).ToList();
					var res = lst.AsEnumerable().Select(x => new Security_SecFinInstrumentsModel()
					{
						ID = x.ID,
						Amount = x.Amount.ToString("0.00"),
						DocumentNumber = x.DocumentNumber,
						BankFinancialCom = x.BankFinancialCom,
						Type = x.Type??"",
						ChargeDate = x.ChargeDate != null ? x.ChargeDate.Value.ToShortDateString() : ""
					}).ToList();
					glog.Debug("GetAmountAndDocumentNumber: Exit");
					return res;
				}
				catch (Exception ex)
				{
					glog.Error("GetAmountAndDocumentNumber Exception: " + ex.Message);
					return new List<Security_SecFinInstrumentsModel>();
				}
			}
		}
		
		public List<Security_CashEquivalentIndModel> GetSecurityDepositBillToAmount(string cm_client_cod)
		{
			glog.Debug("GetSecurityDepositBillToAmount: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var lst = (from s in db.Security_SecDeposit// outer sequence
							   join st in db.Security_SecDepositCustomer //inner sequence 
							   on s.ID equals st.MasterID // key selector 
							   where s.Status == "O" && st.Status == "O"
							   && st.Customer == cm_client_cod							  
							   select s).ToList();
					var res = lst.AsEnumerable().Select(x => new Security_CashEquivalentIndModel()
					{
						ID = x.ID,
						BillToCustomer = x.BillToCustomer,
						Amount = x.Amount.ToString("0.00"),
						Refundable = x.Refundable == "Y" ? "Yes" : "No",
						BillToAddress = x.BillToAddress,
						BillToConPerson = x.BillToConPerson,
						BillToDept = x.BillToDept
					}).ToList();
					glog.Debug("GetSecurityDepositBillToAmount: Exit");
					return res;
				}
				catch (Exception ex)
				{
					glog.Error("GetSecurityDepositBillToAmount Exception: " + ex.Message);
					return new List<Security_CashEquivalentIndModel>();
				}
			}
		}

		#region Printing Report 

		#region Get Contract Number Is Valid Or not
		public int GetIsContractNumberValid(string strContractNumber)
		{
			glog.Debug("GetIsContractNumberValid: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<int>(
						"exec GetIsContractNumberValid @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetIsContractNumberValid: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetIsContractNumberValid Exception: " + Ex.Message);
				throw;
			}

		}

		#endregion

		#region Get GroupCode By Group Member Desc
		public string GetGroupCodeByGroupMemberDesc(string strGroupMemberDesc)
		{
			glog.Debug("GetGroupCodeByGroupMemberDesc: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<string>(
						"exec GetGroupCodeByGroupMemberDesc @GroupMemberDesc",
						new SqlParameter("@GroupMemberDesc", string.IsNullOrWhiteSpace(strGroupMemberDesc) ? "" : strGroupMemberDesc)).FirstOrDefault();
					glog.Debug("GetGroupCodeByGroupMemberDesc: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetGroupCodeByGroupMemberDesc Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion
		
		#region Get SubContract Type
		public string FnGetPreContractData(string strContractNumber)
		{
			glog.Debug("FnGetPreContractData: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.PreContract_Master.Where(x => x.ContractNumber == strContractNumber).Select(x => x.SubContractType).FirstOrDefault();
					glog.Debug("FnGetPreContractData: Exit");
					return result;
				}

			}
			catch (Exception Ex)
			{
				glog.Error("FnGetPreContractData Exception: " + Ex.Message);
				throw;
			}
			glog.Debug("FnGetPreContractData: Exit");
		}
		#endregion

		#region Get Sub Contract Type By Contract Number
		public string FnGetContractData(string strContractNumber)
		{
			glog.Debug("FnGetContractData: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Contract_Master.Where(x => x.ContractNumber == strContractNumber).Select(x => x.SubContractType).FirstOrDefault();
					glog.Debug("FnGetContractData: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("FnGetContractData Exception: " + Ex.Message);
				throw;
			}


		}
		#endregion

		#region Get PreContract Details By ContractNumber
		public ContractDetailsViewModel GetContractDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractDetailsByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<ContractDetailsViewModel>(
						"exec GetContractDetailsByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract Details By ContractNumber
		public ContractDetailsViewModel GetPreContractDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractDetailsByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<ContractDetailsViewModel>(
						"exec GetPreContractDetailsByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract_SecuityItem By SecurityListLevel2 And Contract Number 
		public SecurityItem_ByType_ViewModel GetContractSecuityItemDetailsBySecurityListLevel2AndContractNumber(string strContractNumber, string strGroupCode)
		{
			glog.Debug("GetContractSecuityItemDetailsBySecurityListLevel2AndContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<SecurityItem_ByType_ViewModel>(
						"exec GetContractSecuityItemDetailsBySecurityListLevel2AndContractNumber @ContractNumber,@GroupCode",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber),
						new SqlParameter("@GroupCode", string.IsNullOrWhiteSpace(strGroupCode) ? "" : strGroupCode)).FirstOrDefault();

					glog.Debug("GetContractSecuityItemDetailsBySecurityListLevel2AndContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractSecuityItemDetailsBySecurityListLevel2AndContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract_SecuityItem By SecurityListLevel2 And Contract Number 
		public SecurityItem_ByType_ViewModel GetPreContractSecuityItemDetailsBySecurityListLevel2AndContractNumber(string strContractNumber, string strGroupCode)
		{
			glog.Debug("GetPreContractSecuityItemDetailsBySecurityListLevel2AndContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<SecurityItem_ByType_ViewModel>(
						"exec GetPreContractSecuityItemDetailsBySecurityListLevel2AndContractNumber @ContractNumber,@GroupCode",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber),
						new SqlParameter("@GroupCode", string.IsNullOrWhiteSpace(strGroupCode) ? "" : strGroupCode)).FirstOrDefault();

					glog.Debug("GetPreContractSecuityItemDetailsBySecurityListLevel2AndContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractSecuityItemDetailsBySecurityListLevel2AndContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract Property Data Details By Contract Number
		public List<Security_PropertyModel_ViewModel> GetPreContractPropertyDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractPropertyDetailsByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<Security_PropertyModel_ViewModel>(
						"exec GetPreContractPropertyDetailsByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetPreContractPropertyDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractPropertyDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract Property Data Details By Contract Number
		public List<Security_PropertyModel_ViewModel> GetContractPropertyDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractPropertyDetailsByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<Security_PropertyModel_ViewModel>(
						"exec GetContractPropertyDetailsByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetContractPropertyDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractPropertyDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Property Mortgagor Details By SecurityID
		public List<SecurityPropertyMortgagorDetailsViewModel> GetPropertyMortgagorDetailsBySecurityID(string strSecurityID)
		{
			glog.Debug("GetPropertyMortgagorDetailsBySecurityID: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<SecurityPropertyMortgagorDetailsViewModel>(
						"exec GetPropertyMortgagorDetailsBySecurityId @OrixDB_Name,@SecurityID",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@SecurityID", string.IsNullOrWhiteSpace(strSecurityID) ? "" : strSecurityID)).ToList();
					glog.Debug("GetPropertyMortgagorDetailsBySecurityID: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPropertyMortgagorDetailsBySecurityID Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract Schedule Details By Contract Number
		public ContractScheduleViewModel GetContractScheduleDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractScheduleDetailsByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<ContractScheduleViewModel>(
						"exec GetContractScheduleDetailsByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractScheduleDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractScheduleDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract Schedule Details By Contract Number
		public ContractScheduleViewModel GetPreContractScheduleDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractScheduleDetailsByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<ContractScheduleViewModel>(
						"exec GetPreContractScheduleDetailsByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractScheduleDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractScheduleDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract Schedule Details By Contract Number And UpfrontPaymentMth
		public ContractScheduleViewModel GetContractScheduleDetailsByContractNumberAndUpfrontPaymentMth(string strContractNumber, int? intUpfrontPaymentMth)
		{
			glog.Debug("GetContractScheduleDetailsByContractNumberAndUpfrontPaymentMth: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<ContractScheduleViewModel>(
						"exec GetContractScheduleDetailsByContractNumberAndUpfrontPaymentMth @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber),
						new SqlParameter("@UpfrontPaymentMth", intUpfrontPaymentMth)).FirstOrDefault();
					glog.Debug("GetContractScheduleDetailsByContractNumberAndUpfrontPaymentMth: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractScheduleDetailsByContractNumberAndUpfrontPaymentMth Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract Schedule Details By Contract Number And UpfrontPaymentMth
		public ContractScheduleViewModel GetPreContractScheduleDetailsByContractNumberAndUpfrontPaymentMth(string strContractNumber, int? intUpfrontPaymentMth)
		{
			glog.Debug("GetPreContractScheduleDetailsByContractNumberAndUpfrontPaymentMth: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<ContractScheduleViewModel>(
						"exec GetPreContractScheduleDetailsByContractNumberAndUpfrontPaymentMth @ContractNumber,@UpfrontPaymentMth",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber),
						new SqlParameter("@UpfrontPaymentMth", intUpfrontPaymentMth)).FirstOrDefault();
					glog.Debug("GetPreContractScheduleDetailsByContractNumberAndUpfrontPaymentMth: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractScheduleDetailsByContractNumberAndUpfrontPaymentMth Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract Repayment Schedule Details By Contract Number
		public ContractScheduleViewModel GetContractRepaymentScheduleDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractRepaymentScheduleDetailsByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<ContractScheduleViewModel>(
						"exec GetContractRepaymentScheduleDetailsByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractRepaymentScheduleDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractRepaymentScheduleDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract Repayment Schedule Details By Contract Number
		public ContractScheduleViewModel GetPreContractRepaymentScheduleDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractRepaymentScheduleDetailsByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<ContractScheduleViewModel>(
						"exec GetPreContractRepaymentScheduleDetailsByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractRepaymentScheduleDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractRepaymentScheduleDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract WithdrawSuitInd By Contact Number
		public WithdrawSuitIndDetailsViewModel GetContractWithdrawSuitIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractWithdrawSuitIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<WithdrawSuitIndDetailsViewModel>(
						"exec GetContractWithdrawSuitIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractWithdrawSuitIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractWithdrawSuitIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract WithdrawSuitInd By Contact Number
		public WithdrawSuitIndDetailsViewModel GetPreContractWithdrawSuitIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractWithdrawSuitIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<WithdrawSuitIndDetailsViewModel>(
						"exec GetPreContractWithdrawSuitIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractWithdrawSuitIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractWithdrawSuitIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract DiscontSuitInd By Contact Number
		public DiscontSuitIndDetailsViewModel GetContractDiscontSuitIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractDiscontSuitIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<DiscontSuitIndDetailsViewModel>(
						"exec GetContractDiscontSuitIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractDiscontSuitIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractDiscontSuitIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract DiscontSuitInd By Contact Number
		public DiscontSuitIndDetailsViewModel GetPreContractDiscontSuitIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractDiscontSuitIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<DiscontSuitIndDetailsViewModel>(
						"exec GetPreContractDiscontSuitIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractDiscontSuitIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractDiscontSuitIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract CaveatInd By Contact Number
		public CaveatIndDetailsViewModel GetContractCaveatIndDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractCaveatIndDetailsByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<CaveatIndDetailsViewModel>(
						"exec GetContractCaveatIndDetailsByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractCaveatIndDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractCaveatIndDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract CaveatInd By Contact Number
		public CaveatIndDetailsViewModel GetPreContractCaveatIndDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractCaveatIndDetailsByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<CaveatIndDetailsViewModel>(
						"exec GetPreContractCaveatIndDetailsByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractCaveatIndDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractCaveatIndDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract MentalCapacityInd By Contact Number
		public string GetContractMentalCapacityIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractMentalCapacityIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Contract_SecurityItem.Where(x => x.ContractNumber == strContractNumber && x.MentalCapacityInd == "Y").Select(x => x.MentalCapacityInd).FirstOrDefault();
					glog.Debug("GetContractMentalCapacityIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractMentalCapacityIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract MentalCapacityInd By Contract Number
		public string GetPreContractMentalCapacityIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractMentalCapacityIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.PreContract_SecurityItem.Where(x => x.ContractNumber == strContractNumber && x.MentalCapacityInd == "Y").Select(x => x.MentalCapacityInd).FirstOrDefault();
					glog.Debug("GetPreContractMentalCapacityIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractMentalCapacityIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract CPFDischargeInd By Contract Number
		public string GetContractCPFDischargeIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractCPFDischargeIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<string>(
						"exec GetContractCPFDischargeIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractCPFDischargeIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractCPFDischargeIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract CPFDischargeInd By Contract Number
		public string GetPreContractCPFDischargeIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractCPFDischargeIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<string>(
						"exec GetPreContractCPFDischargeIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractCPFDischargeIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractCPFDischargeIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract AddOnLoanInd By Contact Number
		public AddOnLoanIndDetailsViewModel GetContractAddOnLoanIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractAddOnLoanIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<AddOnLoanIndDetailsViewModel>(
						"exec GetContractAddOnLoanIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractAddOnLoanIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractAddOnLoanIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract AddOnLoanInd By Contract Number
		public AddOnLoanIndDetailsViewModel GetPreContractAddOnLoanIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractAddOnLoanIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<AddOnLoanIndDetailsViewModel>(
						"exec GetPreContractAddOnLoanIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractAddOnLoanIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractAddOnLoanIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract PartialPrepayInd By Contact Number
		public List<PartialPrepayIndDetailsViewModel> GetContractPartialPrepayIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractPartialPrepayIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<PartialPrepayIndDetailsViewModel>(
						"exec GetContractPartialPrepayIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetContractPartialPrepayIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractPartialPrepayIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract PartialPrepayInd By Contract Number
		public List<PartialPrepayIndDetailsViewModel> GetPreContractPartialPrepayIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractPartialPrepayIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<PartialPrepayIndDetailsViewModel>(
						"exec GetPreContractPartialPrepayIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetPreContractPartialPrepayIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractPartialPrepayIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract CrossCollaInd By Contact Number
		public CrossCollaIndDetailsViewModel GetContractCrossCollaIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractCrossCollaIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<CrossCollaIndDetailsViewModel>(
						   "exec GetContractCrossCollaIndByContractNumber @ContractNumber",
						   new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractCrossCollaIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractCrossCollaIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract CrossCollaInd By Contract Number
		public CrossCollaIndDetailsViewModel GetPreContractCrossCollaIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractCrossCollaIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<CrossCollaIndDetailsViewModel>(
						   "exec GetPreContractCrossCollaIndByContractNumber @ContractNumber",
						   new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractCrossCollaIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractCrossCollaIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract DeceasedInd By Contract Number
		public string GetContractDeceasedIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractDeceasedIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<string>(
						"exec GetContractDeceasedIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractDeceasedIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractDeceasedIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract DeceasedInd By Contract Number
		public string GetPreContractDeceasedIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractDeceasedIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<string>(
						"exec GetPreContractDeceasedIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractDeceasedIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractDeceasedIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract ChildConsentInd By Contract Number
		public string GetContractChildConsentIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractChildConsentIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<string>(
						"exec GetContractChildConsentIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractChildConsentIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractChildConsentIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract ChildConsentInd By Contract Number
		public string GetPreContractChildConsentIndByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractChildConsentIndByContractNumber: Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<string>(
						"exec GetPreContractChildConsentIndByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractChildConsentIndByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractChildConsentIndByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract GuarantorList By ContractNumber
		public List<GuarantorOrMortgagorListViewModel> GetPreContractGuarantorListByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractGuarantorListByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<GuarantorOrMortgagorListViewModel>(
						"exec GetPreContractGuarantorListByContractNumber @OrixDB_Name,@ContractNumber",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetPreContractGuarantorListByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractGuarantorListByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract GuarantorList By ContractNumber
		public List<GuarantorOrMortgagorListViewModel> GetContractGuarantorListByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractGuarantorListByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<GuarantorOrMortgagorListViewModel>(
						"exec GetContractGuarantorListByContractNumber @OrixDB_Name,@ContractNumber",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetContractGuarantorListByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractGuarantorListByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract MortgagorList By ContractNumber
		public List<GuarantorOrMortgagorListViewModel> GetPreContractMortgagorListByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractMortgagorListByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<GuarantorOrMortgagorListViewModel>(
						"exec GetPreContractMortgagorListByContractNumber @OrixDB_Name,@ContractNumber",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetPreContractMortgagorListByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractMortgagorListByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract MortgagorList By ContractNumber
		public List<GuarantorOrMortgagorListViewModel> GetContractMortgagorListByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractMortgagorListByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<GuarantorOrMortgagorListViewModel>(
						"exec GetContractMortgagorListByContractNumber @OrixDB_Name,@ContractNumber",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetContractMortgagorListByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractMortgagorListByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract MortgagorList For Acknowledgement By ContractNumber
		public List<GuarantorOrMortgagorListViewModel> GetPreContractMortgagorListForAcknowledgementByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractMortgagorListForAcknowledgementByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<GuarantorOrMortgagorListViewModel>(
						"exec GetPreContractMortgagorListForAcknowledgementByContractNumber @OrixDB_Name,@ContractNumber",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetPreContractMortgagorListForAcknowledgementByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractMortgagorListForAcknowledgementByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract MortgagorList For Acknowledgement By ContractNumber
		public List<GuarantorOrMortgagorListViewModel> GetContractMortgagorListForAcknowledgementByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractMortgagorListForAcknowledgementByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<GuarantorOrMortgagorListViewModel>(
						"exec GetContractMortgagorListForAcknowledgementByContractNumber @OrixDB_Name,@ContractNumber",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetContractMortgagorListForAcknowledgementByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractMortgagorListForAcknowledgementByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract DeceasedInd And CpfDischargeInd Details By ContractNumber
		public List<DeceasedIndAndCpfDischargeIndViewModel> GetPreContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<DeceasedIndAndCpfDischargeIndViewModel>(
						"exec GetPreContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber @OrixDB_Name,@ContractNumber",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetPreContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract DeceasedInd And CpfDischargeInd Details By ContractNumber
		public List<DeceasedIndAndCpfDischargeIndViewModel> GetContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<DeceasedIndAndCpfDischargeIndViewModel>(
						"exec GetContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber @OrixDB_Name,@ContractNumber",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractDeceasedIndAndCpfDischargeIndDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract ChildConsentInd Details List By ContractNumber
		public List<ChildConsentIndViewModel> GetPreContractChildConsentIndDetailsByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractChildConsentIndDetailsByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<ChildConsentIndViewModel>(
						"exec GetPreContractChildConsentIndDetailsByContractNumber @OrixDB_Name,@ContractNumber",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetPreContractChildConsentIndDetailsByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractChildConsentIndDetailsByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract  ChildConsentInd Details List By ContractNumber
		public List<ChildConsentIndViewModel> GetContractChildConsentIndByDetailsContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractChildConsentIndByDetailsContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<ChildConsentIndViewModel>(
						"exec GetContractChildConsentIndByDetailsContractNumber @OrixDB_Name,@ContractNumber",
						new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).ToList();
					glog.Debug("GetContractChildConsentIndByDetailsContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractChildConsentIndByDetailsContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract Sum Of IndicativeValuation By ContractNumber
		public decimal GetPreContractSumOfIndicativeValuationByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractSumOfIndicativeValuationByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<decimal>(
						"exec GetPreContractSumOfIndicativeValuationByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractSumOfIndicativeValuationByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractSumOfIndicativeValuationByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract  Sum Of IndicativeValuation Details List By ContractNumber
		public decimal GetContractSumOfIndicativeValuationByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractSumOfIndicativeValuationByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<decimal>(
						"exec GetContractSumOfIndicativeValuationByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractSumOfIndicativeValuationByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractSumOfIndicativeValuationByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get PreContract Sum Of LTVPercentage By ContractNumber
		public decimal GetPreContractSumOfLTVPercentageByContractNumber(string strContractNumber)
		{
			glog.Debug("GetPreContractSumOfLTVPercentageByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<decimal>(
						"exec GetPreContractSumOfLTVPercentageByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetPreContractSumOfLTVPercentageByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetPreContractSumOfLTVPercentageByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#region Get Contract  Sum Of LTVPercentage Details List By ContractNumber
		public decimal GetContractSumOfLTVPercentageByContractNumber(string strContractNumber)
		{
			glog.Debug("GetContractSumOfLTVPercentageByContractNumber : Entry");
			try
			{
				using (var db = new MainDbContext())
				{
					var result = db.Database.SqlQuery<decimal>(
						"exec GetContractSumOfLTVPercentageByContractNumber @ContractNumber",
						new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(strContractNumber) ? "" : strContractNumber)).FirstOrDefault();
					glog.Debug("GetContractSumOfLTVPercentageByContractNumber: Exit");
					return result;
				}
			}
			catch (Exception Ex)
			{
				glog.Error("GetContractSumOfLTVPercentageByContractNumber Exception: " + Ex.Message);
				throw;
			}

		}
		#endregion

		#endregion

	}
}