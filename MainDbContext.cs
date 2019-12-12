using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using log4net;

namespace EthozCapital
{
    public class MainDbContext : DbContext
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(MainDbContext));

        public MainDbContext()
            : base("name=DefaultConnection")
        {
            glog.Debug("MainDbContext");
        }

        #region Sys
        public DbSet<Sys_Users> Sys_Users { get; set; }
        public DbSet<Sys_UserGroupMembers> Sys_UserGroupMembers { get; set; }
        public DbSet<Sys_UserGroupMenuAccess> Sys_UserGroupMenuAccess { get; set; }
        public DbSet<Sys_ListOfValue> Sys_ListOfValue { get; set; }
        public DbSet<Sys_TypeMatrixFieldPropertiesMaster> Sys_TypeMatrixFieldPropertiesMaster { get; set; }
        public DbSet<Sys_TypeMatrixFieldProperties> Sys_TypeMatrixFieldProperties { get; set; }
        public DbSet<Sys_Parameters> Sys_Parameters { get; set; }
        public DbSet<Sys_ParameterValue> Sys_ParameterValue { get; set; }
        public DbSet<Sys_TypeMatrixParameterMaster> Sys_TypeMatrixParameterMaster { get; set; }
        public DbSet<Sys_TypeMatrixParameterValue> Sys_TypeMatrixParameterValue { get; set; }
        public DbSet<Sys_TypeMatrixFunctionMaster> Sys_TypeMatrixFunctionMaster { get; set; }
        public DbSet<Sys_TypeMatrixFunctionLogicMaster> Sys_TypeMatrixFunctionLogicMaster { get; set; }
        public DbSet<Sys_TypeMatrixFunction> Sys_TypeMatrixFunction { get; set; }
        public DbSet<Sys_AutoGenerateIdMaster> Sys_AutoGenerateIdMaster { get; set; }
        public DbSet<Sys_AutoGenerateIdChild> Sys_AutoGenerateIdChild { get; set; }
        public DbSet<Sys_FieldChangeHistory> Sys_FieldChangeHistory { get; set; }
        public DbSet<Sys_ContractNumberMapping> Sys_ContractNumberMapping { get; set; }        
        public DbSet<Sys_PageURL> Sys_PageURL { get; set; }
		
		#endregion

		#region Security
		public DbSet<Security_Property> Security_Property { get; set; }
        public DbSet<Security_PropertyMortgagor> Security_PropertyMortgagor { get; set; }
        public DbSet<Security_PropertyCustomer> Security_PropertyCustomer { get; set; }
        public DbSet<Security_Vessel> Security_Vessel { get; set; }
        public DbSet<Security_VesselMortgagor> Security_VesselMortgagor { get; set; }
        public DbSet<Security_VesselInsurance> Security_VesselInsurance { get; set; }
        public DbSet<Security_VesselCustomer> Security_VesselCustomer { get; set; }
        public DbSet<Security_Vehicle> Security_Vehicle { get; set; }
        public DbSet<Security_VehicleCustomer> Security_VehicleCustomer { get; set; }
        public DbSet<Security_ConstructionEquip> Security_ConstructionEquip { get; set; }
        public DbSet<Security_ConstructionEquipCustomer> Security_ConstructionEquipCustomer { get; set; }
        public DbSet<Security_IndustrialEquip> Security_IndustrialEquip { get; set; }
        public DbSet<Security_IndustrialEquipCustomer> Security_IndustrialEquipCustomer { get; set; }
        public DbSet<Security_Inventory> Security_Inventory { get; set; }
        public DbSet<Security_InventoryCustomer> Security_InventoryCustomer { get; set; }
        public DbSet<Security_Receivable> Security_Receivable { get; set; }
        public DbSet<Security_ReceivableCustomer> Security_ReceivableCustomer { get; set; }
        public DbSet<Security_CashEquivalentInd> Security_CashEquivalentInd { get; set; }
        public DbSet<Security_CashEquivalentIndCustomer> Security_CashEquivalentIndCustomer { get; set; }
        public DbSet<Security_CashEquivalentCom> Security_CashEquivalentCom { get; set; }
        public DbSet<Security_CashEquivalentComCustomer> Security_CashEquivalentComCustomer { get; set; }
        public DbSet<Security_SecFinInstruments> Security_SecFinInstruments { get; set; }
        public DbSet<Security_SecFinInstrumentsCustomer> Security_SecFinInstrumentsCustomer { get; set; }
        public DbSet<Security_SecDeposit> Security_SecDeposit { get; set; }
        public DbSet<Security_SecDepositCustomer> Security_SecDepositCustomer { get; set; }
        #endregion

        #region Maintenance
        public DbSet<Maintenance_LEFSInterestCode> Maintenance_LEFSInterestCode { get; set; }
        #endregion

        #region PreContract
        public DbSet<PreContract_Master> PreContract_Master { get; set; }
        public DbSet<PreContract_SecurityList> PreContract_SecurityList { get; set; }
        public DbSet<PreContract_SecurityItem> PreContract_SecurityItem { get; set; }
        public DbSet<PreContract_BuyBackGuarantor> PreContract_BuyBackGuarantor { get; set; }
        public DbSet<PreContract_BuyBackGuarantor_Amount> PreContract_BuyBackGuarantor_Amount { get; set; }
        public DbSet<PreContract_RecourseGuarantor> PreContract_RecourseGuarantor { get; set; }
        public DbSet<PreContract_RecourseGuarantor_Amount> PreContract_RecourseGuarantor_Amount { get; set; }
        #endregion

        #region Contract
        public DbSet<Contract_Master> Contract_Master { get; set; }
        public DbSet<Contract_SecurityItem> Contract_SecurityItem { get; set; }
		#endregion

		#region PaymentModule
		public DbSet<Spotter_Master> Spotter_Master { get; set; }
		public DbSet<Spotter_Detail> Spotter_Detail { get; set; }
		public DbSet<Contract_Spotter> Contract_Spotter { get; set; }
		public DbSet<sys_ProcessLock> sys_ProcessLock { get; set; }
		#endregion

		public DbSet<Cfstb_serial_num> Cfstb_serial_num { get; set; }
        public DbSet<Cfstb_ctr_mas> Cfstb_ctr_mas { get; set; }
        public DbSet<cfstb_ctr_chd> cfstb_ctr_chd { get; set; }

		public DbSet<Sys_Approval> Sys_Approval { get; set; }
		public DbSet<Approval_Process> Approval_Process { get; set; }
		public DbSet<Approval_ProcessDetail> Approval_ProcessDetail { get; set; }
		public DbSet<Approval_ProcessEvent> Approval_ProcessEvent { get; set; }
	}
}