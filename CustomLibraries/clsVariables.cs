using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthozCapital.CustomLibraries
{
	public static class clsVariables
    {
        #region GroupCode from Sys_ListOfValue
        
        #region Instalment Option: Straight/ Step Up/ Step Down
        public static string setup_InsOpt_Straight = "IO-100"; //Straight        
        #endregion

        #region Rate Option
        public static string setup_RateOpt_Flat = "RO-100"; //Flat   
        #endregion

        #region Mode of Payment
        public static string setup_PaymentMode_Giro = "PM-102"; //Giro
        #endregion

        #region LEFS Interest Type
        public static string setup_IntrestType_Code = "LIT-1000";//Fixed Rate
        #endregion

        #region Security List
        public static string ConstGuarantor = "SLL2-1000";
        public static string ConstMortgageProperty = "SLL2-1001";
        public static string ConstMortgageVessel = "SLL2-1002";
        public static string ConstDebentureVehicle = "SLL2-1003";
        public static string ConstDebentureConstructionEquipment = "SLL2-1004";
        public static string ConstDebentureIndustrialEquipment = "SLL2-1005";
        public static string ConstDebentureInventories = "SLL2-1006";
        public static string ConstDebentureReceivables = "SLL2-1007";
        public static string CashEquivalentIndividual = "SLL2-1008";
        public static string CashEquivalentCompany = "SLL2-1009";
        public static string SecFinInstruments = "SLL2-1010";
        public static string SecurityDeposit = "SLL2-1011";
        #endregion

        #endregion

        #region Logic Code from Sys_TypeMatrixFunctionLogicMaster
        
        #region Contract schedule calculation
        public static string logic_RunSche_ISS = "L-10000";//Interest Servicing Loan
        public static string logic_RunSche_NonSpringOthers = "L-10001";//Non Spring
        public static string logic_RunSche_Spring = "L-10002";//Spring
        public static string logic_RunSche_NonSpringLeasing = "L-10005";//Non Spring Leasing
        #endregion

        #region Contract maturity date default value
        public static string logic_MaturityDate_ISS = "L-10003";//Interest Servicing Loan
        #endregion

        #region Contract renewal months default value
        public static string logic_RenewalMths_ISS = "L-10004";//Interest Servicing Loan
        #endregion
        
        #endregion

        #region Others
        public static string Individual = "Individual";
        public static string Corporate = "Corporate";
		#endregion

		#region Payment Module
		public static string SpotterMailSubject = "Spotter Fee Payments for your Approval";
		public static string SpotterMailType = "SP";

		#endregion

		#region Approval Module
		public static string ApprovalMailType = "AP";
		public static string Reject = "Reject";
		public static string Approved = "Approved";

		#endregion
	}
}