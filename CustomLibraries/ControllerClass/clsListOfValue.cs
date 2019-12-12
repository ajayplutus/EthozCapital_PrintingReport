using log4net;
using EthozCapital.CustomLibraries;
using EthozCapital.Data;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthozCapital.CustomLibraries.ControllerClass
{
    public class clsListOfValue
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(clsListOfValue));
        private clsGlobal _clsGlobal;

        public clsListOfValue()
        {
            _clsGlobal = new clsGlobal();
        }

        public ListOfValueDropDownModel LoadDropDownData()
        {
            ListOfValueDropDownModel model = new ListOfValueDropDownModel();
            using (var db = new MainDbContext())
            {
                List<SelectListItem> listExistGroupType = _clsGlobal.GetAllGroupType();
                model.lstGroupType = listExistGroupType;
                
            }
            return model;
        }

        public ResultViewModel CheckGroupData(string GroupType, string GroupCodePrefix)
        {
            var result = new ResultViewModel();
            glog.Debug("CheckGroupData: Entry");
            string UpperGroupCode = GroupCodePrefix.ToUpper();
            string UpperGroupType = GroupType.ToUpper();
            using (var db = new MainDbContext())
            {

                try
                {
                    bool valid = db.Sys_ListOfValue.Any(f => f.GroupType != UpperGroupType);
                    bool invalid_0 = db.Sys_ListOfValue.Any(f => f.GroupType.Equals(UpperGroupType) && f.GroupCode.StartsWith(UpperGroupCode));
                    bool invalid_1 = db.Sys_ListOfValue.Any(f => f.GroupType.Equals(UpperGroupType));
                    bool invalid_2 = db.Sys_ListOfValue.Any(f => f.GroupCode.StartsWith(UpperGroupCode));
                    bool invalid_3 = GroupType.Contains(" ");
                    bool checkPrefix = db.Sys_ListOfValue.Any(f => f.GroupCode.StartsWith(UpperGroupCode));

                    if (valid && !checkPrefix && !invalid_3) 
                    {
                        result.Status = 1;
                        result.Message = "Add new group type successfully!(valid data)";
                        return result;
                    }

                    if (invalid_1 || invalid_0) 
                    {
                        result.Status = 2;
                        result.Message = "New Group Type is already exist!";
                        return result;
                    }

                    if (invalid_2) 
                    {
                        result.Status = 3;
                        result.Message = "New Group Type - Prefix has been used by other Group Type!";
                        return result;
                    }

                    if (invalid_3) 
                    {
                        result.Status = 4;
                        result.Message = "New Group Type is not allowed to contain blank space!";
                        return result;
                    }
                    
                }
                catch (Exception ex)
                {
                    glog.Error("CheckGroupData Exception: " + ex.Message);
                    result.Status = 0;
                    result.Message = "Please contact MIS, error: " + ex.Message;             
                }
                return result;
            }
        }

        public List<ListOfValueList> GetListOfValueList(string GroupType)
        {
            using (var db = new MainDbContext())
            {
                return db.Database.SqlQuery<ListOfValueList>(
                    "exec GetListOfValueList @GroupType",
                    new SqlParameter("@GroupType", string.IsNullOrWhiteSpace(GroupType) ? "" : GroupType)).ToList();
            }
        }

        public void LoadListOfValueDropDownModel(ParentGroupDropDownModel model)
        {
            using (var db = new MainDbContext())
            {
                model.ParentGroupType = _clsGlobal.GetParentGroupType(true);
                    
            }

        }


        
    }
}