using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EthozCapital.CustomLibraries;
using System.Diagnostics;
using log4net;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Newtonsoft.Json;

namespace EthozCapital.Models.ViewModels
{
    public class ListOfValueViewModel
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(ListOfValueViewModel));

        [Key]
        public int Id { get; set; }
        [Required]
        public string GroupCode { get; set; }
        public string GroupType { get; set; }
        public string GroupMemberDesc { get; set; }
        public string ParentID { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string SelectGroupType { get; set; }

    }

    public class ListOfValueDropDownModel
    {
        public List<SelectListItem> lstGroupType { get; set; } //existing group type
    }

    public class ListOfValueListViewModel
    {
        public ListOfValueListViewModel()
        {
            ListOfValueList = new List<ListOfValueList>();
        }

        [Display(Name = "ListOfValueList")]
        public List<ListOfValueList> ListOfValueList { get; set; }
    }

    public class ListOfValueList
    {
        //public int Id { get; set; }
        public string GroupCode { get; set; }
        public string GroupType { get; set; }
        public string ParentID { get; set; }
        public string GroupMemberDesc { get; set; }
        public string ParentGroupMemberDesc { get; set; }
    }

    public class ParentGroupDropDownModel
    {
        public IEnumerable<CommonDropDown> ParentGroupType { get; set; }
        public IEnumerable<CommonDropDown> ParentGroupMemDesc { get; set; }
    }


}