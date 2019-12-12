using System;
using System.Collections.Generic;
using System.Data;
using EthozCapital.CustomLibraries;
using EthozCapital.Data;
using EthozCapital.Models;
using EthozCapital.Models.ViewModels;
using log4net;
using Newtonsoft.Json;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

namespace EthozCapital.CustomLibraries
{
	public class clsAsset
	{
		public clsAsset()
		{
		}

		//TableFieldInd -> is the field in a table? If yes, then replace special characters "<" and ">"
		public IEnumerable<CommonDropDown> GetBrand(Boolean TableFieldInd)
		{
			using (var db = new OrixDBEntities())
			{
				var brand = db.cfstb_asset_brand_mas.AsNoTracking().Where(f => f.cfs_brand_sta_ind != "X" && f.cfs_brand_vehicle_ind == "N" && f.cfs_brand_code != "").OrderBy(f => f.cfs_brand_name).ToList();

				var result = brand.Select(f => new CommonDropDown()
				{
					value = f.cfs_brand_code,
					label = f.cfs_brand_name
				}).ToList();

				if (TableFieldInd)
				{
					result = brand.Select(f => new CommonDropDown()
					{
						value = f.cfs_brand_code,
						label = f.cfs_brand_name.Replace("\"", "&#34").Replace("\'", "&#39").Replace("<", "&#12296").Replace(">", "&#12297")
					}).ToList();
				}

				return result;
			}
		}

		//TableFieldInd -> is the field in a table? If yes, then replace special characters "<" and ">"
		public IEnumerable<CommonDropDown> GetModelByBrand(string code, Boolean TableFieldInd)
		{
			using (var db = new OrixDBEntities())
			{
				var model = db.cfstb_asset_model_chd.AsNoTracking().Where(f => f.cfstb_asset_model_mas.cfs_model_sta_ind == "O" && f.cfs_chd_model_sta_ind == "O" && f.cfs_chd_brand_code == code).OrderBy(f => f.cfstb_asset_model_mas.cfs_model_name).ToList();

				var result = model.Select(f => new CommonDropDown()
				{
					value = f.cfstb_asset_model_mas.cfs_model_code,
					label = f.cfstb_asset_model_mas.cfs_model_name
				}).ToList();

				if (TableFieldInd)
				{
					result = model.Select(f => new CommonDropDown()
					{
						value = f.cfstb_asset_model_mas.cfs_model_code,
						label = f.cfstb_asset_model_mas.cfs_model_name.Replace("\"", "&#34").Replace("\'", "&#39").Replace("<", "&#12296").Replace(">", "&#12297")
					}).ToList();
				}
				return result;
			}
		}

		//TableFieldInd -> is the field in a table? If yes, then replace special characters "<" and ">"
		public IEnumerable<CommonDropDown> GetVehicleMake(Boolean TableFieldInd)
		{
			using (var db = new OrixDBEntities())
			{
				var vehMake = db.cfstb_asset_brand_mas.AsNoTracking().Where(f => f.cfs_brand_sta_ind != "X" && f.cfs_brand_vehicle_ind == "Y").OrderBy(f => f.cfs_brand_name).ToList();

				var result = vehMake.Select(f => new CommonDropDown()
				{
					value = f.cfs_brand_code,
					label = f.cfs_brand_name
				});

				if (TableFieldInd)
				{
					result = vehMake.Select(f => new CommonDropDown()
					{
						value = f.cfs_brand_code,
						label = f.cfs_brand_name.Replace("\"", "&#34").Replace("\'", "&#39").Replace("<", "&#12296").Replace(">", "&#12297")
					});
				}
				return result;
			}
		}

		//TableFieldInd -> is the field in a table? If yes, then replace special characters "<" and ">"
		public IEnumerable<CommonDropDown> GetVehicleModelByVehicleMake(string code, Boolean TableFieldInd)
		{
			using (var db = new OrixDBEntities())
			{
				var vehModel = db.cfstb_asset_model_chd.AsNoTracking().Where(f => f.cfstb_asset_model_mas.cfs_model_sta_ind == "O" && f.cfs_chd_model_sta_ind == "O" && f.cfs_chd_brand_code == code).OrderBy(f => f.cfstb_asset_model_mas.cfs_model_name).ToList();

				var result = vehModel.Select(f => new CommonDropDown()
				{
					value = f.cfstb_asset_model_mas.cfs_model_code,
					label = f.cfstb_asset_model_mas.cfs_model_name
				}).ToList();

				if (TableFieldInd)
				{
					result = vehModel.Select(f => new CommonDropDown()
					{
						value = f.cfstb_asset_model_mas.cfs_model_code,
						label = f.cfstb_asset_model_mas.cfs_model_name.Replace("\"", "&#34").Replace("\'", "&#39").Replace("<", "&#12296").Replace(">", "&#12297")
					}).ToList();
				}
				return result;
			}
		}

