using System.Configuration;
using EthozCapital.CustomLibraries;
using EthozCapital.Data;
using EthozCapital.Data.OrixEss;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace EthozCapital.CustomLibraries
{
	public class clsGlobal
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(clsGlobal));

		public static String datetimeFormat = "yyyy-mm-dd";
		public static String MatrixSubCtrTypeCode = System.Configuration.ConfigurationManager.AppSettings["MatrixSubCtrTypeCode"]; //Matrix Group Type - Sub Ctr Type
		public static String MatrixSubProdTypeCode = System.Configuration.ConfigurationManager.AppSettings["MatrixSubProdTypeCode"]; //Matrix Group Type - Sub Prod Type
		public static String UserGroupTypeCode = System.Configuration.ConfigurationManager.AppSettings["UserGroupTypeCode"];
		public static String UserGroupLogin;

		public static string LoginID = string.Empty;
		public static string Password = string.Empty;

		#region sweet alert
		public static string SwalTitle_Success = "Success";
		public static string SwalTitle_Fail = "Fail";
		public static string SwalTitle_Warning = "Warning";
		public static string SwalTitle_Error = "Error";
		public static string SwalTitle_Confirm = "Do you want to proceed?";

		//SwalContent_Error Refer to Error Code 

		public static string SwalType_Success = "success";
		public static string SwalType_Warning = "warning";
		public static string SwalType_Error = "error";
		#endregion

		#region general error type
		public static string Error_InvalidData = "Invalid Data Input!";
		public static string Error206 = "Invalid (Error 206: Please fill required field(s))";
		#endregion

		#region error free
		public static string ErrorFree_Success = "S";
		public static string ErrorFree_Fail = "F";
		#endregion

		#region database name
		public static string Orix_DB = System.Configuration.ConfigurationManager.AppSettings["OrixDatabase"];
		#endregion

		public clsGlobal()
		{
		}

		public static string GetConnection()
		{
			glog.Debug("GetConnection: Entry");
			string result = "";
			try
			{
				//SQL server authentication
				result = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
				//string[] values = result.Split(';');
				//values[2] = "user id='" + clsGlobal.LoginID + "'";
				//values[3] = "password='" + clsGlobal.Password + "'";
				//result = string.Join(";", values);
				try
				{
					using (SqlConnection conn = new SqlConnection(result))
					{
						conn.Open(); // throws if invalid
					}
				}
				catch (Exception ex)
				{
					////Window authentication (ORIX\gimjin.ongli)
					//result = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
					//string[] values2 = result.Split(';');
					//values2[2] = "Integrated Security=SSPI";
					//values2[3] = "";
					//values2[4] = "";
					//result = string.Join(";", values2);
					//result = result.Remove(result.Length - 2);
					//try
					//{
					//    using (SqlConnection conn = new SqlConnection(result))
					//    {
					//        conn.Open(); // throws if invalid
					//    }
					//}
					//catch (Exception ex)
					//{
					glog.Error(ex.Message);
					result = "";
					//}
				}
			}
			catch (Exception ex)
			{
				glog.Error(ex.Message);
			}
			glog.Debug("GetConnection: Exit");
			return result;
		}

		public static string GetConnectionOrixDB()
		{
			glog.Debug("GetConnectionOrixDB: Entry");
			string result = "";
			try
			{
				result = ConfigurationManager.ConnectionStrings["OrixConnection"].ConnectionString;
			}
			catch (Exception ex)
			{
				glog.Error(ex.Message);
			}
			glog.Debug("GetConnectionOrixDB: Exit");
			return result;
		}

		#region Auto Generate System ID
		public static SysAutoGenerateReturn GetSystemID(string Group, string Code, string Year, string Month)
		{
			SysAutoGenerateReturn data = new SysAutoGenerateReturn();
			if (Month != null)
			{
				if ((Convert.ToInt32(Month) <= 9) && Month.Length != 2)
				{
					Month = "0" + Month;
				}
			}
			using (var db = new MainDbContext())
			{
				var Master = db.Sys_AutoGenerateIdMaster.Where(f => f.Group == Group && f.Code == Code && f.Status == "O");
				var Child = db.Sys_AutoGenerateIdChild;
				IQueryable<Sys_AutoGenerateIdChild> qChild = null;
				var YearFilter = Master.FirstOrDefault();// ? Year.Substring(Year.Length - 2) : "00";
				string YearInd = "N";
				string MonthInd = "N";
				string YearCheck = "00";
				string MonthCheck = "00";
				string LastNumber = "0";
				//data.MasterID = YearFilter.ID;
				data.MasterCode = YearFilter.Code;
				string Prefix = null; string PrefixPosition = null; string PrefixSeperator = null;
				if (YearFilter != null)
				{
					//qChild = Child.Where(f => f.MasterID == YearFilter.ID);
					qChild = Child.Where(f => f.MasterCode == YearFilter.Code);
					YearInd = YearFilter.YearInd;
					MonthInd = YearFilter.MonthInd;
					YearCheck = Year.Substring(Year.Length - 2);
				}

				#region CheckYY
				if (YearInd == "N" && MonthInd == "N")
				{
					var newChild = qChild.FirstOrDefault();
					if (newChild != null)
					{
						data.ID = newChild.ID;
						YearCheck = newChild.YY;
						MonthCheck = newChild.MM;
						LastNumber = newChild.LastNumber;
					}
					else
					{
						LastNumber = "0";
					}
				}
				else if (YearInd == "Y" && MonthInd == "N")
				{
					var newChild = qChild.FirstOrDefault(f => f.YY == YearCheck);
					if (newChild != null)
					{
						data.ID = newChild.ID;
						YearCheck = newChild.YY;
						LastNumber = newChild.LastNumber;
					}
					else
					{
						//YearCheck = Year;
						LastNumber = "0";
					}
				}
				else if (YearInd == "Y" && MonthInd == "Y")
				{
					var newChild = qChild.FirstOrDefault(f => f.YY == YearCheck && f.MM == Month);
					if (newChild != null)
					{
						data.ID = newChild.ID;
						YearCheck = newChild.YY;
						MonthCheck = newChild.MM;
						LastNumber = newChild.LastNumber;
					}
					else
					{
						MonthCheck = Month;
						LastNumber = "0";
					}
				}

				#endregion

				#region CheckMM
				if (MonthInd == "Y")
				{
					var newChild = qChild.FirstOrDefault(f => f.MM == Month);
					if (newChild != null)
					{
						data.ID = newChild.ID;
						MonthCheck = newChild.MM;
						LastNumber = newChild.LastNumber;
					}
					else
					{
						MonthCheck = Month;
						LastNumber = "0";
					}
				}
				else if (YearInd == "N" && MonthInd == "Y")
				{
					var newChild = qChild.FirstOrDefault(f => f.MM == Month);
					if (newChild != null)
					{
						data.ID = newChild.ID;
						MonthCheck = newChild.MM;
						LastNumber = newChild.LastNumber;
					}
					else
					{
						MonthCheck = Month;
						LastNumber = "0";
					}
				}
				else
				{
					var newChild = qChild.FirstOrDefault();
					if (newChild != null)
					{
						data.ID = newChild.ID;
						MonthCheck = newChild.MM;
						LastNumber = newChild.LastNumber;
					}
				}
				#endregion

				#region CheckPrefix
				Prefix = YearFilter.Prefix;
				#endregion

				#region CheckPrefixPosition
				PrefixPosition = YearFilter.PrefixPosition;
				#endregion

				#region PrefixSeperator
				PrefixSeperator = YearFilter.PrefixSeparator != null ? (PrefixPosition == "L" ? (Prefix + YearFilter.PrefixSeparator) : YearFilter.PrefixSeparator + Prefix) : Prefix;
				#endregion

				string Number = null;
				string YearIndNumber = null;

				#region YearIndCheck
				if (YearInd == "Y")
				{
					string YearSeperator = YearFilter.YearSeparator != null ? YearFilter.YearSeparator : "";
					YearIndNumber = YearCheck + YearSeperator;
				}
				#endregion
				string MonthSeperator = YearFilter.MonthSeparator != null ? YearFilter.MonthSeparator : "";
				#region CheckMonthInd
				string MonthNumber = MonthCheck == "00" ? "" : MonthCheck;
				if (YearInd == "N" && MonthInd == "Y")
				{
					Number = MonthNumber + MonthSeperator;
				}
				else if (YearInd == "Y" && MonthInd == "Y")
				{
					Number = YearIndNumber + MonthNumber + MonthSeperator;
				}
				else if (YearInd == "Y" && MonthInd == "N")
				{
					Number = YearIndNumber + MonthNumber;
				}
				#endregion

				#region FinalNumber
				int Width = YearFilter.Width ?? 0;
				LastNumber = (Convert.ToInt32(LastNumber) + 1).ToString();
				int TotalPadding = Width - ((Number == null ? 0 : Number.Length) + LastNumber.Length + PrefixSeperator.Length);
				string Final = PrefixPosition == "L" ? (PrefixSeperator + Number + FormattedString(LastNumber, TotalPadding))
				: (Number + FormattedString(LastNumber, TotalPadding) + PrefixSeperator);
				data.LastNumber = LastNumber;
				data.YearCheck = YearCheck;
				data.MonthCheck = MonthCheck;
				data.NewId = Final;
				return data;
				#endregion
			}
		}

		public static bool UpdateSystemIDLastNum(SysAutoGenerateReturn data, string userName, MainDbContext db)
		{
			try
			{
				var Child = db.Sys_AutoGenerateIdChild;
				//var YearFilter = db.Sys_AutoGenerateIdMaster.FirstOrDefault(f => f.ID == data.MasterID);// ? Year.Substring(Year.Length - 2) : "00";
				var YearFilter = db.Sys_AutoGenerateIdMaster.FirstOrDefault(f => f.Code == data.MasterCode);// ? Year.Substring(Year.Length - 2) : "00";
				string YearInd = "N";
				string MonthInd = "N";
				string YearCheck = data.YearCheck;
				string MonthCheck = data.MonthCheck;
				string LastNumber = data.LastNumber;
				//data.MasterID = YearFilter.ID;
				data.MasterCode = YearFilter.Code;
				if (YearFilter != null)
				{
					YearInd = YearFilter.YearInd;
					MonthInd = YearFilter.MonthInd;
				}
				var newChild = Child.FirstOrDefault(f => f.ID == data.ID);
				if (YearInd == "N" && MonthInd == "N")
				{
					if (newChild != null)
					{
						newChild.LastNumber = data.LastNumber;
						newChild.UpdatedBy = userName;
						newChild.UpdatedDate = DateTime.UtcNow;
						db.Entry(newChild).State = System.Data.Entity.EntityState.Modified;

					}
					else
					{
						db.Sys_AutoGenerateIdChild.Add(new Sys_AutoGenerateIdChild()
						{
							//MasterID = data.MasterID,
							MasterCode = data.MasterCode,
							LastNumber = data.LastNumber,
							Status = "O",
							MM = "00",
							YY = "00",
							CreatedBy = userName,
							CreatedDate = DateTime.UtcNow
						});
					}
				}
				else if (YearInd == "Y" && MonthInd == "N")
				{
					if (newChild != null)
					{
						newChild.LastNumber = data.LastNumber;
						newChild.UpdatedBy = userName;
						newChild.UpdatedDate = DateTime.UtcNow;
						db.Entry(newChild).State = System.Data.Entity.EntityState.Modified;
						db.SaveChanges();
					}
					else
					{
						db.Sys_AutoGenerateIdChild.Add(new Sys_AutoGenerateIdChild()
						{
							//MasterID = data.MasterID,
							MasterCode = data.MasterCode,
							LastNumber = data.LastNumber,
							Status = "O",
							MM = "00",
							YY = data.YearCheck,
							CreatedBy = userName,
							CreatedDate = DateTime.UtcNow
						});
					}
				}
				else if (YearInd == "Y" && MonthInd == "Y")
				{
					if (newChild != null)
					{
						newChild.LastNumber = data.LastNumber;
						newChild.UpdatedBy = userName;
						newChild.UpdatedDate = DateTime.UtcNow;
						db.Entry(newChild).State = System.Data.Entity.EntityState.Modified;
						db.SaveChanges();
					}
					else
					{
						db.Sys_AutoGenerateIdChild.Add(new Sys_AutoGenerateIdChild()
						{
							//MasterID = data.MasterID,
							MasterCode = data.MasterCode,
							LastNumber = data.LastNumber,
							Status = "O",
							MM = data.MonthCheck,
							YY = data.YearCheck,
							CreatedBy = userName,
							CreatedDate = DateTime.UtcNow
						});
					}
				}
				db.SaveChanges();

				return true;
			}
			catch (Exception ex)
			{
				glog.Error(ex.Message);
				return false;
			}
		}

		//Auto generate Pre-Contract/Contract number
		//ContractStatus = P (Pre), O (Live)
		public SysAutoGenerateReturn GetNewContractNumber(string strContractStatus, string strSubContractType, string strSubProductType, string strLEFSInterestCode)
		{
			SysAutoGenerateReturn data = new SysAutoGenerateReturn();
			using (var db = new MainDbContext())
			{
				string strGroup = "";
				string strCode = "";

				if (strContractStatus.Trim().ToUpper() == "P")
					strGroup = "PreContract";
				else
					strGroup = "Contract";

				strCode = FnGetContractNumberCode(strContractStatus, strSubContractType, strSubProductType, strLEFSInterestCode);
				data = GetSystemID(strGroup, strCode, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
			}
			return data;
		}

		public string FnGetContractNumberCode(string strContractStatus, string strSubContractType, string strSubProductType, string strLEFSInterestCode)
		{
			string strCode = "";
			string strLEFSInterestType = "";
			using (var db = new MainDbContext())
			{
				strLEFSInterestType = db.Maintenance_LEFSInterestCode.Where(f => f.InterestCode == strLEFSInterestType
					&& f.Status == "O").Select(f => f.InterestType).FirstOrDefault();

				var Mapping = db.Sys_ContractNumberMapping.Where(f => f.SubContractTypeCode == strSubContractType
					&& f.ContractStatus == strContractStatus && f.Status == "O");
				var Filter = Mapping.FirstOrDefault();
				if (Filter != null)
				{
					strCode = Filter.AutoGenerateIdMasterCode;

					//If SubProductType is matched
					//For Secured Term Loan with product type = Secured Loan - Vessel
					if ((!string.IsNullOrEmpty(Filter.Var1_SubProductType)) &&
						(!string.IsNullOrEmpty(strSubProductType)))
					{
						if (Filter.Var1_SubProductType == strSubProductType)
							strCode = Filter.Var1_AutoGenerateIdMasterCode;
					}

					//If LEFS Interest Type is variable
					if ((!string.IsNullOrEmpty(Filter.Var2_LEFSRateType)) &&
						(!string.IsNullOrEmpty(strLEFSInterestType)))
					{
						if (Filter.Var2_LEFSRateType == strLEFSInterestType)
							strCode = Filter.Var2_AutoGenerateIdMasterCode;
					}
				}
			}

			return strCode;
		}

		private static string FormattedString(string s, int count)
		{
			string append = "";
			for (int i = 0; i < count; i++)
			{
				append += "0";
			}
			return append + s;
		}
		#endregion

		#region Get System Setup Data

		//Get menu by user group
		public DataTable GetMenu(string UserGroupCode, string VirtualDirectory)
		{
			DataTable dt = new DataTable();
			try
			{
				using (SqlConnection cnn = new SqlConnection(GetConnection()))
				{
					using (SqlCommand cmd = new SqlCommand("MMenuFunction_SelectHTMLCode_ECA", cnn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						cmd.Parameters.AddWithValue("@UserGroupCode", UserGroupCode.ToString());
						cmd.Parameters.AddWithValue("@VirtualDirectory", System.Configuration.ConfigurationManager.AppSettings["VirtualDirectory"]);

						SqlDataAdapter ds = new SqlDataAdapter(cmd);
						ds.Fill(dt);
					}
				}
			}
			catch (Exception ex)
			{
				glog.Error(ex.Message);
			}
			return dt;
		}

		//Get default value from parameter table
		public string GetDefaultValue(string code, DateTime EffDate)
		{
			string ParaValue = "";
			try
			{
				using (var db = new MainDbContext())
				{
					string ParaCode = db.Sys_Parameters.Where(f => f.ParameterCode == code && f.Status == "O")
										.Select(f => f.ParameterCode).FirstOrDefault();

					if (!string.IsNullOrEmpty(ParaCode))
					{
						ParaValue = db.Sys_ParameterValue.AsNoTracking().Where(f => f.MasterParameterCode == ParaCode
										   && f.Status == "O" && f.EffectiveDate <= EffDate)
										   .OrderByDescending(f => f.EffectiveDate).FirstOrDefault().Value;
					}
				}
			}
			catch (Exception ex)
			{
				glog.Error(ex.Message);
			}

			return ParaValue;
		}

		//Get default value from matrix parameter table by sub ctr type / sub prod type / user group
		public string GetDefaultValueMatrix(string code, string groupCode, string groupType, DateTime EffDate)
		{
			string ParaValue = "";
			using (var db = new MainDbContext())
			{
				string ParaCode = db.Sys_TypeMatrixParameterMaster.Where(f => f.ParameterCode == code && f.Status == "O")
									.Select(f => f.ParameterCode).FirstOrDefault();

				if (!string.IsNullOrEmpty(ParaCode))
				{
					ParaValue = db.Sys_TypeMatrixParameterValue.AsNoTracking().Where(f => f.MasterParameterCode == ParaCode
									   && f.GroupCode == groupCode && f.MatrixGroupTypeCode == groupType
									   && f.Status == "O" && f.EffectiveDate <= EffDate)
									   .OrderByDescending(f => f.EffectiveDate).FirstOrDefault().Value;
				}
			}
			return ParaValue;
		}

		//Get default field visibility = true/false by sub ctr type / sub prod type / user group
		public bool GetFieldVisible(string code, string groupCode, string groupType, DateTime EffDate)
		{
			using (var db = new MainDbContext())
			{
				string PageFieldCode = db.Sys_TypeMatrixFieldPropertiesMaster.Where(f => f.PageFieldCode == code && f.Status == "O")
									.Select(f => f.PageFieldCode).FirstOrDefault();

				var data = db.Sys_TypeMatrixFieldProperties.AsNoTracking().Where(f => f.MasterPageFieldCode == PageFieldCode
								   && f.GroupCode == groupCode && f.MatrixGroupTypeCode == groupType
								   && f.Status == "O" && f.EffectiveDate <= EffDate)
								   .OrderByDescending(f => f.EffectiveDate).FirstOrDefault();

				return data == null ? false : (data.FP1Visible ?? false);
			}
		}

		//Get default field enabled = true/false by sub ctr type / sub prod type / user group
		public bool GetFieldEnabled(string code, string groupCode, string groupType, DateTime EffDate)
		{
			using (var db = new MainDbContext())
			{
				string PageFieldCode = db.Sys_TypeMatrixFieldPropertiesMaster.Where(f => f.PageFieldCode == code && f.Status == "O")
									.Select(f => f.PageFieldCode).FirstOrDefault();

				var data = db.Sys_TypeMatrixFieldProperties.AsNoTracking().Where(f => f.MasterPageFieldCode == PageFieldCode
								   && f.GroupCode == groupCode && f.MatrixGroupTypeCode == groupType
								   && f.Status == "O" && f.EffectiveDate <= EffDate)
								   .OrderByDescending(f => f.EffectiveDate).FirstOrDefault();

				return data == null ? false : (data.FP2Enabled ?? false);
			}
		}

		//Get default field mandatory = true/false by sub ctr type / sub prod type / user group
		public bool GetFieldMandatory(string code, string groupCode, string groupType, DateTime EffDate)
		{
			using (var db = new MainDbContext())
			{
				string PageFieldCode = db.Sys_TypeMatrixFieldPropertiesMaster.Where(f => f.PageFieldCode == code && f.Status == "O")
									.Select(f => f.PageFieldCode).FirstOrDefault();

				var data = db.Sys_TypeMatrixFieldProperties.AsNoTracking().Where(f => f.MasterPageFieldCode == PageFieldCode
								   && f.GroupCode == groupCode && f.MatrixGroupTypeCode == groupType
								   && f.Status == "O" && f.EffectiveDate <= EffDate)
								   .OrderByDescending(f => f.EffectiveDate).FirstOrDefault();

				return data == null ? false : (data.FP4Mandatory ?? false);
			}
		}

		//Get checkbox default value = true/false by sub ctr type / sub prod type / user group
		public bool GetFieldDefaultCheck(string code, string groupCode, string groupType, DateTime EffDate)
		{
			using (var db = new MainDbContext())
			{
				string PageFieldCode = db.Sys_TypeMatrixFieldPropertiesMaster.Where(f => f.PageFieldCode == code && f.Status == "O")
									.Select(f => f.PageFieldCode).FirstOrDefault();

				var data = db.Sys_TypeMatrixFieldProperties.AsNoTracking().Where(f => f.MasterPageFieldCode == PageFieldCode
								   && f.GroupCode == groupCode && f.MatrixGroupTypeCode == groupType
								   && f.Status == "O" && f.EffectiveDate <= EffDate)
								   .OrderByDescending(f => f.EffectiveDate).FirstOrDefault();

				return data == null ? false : (data.FP3DefaultCheck ?? false);
			}
		}

		//Get logic code by sub ctr type / sub prod type / user group
		public string GetLogicCode(string code, string groupCode, string groupType, DateTime EffDate)
		{
			using (var db = new MainDbContext())
			{
				string FunctionLogicCode = "";
				try
				{
					string FunctionCode = db.Sys_TypeMatrixFunctionMaster.Where(f => f.FunctionCode == code && f.Status == "O")
									.Select(f => f.FunctionCode).FirstOrDefault();

					string LogicCode = db.Sys_TypeMatrixFunction.Where(f => f.MasterFunctionCode == FunctionCode && f.MatrixGroupTypeCode == groupType &&
						f.GroupCode == groupCode && f.Status == "O" &&
						f.EffectiveDate <= EffDate).OrderByDescending(f => f.LogicCode).FirstOrDefault().LogicCode;

					FunctionLogicCode = db.Sys_TypeMatrixFunctionLogicMaster.Where(f => f.LogicCode == LogicCode && f.Status == "O")
									.Select(f => f.LogicCode).FirstOrDefault();
				}
				catch (Exception ex)
				{
					FunctionLogicCode = "";
					glog.Error(ex.Message);
				}
				return FunctionLogicCode;
			}
		}

		//Get list of value for drop down
		public List<SelectListItem> GetListOfValue(string groupType, string parentId, string status, string selectedText, string selectedValue)
		{
			List<SelectListItem> listOfValue = new List<SelectListItem>();
			try
			{
				using (var db = new MainDbContext())
				{
					//GroupType
					if (String.IsNullOrEmpty(parentId) && String.IsNullOrEmpty(status))
					{
						listOfValue = db.Sys_ListOfValue.Where(u => u.GroupType.ToString().Trim() == groupType)
																.OrderBy(o => o.GroupMemberDesc)
																.Select(x =>
																new SelectListItem()
																{
																	Text = x.GroupMemberDesc.ToString(),
																	Value = x.GroupCode.ToString()
																}).ToList();
					}
					//GroupType + Status
					else if (String.IsNullOrEmpty(parentId))
					{
						listOfValue = db.Sys_ListOfValue.Where(u => u.GroupType.ToString().Trim() == groupType && u.Status.ToString().Trim() == status)
										.OrderBy(o => o.GroupMemberDesc)
										.Select(x =>
										new SelectListItem()
										{
											Text = x.GroupMemberDesc.ToString(),
											Value = x.GroupCode.ToString()
										}).ToList();
					}
					//GroupType + ParentId
					else if (String.IsNullOrEmpty(status))
					{
						listOfValue = db.Sys_ListOfValue.Where(u => u.GroupType.ToString().Trim() == groupType && u.ParentID.ToString().Trim() == parentId)
										.OrderBy(o => o.GroupMemberDesc)
										.Select(x =>
										new SelectListItem()
										{
											Text = x.GroupMemberDesc.ToString(),
											Value = x.GroupCode.ToString()
										}).ToList();
					}
					//GroupType + ParentId + Status
					else
					{
						listOfValue = db.Sys_ListOfValue.Where(u => u.GroupType.ToString().Trim() == groupType && u.Status.ToString().Trim() == status && u.ParentID.ToString().Trim() == parentId)
											.OrderBy(o => o.GroupMemberDesc)
											.Select(x =>
											new SelectListItem()
											{
												Text = x.GroupMemberDesc.ToString(),
												Value = x.GroupCode.ToString()
											}).ToList();
					}
					if (groupType == "FINANCIAL_SERVICES_SALES_DEPARTMENT")
					{
						var orixDb = new ORIX_ESS_DB_DevEntities();
						var groupCode = listOfValue.Select(x => x.Value).ToList();
						var dateToday = DateTime.Now;
						var list = orixDb.Set<ss_emp_mas>().Where(x => groupCode.Contains(x.em_dept_cod) && x.em_sta_ind == "O" && (x.em_rsg_dat == null || x.em_rsg_dat > dateToday))
										  .OrderBy(x => x.em_sht_nam).ToList();
						listOfValue = list.Select(x => new SelectListItem()
						{
							Text = x.em_sht_nam,
							Value = x.em_emp_cod
						}).ToList();
					}
					//Looping to set selected item
					for (int i = 0; i < listOfValue.Count; i++)
					{
						if (listOfValue[i].Text.ToString().Trim() == selectedText)
							listOfValue[i].Selected = true;

						if (listOfValue[i].Value.ToString().Trim() == selectedValue)
							listOfValue[i].Selected = true;
					}
				}
			}
			catch (Exception ex)
			{
				glog.Error(ex.Message);
			}
			return listOfValue;
		}
		#endregion

		//Get list of group type distinct
		public List<SelectListItem> GetAllGroupType()
		{
			List<SelectListItem> listOfGroupType = new List<SelectListItem>();

			using (var db = new MainDbContext())
			{
				//existing group type
				listOfGroupType = db.Sys_ListOfValue.Select(x => new SelectListItem()
				{
					Text = x.GroupType.ToString(),
					Value = x.GroupType.ToString()
				}).Distinct().ToList();
			}
			return listOfGroupType;
		}

		public IEnumerable<CommonDropDown> GetParentGroupType(Boolean TableFieldInd)
		{
			using (var db = new MainDbContext())
			{
				var list = db.Sys_ListOfValue.Select(x => new CommonDropDown()
				{
					value = x.GroupType.ToString(),
					label = x.GroupType.ToString()
				}).Distinct().ToList();
				return list;
			}

		}

		public IEnumerable<CommonDropDown> GetDescByGroupType(String code)
		{
			using (var db = new MainDbContext())
			{
				var desc = db.Sys_ListOfValue.Where(f => f.GroupType == code).ToList();

				var result = desc.Select(x => new CommonDropDown()
				{
					label = x.GroupMemberDesc.ToString()
				}).ToList();

				return result;
			}
		}



		//public bool CheckGroupType(string groupType, string groupCode) 
		//{
		//    using (var db = new MainDbContext())
		//    {
		//        //check group type and group code already exist or not

		//        db.Sys_ListOfValue.Where(f => f.GroupType != groupType && f.GroupCode != groupCode);


		//        return true;
		//    }

		//}

		#region Check Access by User Group
		public bool CheckUserGroup(string UserGroupLogin, int SubMenuId)
		{
			using (var db = new MainDbContext())
			{
				string GroupCode = "";
				try
				{
					GroupCode = db.Sys_UserGroupMenuAccess.Where(f => f.GroupCode == UserGroupLogin && f.SubMenuId == SubMenuId &&
					f.Status == "O").OrderByDescending(f => f.SubMenuId).FirstOrDefault().GroupCode;
					return true;
				}
				catch (Exception ex)
				{
					GroupCode = "";
					glog.Error(ex.Message);
					return false;
				}
			}
		}
		#endregion

		#region FieldChangeHistory
		public int InsertFieldChangeHistory(MainDbContext db,
			string TableName, string FieldName, string FieldValue, string PKFieldName1, string PKFieldValue1,
			string Status, string UpdatedBy, DateTime UpdatedDate)
		{
			int changedId = 0;
			Sys_FieldChangeHistory changeHistory = new Sys_FieldChangeHistory();
			changeHistory.TableName = TableName;
			changeHistory.FieldName = FieldName;
			changeHistory.FieldValue = FieldValue;
			changeHistory.PKFieldName1 = PKFieldName1;
			changeHistory.PKFieldValue1 = PKFieldValue1;
			changeHistory.Status = Status;
			changeHistory.UpdatedBy = UpdatedBy;
			changeHistory.UpdatedDate = UpdatedDate;

			db.Sys_FieldChangeHistory.Add(changeHistory);
			db.Entry(changeHistory).State = System.Data.Entity.EntityState.Added;
			changedId = db.SaveChanges();
			return changedId;
		}

		public IEnumerable<Sys_FieldChangeHistory> GetFieldChangeHistory(MainDbContext db, string TableName, string PKFieldName1, string PKFieldValue1, string PKFieldName2 = null, string PKFieldValue2 = null)
		{
			return db.Sys_FieldChangeHistory.ToList().OrderBy(o => o.FieldName).ThenByDescending(o => o.UpdatedDate).Where(w => w.TableName == TableName && w.PKFieldName1 == "ID" && w.PKFieldValue1 == PKFieldValue1 && w.Status == "O");
		}
		#endregion

		#region General Drop Down Value
		public List<SelectListItem> GetCountry()
		{
			glog.Debug("GetCountry: Entry");
			using (var db = new OrixDBEntities())
			{
				try
				{
					var country = db.gentb_country_mas.Where(x => x.cm_sta_ind == "O").Select(p => new SelectListItem()
					{
						Value = p.cm_country_cod,
						Text = p.cm_country_nam
					}).ToList();
					glog.Debug("GetCountry: Exit");
					return country;
				}
				catch (Exception ex)
				{
					glog.Error("GetCountry Exception: " + ex.Message);
					return new List<SelectListItem>();
				}
			}
		}
		#endregion

		#region Send Email
		public bool FnEmailNotification(string mailType, string recipient, string cclist, string subject, string body, string sender)
		{
			bool blError = false;

			return blError;//return true = has error
		}
		#endregion

		#region CheckRecordForEditing
		public ResultViewModel CheckRecordForEditing(string module, string refNumber, string userName)
		{
			var result = new ResultViewModel();
			glog.Debug("CheckRecordForEditing: Entry");
			using (var db = new MainDbContext())
			{
				try
				{
					var userMail = db.Sys_Users.FirstOrDefault(x => x.Name == userName).Email;
					var record = db.sys_ProcessLock.Where(w => w.Module == module && w.RefNo == refNumber && w.CreatedBy != userMail).FirstOrDefault();
					if (record != null)
					{
						result.Status = 1;
						result.Message = "Record is being used by " + record.CreatedBy + ". Editing is not allowed!";
					}
				}
				catch (Exception ex)
				{
					glog.Error("LockRecord Exception: " + ex.Message + ex.InnerException);
				}
			}
			glog.Debug("CheckRecordForEditing: Entry");
			return result;
		}
		#endregion

		#region LockRecord
		public ResultViewModel LockRecord(string module, string refNumber, string userName)
		{
			int changedId = 0;
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						glog.Debug("LockRecord: Entry");
						var userMail = db.Sys_Users.FirstOrDefault(x => x.Name == userName).Email;
						var Locked = db.sys_ProcessLock.Any(w => w.Module == module && w.RefNo == refNumber);
						if (!Locked)
						{
							sys_ProcessLock processLock = new sys_ProcessLock();
							processLock.RefNo = refNumber;
							processLock.Module = module;
							processLock.CreatedBy = userMail;
							processLock.CreatedDate = System.DateTime.UtcNow;
							db.sys_ProcessLock.Add(processLock);
							db.Entry(processLock).State = System.Data.Entity.EntityState.Added;
							changedId = db.SaveChanges();
						}
						if (changedId > 0)
						{
							result.Status = 1;
							result.Message = "Record Locked successfully";
						}
						return result;
					}
					catch (Exception ex)
					{
						glog.Error("LockRecord Exception: " + ex.Message + ex.InnerException);
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();

					}
					glog.Debug("LockRecord: Exit");
					return result;
				}

			}

		}
		#endregion

		#region RemoveLockRecord
		public ResultViewModel RemoveLockRecord(string module, string refNumber, string userName)
		{
			int changedId = 0;
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						glog.Debug("RemoveLockRecord: Entry");
						var userMail = db.Sys_Users.FirstOrDefault(x => x.Name == userName).Email;
						var Locked = db.sys_ProcessLock.Any(w => w.Module == module && w.RefNo == refNumber && w.CreatedBy == userMail);
						if (Locked)
						{
							sys_ProcessLock processLock = new sys_ProcessLock();
							processLock.RefNo = refNumber;
							processLock.Module = module;
							processLock.CreatedBy = userMail;
							db.sys_ProcessLock.Add(processLock);
							db.Entry(processLock).State = System.Data.Entity.EntityState.Deleted;
							changedId = db.SaveChanges();
						}
						if (changedId > 0)
						{
							result.Status = 1;
							result.Message = "Record deleted successfully";
						}
					}
					catch (Exception ex)
					{
						glog.Error("RemoveLockRecord Exception: " + ex.Message + ex.InnerException);
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();

					}
					glog.Debug("RemoveLockRecord: Exit");

				}

			}
			return result;

		}
		#endregion

		#region Get Employee Details By Employee Code
		public ss_emp_mas GetEmployeeDetails(string strEmployeeCode)
		{
			glog.Debug("GetEmployeeDetails: Entry");
			using (var db = new ORIX_ESS_DB_DevEntities())
			{
				try
				{
					var dateToday = DateTime.Now;
					var list = db.Set<ss_emp_mas>().Where(x => x.em_emp_cod == strEmployeeCode && x.em_sta_ind == "O" && (x.em_rsg_dat == null || x.em_rsg_dat > dateToday)).OrderBy(x => x.em_sht_nam).FirstOrDefault();
					glog.Debug("GetEmployeeDetails: Exit");
					return list;
				}
				catch (Exception ex)
				{
					glog.Error("GetEmployeeDetails Exception: " + ex.Message);
					throw;
				}
			}
		}
		#endregion

		#region Get Designation By Employee Design Code
		public string GetDesignationByEmployeeDesignCode(string strEmployeeDesignCode)
		{
			glog.Debug("GetDesignationByEmployeeDesignCode: Entry");
			using (var db = new ORIX_ESS_DB_DevEntities())
			{
				try
				{
					var result = db.ss_para_meter.Where(x => x.pm_par_cod == strEmployeeDesignCode).Select(x =>
					  x.pm_par_desc).FirstOrDefault();

					glog.Debug("GetDesignationByEmployeeDesignCode: Exit");
					return result;
				}
				catch (Exception ex)
				{
					glog.Error("GetDesignationByEmployeeDesignCode Exception: " + ex.Message);
					throw;
				}
			}
		}
		#endregion

		#region Get Fax Number By Employee Department Code
		public string GetFaxNumberByEmployeeDepartmentCode(string strEmployeeDepartmentCode)
		{
			glog.Debug("GetFaxNumberByEmployeeDepartmentCode: Entry");
			using (var db = new ORIX_ESS_DB_DevEntities())
			{
				try
				{
					var result = db.ss_dept_det.Where(x => x.dd_dept_cod == strEmployeeDepartmentCode).Select(x =>
					  x.dd_fax_num).FirstOrDefault();

					glog.Debug("GetFaxNumberByEmployeeDepartmentCode: Exit");
					return result;
				}
				catch (Exception ex)
				{
					glog.Error("GetFaxNumberByEmployeeDepartmentCode Exception: " + ex.Message);
					throw;
				}
			}
		}
		#endregion

		public List<ss_emp_mas> GetEmployeeList()
		{
			List<ss_emp_mas> data = new List<ss_emp_mas>();
			using (var db = new ORIX_ESS_DB_DevEntities())
			{
				data = db.ss_emp_mas.Where(w => w.em_sta_ind == "O" && (w.em_rsg_dat == null || w.em_rsg_dat >= DateTime.Now)).ToList();
			}
			return data;
		}

		public List<Sys_Approval> GetApprovingModule()
		{
			List<Sys_Approval> data = new List<Sys_Approval>();
			using (var db = new MainDbContext())
			{
				data = db.Sys_Approval.Where(w => w.EffectiveTo > DateTime.Now && w.EffectiveFrom < DateTime.Now).ToList();
			}
			return data;
		}
	}
}