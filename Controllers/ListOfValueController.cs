using EthozCapital.CustomLibraries;
using EthozCapital.CustomLibraries.ControllerClass;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthozCapital.Controllers
{
    public class ListOfValueController : Controller
    {
        private clsGlobal _clsGlobal;
        private clsListOfValue _clsListOfValue;
        private clsPreCon _clsPreCon;
        ListOfValueDropDownModel modelDropDown = new ListOfValueDropDownModel();

        public ListOfValueController()
        {
            _clsGlobal = new clsGlobal();
            _clsListOfValue = new clsListOfValue();
            _clsPreCon = new clsPreCon();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListOfValueNew()
        {
            Session.Clear();

            //ListOfValueViewModel obj = new ListOfValueViewModel();

            modelDropDown = _clsListOfValue.LoadDropDownData();
            ViewBag.DropDownData = modelDropDown;
            
            //List<SelectListItem> lstExistingGroupType = _clsGlobal.GetAllGroupType();
            //ViewData["getExistType"] = lstExistingGroupType;

            ViewBag.GroupDropDownData = GetParentGroupDropDownData();
            
            return View();
        }

        [HttpPost]
        public ActionResult CheckValidGroupType(string GroupType, string GroupCodePrefix)
        {
            if (ModelState.IsValid)
            {
                var result = _clsListOfValue.CheckGroupData(GroupType, GroupCodePrefix);
                var res = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return res;
            }
            else
            {
                return Json(new
                {
                    NotificationTitle = clsGlobal.SwalTitle_Fail,
                    NotificationContent = "Invalid Input",
                    NotificationType = clsGlobal.SwalTitle_Error
                });
            }
        }

        public PartialViewResult ListOfValueListView(string GroupType)
        {
            
            var GroupList = _clsListOfValue.GetListOfValueList(GroupType);

            ListOfValueListViewModel lstTable = new ListOfValueListViewModel();
            lstTable.ListOfValueList.AddRange(GroupList);

            return PartialView("_ListOfValueListView", lstTable);
        }

        public JsonResult GetParentGroupDropDownData()
        {
            ParentGroupDropDownModel model = new ParentGroupDropDownModel();
            _clsListOfValue.LoadListOfValueDropDownModel(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDescByGroupType(string code)
        {
            return Json(_clsGlobal.GetDescByGroupType(code), JsonRequestBehavior.AllowGet);
        }   
    }
}