		public List<Security_ConstructionEquipModel> GetEquipment(string cm_client_cod)
		{
			using (var mdb = new MainDbContext())
			using (var db = new OrixDBEntities())
			{
				try
				{
					var res = (from s in mdb.Security_ConstructionEquip // outer sequence
										 join st in mdb.Security_ConstructionEquipCustomer //inner sequence 
										 on s.ID equals st.MasterID // key selector                          
										 where s.Status == "O" && st.Status == "O"
										 && st.Customer == cm_client_cod
										 select s).ToList();

					var result = (from s in res
												join cabm in db.cfstb_asset_brand_mas
												on s.EquipBrand equals cabm.cfs_brand_code
												join camm in db.cfstb_asset_model_mas
													on s.EquipModel equals camm.cfs_model_code
												where cabm.cfs_brand_sta_ind != "X" && camm.cfs_model_sta_ind != "X"
												orderby s.EquipBrand, s.EquipModel
												select new { s, cabm, camm }).ToList();
					result.ForEach(x =>
					{
						x.s.EquipBrand = x.cabm.cfs_brand_name;
						x.s.EquipModel = x.camm.cfs_model_name;
					});
					var finalres = result.Select(x => new Security_ConstructionEquipModel()
					{
						ChargeDate = x.s.ChargeDate != null ? x.s.ChargeDate.Value.ToShortDateString() : "",
						ChargeNumber = x.s.ChargeNumber,
						EngineNumber = x.s.EngineNumber,
						EquipBrand = x.s.EquipBrand,
						EquipDesc = x.s.EquipDesc,
						EquipModel = x.s.EquipModel,
						ID = x.s.ID,
						SecuredAmount = x.s.SecuredAmount != null ? x.s.SecuredAmount.Value.ToString("0.00") : "0.00",
						SerialNumber = x.s.SerialNumber,
						YearOfManufacture = x.s.YearOfManufacture
					}).ToList();
					return finalres;
				}
				catch (Exception e)
				{
					return new List<Security_ConstructionEquipModel>();
				}
			}
		}

		public List<Security_ConstructionEquipModel> GetIndustrial(string cm_client_cod)
		{
			using (var mdb = new MainDbContext())
			using (var db = new OrixDBEntities())
			{
				try
				{
					var res = (from s in mdb.Security_IndustrialEquip // outer sequence
										 join st in mdb.Security_IndustrialEquipCustomer //inner sequence 
										 on s.ID equals st.MasterID // key selector                          
										 where s.Status == "O" && st.Status == "O"
										 && st.Customer == cm_client_cod
										 select s).ToList();

					var result = (from s in res
												join cabm in db.cfstb_asset_brand_mas
												on s.EquipBrand equals cabm.cfs_brand_code
												join camm in db.cfstb_asset_model_mas
												on s.EquipModel equals camm.cfs_model_code
												where cabm.cfs_brand_sta_ind != "X" && camm.cfs_model_sta_ind != "X"
												orderby s.EquipBrand, s.EquipModel
												select new { s, cabm, camm }).ToList();
					result.ForEach(x =>
					{
						x.s.EquipBrand = x.cabm.cfs_brand_name;
						x.s.EquipModel = x.camm.cfs_model_name;
					});
					var finalres = result.Select(x => new Security_ConstructionEquipModel()
					{
						ChargeDate = x.s.ChargeDate != null ? x.s.ChargeDate.Value.ToShortDateString() : "",
						ChargeNumber = x.s.ChargeNumber,
						EngineNumber = x.s.EngineNumber,
						EquipBrand = x.s.EquipBrand,
						EquipDesc = x.s.EquipDesc,
						EquipModel = x.s.EquipModel,
						ID = x.s.ID,
						SecuredAmount = x.s.SecuredAmount != null ? x.s.SecuredAmount.Value.ToString("0.00") : "0.00",
						SerialNumber = x.s.SerialNumber,
						YearOfManufacture = x.s.YearOfManufacture
					}).ToList();
					return finalres;
				}
				catch (Exception e)
				{
					return new List<Security_ConstructionEquipModel>();
				}
			}
		}
	}
}