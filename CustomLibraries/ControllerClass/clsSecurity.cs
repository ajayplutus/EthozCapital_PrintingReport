using EthozCapital.CustomLibraries;
using EthozCapital.Data;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace EthozCapital.CustomLibraries
{
	public class clsSecurity
	{
		private static ILog glog = log4net.LogManager.GetLogger(typeof(clsSecurity));
		private clsGlobal _clsGlobal;
		private clsCRM _clsCRM;

		public clsSecurity()
		{
			_clsGlobal = new clsGlobal();
			_clsCRM = new clsCRM();

		}

		public ResultViewModel InsertPropertyDetails(PropertyModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertPropertyDetails: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var propertyAddress = db.Security_Property.Any(x => x.Status == "O" && x.PropertyAddress == model.PropertyAddress && x.ID != model.Id);
						if (propertyAddress)
						{
							result.Status = 2;
							result.Message = "Property address is already exist!";
							return result;
						}
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "MPP", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_Property property = new Security_Property();
						property.ID = String.IsNullOrEmpty(model.Id) ? NewId.NewId : model.Id;
						property.PropertyAddress = model.PropertyAddress;
						property.PropertyTypeLevel1 = model.PropertyDetails.PropertyTypeLevel1;
						property.PropertyTypeLevel2 = model.PropertyDetails.PropertyTypeLevel2;
						property.FirstThirdParty = model.PropertyDetails.PartyType == "First Party" ? "F" : "T";
						property.FormalValuation = model.PropertyDetails.FormalValuation;
						property.CreditLimit = model.PropertyDetails.CreditLimit;
						property.IndicativeValuation = model.PropertyDetails.IndicativeValuation;
						property.TitleNumber = model.PropertyDetails.TitleNumber;
						property.MortgageNumber = model.PropertyDetails.MortgagorNumber;
						property.ChargeNumber = model.PropertyDetails.ChangeNumber;
						property.ChargeDate = !string.IsNullOrEmpty(model.PropertyDetails.ChargeDate) ? DateTime.ParseExact(model.PropertyDetails.ChargeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
						property.Status = String.IsNullOrEmpty(model.Id) ? "O" : model.Status;
						property.CreatedBy = String.IsNullOrEmpty(model.Id) ? userMail : model.CreatedBy;
						property.CreatedDate = String.IsNullOrEmpty(model.Id) ? DateTime.Now : DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
						property.UpdatedBy = String.IsNullOrEmpty(model.Id) ? null : userMail;
						property.UpdatedDate = String.IsNullOrEmpty(model.Id) ? (DateTime?)null : DateTime.Now;
						property.SecurityListLevel2 = clsVariables.ConstMortgageProperty; //"SLL2-1001"
						db.Security_Property.Add(property);

						if (String.IsNullOrEmpty(model.Id))
						{
							db.Entry(property).State = EntityState.Added;
						}
						else
						{
							db.Entry(property).State = EntityState.Modified;
						}
						var id = db.SaveChanges();

						#region Mortgagor
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_PropertyMortgagor.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.Mortgagor)
							{
								masterIdCount++;
								Security_PropertyMortgagor propertyMortgagor = new Security_PropertyMortgagor()
								{
									MasterID = property.ID,
									ItemNumber = masterIdCount,
									MainMortgagor = data.MainType == "Main" ? "Y" : "N",
									MortgagorType = data.IndividualCorporate == clsVariables.Corporate ? "C" : "I",
									Mortgagor = data.Mortgagor,
									MortgagorAddress = data.Address,
									MortgagorDept = data.Department,
									MortgagorConPerson = data.ContactPerson,
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_PropertyMortgagor.Add(propertyMortgagor);
								if (String.IsNullOrEmpty(model.Id))
								{
									db.Entry(propertyMortgagor).State = EntityState.Added;
								}
								else
								{
									db.Entry(propertyMortgagor).State = EntityState.Modified;
								}
								var masterId = db.SaveChanges();
							}
						}
						else
						{
							var ExistMortgagor = GetSecurityPropertyMortgagor(model.Id);
							var masterIdCountMor = db.Security_PropertyMortgagor.Where(x => x.MasterID == model.Id).Count();

							Security_PropertyMortgagor propertyMortgagor = new Security_PropertyMortgagor();
							var deletedMortgagor = ExistMortgagor.Where(p => !model.Mortgagor.Any(p2 => p2.Mortgagor == p.Mortgagor && p2.ItemNumber != 0)).ToList();
							var UpdateMortgagor = model.Mortgagor.Where(p => ExistMortgagor.Any(p2 => p2.Mortgagor == p.Mortgagor && p2.ItemNumber != 0)).ToList();
							var newMortgagor = model.Mortgagor.Where(p => !ExistMortgagor.Any(p2 => p2.Mortgagor == p.Mortgagor && p2.ItemNumber != 0)).ToList();
							if (deletedMortgagor.Count > 0)
							{
								foreach (var data in deletedMortgagor)
								{
									propertyMortgagor = new Security_PropertyMortgagor()
									{
										MasterID = property.ID,
										ItemNumber = data.ItemNumber,
										MainMortgagor = data.MainMortgagor,
										MortgagorType = data.MortgagorType,
										Mortgagor = data.Mortgagor,
										MortgagorAddress = data.MortgagorAddress,
										MortgagorDept = data.MortgagorDept,
										MortgagorConPerson = data.MortgagorConPerson,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_PropertyMortgagor.Add(propertyMortgagor);
									db.Entry(propertyMortgagor).State = EntityState.Modified;
								}

							}
							if (UpdateMortgagor.Count > 0)
							{
								foreach (var data in UpdateMortgagor)
								{
									propertyMortgagor = new Security_PropertyMortgagor()
									{
										MasterID = property.ID,
										ItemNumber = data.ItemNumber,
										MainMortgagor = data.MainType == "Main" ? "Y" : "N",
										MortgagorType = data.IndividualCorporate == clsVariables.Corporate ? "C" : "I",
										Mortgagor = data.Mortgagor,
										MortgagorAddress = data.Address,
										MortgagorDept = data.Department,
										MortgagorConPerson = data.ContactPerson,
										CreatedBy = model.CreatedBy,
										CreatedDate = DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_PropertyMortgagor.Add(propertyMortgagor);
									db.Entry(propertyMortgagor).State = EntityState.Modified;
								}
							}
							if (newMortgagor.Count > 0)
							{
								foreach (var item in newMortgagor)
								{
									masterIdCountMor++;
									propertyMortgagor = new Security_PropertyMortgagor()
									{
										MasterID = property.ID,
										ItemNumber = masterIdCountMor,
										MainMortgagor = item.MainType == "Main" ? "Y" : "N",
										MortgagorType = item.IndividualCorporate == clsVariables.Corporate ? "C" : "I",
										Mortgagor = item.Mortgagor,
										MortgagorAddress = item.Address,
										MortgagorDept = item.Department,
										MortgagorConPerson = item.ContactPerson,
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_PropertyMortgagor.Add(propertyMortgagor);
									db.Entry(propertyMortgagor).State = EntityState.Added;
								}
							}
							var masterId = db.SaveChanges();
						}
						#endregion

						#region Customer
						if (String.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCount++;
								Security_PropertyCustomer propertyCustomer = new Security_PropertyCustomer()
								{
									MasterID = property.ID,
									ItemNumber = masterIdCount,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Customer = data.Customer,
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_PropertyCustomer.Add(propertyCustomer);
								db.Entry(propertyCustomer).State = EntityState.Added;
							}
							var customerId = db.SaveChanges();
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_PropertyCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_PropertyCustomer customer = new Security_PropertyCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_PropertyCustomer()
									{
										MasterID = property.ID,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_PropertyCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_PropertyCustomer()
									{
										MasterID = property.ID,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_PropertyCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_PropertyCustomer()
									{
										MasterID = property.ID,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_PropertyCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							var customerId = db.SaveChanges();
						}

						#endregion

						#region InsertFieldChangeHistory
						string TableName = "Security_Property";
						int changedId = 0;

						if (model.PropertyDetails.FormalValuation >= Convert.ToDecimal(0.00))
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "FormalValuation", model.PropertyDetails.FormalValuation.ToString(),
								"ID", property.ID, "O", userMail, DateTime.Now);
								changedId += res;
							}
							else
							{
								if (model.PropertyDetails.FormalValuation != model.PropertyDetails.FormalValuationOld)
								{
									int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "FormalValuation", model.PropertyDetails.FormalValuation.ToString(),
									"ID", property.ID, "O", userMail, DateTime.Now);
									changedId += res;
								}
								else
								{
									changedId = 1;
								}
							}

						}

						if (model.PropertyDetails.IndicativeValuation > Convert.ToDecimal(0.00))
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "IndicativeValuation", model.PropertyDetails.IndicativeValuation.ToString(),
								"ID", property.ID, "O", userMail, DateTime.Now);
								changedId += res;
							}
							else
							{
								if (model.PropertyDetails.IndicativeValuation != model.PropertyDetails.IndicativeValuationOld)
								{
									int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "IndicativeValuation", model.PropertyDetails.IndicativeValuation.ToString(),
									"ID", property.ID, "O", userMail, DateTime.Now);
									changedId += res;
								}
								else
								{
									changedId = 1;
								}
							}
						}

						if (model.PropertyDetails.CreditLimit > Convert.ToDecimal(0.00))
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "CreditLimit", model.PropertyDetails.CreditLimit.ToString(),
								"ID", property.ID, "O", userMail, DateTime.Now);
								changedId += res;
							}
							else
							{
								if (model.PropertyDetails.CreditLimit != model.PropertyDetails.CreditLimitOld)
								{
									int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "CreditLimit", model.PropertyDetails.CreditLimit.ToString(),
									"ID", property.ID, "O", userMail, DateTime.Now);
									changedId += res;
								}
								else
								{
									changedId = 1;
								}
							}
						}

						#endregion

						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (changedId > 0)
						{
							if (!String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
							else
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + property.ID;
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertPropertyDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{

						transaction.Dispose();
					}
					glog.Debug("InsertPropertyDetails: Exit");
					return result;
				}
			}
		}

		public ResultViewModel InsertVesselDetails(VesselModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertVesselDetails: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var hullnumber = db.Security_Vessel.Any(x => x.HullNumber == model.HullNumber && x.Status == "O" && x.ID != model.Id);
						if (hullnumber)
						{
							result.Status = 2;
							result.Message = "Vessel hull number is already exist!";
							return result;
						}
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var FieldValue = new Security_Vessel();
						if (string.IsNullOrEmpty(model.Id))
						{
							FieldValue = db.Security_Vessel.Where(x => x.Status == "O" && x.ID == model.Id).FirstOrDefault();
						}
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "MVS", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_Vessel vessel = new Security_Vessel();
						vessel.ID = String.IsNullOrEmpty(model.Id) ? NewId.NewId : model.Id;
						vessel.HullNumber = model.HullNumber;
						vessel.VesselName = model.VesselName;
						vessel.CountryOfReg = model.VesselDetails.CountryofRegistration;
						vessel.ChargeNumber = model.VesselDetails.ChargeNumber;
						vessel.ChargeDate = !string.IsNullOrEmpty(model.VesselDetails.ChargeDate) ? DateTime.ParseExact(model.VesselDetails.ChargeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
						vessel.MortgageNumber = model.VesselDetails.MortgageNumber;
						vessel.FormalValuation = model.VesselDetails.FormalValuation;
						vessel.IndicativeValuation = model.VesselDetails.IndicativeValuation;
						vessel.CreditLimit = model.VesselDetails.CreditLimit;
						vessel.Status = String.IsNullOrEmpty(model.Id) ? "O" : model.Status;
						vessel.CreatedBy = String.IsNullOrEmpty(model.Id) ? userMail : model.CreatedBy;
						vessel.CreatedDate = String.IsNullOrEmpty(model.Id) ? DateTime.Now : DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
						vessel.UpdatedBy = String.IsNullOrEmpty(model.Id) ? null : userMail;
						vessel.UpdatedDate = String.IsNullOrEmpty(model.Id) ? (DateTime?)null : DateTime.Now;
						vessel.SecurityListLevel2 = clsVariables.ConstMortgageVessel; //"SLL2-1002";
						db.Security_Vessel.Add(vessel);
						if (String.IsNullOrEmpty(model.Id))
						{
							db.Entry(vessel).State = EntityState.Added;
						}
						else
						{
							db.Entry(vessel).State = EntityState.Modified;
						}
						var id = db.SaveChanges();
						#region Mortgagor
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_VesselMortgagor.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.Mortgagor)
							{
								masterIdCount++;
								Security_VesselMortgagor vesselMortgagor = new Security_VesselMortgagor()
								{
									MasterID = vessel.ID,
									ItemNumber = masterIdCount,
									MainMortgagor = data.MainType == "Main" ? "Y" : "N",
									MortgagorType = data.IndividualCorporate == clsVariables.Corporate ? "C" : "I",
									Mortgagor = data.Mortgagor,
									MortgagorAddress = data.Address,
									MortgagorDept = data.Department,
									MortgagorConPerson = data.ContactPerson,
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_VesselMortgagor.Add(vesselMortgagor);
								db.Entry(vesselMortgagor).State = EntityState.Added;
								var masterId = db.SaveChanges();
							}
						}
						else
						{
							var ExistMortgagor = GetSecurityVesselMortgagor(model.Id);
							var masterIdCountMor = db.Security_VesselMortgagor.Where(x => x.MasterID == model.Id).Count();

							Security_VesselMortgagor vesselMortgagor = new Security_VesselMortgagor();
							var deletedMortgagor = ExistMortgagor.Where(p => !model.Mortgagor.Any(p2 => p2.Mortgagor == p.Mortgagor && p2.ItemNumber != 0)).ToList();
							var UpdateMortgagor = model.Mortgagor.Where(p => ExistMortgagor.Any(p2 => p2.Mortgagor == p.Mortgagor && p2.ItemNumber != 0)).ToList();
							var newMortgagor = model.Mortgagor.Where(p => !ExistMortgagor.Any(p2 => p2.Mortgagor == p.Mortgagor && p2.ItemNumber != 0)).ToList();
							if (deletedMortgagor.Count > 0)
							{
								foreach (var data in deletedMortgagor)
								{
									vesselMortgagor = new Security_VesselMortgagor()
									{
										MasterID = vessel.ID,
										ItemNumber = data.ItemNumber,
										MainMortgagor = data.MainMortgagor,
										MortgagorType = data.MortgagorType,
										Mortgagor = data.Mortgagor,
										MortgagorAddress = data.MortgagorAddress,
										MortgagorDept = data.MortgagorDept,
										MortgagorConPerson = data.MortgagorConPerson,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_VesselMortgagor.Add(vesselMortgagor);
									db.Entry(vesselMortgagor).State = EntityState.Modified;
								}

							}
							if (UpdateMortgagor.Count > 0)
							{
								foreach (var data in UpdateMortgagor)
								{
									vesselMortgagor = new Security_VesselMortgagor()
									{
										MasterID = vessel.ID,
										ItemNumber = data.ItemNumber,
										MainMortgagor = data.MainType == "Main" ? "Y" : "N",
										MortgagorType = data.IndividualCorporate == clsVariables.Corporate ? "C" : "I",
										Mortgagor = data.Mortgagor,
										MortgagorAddress = data.Address,
										MortgagorDept = data.Department,
										MortgagorConPerson = data.ContactPerson,
										CreatedBy = model.CreatedBy,
										CreatedDate = DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_VesselMortgagor.Add(vesselMortgagor);
									db.Entry(vesselMortgagor).State = EntityState.Modified;
								}
							}
							if (newMortgagor.Count > 0)
							{
								foreach (var item in newMortgagor)
								{
									masterIdCountMor++;
									vesselMortgagor = new Security_VesselMortgagor()
									{
										MasterID = vessel.ID,
										ItemNumber = masterIdCountMor,
										MainMortgagor = item.MainType == "Main" ? "Y" : "N",
										MortgagorType = item.IndividualCorporate == clsVariables.Corporate ? "C" : "I",
										Mortgagor = item.Mortgagor,
										MortgagorAddress = item.Address,
										MortgagorDept = item.Department,
										MortgagorConPerson = item.ContactPerson,
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_VesselMortgagor.Add(vesselMortgagor);
									db.Entry(vesselMortgagor).State = EntityState.Added;
								}
							}
							var masterId = db.SaveChanges();
						}
						#endregion
						#region Insurance
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_VesselInsurance.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.InsuranceDetail)
							{
								masterIdCount++;
								Security_VesselInsurance insurance = new Security_VesselInsurance()
								{
									MasterID = vessel.ID,
									ItemNumber = masterIdCount,
									InsuranceType = data.InsuranceType,
									ExpiryDate = !string.IsNullOrEmpty(data.ExpiryDate) ? DateTime.ParseExact(data.ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null,
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_VesselInsurance.Add(insurance);
								db.Entry(insurance).State = EntityState.Added;
								var insuranceId = db.SaveChanges();
							}
						}
						else
						{
							var ExistInsaurance = GetSecurityVesselInsurances(model.Id);
							var masterIdCount = db.Security_VesselInsurance.Where(x => x.MasterID == model.Id).Count();

							Security_VesselInsurance insurance = new Security_VesselInsurance();
							var masterIdCountIns = ExistInsaurance.Count();
							var deletedInsaurance = ExistInsaurance.Where(p => !model.InsuranceDetail.Any(p2 => p2.InsuranceType == p.InsuranceType)).ToList();
							var UpdateInsaurance = model.InsuranceDetail.Where(p => ExistInsaurance.Any(p2 => p2.InsuranceType == p.InsuranceType && p.ItemNumber != 0)).ToList();
							var newInsaurance = model.InsuranceDetail.Where(p => !ExistInsaurance.Any(p2 => p2.InsuranceType == p.InsuranceType && p2.ItemNumber!=0 && p2.Status !="X")).ToList();
							if (deletedInsaurance.Count > 0)
							{
								foreach (var data in deletedInsaurance)
								{
									insurance = new Security_VesselInsurance()
									{
										MasterID = vessel.ID,
										ItemNumber = data.ItemNumber,
										InsuranceType = data.InsuranceType,
										ExpiryDate = data.ExpiryDate,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = model.CreatedBy,
										CreatedDate = DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_VesselInsurance.Add(insurance);
									db.Entry(insurance).State = EntityState.Modified;
								}

							}
							if (UpdateInsaurance.Count > 0)
							{
								foreach (var data in UpdateInsaurance)
								{
									insurance = new Security_VesselInsurance()
									{
										MasterID = vessel.ID,
										ItemNumber = data.ItemNumber,
										InsuranceType = data.InsuranceType,
										ExpiryDate = DateTime.ParseExact(data.ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
										CreatedBy = model.CreatedBy,
										CreatedDate = DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_VesselInsurance.Add(insurance);
									db.Entry(insurance).State = EntityState.Modified;
								}
							}
							if (newInsaurance.Count > 0)
							{
								foreach (var item in newInsaurance)
								{
									masterIdCountIns++;
									insurance = new Security_VesselInsurance()
									{
										MasterID = vessel.ID,
										ItemNumber = masterIdCountIns,
										InsuranceType = item.InsuranceType,
										ExpiryDate = !string.IsNullOrEmpty(item.ExpiryDate) ? DateTime.ParseExact(item.ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null,
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_VesselInsurance.Add(insurance);
									db.Entry(insurance).State = EntityState.Added;
								}
							}
							var insuranceId = db.SaveChanges();
						}
						#endregion
						#region Customer
						var customerId = 0;
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCountCust = db.Security_VesselCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCountCust++;
								Security_VesselCustomer customer = new Security_VesselCustomer()
								{
									MasterID = vessel.ID,
									ItemNumber = masterIdCountCust,
									Customer = data.Customer,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_VesselCustomer.Add(customer);
								db.Entry(customer).State = EntityState.Added;
								customerId = db.SaveChanges();
							}
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_VesselCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_VesselCustomer customer = new Security_VesselCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_VesselCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_VesselCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_VesselCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_VesselCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_VesselCustomer()
									{
										MasterID = model.Id,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_VesselCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							customerId = db.SaveChanges();
						}
						#endregion

						#region InsertFieldChangeHistory
						string TableName = "Security_Vessel";
						int changedId = 0;

						if (model.VesselDetails.FormalValuation >= Convert.ToDecimal(0.00))
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "FormalValuation", model.VesselDetails.FormalValuation.ToString(),
									"ID", vessel.ID, "O", userMail, DateTime.Now);
								changedId += res;
							}
							else
							{
								if (model.VesselDetails.FormalValuation != model.VesselDetails.FormalValuationOld)
								{
									int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "FormalValuation", model.VesselDetails.FormalValuation.ToString(),
									"ID", vessel.ID, "O", userMail, DateTime.Now);
									changedId += res;
								}
								else
								{
									changedId = 1;
								}
							}
						}

						if (model.VesselDetails.IndicativeValuation > Convert.ToDecimal(0.00))
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "IndicativeValuation", model.VesselDetails.IndicativeValuation.ToString(),
									"ID", vessel.ID, "O", userMail, DateTime.Now);
								changedId += res;
							}
							else
							{
								if (model.VesselDetails.IndicativeValuation != model.VesselDetails.IndicativeValuationOld)
								{
									int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "IndicativeValuation", model.VesselDetails.IndicativeValuation.ToString(),
									"ID", vessel.ID, "O", userMail, DateTime.Now);
									changedId += res;
								}
								else
								{
									changedId = 1;
								}
							}
						}

						if (model.VesselDetails.CreditLimit > Convert.ToDecimal(0.00))
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "CreditLimit", model.VesselDetails.CreditLimit.ToString(),
									"ID", vessel.ID, "O", userMail, DateTime.Now);
								changedId += res;
							}
							else
							{
								if (model.VesselDetails.CreditLimit != model.VesselDetails.CreditLimitOld)
								{
									int res = _clsGlobal.InsertFieldChangeHistory(db, TableName, "CreditLimit", model.VesselDetails.CreditLimit.ToString(),
											"ID", vessel.ID, "O", userMail, DateTime.Now);
									changedId += res;
								}
								else
								{
									changedId = 1;
								}
							}
						}
						#endregion

						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (changedId > 0)
						{
							if (!String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
							else
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + vessel.ID;
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}

					catch (Exception ex)
					{
						glog.Error("InsertVesselDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();

					}
					glog.Debug("InsertVesselDetails: Exit");
					return result;
				}
			}
		}

		public ResultViewModel InsertVehicleDetails(VehicleModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertVehicleDetails: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var chassisNumber = db.Security_Vehicle.Any(x => x.Status == "O" && x.ChassisNumber == model.ChassisNumber && x.ID != model.Id);
						if (chassisNumber)
						{
							result.Status = 2;
							result.Message = "Vehicle chassis number is already exist!";
							return result;
						}
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "DVH", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_Vehicle vehicle = new Security_Vehicle();
						vehicle.ChassisNumber = model.ChassisNumber.ToUpper();
						vehicle.RegNumber = model.VehicleRegNumber.ToUpper();
						vehicle.VehicleMake = model.VehicleDetails.VehicleMakeId;
						vehicle.VehicleModel = model.VehicleDetails.VehicleModelId;
						vehicle.VehicleType = model.VehicleDetails.VehicleType;
						vehicle.Value = model.VehicleDetails.Value;
						vehicle.ChargeDate = !string.IsNullOrEmpty(model.VehicleDetails.ChargeDate) ? DateTime.ParseExact(model.VehicleDetails.ChargeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
						vehicle.ChargeNumber = model.VehicleDetails.ChargeNumber;
						vehicle.COE_ExpiryDate = !string.IsNullOrEmpty(model.VehicleDetails.COEExpiryDate) ? DateTime.ParseExact(model.VehicleDetails.COEExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
						vehicle.EngineNumber = model.VehicleDetails.EngineNumber;
						vehicle.CreatedBy = userMail;
						vehicle.CreatedDate = DateTime.Now;
						vehicle.Status = "O";
						vehicle.UpdatedBy = null;
						vehicle.UpdatedDate = null;
						vehicle.SecurityListLevel2 = clsVariables.ConstDebentureVehicle;// "SLL2-1003";
						db.Security_Vehicle.Add(vehicle);
						if (!String.IsNullOrEmpty(model.Id))
						{
							vehicle.ID = model.Id;
							vehicle.CreatedBy = model.CreatedBy;
							vehicle.CreatedDate = DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
							vehicle.UpdatedBy = userMail;
							vehicle.UpdatedDate = DateTime.Now;
							db.Entry(vehicle).State = EntityState.Modified;
						}
						else
						{
							vehicle.ID = NewId.NewId;
							vehicle.CreatedBy = userMail;
							vehicle.CreatedDate = DateTime.Now;
							vehicle.UpdatedBy = null;
							vehicle.UpdatedDate = null;
							db.Entry(vehicle).State = EntityState.Added;
						}
						var Id = db.SaveChanges();

						#region Customer
						var customerId = 0;
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_VehicleCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCount++;
								Security_VehicleCustomer customer = new Security_VehicleCustomer()
								{
									MasterID = vehicle.ID,
									ItemNumber = masterIdCount,
									Customer = data.Customer,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_VehicleCustomer.Add(customer);
								db.Entry(customer).State = EntityState.Added;
							}
							customerId = db.SaveChanges();
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_VehicleCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_VehicleCustomer customer = new Security_VehicleCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_VehicleCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_VehicleCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_VehicleCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_VehicleCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_VehicleCustomer()
									{
										MasterID = model.Id,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_VehicleCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							customerId = db.SaveChanges();
						}
						#endregion

						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (customerId > 0)
						{
							if (!String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
							else
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + vehicle.ID;
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertVehicleDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertVehicleDetails: Exit");
					return result;
				}
			}
		}

		public ResultViewModel InsertConstructionEquipDetails(EquipmentModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertConstructionEquipDetails: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "DCE", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_ConstructionEquip equipment = new Security_ConstructionEquip();
						equipment.ID = String.IsNullOrEmpty(model.Id) ? NewId.NewId : model.Id;
						equipment.EquipBrand = model.Brand;
						equipment.EquipDesc = model.EquipmentDescription;
						equipment.EquipModel = model.Model;
						equipment.ChargeDate = !string.IsNullOrEmpty(model.EquipmentDetails.ChargeDate) ? DateTime.ParseExact(model.EquipmentDetails.ChargeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
						equipment.ChargeNumber = model.EquipmentDetails.ChargeNumber;
						equipment.EngineNumber = model.EquipmentDetails.EngineNumber;
						equipment.SerialNumber = model.EquipmentDetails.SerialNumber;
						equipment.SecuredAmount = model.EquipmentDetails.SecuredAmount;
						equipment.YearOfManufacture = model.EquipmentDetails.ManufactureYear;
						equipment.Status = String.IsNullOrEmpty(model.Id) ? "O" : model.Status;
						equipment.CreatedBy = String.IsNullOrEmpty(model.Id) ? userMail : model.CreatedBy;
						equipment.CreatedDate = String.IsNullOrEmpty(model.Id) ? DateTime.Now : DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
						equipment.UpdatedBy = String.IsNullOrEmpty(model.Id) ? null : userMail;
						equipment.UpdatedDate = String.IsNullOrEmpty(model.Id) ? (DateTime?)null : DateTime.Now;
						equipment.SecurityListLevel2 = clsVariables.ConstDebentureConstructionEquipment; //"SLL2-1004";
						db.Security_ConstructionEquip.Add(equipment);
						if (String.IsNullOrEmpty(model.Id))
						{
							db.Entry(equipment).State = EntityState.Added;
						}
						else
						{
							db.Entry(equipment).State = EntityState.Modified;
						}
						var Id = db.SaveChanges();

						#region Customer
						var customerId = 0;
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_ConstructionEquipCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCount++;
								Security_ConstructionEquipCustomer customer = new Security_ConstructionEquipCustomer()
								{
									MasterID = equipment.ID,
									ItemNumber = masterIdCount,
									Customer = data.Customer,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_ConstructionEquipCustomer.Add(customer);
								db.Entry(customer).State = EntityState.Added;
							}
							customerId = db.SaveChanges();
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_ConstructionEquipCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_ConstructionEquipCustomer customer = new Security_ConstructionEquipCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_ConstructionEquipCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_ConstructionEquipCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_ConstructionEquipCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_ConstructionEquipCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_ConstructionEquipCustomer()
									{
										MasterID = model.Id,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_ConstructionEquipCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							customerId = db.SaveChanges();
						}
						#endregion

						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (customerId > 0)
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + equipment.ID;
							}
							else
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertConstructionEquipDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertConstructionEquipDetails: Exit");
					return result;
				}
			}
		}

		public ResultViewModel InsertIndustrialEquipDetails(EquipmentModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertIndustrialEquipDetails: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "DIE", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_IndustrialEquip equipment = new Security_IndustrialEquip();
						equipment.ID = String.IsNullOrEmpty(model.Id) ? NewId.NewId : model.Id;
						equipment.EquipBrand = model.Brand;
						equipment.EquipDesc = model.EquipmentDescription;
						equipment.EquipModel = model.Model;
						equipment.ChargeDate = !string.IsNullOrEmpty(model.EquipmentDetails.ChargeDate) ? DateTime.ParseExact(model.EquipmentDetails.ChargeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
						equipment.ChargeNumber = model.EquipmentDetails.ChargeNumber;
						equipment.EngineNumber = model.EquipmentDetails.EngineNumber;
						equipment.SerialNumber = model.EquipmentDetails.SerialNumber;
						equipment.SecuredAmount = model.EquipmentDetails.SecuredAmount;
						equipment.YearOfManufacture = model.EquipmentDetails.ManufactureYear;
						equipment.Status = String.IsNullOrEmpty(model.Id) ? "O" : model.Status;
						equipment.CreatedBy = String.IsNullOrEmpty(model.Id) ? userMail : model.CreatedBy;
						equipment.CreatedDate = String.IsNullOrEmpty(model.Id) ? DateTime.Now : DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture); ;
						equipment.UpdatedBy = String.IsNullOrEmpty(model.Id) ? null : userMail;
						equipment.UpdatedDate = String.IsNullOrEmpty(model.Id) ? (DateTime?)null : DateTime.Now;
						equipment.SecurityListLevel2 = clsVariables.ConstDebentureIndustrialEquipment;// "SLL2-1005";
						db.Security_IndustrialEquip.Add(equipment);
						if (String.IsNullOrEmpty(model.Id))
						{
							db.Entry(equipment).State = EntityState.Added;
						}
						else
						{
							db.Entry(equipment).State = EntityState.Modified;
						}
						var Id = db.SaveChanges();

						#region Customer
						var customerId = 0;
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_IndustrialEquipCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCount++;
								Security_IndustrialEquipCustomer customer = new Security_IndustrialEquipCustomer()
								{
									MasterID = equipment.ID,
									ItemNumber = masterIdCount,
									Customer = data.Customer,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_IndustrialEquipCustomer.Add(customer);
								db.Entry(customer).State = EntityState.Added;
							}
							customerId = db.SaveChanges();
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_IndustrialEquipCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_IndustrialEquipCustomer customer = new Security_IndustrialEquipCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_IndustrialEquipCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_IndustrialEquipCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_IndustrialEquipCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_IndustrialEquipCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_IndustrialEquipCustomer()
									{
										MasterID = model.Id,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_IndustrialEquipCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							customerId = db.SaveChanges();
						}
						#endregion

						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (customerId > 0)
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + equipment.ID;
							}
							else
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertIndustrialEquipDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertIndustrialEquipDetails: Exit");
					return result;
				}
			}
		}

		public ResultViewModel InsertInventoryDetails(InventoryModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertInventoryDetails: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "DIV", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_Inventory inventry = new Security_Inventory()
						{
							ID = String.IsNullOrEmpty(model.Id) ? NewId.NewId : model.Id,
							Type = model.Type,
							Value = model.Value,
							ChargeNumber = model.ChargeNumber,
							ChargeDate = !string.IsNullOrEmpty(model.ChargeDate) ? DateTime.ParseExact(model.ChargeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null,
							SecurityListLevel2 = clsVariables.ConstDebentureInventories,// "SLL2-1006",
							Status = String.IsNullOrEmpty(model.Id) ? "O" : model.Status,
							CreatedBy = String.IsNullOrEmpty(model.Id) ? userMail : model.CreatedBy,
							CreatedDate = String.IsNullOrEmpty(model.Id) ? DateTime.Now : DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
							UpdatedBy = String.IsNullOrEmpty(model.Id) ? null : userMail,
							UpdatedDate = String.IsNullOrEmpty(model.Id) ? (DateTime?)null : DateTime.Now
						};
						db.Security_Inventory.Add(inventry);
						if (String.IsNullOrEmpty(model.Id))
						{
							db.Entry(inventry).State = EntityState.Added;
						}
						else
						{
							db.Entry(inventry).State = EntityState.Modified;
						}
						var Id = db.SaveChanges();

						#region Customer
						var customerId = 0;
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_InventoryCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCount++;
								Security_InventoryCustomer customer = new Security_InventoryCustomer()
								{
									MasterID = inventry.ID,
									ItemNumber = masterIdCount,
									Customer = data.Customer,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_InventoryCustomer.Add(customer);
								db.Entry(customer).State = EntityState.Added;
							}
							customerId = db.SaveChanges();
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_InventoryCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_InventoryCustomer customer = new Security_InventoryCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_InventoryCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_InventoryCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_InventoryCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_InventoryCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_InventoryCustomer()
									{
										MasterID = model.Id,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_InventoryCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							customerId = db.SaveChanges();
						}
						#endregion

						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (customerId > 0)
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + inventry.ID;
							}
							else
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertInventoryDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertInventoryDetails: Exit");
					return result;
				}
			}
		}

		public ResultViewModel InsertReceivableDetails(ReceivableModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertReceivableDetails: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "DRC", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_Receivable receivable = new Security_Receivable()
						{
							ID = !String.IsNullOrEmpty(model.Id) ? model.Id : NewId.NewId,
							Amount = model.Amount,
							ChargeNumber = model.ChargeNumber,
							ChargeDate = !String.IsNullOrEmpty(model.ChargeDate) ? DateTime.ParseExact(model.ChargeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null,
							SecurityListLevel2 = clsVariables.ConstDebentureReceivables, //"SLL2-1007",
							Status = String.IsNullOrEmpty(model.Id) ? "O" : model.Status,
							CreatedBy = String.IsNullOrEmpty(model.Id) ? userMail : model.CreatedBy,
							CreatedDate = String.IsNullOrEmpty(model.Id) ? DateTime.Now : DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
							UpdatedBy = String.IsNullOrEmpty(model.Id) ? null : userMail,
							UpdatedDate = String.IsNullOrEmpty(model.Id) ? (DateTime?)null : DateTime.Now,
						};
						db.Security_Receivable.Add(receivable);
						if (!String.IsNullOrEmpty(model.Id))
						{
							db.Entry(receivable).State = EntityState.Modified;
						}
						else
						{
							db.Entry(receivable).State = EntityState.Added;
						}
						var Id = db.SaveChanges();

						#region Customer
						var customerId = 0;
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_ReceivableCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCount++;
								Security_ReceivableCustomer customer = new Security_ReceivableCustomer()
								{
									MasterID = receivable.ID,
									ItemNumber = masterIdCount,
									Customer = data.Customer,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_ReceivableCustomer.Add(customer);
								db.Entry(customer).State = EntityState.Added;
							}
							customerId = db.SaveChanges();
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_ReceivableCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_ReceivableCustomer customer = new Security_ReceivableCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_ReceivableCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_ReceivableCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_ReceivableCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_ReceivableCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_ReceivableCustomer()
									{
										MasterID = model.Id,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_ReceivableCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							customerId = db.SaveChanges();
						}
						#endregion
						
						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (customerId > 0)
						{
							if (!String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
							else
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + receivable.ID;
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertReceivableDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertReceivableDetails: Exit");
					return result;
				}
			}
		}

		public ResultViewModel InsertCashEquivalentIndividualDetail(CashEquivalentModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertCashEquivalentIndividualDetail: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "DEI", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_CashEquivalentInd cashEquivalentInd = new Security_CashEquivalentInd()
						{
							ID = String.IsNullOrEmpty(model.Id) ? NewId.NewId : model.Id,
							Amount = model.Amount,
							Refundable = model.Refundable == "Yes" ? "Y" : "N",
							GuaranteeBondsType = model.GuaranteeBondsType,
							BillToCustomer = model.BillToModel.Customer,
							BillToAddress = model.BillToModel.Address,
							BillToDept = model.BillToModel.Department,
							BillToConPerson = model.BillToModel.ContactPerson,
							SecurityListLevel2 = clsVariables.CashEquivalentIndividual, //"SLL2-1008",
							Status = String.IsNullOrEmpty(model.Id) ? "O" : model.Status,
							CreatedBy = String.IsNullOrEmpty(model.Id) ? userMail : model.CreatedBy,
							CreatedDate = String.IsNullOrEmpty(model.Id) ? DateTime.Now : DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
							UpdatedBy = String.IsNullOrEmpty(model.Id) ? null : userMail,
							UpdatedDate = String.IsNullOrEmpty(model.Id) ? (DateTime?)null : DateTime.Now
						};
						db.Security_CashEquivalentInd.Add(cashEquivalentInd);
						if (String.IsNullOrEmpty(model.Id))
						{
							db.Entry(cashEquivalentInd).State = EntityState.Added;
						}
						else
						{
							db.Entry(cashEquivalentInd).State = EntityState.Modified;
						}
						var Id = db.SaveChanges();

						#region Customer
						var customerId = 0;
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_CashEquivalentIndCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCount++;
								Security_CashEquivalentIndCustomer customer = new Security_CashEquivalentIndCustomer()
								{
									MasterID = cashEquivalentInd.ID,
									ItemNumber = masterIdCount,
									Customer = data.Customer,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_CashEquivalentIndCustomer.Add(customer);
								db.Entry(customer).State = EntityState.Added;
							}
							customerId = db.SaveChanges();
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_CashEquivalentIndCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_CashEquivalentIndCustomer customer = new Security_CashEquivalentIndCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_CashEquivalentIndCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_CashEquivalentIndCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_CashEquivalentIndCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_CashEquivalentIndCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_CashEquivalentIndCustomer()
									{
										MasterID = model.Id,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_CashEquivalentIndCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							customerId = db.SaveChanges();
						}
						#endregion

						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (customerId > 0)
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + cashEquivalentInd.ID;
							}
							else
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertCashEquivalentIndividualDetail Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertCashEquivalentIndividualDetail: Exit");
					return result;
				}
			}
		}

		public ResultViewModel InsertCashEquivalentCompanyDetail(CashEquivalentModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertCashEquivalentCompanyDetail: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "DEC", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_CashEquivalentCom cashEquivalentCom = new Security_CashEquivalentCom()
						{
							ID = String.IsNullOrEmpty(model.Id) ? NewId.NewId : model.Id,
							Amount = model.Amount,
							Refundable = model.Refundable == "Yes" ? "Y" : "N",
							GuaranteeBondsType = model.GuaranteeBondsType,
							BillToCustomer = model.BillToModel.Customer,
							BillToAddress = model.BillToModel.Address,
							BillToDept = model.BillToModel.Department,
							BillToConPerson = model.BillToModel.ContactPerson,
							SecurityListLevel2 = clsVariables.CashEquivalentCompany, //"SLL2-1009",
							Status = String.IsNullOrEmpty(model.Id) ? "O" : model.Status,
							CreatedBy = String.IsNullOrEmpty(model.Id) ? userMail : model.CreatedBy,
							CreatedDate = String.IsNullOrEmpty(model.Id) ? DateTime.Now : DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
							UpdatedBy = String.IsNullOrEmpty(model.Id) ? null : userMail,
							UpdatedDate = String.IsNullOrEmpty(model.Id) ? (DateTime?)null : DateTime.Now
						};
						db.Security_CashEquivalentCom.Add(cashEquivalentCom);
						if (String.IsNullOrEmpty(model.Id))
						{
							db.Entry(cashEquivalentCom).State = EntityState.Added;
						}
						else
						{
							db.Entry(cashEquivalentCom).State = EntityState.Modified;
						}
						var Id = db.SaveChanges();

						#region Customer
						var customerId = 0;
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_CashEquivalentIndCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCount++;
								Security_CashEquivalentComCustomer customer = new Security_CashEquivalentComCustomer()
								{
									MasterID = cashEquivalentCom.ID,
									ItemNumber = masterIdCount,
									Customer = data.Customer,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_CashEquivalentComCustomer.Add(customer);
								db.Entry(customer).State = EntityState.Added;
							}
							customerId = db.SaveChanges();
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_CashEquivalentComCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_CashEquivalentComCustomer customer = new Security_CashEquivalentComCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_CashEquivalentComCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_CashEquivalentComCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_CashEquivalentComCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_CashEquivalentComCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_CashEquivalentComCustomer()
									{
										MasterID = model.Id,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_CashEquivalentComCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							customerId = db.SaveChanges();
						}
						#endregion

						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (customerId > 0)
						{
							if (String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + cashEquivalentCom.ID;
							}
							else
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertCashEquivalentCompanyDetail Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertCashEquivalentCompanyDetail: Exit");
					return result;
				}
			}
		}

		public ResultViewModel InsertSecFinInstruments(SecFinInstrumentModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertSecFinInstruments: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "DSF", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_SecFinInstruments SecFinInstrument = new Security_SecFinInstruments()
						{
							ID = String.IsNullOrEmpty(model.Id) ? NewId.NewId : model.Id,
							Type = model.FinancialInstrumentType,
							Amount = model.Amount,
							DocumentNumber = model.SecurityorFinancialInstrumentDetails.DocumentNumber,
							BankFinancialCom = model.SecurityorFinancialInstrumentDetails.BankNameorFinancialCompany,
							ChargeDate = !string.IsNullOrEmpty(model.SecurityorFinancialInstrumentDetails.ChargeDate) ? DateTime.ParseExact(model.SecurityorFinancialInstrumentDetails.ChargeDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null,
							SecurityListLevel2 = clsVariables.SecFinInstruments,// "SLL2-1010",
							Status = String.IsNullOrEmpty(model.Id) ? "O" : model.Status,
							CreatedBy = String.IsNullOrEmpty(model.Id) ? userMail : model.CreatedBy,
							CreatedDate = String.IsNullOrEmpty(model.Id) ? DateTime.Now : DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
							UpdatedBy = String.IsNullOrEmpty(model.Id) ? null : userMail,
							UpdatedDate = String.IsNullOrEmpty(model.Id) ? (DateTime?)null : DateTime.Now
						};
						db.Security_SecFinInstruments.Add(SecFinInstrument);
						if (String.IsNullOrEmpty(model.Id))
						{
							db.Entry(SecFinInstrument).State = EntityState.Added;
						}
						else
						{
							db.Entry(SecFinInstrument).State = EntityState.Modified;
						}
						var Id = db.SaveChanges();

						#region Customer
						var customerId = 0;
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_SecFinInstrumentsCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCount++;
								Security_SecFinInstrumentsCustomer customer = new Security_SecFinInstrumentsCustomer()
								{
									MasterID = SecFinInstrument.ID,
									ItemNumber = masterIdCount,
									Customer = data.Customer,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_SecFinInstrumentsCustomer.Add(customer);
								db.Entry(customer).State = EntityState.Added;
							}
							customerId = db.SaveChanges();
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_SecFinInstrumentsCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_SecFinInstrumentsCustomer customer = new Security_SecFinInstrumentsCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_SecFinInstrumentsCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_SecFinInstrumentsCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_SecFinInstrumentsCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_SecFinInstrumentsCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_SecFinInstrumentsCustomer()
									{
										MasterID = model.Id,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_SecFinInstrumentsCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							customerId = db.SaveChanges();
						}
						#endregion

						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (customerId > 0)
						{
							if (!String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
							else
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + SecFinInstrument.ID;
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertSecFinInstruments Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertSecFinInstruments: Exit");
					return result;
				}
			}
		}

		public ResultViewModel InsertSecurityDepositDetail(SecurityDepositModel model, string UserName)
		{
			string userMail = "";
			glog.Debug("InsertSecurityDepositDetail: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						var NewId = clsGlobal.GetSystemID("SecurityMaster", "DSD", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
						Security_SecDeposit SecDeposit = new Security_SecDeposit()
						{
							ID = String.IsNullOrEmpty(model.Id) ? NewId.NewId : model.Id,
							Refundable = model.Refundable.ToUpper() == "YES" ? "Y" : "N",
							Amount = model.Amount,
							BillToType = model.BillToDetailModel.IndividualCorporate == clsVariables.Individual ? "I" : "C",
							BillToCustomer = model.BillToDetailModel.Customer,
							BillToAddress = model.BillToDetailModel.Address,
							BillToDept = model.BillToDetailModel.Department,
							BillToConPerson = model.BillToDetailModel.ContactPerson,
							SecurityListLevel2 = clsVariables.SecurityDeposit, //"SLL2-1011",
							Status = String.IsNullOrEmpty(model.Id) ? "O" : model.Status,
							CreatedBy = String.IsNullOrEmpty(model.Id) ? userMail : model.CreatedBy,
							CreatedDate = String.IsNullOrEmpty(model.Id) ? DateTime.Now : DateTime.ParseExact(model.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
							UpdatedBy = String.IsNullOrEmpty(model.Id) ? null : userMail,
							UpdatedDate = String.IsNullOrEmpty(model.Id) ? (DateTime?)null : DateTime.Now
						};
						db.Security_SecDeposit.Add(SecDeposit);
						
						if (!String.IsNullOrEmpty(model.Id))
						{
							db.Entry(SecDeposit).State = System.Data.Entity.EntityState.Modified;
						}
						else
						{
							db.Entry(SecDeposit).State = System.Data.Entity.EntityState.Added;
						}

						var Id = db.SaveChanges();

						#region Customer
						var customerId = 0;
						if (string.IsNullOrEmpty(model.Id))
						{
							var masterIdCount = db.Security_SecDepositCustomer.Where(x => x.Status == "O" && x.MasterID == NewId.NewId).Count();
							foreach (var data in model.CustomerToAccess)
							{
								masterIdCount++;
								Security_SecDepositCustomer customer = new Security_SecDepositCustomer()
								{
									MasterID = SecDeposit.ID,
									ItemNumber = masterIdCount,
									Customer = data.Customer,
									CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
									Status = "O",
									CreatedBy = userMail,
									CreatedDate = DateTime.Now,
									UpdatedBy = null,
									UpdatedDate = (DateTime?)null,
								};
								db.Security_SecDepositCustomer.Add(customer);
								db.Entry(customer).State = EntityState.Added;
							}
							customerId = db.SaveChanges();
						}
						else
						{
							var ExistCustomer = GetExistCustomer(model.Id, "Security_SecDepositCustomer");
							var masterIdCount = db.Security_PropertyCustomer.Where(x => x.MasterID == model.Id).Count();

							Security_SecDepositCustomer customer = new Security_SecDepositCustomer();
							var masterIdCountCust = ExistCustomer.Count();
							var deletedCustomer = ExistCustomer.Where(p => !model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var UpdateCustomer = ExistCustomer.Where(p => model.CustomerToAccess.Any(p2 => p2.Customer == p.Customer)).ToList();
							var newCustomer = model.CustomerToAccess.Where(p => !ExistCustomer.Any(p2 => p2.Customer == p.Customer)).ToList();
							if (deletedCustomer.Count > 0)
							{
								foreach (var data in deletedCustomer)
								{
									customer = new Security_SecDepositCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										ItemNumber = data.ItemNumber,
										DeletedBy = userMail,
										DeletedDate = DateTime.Now,
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = data.UpdatedBy,
										UpdatedDate = data.UpdatedDate,
										Status = "X"
									};
									db.Security_SecDepositCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}

							}
							if (UpdateCustomer.Count > 0)
							{
								foreach (var data in UpdateCustomer)
								{
									customer = new Security_SecDepositCustomer()
									{
										MasterID = model.Id,
										Customer = data.Customer,
										ItemNumber = data.ItemNumber,
										CustomerType = data.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										CreatedBy = data.CreatedBy,
										CreatedDate = data.CreatedDate,
										UpdatedBy = userMail,
										UpdatedDate = DateTime.Now,
										Status = "O",
									};
									db.Security_SecDepositCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Modified;
								}
							}
							if (newCustomer.Count > 0)
							{
								foreach (var item in newCustomer)
								{
									masterIdCountCust++;
									customer = new Security_SecDepositCustomer()
									{
										MasterID = model.Id,
										ItemNumber = masterIdCountCust,
										Customer = item.Customer,
										CustomerType = item.IndividualCorporate == clsVariables.Individual ? "I" : "C",
										Status = "O",
										CreatedBy = userMail,
										CreatedDate = DateTime.Now,
										UpdatedBy = null,
										UpdatedDate = (DateTime?)null,
									};
									db.Security_SecDepositCustomer.Add(customer);
									db.Entry(customer).State = EntityState.Added;
								}
							}
							customerId = db.SaveChanges();
						}
						#endregion

						if (String.IsNullOrEmpty(model.Id))
						{
							clsGlobal.UpdateSystemIDLastNum(NewId, UserName, db);
						}

						transaction.Commit();

						if (customerId > 0)
						{
							if (!String.IsNullOrEmpty(model.Id))
							{
								result.Status = 1;
								result.Message = "Modification saved successfully.";
							}
							else
							{
								result.Status = 1;
								result.Message = "Security master saved successfully with ID – " + SecDeposit.ID;
							}
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when saving security master";
						}
					}
					catch (Exception ex)
					{
						glog.Error("InsertSecurityDepositDetail Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("InsertSecurityDepositDetail: Exit");
					return result;
				}
			}
		}

		public List<SecurityMasterInqList> GetSecurityEnquiryList(SecurityMasterInqParam Param, string GroupType)
		{
			try
			{
				using (var db = new MainDbContext())
				{
					return db.Database.SqlQuery<SecurityMasterInqList>(
					"exec SecurityEnquiry @OrixDB_Name,@SecurityTypeLevel1,@SecurityTypeLevel2,@SecuritySystemId,@SecurityItemStatus,@CreatedDateFrom,@CreatedDateTo,@ChargeDateFrom,@ChargeDateTo,@SecurityItemsIndividual," +
					"@SecurityItemsCustomer,@ContractsIndividual,@ContractsCustomer,@ContractNumber,@ContractRolloverNo,@ContractsStatus,@GroupType",
					new SqlParameter("@OrixDB_Name", clsGlobal.Orix_DB),
					new SqlParameter("@SecurityTypeLevel1", string.IsNullOrWhiteSpace(Param.SecurityTypeLevel1) ? "" : Param.SecurityTypeLevel1),
					new SqlParameter("@SecurityTypeLevel2", string.IsNullOrWhiteSpace(Param.SecurityTypeLevel2) ? "" : Param.SecurityTypeLevel2),
					new SqlParameter("@SecuritySystemId", string.IsNullOrWhiteSpace(Param.SecuritySystemId) ? "" : Param.SecuritySystemId),
					new SqlParameter("@SecurityItemStatus", string.IsNullOrWhiteSpace(Param.SecurityItemStatus) ? "" : String.Format("'{0}'", Param.SecurityItemStatus)),
					new SqlParameter("@CreatedDateFrom", string.IsNullOrWhiteSpace(Param.CreatedDateFrom) ? "" : DateTime.ParseExact(Param.CreatedDateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")),
					new SqlParameter("@CreatedDateTo", string.IsNullOrWhiteSpace(Param.CreatedDateTo) ? "" : DateTime.ParseExact(Param.CreatedDateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")),
					new SqlParameter("@ChargeDateFrom", string.IsNullOrWhiteSpace(Param.ChargeDateFrom) ? "" : DateTime.ParseExact(Param.ChargeDateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")),
					new SqlParameter("@ChargeDateTo", string.IsNullOrWhiteSpace(Param.ChargeDateTo) ? "" : DateTime.ParseExact(Param.ChargeDateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")),
					new SqlParameter("@SecurityItemsIndividual", string.IsNullOrWhiteSpace(Param.SecurityItemsIndividual) ? "" : Param.SecurityItemsIndividual),
					new SqlParameter("@SecurityItemsCustomer", string.IsNullOrWhiteSpace(Param.SecurityItemsCustomer) ? "" : Param.SecurityItemsCustomer),
					new SqlParameter("@ContractsIndividual", string.IsNullOrWhiteSpace(Param.ContractsIndividual) ? "" : Param.ContractsIndividual),
					new SqlParameter("@ContractsCustomer", string.IsNullOrWhiteSpace(Param.ContractsCustomer) ? "" : Param.ContractsCustomer),
					new SqlParameter("@ContractNumber", string.IsNullOrWhiteSpace(Param.ContractNumber) ? "" : Param.ContractNumber),
					new SqlParameter("@ContractRolloverNo", string.IsNullOrWhiteSpace(Param.ContractRolloverNo) ? "" : Param.ContractRolloverNo),
					new SqlParameter("@ContractsStatus", string.IsNullOrWhiteSpace(Param.ContractsStatus) ? "" : String.Format("'{0}'", Param.ContractsStatus)),
					new SqlParameter("@GroupType", GroupType)
					).ToList();
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public List<SecurityContractViewModel> GetContractOfSecurityItem(string securityID, string securityListLevel2)
		{
			using (var db = new MainDbContext())
			{
				List<SecurityContractViewModel> securityContract = new List<SecurityContractViewModel>();
				securityContract = db.Database.SqlQuery<SecurityContractViewModel>(
				"exec GetContractDetails @SecurityID,@SecurityListLevel2",
				new SqlParameter("@SecurityID", string.IsNullOrWhiteSpace(securityID) ? "" : securityID),
				new SqlParameter("@SecurityListLevel2", string.IsNullOrWhiteSpace(securityListLevel2) ? "" : securityListLevel2)
				).ToList();

				foreach (var contract in securityContract)
				{
					var CustomerName = _clsCRM.getClientNameByCode(contract.ContractsCustomer);
					contract.ContractsCustomerName = CustomerName;
				}
				return securityContract;
			}
		}

		public RedirectToPageModel GetPageURlBySecurityListLevel2Code(string Level2Code, string GroupType, string Status)
		{
			glog.Debug("GetPageURlBySecurityListLevel2Code: Entry");
			using (var db = new MainDbContext())
			{
				var redirectToPageModel = db.Database.SqlQuery<RedirectToPageModel>(
				"exec GetRedirectionUrl @Level2Code, @GroupType, @Status",
				new SqlParameter("@Level2Code", string.IsNullOrWhiteSpace(Level2Code) ? "" : Level2Code),
				new SqlParameter("@GroupType", string.IsNullOrWhiteSpace(GroupType) ? "" : GroupType),
				new SqlParameter("@Status", string.IsNullOrWhiteSpace(Status) ? "" : Status)
				).FirstOrDefault();
				glog.Debug("GetPageURlBySecurityListLevel2Code: Exit");

				return redirectToPageModel;
			}
		}

		public SecurityPropertyViewModel GetSecurityPropertyDetails(string SecurityID, string mortagatorTbl)
		{
			SecurityPropertyViewModel model = new SecurityPropertyViewModel();
			model.PropertyDetails = new PropertyDetailsModel();
			model.Mortgagor = new List<MortgagorModel>();
			model.ContractDetails = new List<SecurityContractViewModel>();
			model.FieldChangeHistory = new List<FieldChangeHistoryModel>();
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var property = db.Security_Property.FirstOrDefault(s => s.ID == SecurityID);
						if (property != null)
						{

							model.Id = property.ID;
							model.PropertyAddress = property.PropertyAddress;
							model.PropertyDetails.PropertyTypeLevel1 = property.PropertyTypeLevel1;
							model.PropertyDetails.PropertyTypeLevel2 = property.PropertyTypeLevel2;
							model.PropertyDetails.PartyType = property.FirstThirdParty;
							model.PropertyDetails.FormalValuation = property.FormalValuation != null ? Math.Round((decimal)property.FormalValuation, 2) : (decimal)0.00;
							model.PropertyDetails.IndicativeValuation = property.IndicativeValuation != null ? Math.Round((decimal)property.IndicativeValuation, 2) : (decimal)0.00;
							model.PropertyDetails.CreditLimit = property.CreditLimit != null ? Math.Round((decimal)property.CreditLimit, 2) : (decimal)0.00;
							model.PropertyDetails.FormalValuationOld = property.FormalValuation != null ? Math.Round((decimal)property.FormalValuation, 2) : (decimal)0.00;
							model.PropertyDetails.IndicativeValuationOld = property.IndicativeValuation != null ? Math.Round((decimal)property.IndicativeValuation, 2) : (decimal)0.00;
							model.PropertyDetails.CreditLimitOld = property.CreditLimit != null ? Math.Round((decimal)property.CreditLimit, 2) : (decimal)0.00;
							model.PropertyDetails.TitleNumber = property.TitleNumber;
							model.PropertyDetails.MortgagorNumber = property.MortgageNumber;
							model.PropertyDetails.ChangeNumber = property.ChargeNumber;
							model.PropertyDetails.ChargeDate = property.ChargeDate != null ? Convert.ToDateTime(property.ChargeDate).ToString("dd/MM/yyyy") : "";
							model.CreatedBy = property.CreatedBy;
							model.CreatedDate = property.CreatedDate != null ? Convert.ToDateTime(property.CreatedDate).ToString("dd/MM/yyyy") : "";
							model.UpdatedBy = property.UpdatedBy;
							model.UpdatedDate = property.UpdatedDate != null ? Convert.ToDateTime(property.UpdatedDate).ToString("dd/MM/yyyy") : "";
							model.GroupCode = property.Status;
							model.Status = property.Status;
						}

						var mortgagor = GetSecurityMortgator(db, mortagatorTbl, SecurityID);
						model.Mortgagor = mortgagor;
						var customerToAccess = new List<CustomerToAccess>();
						var customer = db.Security_PropertyCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
						if (customer != null)
						{
							customerToAccess = customer.Select(x => new CustomerToAccess()
							{
								IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								Customer = x.Customer,
								CustomerName = _clsCRM.getClientNameByCode(x.Customer),
								ItemNumber = x.ItemNumber,
							}).ToList();
						}
						model.CustomerToAccess = customerToAccess;

						model.ContractDetails = GetContractOfSecurityItem(SecurityID, property.SecurityListLevel2);

						var fieldHistory = _clsGlobal.GetFieldChangeHistory(db, "Security_Property", "ID", SecurityID);
						var fieldChangeHistory = new List<FieldChangeHistoryModel>();
						if (fieldHistory != null)
						{
							fieldChangeHistory = fieldHistory.Select(x => new FieldChangeHistoryModel()
							{
								FieldName = x.FieldName,
								FieldValue = x.FieldValue,
								UpdatedDate = x.UpdatedDate != null ? Convert.ToDateTime(x.UpdatedDate).ToString("dd/MM/yyyy") : "",
								UpdatedBy = x.UpdatedBy,
							}).ToList();
						}

						model.FieldChangeHistory = fieldChangeHistory;
					}
					catch (Exception ex)
					{
						glog.Error("GetSecurityPropertyDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{

						transaction.Dispose();
					}
				}
			}
			return model;
		}

		public VesselModel GetSecurityVesselDetails(string SecurityID, string mortagatorTbl)
		{
			VesselModel model = new VesselModel();
			model.VesselDetails = new VesselDetailModel();
			model.Mortgagor = new List<MortgagorModel>();
			model.ContractDetails = new List<SecurityContractViewModel>();
			model.FieldChangeHistory = new List<FieldChangeHistoryModel>();
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var vessel = db.Security_Vessel.FirstOrDefault(s => s.ID == SecurityID);
						if (vessel != null)
						{
							model.VesselDetails = new VesselDetailModel();
							model.Id = vessel.ID;
							model.HullNumber = vessel.HullNumber;
							model.VesselName = vessel.VesselName;

							model.VesselDetails.CountryofRegistration = vessel.CountryOfReg;
							model.VesselDetails.MortgageNumber = vessel.MortgageNumber;
							model.VesselDetails.ChargeNumber = vessel.ChargeNumber;
							model.VesselDetails.FormalValuation = vessel.FormalValuation != null ? Math.Round((decimal)vessel.FormalValuation, 2) : (decimal)0.00;
							model.VesselDetails.IndicativeValuation = vessel.IndicativeValuation != null ? Math.Round((decimal)vessel.IndicativeValuation, 2) : (decimal)0.00;
							model.VesselDetails.CreditLimit = vessel.CreditLimit != null ? Math.Round((decimal)vessel.CreditLimit, 2) : (decimal)0.00;
							model.VesselDetails.FormalValuationOld = vessel.FormalValuation != null ? Math.Round((decimal)vessel.FormalValuation, 2) : (decimal)0.00;
							model.VesselDetails.IndicativeValuationOld = vessel.IndicativeValuation != null ? Math.Round((decimal)vessel.IndicativeValuation, 2) : (decimal)0.00;
							model.VesselDetails.CreditLimitOld = vessel.CreditLimit != null ? Math.Round((decimal)vessel.CreditLimit, 2) : (decimal)0.00;
							model.VesselDetails.ChargeDate = vessel.ChargeDate != null ? Convert.ToDateTime(vessel.ChargeDate).ToString("dd/MM/yyyy") : "";
							model.CreatedBy = vessel.CreatedBy;
							model.CreatedDate = vessel.CreatedDate != null ? Convert.ToDateTime(vessel.CreatedDate).ToString("dd/MM/yyyy") : "";
							model.UpdatedBy = vessel.UpdatedBy;
							model.UpdatedDate = vessel.UpdatedDate != null ? Convert.ToDateTime(vessel.UpdatedDate).ToString("dd/MM/yyyy") : "";
							model.Status = vessel.Status;
						}

						var mortgagor = GetSecurityMortgator(db, mortagatorTbl, SecurityID);
						model.Mortgagor = mortgagor;
						var insuranceDetail = new List<InsuranceDetail>();
						var insauranceType = _clsGlobal.GetListOfValue("VESSEL_INSURANCE_TYPE", "", "O", "", "").ToList();
						var insurance = db.Security_VesselInsurance.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
						if (insurance != null)
						{
							insuranceDetail = insurance.Select(x => new InsuranceDetail()
							{
								InsuranceType = x.InsuranceType,
								InsuranceTypeDisplay = insauranceType.Where(w => w.Value == x.InsuranceType).Select(s => s.Text).FirstOrDefault().ToString(),
								ExpiryDate = x.ExpiryDate != null ? Convert.ToDateTime(x.ExpiryDate).ToString("dd/MM/yyyy") : "",
								Insurance = insauranceType,
								ItemNumber = x.ItemNumber
							}).ToList();
						}
						model.InsuranceDetail = insuranceDetail;
						var customerToAccess = new List<CustomerToAccess>();
						var customer = db.Security_VesselCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
						if (customer != null)
						{
							customerToAccess = customer.Select(x => new CustomerToAccess()
							{
								IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								Customer = x.Customer,
								CustomerName = _clsCRM.getClientNameByCode(x.Customer),
							}).ToList();
						}
						model.CustomerToAccess = customerToAccess;
						var fieldHistory = _clsGlobal.GetFieldChangeHistory(db, "Security_Vessel", "ID", SecurityID);
						var fieldChangeHistory = new List<FieldChangeHistoryModel>();
						if (fieldHistory != null)
						{
							fieldChangeHistory = fieldHistory.Select(x => new FieldChangeHistoryModel()
							{
								FieldName = x.FieldName,
								FieldValue = x.FieldValue,
								UpdatedDate = x.UpdatedDate != null ? Convert.ToDateTime(x.UpdatedDate).ToString("dd/MM/yyyy") : "",
								UpdatedBy = x.UpdatedBy,
							}).ToList();
						}

						model.FieldChangeHistory = fieldChangeHistory;
						model.ContractDetails = GetContractOfSecurityItem(SecurityID, vessel.SecurityListLevel2);
					}
					catch (Exception ex)
					{
						glog.Error("GetSecurityPropertyDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{

						transaction.Dispose();
					}
				}
			}
			return model;
		}

		public VehicleModel GetSecurityVehicleDetails(string SecurityID)
		{
			VehicleModel model = new VehicleModel();

			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var vehicle = db.Security_Vehicle.FirstOrDefault(s => s.ID == SecurityID);
						if (vehicle != null)
						{
							model.VehicleDetails = new VehicleDetailModel();
							model.Id = vehicle.ID;
							model.ChassisNumber = vehicle.ChassisNumber;
							model.VehicleRegNumber = vehicle.RegNumber;

							model.VehicleDetails.VehicleMakeId = vehicle.VehicleMake;
							model.VehicleDetails.VehicleModelId = vehicle.VehicleModel;
							model.VehicleDetails.VehicleType = vehicle.VehicleType;
							model.VehicleDetails.COEExpiryDate = vehicle.COE_ExpiryDate != null ? Convert.ToDateTime(vehicle.COE_ExpiryDate).ToString("dd/MM/yyyy") : "";
							model.VehicleDetails.Value = vehicle.Value != null ? Math.Round((decimal)vehicle.Value, 2) : (decimal)0.00;
							model.VehicleDetails.ChargeDate = vehicle.ChargeDate != null ? Convert.ToDateTime(vehicle.ChargeDate).ToString("dd/MM/yyyy") : "";
							model.VehicleDetails.ChargeNumber = vehicle.ChargeNumber;
							model.VehicleDetails.EngineNumber = vehicle.EngineNumber;
							model.CreatedBy = vehicle.CreatedBy;
							model.CreatedDate = vehicle.CreatedDate != null ? Convert.ToDateTime(vehicle.CreatedDate).ToString("dd/MM/yyyy") : "";
							model.UpdatedBy = vehicle.UpdatedBy;
							model.UpdatedDate = vehicle.UpdatedDate != null ? Convert.ToDateTime(vehicle.UpdatedDate).ToString("dd/MM/yyyy") : "";
							model.Status = vehicle.Status;
						}

						var customerToAccess = new List<CustomerToAccess>();
						var customer = db.Security_VehicleCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
						if (customer != null)
						{
							customerToAccess = customer.Select(x => new CustomerToAccess()
							{
								IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								Customer = x.Customer,
								CustomerName = _clsCRM.getClientNameByCode(x.Customer),
							}).ToList();
						}
						model.CustomerToAccess = customerToAccess;
						model.ContractDetails = GetContractOfSecurityItem(SecurityID, vehicle.SecurityListLevel2);
					}
					catch (Exception ex)
					{
						glog.Error("GetSecurityVehicleDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
				}
			}
			return model;
		}

		public EquipmentModel GetEquipmentDetails(string SecurityID, bool IsConstructor)
		{
			EquipmentModel model = new EquipmentModel();
			var customerToAccess = new List<CustomerToAccess>();
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						string SecurityLevel2Code = string.Empty;
						model.EquipmentDetails = new EquipmentDetailModel();
						if (IsConstructor)
						{
							var equipment = db.Security_ConstructionEquip.FirstOrDefault(s => s.ID == SecurityID);
							if (equipment != null)
							{
								model.Id = equipment.ID;
								model.Brand = equipment.EquipBrand;
								model.Model = equipment.EquipModel;
								model.EquipmentDescription = equipment.EquipDesc;
								model.CreatedBy = equipment.CreatedBy;
								model.CreatedDate = equipment.CreatedDate != null ? Convert.ToDateTime(equipment.CreatedDate).ToString("dd/MM/yyyy") : "";
								model.UpdatedBy = equipment.UpdatedBy;
								model.UpdatedDate = equipment.UpdatedDate != null ? Convert.ToDateTime(equipment.UpdatedDate).ToString("dd/MM/yyyy") : "";
								model.Status = equipment.Status;
								model.EquipmentDetails.SerialNumber = equipment.SerialNumber;
								model.EquipmentDetails.SecuredAmount = equipment.SecuredAmount != null ? Math.Round((decimal)equipment.SecuredAmount, 2) : (decimal)0.00;
								model.EquipmentDetails.ManufactureYear = equipment.YearOfManufacture;
								model.EquipmentDetails.EngineNumber = equipment.EngineNumber;
								model.EquipmentDetails.ChargeNumber = equipment.ChargeNumber;
								model.EquipmentDetails.ChargeDate = equipment.ChargeDate != null ? Convert.ToDateTime(equipment.ChargeDate).ToString("dd/MM/yyyy") : "";
								SecurityLevel2Code = equipment.SecurityListLevel2;
							}
							var customer = db.Security_ConstructionEquipCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
							if (customer != null)
							{
								customerToAccess = customer.Select(x => new CustomerToAccess()
								{
									IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
									Customer = x.Customer,
									CustomerName = _clsCRM.getClientNameByCode(x.Customer),
								}).ToList();
							}
						}
						else
						{
							var equipment = db.Security_IndustrialEquip.FirstOrDefault(s => s.ID == SecurityID);
							if (equipment != null)
							{
								model.Id = equipment.ID;
								model.Brand = equipment.EquipBrand;
								model.Model = equipment.EquipModel;
								model.EquipmentDescription = equipment.EquipDesc;
								model.CreatedBy = equipment.CreatedBy;
								model.CreatedDate = equipment.CreatedDate != null ? Convert.ToDateTime(equipment.CreatedDate).ToString("dd/MM/yyyy") : "";
								model.UpdatedBy = equipment.UpdatedBy;
								model.UpdatedDate = equipment.UpdatedDate != null ? Convert.ToDateTime(equipment.UpdatedDate).ToString("dd/MM/yyyy") : "";
								model.Status = equipment.Status;
								model.EquipmentDetails.SerialNumber = equipment.SerialNumber;
								model.EquipmentDetails.SecuredAmount = equipment.SecuredAmount != null ? Math.Round((decimal)equipment.SecuredAmount, 2) : (decimal)0.00;
								model.EquipmentDetails.ManufactureYear = equipment.YearOfManufacture;
								model.EquipmentDetails.EngineNumber = equipment.EngineNumber;
								model.EquipmentDetails.ChargeNumber = equipment.ChargeNumber;
								model.EquipmentDetails.ChargeDate = equipment.ChargeDate != null ? Convert.ToDateTime(equipment.ChargeDate).ToString("dd/MM/yyyy") : "";
								SecurityLevel2Code = equipment.SecurityListLevel2;
							}
							var customer = db.Security_IndustrialEquipCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
							if (customer != null)
							{
								customerToAccess = customer.Select(x => new CustomerToAccess()
								{
									IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
									Customer = x.Customer,
									CustomerName = _clsCRM.getClientNameByCode(x.Customer),
								}).ToList();
							}
						}

						model.CustomerToAccess = customerToAccess;
						model.ContractDetails = GetContractOfSecurityItem(SecurityID, SecurityLevel2Code);

					}
					catch (Exception ex)
					{
						glog.Error("GetSecurityPropertyDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{

						transaction.Dispose();
					}
				}
			}
			return model;
		}

		public InventoryModel GetSecurityInventoryDetails(string SecurityID)
		{
			InventoryModel model = new InventoryModel();
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var inventory = db.Security_Inventory.FirstOrDefault(s => s.ID == SecurityID);
						if (inventory != null)
						{
							model.Id = inventory.ID;
							model.Type = inventory.Type;
							model.Value = Convert.ToDecimal(inventory.Value.ToString("0.00"));
							model.ChargeNumber = inventory.ChargeNumber;
							model.ChargeDate = inventory.ChargeDate != null ? Convert.ToDateTime(inventory.ChargeDate).ToString("dd/MM/yyyy") : "";
							model.CreatedBy = inventory.CreatedBy;
							model.CreatedDate = inventory.CreatedDate != null ? Convert.ToDateTime(inventory.CreatedDate).ToString("dd/MM/yyyy") : "";
							model.UpdatedBy = inventory.UpdatedBy;
							model.UpdatedDate = inventory.UpdatedDate != null ? Convert.ToDateTime(inventory.UpdatedDate).ToString("dd/MM/yyyy") : "";
							model.Status = inventory.Status;
						}

						var customerToAccess = new List<CustomerToAccess>();
						var customer = db.Security_InventoryCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
						if (customer != null)
						{
							customerToAccess = customer.Select(x => new CustomerToAccess()
							{
								IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								Customer = x.Customer,
								CustomerName = _clsCRM.getClientNameByCode(x.Customer),
							}).ToList();
						}

						model.CustomerToAccess = customerToAccess;
						model.ContractDetails = GetContractOfSecurityItem(SecurityID, inventory.SecurityListLevel2);

					}
					catch (Exception ex)
					{
						glog.Error("GetSecurityPropertyDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{

						transaction.Dispose();
					}
				}
			}
			return model;
		}

		public ReceivableModel GetDebentureReceivablesDetails(string SecurityID)
		{
			ReceivableModel model = new ReceivableModel();
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var receivable = db.Security_Receivable.FirstOrDefault(s => s.ID == SecurityID);
						if (receivable != null)
						{
							model.Id = receivable.ID;
							model.Amount = Convert.ToDecimal(receivable.Amount.ToString("0.00"));
							model.ChargeNumber = receivable.ChargeNumber;
							model.ChargeDate = receivable.ChargeDate != null ? Convert.ToDateTime(receivable.ChargeDate).ToString("dd/MM/yyyy") : "";
							model.CreatedBy = receivable.CreatedBy;
							model.CreatedDate = receivable.CreatedDate != null ? Convert.ToDateTime(receivable.CreatedDate).ToString("dd/MM/yyyy") : "";
							model.UpdatedBy = receivable.UpdatedBy;
							model.UpdatedDate = receivable.UpdatedDate != null ? Convert.ToDateTime(receivable.UpdatedDate).ToString("dd/MM/yyyy") : "";

							model.Status = receivable.Status;
						}

						var customerToAccess = new List<CustomerToAccess>();
						var customer = db.Security_ReceivableCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
						if (customer != null)
						{
							customerToAccess = customer.Select(x => new CustomerToAccess()
							{
								IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								Customer = x.Customer,
								CustomerName = _clsCRM.getClientNameByCode(x.Customer),
							}).ToList();
						}

						model.CustomerToAccess = customerToAccess;
						model.ContractDetails = GetContractOfSecurityItem(SecurityID, receivable.SecurityListLevel2);

					}
					catch (Exception ex)
					{
						glog.Error("GetSecurityPropertyDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{

						transaction.Dispose();
					}
				}
			}
			return model;
		}

		public CashEquivalentModel GetCashAndEquivalentIndividualDetails(string SecurityID)
		{
			CashEquivalentModel model = new CashEquivalentModel();
			model.BillToModel = new BillToModel();
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var individual = db.Security_CashEquivalentInd.FirstOrDefault(s => s.ID == SecurityID);
						if (individual != null)
						{
							model.Id = individual.ID;
							model.Amount = Convert.ToDecimal(individual.Amount.ToString("0.00"));
							model.Refundable = individual.Refundable.ToUpper() == "Y" ? "Yes" : "No";
							model.GuaranteeBondsType = individual.GuaranteeBondsType;
							model.CreatedBy = individual.CreatedBy;
							model.CreatedDate = individual.CreatedDate != null ? Convert.ToDateTime(individual.CreatedDate).ToString("dd/MM/yyyy") : "";
							model.UpdatedBy = individual.UpdatedBy;
							model.UpdatedDate = individual.UpdatedDate != null ? Convert.ToDateTime(individual.UpdatedDate).ToString("dd/MM/yyyy") : "";
							model.Status = individual.Status;
							model.BillToModel.Customer = individual.BillToCustomer;
							model.BillToModel.NricFinPassport = "0.00";
							model.BillToModel.Address = individual.BillToAddress;
							model.BillToModel.Department = individual.BillToDept;
							model.BillToModel.ContactPerson = individual.BillToConPerson;
							model.BillToModel.CustomerName = _clsCRM.getClientNameByCode(individual.BillToCustomer);
						}

						var customerToAccess = new List<CustomerToAccess>();
						var customer = db.Security_CashEquivalentIndCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
						if (customer != null)
						{
							customerToAccess = customer.Select(x => new CustomerToAccess()
							{
								IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								Customer = x.Customer,
								CustomerName = _clsCRM.getClientNameByCode(x.Customer),
							}).ToList();
						}

						model.CustomerToAccess = customerToAccess;
						model.ContractDetails = GetContractOfSecurityItem(SecurityID, individual.SecurityListLevel2);
					}
					catch (Exception ex)
					{
						glog.Error("GetSecurityPropertyDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{

						transaction.Dispose();
					}
				}
			}
			return model;
		}

		public CashEquivalentModel GetCashAndEquivalentCompanyDetails(string SecurityID)
		{
			CashEquivalentModel model = new CashEquivalentModel();
			model.BillToModel = new BillToModel();
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var company = db.Security_CashEquivalentCom.FirstOrDefault(s => s.ID == SecurityID);
						if (company != null)
						{
							model.Id = company.ID;
							model.Amount = Convert.ToDecimal(company.Amount.ToString("0.00"));
							model.Refundable = company.Refundable.ToUpper() == "Y" ? "Yes" : "No"; ;
							model.GuaranteeBondsType = company.GuaranteeBondsType;
							model.CreatedBy = company.CreatedBy;
							model.CreatedDate = company.CreatedDate != null ? Convert.ToDateTime(company.CreatedDate).ToString("dd/MM/yyyy") : "";
							model.UpdatedBy = company.UpdatedBy;
							model.UpdatedDate = company.UpdatedDate != null ? Convert.ToDateTime(company.UpdatedDate).ToString("dd/MM/yyyy") : "";
							model.Status = company.Status;
							model.BillToModel.Customer = company.BillToCustomer;
							model.BillToModel.NricFinPassport = "";
							model.BillToModel.Address = company.BillToAddress;
							model.BillToModel.Department = company.BillToDept;
							model.BillToModel.ContactPerson = company.BillToConPerson;
							model.BillToModel.CustomerName = _clsCRM.getClientNameByCode(company.BillToCustomer);
						}

						var customerToAccess = new List<CustomerToAccess>();
						var customer = db.Security_CashEquivalentComCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
						if (customer != null)
						{
							customerToAccess = customer.Select(x => new CustomerToAccess()
							{
								IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								Customer = x.Customer,
								CustomerName = _clsCRM.getClientNameByCode(x.Customer),
							}).ToList();
						}

						model.CustomerToAccess = customerToAccess;
						model.ContractDetails = GetContractOfSecurityItem(SecurityID, company.SecurityListLevel2);
					}
					catch (Exception ex)
					{
						glog.Error("GetCashAndEquivalentCompanyDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
				}
			}
			return model;
		}

		public SecFinInstrumentModel GetSecuritiesOrFinancialInstruments(string SecurityID)
		{
			SecFinInstrumentModel model = new SecFinInstrumentModel();
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var secFinInstrument = db.Security_SecFinInstruments.FirstOrDefault(s => s.ID == SecurityID);
						if (secFinInstrument != null)
						{
							model.Id = secFinInstrument.ID;
							model.FinancialInstrumentType = secFinInstrument.Type;
							model.Amount = Convert.ToDecimal(secFinInstrument.Amount.ToString("0.00"));
							model.SecurityorFinancialInstrumentDetails.DocumentNumber = secFinInstrument.DocumentNumber;
							model.SecurityorFinancialInstrumentDetails.BankNameorFinancialCompany = secFinInstrument.BankFinancialCom;
							model.SecurityorFinancialInstrumentDetails.ChargeDate = secFinInstrument.ChargeDate != null ? Convert.ToDateTime(secFinInstrument.ChargeDate).ToString("dd/MM/yyyy") : "";
							model.CreatedBy = secFinInstrument.CreatedBy;
							model.CreatedDate = secFinInstrument.CreatedDate != null ? Convert.ToDateTime(secFinInstrument.CreatedDate).ToString("dd/MM/yyyy") : "";
							model.UpdatedBy = secFinInstrument.UpdatedBy;
							model.UpdatedDate = secFinInstrument.UpdatedDate != null ? Convert.ToDateTime(secFinInstrument.UpdatedDate).ToString("dd/MM/yyyy") : "";
							model.Status = secFinInstrument.Status;
						}

						var customerToAccess = new List<CustomerToAccess>();
						var customer = db.Security_SecFinInstrumentsCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
						if (customer != null)
						{
							customerToAccess = customer.Select(x => new CustomerToAccess()
							{
								IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								Customer = x.Customer,
								CustomerName = _clsCRM.getClientNameByCode(x.Customer),
							}).ToList();
						}

						model.CustomerToAccess = customerToAccess;
						model.ContractDetails = GetContractOfSecurityItem(SecurityID, secFinInstrument.SecurityListLevel2);
					}
					catch (Exception ex)
					{
						glog.Error("GetSecurityPropertyDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{

						transaction.Dispose();
					}
				}
			}
			return model;
		}

		public SecurityDepositModel GetSecurityDepositDetails(string SecurityID)
		{
			SecurityDepositModel model = new SecurityDepositModel();
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var deposit = db.Security_SecDeposit.FirstOrDefault(s => s.ID == SecurityID);
						if (deposit != null)
						{
							model.Id = deposit.ID;
							model.Amount = Convert.ToDecimal(deposit.Amount.ToString("0.00")); ;
							model.Refundable = deposit.Refundable.ToUpper() == "Y" ? "Yes" : "No"; ;
							model.CreatedBy = deposit.CreatedBy;
							model.CreatedDate = deposit.CreatedDate != null ? Convert.ToDateTime(deposit.CreatedDate).ToString("dd/MM/yyyy") : "";
							model.UpdatedBy = deposit.UpdatedBy;
							model.UpdatedDate = deposit.UpdatedDate != null ? Convert.ToDateTime(deposit.UpdatedDate).ToString("dd/MM/yyyy") : "";
							model.Status = deposit.Status;
							model.BillToDetailModel.IndividualCorporate = deposit.BillToType == "I" ? clsVariables.Individual : clsVariables.Corporate;
							model.BillToDetailModel.Customer = deposit.BillToCustomer;
							model.BillToDetailModel.NRICFINPASSPORT = "";
							model.BillToDetailModel.ROCUEN = "";
							model.BillToDetailModel.Address = deposit.BillToAddress;
							model.BillToDetailModel.Department = deposit.BillToDept;
							model.BillToDetailModel.ContactPerson = deposit.BillToConPerson;
							model.BillToDetailModel.CustomerName = _clsCRM.getClientNameByCode(deposit.BillToCustomer);
						}

						var customerToAccess = new List<CustomerToAccess>();
						var customer = db.Security_SecDepositCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == SecurityID && w.Status == "O");
						if (customer != null)
						{
							customerToAccess = customer.Select(x => new CustomerToAccess()
							{
								IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								Customer = x.Customer,
								CustomerName = _clsCRM.getClientNameByCode(x.Customer),
							}).ToList();
						}

						model.CustomerToAccess = customerToAccess;
						model.ContractDetails = GetContractOfSecurityItem(SecurityID, deposit.SecurityListLevel2);
					}
					catch (Exception ex)
					{
						glog.Error("GetSecurityPropertyDetails Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{

						transaction.Dispose();
					}
				}
			}
			return model;
		}

		public ResultViewModel UpdatePropertyStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdatePropertyStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_Property property = db.Security_Property.Find(Id);
						property.Status = status;
						property.UpdatedBy = userMail;
						property.UpdatedDate = DateTime.Now;
						db.Security_Property.Add(property);
						db.Entry(property).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdatePropertyStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdatePropertyStatus: Exit");
					return result;
				}
			}
		}

		public ResultViewModel UpdateVesselStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdateVesselStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_Vessel vessel = db.Security_Vessel.Find(Id);
						vessel.Status = status;
						vessel.UpdatedBy = userMail;
						vessel.UpdatedDate = DateTime.Now;
						db.Security_Vessel.Add(vessel);
						db.Entry(vessel).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdateVesselStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdateVesselStatus: Exit");
					return result;
				}
			}
		}

		public ResultViewModel UpdateVehicleStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdateVehicleStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_Vehicle vehicle = db.Security_Vehicle.Find(Id);
						vehicle.Status = status;
						vehicle.UpdatedBy = userMail;
						vehicle.UpdatedDate = DateTime.Now;
						vehicle.DischargeBy = userMail;
						vehicle.DischargeDate = DateTime.Now;
						db.Security_Vehicle.Add(vehicle);
						db.Entry(vehicle).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdateVehicleStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdateVehicleStatus: Exit");
					return result;
				}
			}
		}

		public ResultViewModel UpdateReceivableStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdateReceivableStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_Receivable receivable = db.Security_Receivable.Find(Id);
						receivable.Status = status;
						receivable.UpdatedBy = userMail;
						receivable.UpdatedDate = DateTime.Now;
						receivable.DischargeBy = userMail;
						receivable.DischargeDate = DateTime.Now;
						db.Security_Receivable.Add(receivable);
						db.Entry(receivable).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdateReceivableStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdateReceivableStatus: Exit");
					return result;
				}
			}
		}

		public ResultViewModel UpdateCashAndEquivalentIndStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdateCashAndEquivalentIndStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_CashEquivalentInd CashEquivalentInd = db.Security_CashEquivalentInd.Find(Id);
						CashEquivalentInd.Status = status;
						CashEquivalentInd.UpdatedBy = userMail;
						CashEquivalentInd.UpdatedDate = DateTime.Now;
						CashEquivalentInd.DischargeBy = userMail;
						CashEquivalentInd.DischargeDate = DateTime.Now;
						db.Security_CashEquivalentInd.Add(CashEquivalentInd);
						db.Entry(CashEquivalentInd).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdateCashAndEquivalentIndStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdateCashAndEquivalentIndStatus: Exit");
					return result;
				}
			}
		}

		public ResultViewModel UpdateCashAndEquivalentComStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdateCashAndEquivalentComStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_CashEquivalentCom CashEquivalentCom = db.Security_CashEquivalentCom.Find(Id);
						CashEquivalentCom.Status = status;
						CashEquivalentCom.UpdatedBy = userMail;
						CashEquivalentCom.UpdatedDate = DateTime.Now;
						CashEquivalentCom.DischargeBy = userMail;
						CashEquivalentCom.DischargeDate = DateTime.Now;
						db.Security_CashEquivalentCom.Add(CashEquivalentCom);
						db.Entry(CashEquivalentCom).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdateCashAndEquivalentComStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdateCashAndEquivalentComStatus: Exit");
					return result;
				}
			}
		}

		public ResultViewModel UpdateSecFinInstrumentsStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdateReceivableStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_SecFinInstruments receivable = db.Security_SecFinInstruments.Find(Id);
						receivable.Status = status;
						receivable.UpdatedBy = userMail;
						receivable.UpdatedDate = DateTime.Now;
						receivable.DischargeBy = userMail;
						receivable.DischargeDate = DateTime.Now;
						db.Security_SecFinInstruments.Add(receivable);
						db.Entry(receivable).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdateReceivableStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdateReceivableStatus: Exit");
					return result;
				}
			}
		}

		public ResultViewModel UpdateInventoryStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdateInventoryStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_Inventory inventory = db.Security_Inventory.Find(Id);
						inventory.Status = status;
						inventory.UpdatedBy = userMail;
						inventory.UpdatedDate = DateTime.Now;
						inventory.DischargeBy = userMail;
						inventory.DischargeDate = DateTime.Now;
						db.Security_Inventory.Add(inventory);
						db.Entry(inventory).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdateInventoryStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdateInventoryStatus: Exit");
					return result;
				}
			}
		}

		public ResultViewModel UpdateConstructionStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdateConstructionStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_ConstructionEquip Construction = db.Security_ConstructionEquip.Find(Id);
						Construction.Status = status;
						Construction.UpdatedBy = userMail;
						Construction.UpdatedDate = DateTime.Now;
						Construction.DischargeBy = userMail;
						Construction.DischargeDate = DateTime.Now;
						db.Security_ConstructionEquip.Add(Construction);
						db.Entry(Construction).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdateConstructionStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdateConstructionStatus: Exit");
					return result;
				}
			}
		}

		public ResultViewModel UpdateIndustrialStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdateIndustrialStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_IndustrialEquip industrial = db.Security_IndustrialEquip.Find(Id);
						industrial.Status = status;
						industrial.UpdatedBy = userMail;
						industrial.UpdatedDate = DateTime.Now;
						industrial.DischargeBy = userMail;
						industrial.DischargeDate = DateTime.Now;
						db.Security_IndustrialEquip.Add(industrial);
						db.Entry(industrial).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdateIndustrialStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdateIndustrialStatus: Exit");
					return result;
				}
			}
		}

		public ResultViewModel UpdateSecurityDepositStatus(string status, string Id, string UserName)
		{
			string userMail = "";
			glog.Debug("UpdateSecurityDepositStatus: Entry");
			var result = new ResultViewModel();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						userMail = db.Sys_Users.FirstOrDefault(x => x.Name == UserName).Email;
						Security_SecDeposit SecDeposit = db.Security_SecDeposit.Find(Id);
						SecDeposit.Status = status;
						SecDeposit.UpdatedBy = userMail;
						SecDeposit.UpdatedDate = DateTime.Now;
						SecDeposit.DischargeBy = userMail;
						SecDeposit.DischargeDate = DateTime.Now;
						db.Security_SecDeposit.Add(SecDeposit);
						db.Entry(SecDeposit).State = System.Data.Entity.EntityState.Modified;
						var id = db.SaveChanges();

						transaction.Commit();
						if (id > 0)
						{
							result.Status = 1;
							result.Message = "Status updated successfully.";
						}
						else
						{
							result.Status = 0;
							result.Message = "Some error occured when updating status";
						}
					}
					catch (Exception ex)
					{
						glog.Error("UpdateSecurityDepositStatus Exception: " + ex.Message + ex.InnerException);
						result.Status = 0;
						result.Message = "Please contact MIS, error: " + ex.Message;
						transaction.Rollback();
					}
					finally
					{
						transaction.Dispose();
					}
					glog.Debug("UpdateSecurityDepositStatus: Exit");
					return result;
				}
			}
		}

		private List<MortgagorModel> GetSecurityMortgator(MainDbContext db, string mortgator, string SecurityID)
		{
			List<MortgagorModel> model = new List<MortgagorModel>();

			SecurityMortagorTable mortgatorTable = (SecurityMortagorTable)Enum.Parse(typeof(SecurityMortagorTable), mortgator);
			switch (mortgatorTable)
			{
				case SecurityMortagorTable.Security_PropertyMortgagor:
					{
						var propertyMortgagor = db.Security_PropertyMortgagor.OrderBy(o => o.ItemNumber).Where(s => s.MasterID == SecurityID && s.Status == "O").ToList();
						if (propertyMortgagor != null)
						{
							var mortgagor = propertyMortgagor.Select(x => new MortgagorModel()
							{
								IndividualCorporate = x.MortgagorType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								ItemNumber = x.ItemNumber,
								Mortgagor = x.Mortgagor,
								MortgagorDisplay = _clsCRM.GetCRMProfileByClientId(x.Mortgagor),
								MainType = x.MainMortgagor,
								MainDisplay = x.MainMortgagor == "Y" ? "Main" : "Secondary",
								NRICType = x.MortgagorType == "I" ? _clsCRM.getNricFinPassportType(x.Mortgagor) : string.Empty,
								ROCType = x.MortgagorType == "C" ? _clsCRM.getRocUenType(x.Mortgagor) : string.Empty,
								Address = x.MortgagorAddress,
								AddressDisplay = x.Mortgagor != null ? _clsCRM.getAddress(x.Mortgagor, x.MortgagorType == "I" ? clsVariables.Individual : clsVariables.Corporate).Select(s => s.Address).FirstOrDefault().ToString() : "",
								Department = x.MortgagorDept,
								DepartmentDisplay = x.MortgagorAddress != null ? _clsCRM.getDepartmentList(x.MortgagorAddress).Select(s => s.cd_dept_desc).FirstOrDefault().ToString() : "",
								ContactPerson = x.MortgagorConPerson,
								ContactPersonDisplay = x.MortgagorDept != null ? _clsCRM.getContactPerson(x.MortgagorDept).Select(s => s.Contact).FirstOrDefault().ToString() : ""
							}).ToList();

							return mortgagor;
						}
						break;
					}
				case SecurityMortagorTable.Security_VesselMortgagor:
					{
						var propertyMortgagor = db.Security_VesselMortgagor.OrderBy(o => o.ItemNumber).Where(s => s.MasterID == SecurityID && s.Status == "O").ToList();
						if (propertyMortgagor != null)
						{
							var mortgagor = propertyMortgagor.Select(x => new MortgagorModel()
							{
								IndividualCorporate = x.MortgagorType == "I" ? clsVariables.Individual : clsVariables.Corporate,
								ItemNumber = x.ItemNumber,
								Mortgagor = x.Mortgagor,
								MortgagorDisplay = _clsCRM.GetCRMProfileByClientId(x.Mortgagor),
								MainType = x.MainMortgagor,
								MainDisplay = x.MainMortgagor == "Y" ? "Main" : "Secondary",
								NRICType = x.MortgagorType == "I" ? _clsCRM.getNricFinPassportType(x.Mortgagor) : string.Empty,
								ROCType = x.MortgagorType == "C" ? _clsCRM.getRocUenType(x.Mortgagor) : string.Empty,
								Address = x.MortgagorAddress,
								AddressDisplay = x.Mortgagor != null ? _clsCRM.getAddress(x.Mortgagor, x.MortgagorType == "I" ? clsVariables.Individual : clsVariables.Corporate).Select(s => s.Address).FirstOrDefault().ToString() : "",
								Department = x.MortgagorDept,
								DepartmentDisplay = x.MortgagorAddress != null ? _clsCRM.getDepartmentList(x.MortgagorAddress).Select(s => s.cd_dept_desc).FirstOrDefault().ToString() : "",
								ContactPerson = x.MortgagorConPerson,
								ContactPersonDisplay = x.MortgagorDept != null ? _clsCRM.getContactPerson(x.MortgagorDept).Select(s => s.Contact).FirstOrDefault().ToString() : ""
							}).ToList();

							return mortgagor;
						}
						break;
					}
				default: break;
			}
			return model;
		}

		private List<CustomerToAccess> GetExistCustomer(string id, string TableName)
		{
			List<CustomerToAccess> model = new List<CustomerToAccess>();
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					var customerToAccess = new List<CustomerToAccess>();
					SecurityCustomer customerTable = (SecurityCustomer)Enum.Parse(typeof(SecurityCustomer), TableName);
					switch (customerTable)
					{
						case SecurityCustomer.Security_PropertyCustomer:
							{
								var customer = db.Security_PropertyCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						case SecurityCustomer.Security_VesselCustomer:
							{
								var customer = db.Security_VesselCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						case SecurityCustomer.Security_VehicleCustomer:
							{
								var customer = db.Security_VehicleCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						case SecurityCustomer.Security_ConstructionEquipCustomer:
							{
								var customer = db.Security_ConstructionEquipCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						case SecurityCustomer.Security_IndustrialEquipCustomer:
							{
								var customer = db.Security_IndustrialEquipCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						case SecurityCustomer.Security_InventoryCustomer:
							{
								var customer = db.Security_InventoryCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						case SecurityCustomer.Security_ReceivableCustomer:
							{
								var customer = db.Security_ReceivableCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						case SecurityCustomer.Security_CashEquivalentIndCustomer:
							{
								var customer = db.Security_CashEquivalentIndCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						case SecurityCustomer.Security_CashEquivalentComCustomer:
							{
								var customer = db.Security_CashEquivalentComCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						case SecurityCustomer.Security_SecFinInstrumentsCustomer:
							{
								var customer = db.Security_SecFinInstrumentsCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						case SecurityCustomer.Security_SecDepositCustomer:
							{
								var customer = db.Security_SecDepositCustomer.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == id);
								if (customer != null)
								{
									customerToAccess = customer.Select(x => new CustomerToAccess()
									{
										IndividualCorporate = x.CustomerType == "I" ? clsVariables.Individual : clsVariables.Corporate,
										Customer = x.Customer,
										ItemNumber = x.ItemNumber,
										CreatedBy = x.CreatedBy,
										CreatedDate = x.CreatedDate,
										UpdatedBy = x.UpdatedBy,
										UpdatedDate = x.UpdatedDate,
										Status = x.Status,
									}).ToList();
									return customerToAccess;
								}
								break;
							}
						default: break;
					}
				}
			}
			return model;
		}

		private List<Security_PropertyMortgagor> GetSecurityPropertyMortgagor(string masterId)
		{
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					return db.Security_PropertyMortgagor.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == masterId && w.Status == "O").ToList();
				}
			}
		}

		private List<Security_VesselMortgagor> GetSecurityVesselMortgagor(string masterId)
		{
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					return db.Security_VesselMortgagor.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == masterId && w.Status == "O").ToList();
				}
			}
		}

		private List<Security_VesselInsurance> GetSecurityVesselInsurances(string masterId)
		{
			using (var db = new MainDbContext())
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					return db.Security_VesselInsurance.OrderBy(o => o.ItemNumber).ToList().Where(w => w.MasterID == masterId).ToList();
				}
			}
		}
	}
}