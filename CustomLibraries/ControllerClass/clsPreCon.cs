using EthozCapital.CustomLibraries;
using EthozCapital.Data;
using EthozCapital.Models;
using EthozCapital.Models.Tables;
using EthozCapital.Models.ViewModels;
using log4net;
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
    public class clsPreCon
    {
        private static ILog glog = log4net.LogManager.GetLogger(typeof(clsPreCon));
        private clsGlobal _clsGlobal;
        private clsAsset _clsAsset;
        private clsCRM _clsCRM;
        private clsContractGeneral _clsContractGeneral;

        public clsPreCon()
        {
            _clsGlobal = new clsGlobal();
            _clsAsset = new clsAsset();
            _clsCRM = new clsCRM();
            _clsContractGeneral = new clsContractGeneral();
        }

        #region Standard Function
        //Load Field Properties (Enabled, Visible, Mandatory, Checkbox default tick) by Sub Ctr Type 
        public void LoadFieldProperties(List<FieldPropertiesModel> models, string SubConGroupCode)
        {
            using (var db = new MainDbContext())
            {
                #region Main
                //Rate Option
                bool cboRateOptionVisible = false;
                bool cboRateOptionEnable = false;
                bool cboRateOptionMandatory = false;
                cboRateOptionVisible = _clsGlobal.GetFieldVisible("PF-100003", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                cboRateOptionEnable = _clsGlobal.GetFieldEnabled("PF-100003", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                cboRateOptionMandatory = _clsGlobal.GetFieldMandatory("PF-100003", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = cboRateOptionVisible, IsEnable = cboRateOptionEnable, IsMandatory = cboRateOptionMandatory, ColumnName = "cboRateOption" });

                //Creation Date
                bool dtpCreationDateVisible = false;
                bool dtpCreationDateEnable = false;
                bool dtpCreationDateMandatory = false;
                dtpCreationDateVisible = _clsGlobal.GetFieldVisible("PF-105000", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpCreationDateEnable = _clsGlobal.GetFieldEnabled("PF-105000", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpCreationDateMandatory = _clsGlobal.GetFieldMandatory("PF-105000", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = dtpCreationDateVisible, IsEnable = dtpCreationDateEnable, IsMandatory = dtpCreationDateMandatory, ColumnName = "dtpCreationDate" });

                //Letter of Offer Date
                bool dtpLODateVisible = false;
                bool dtpLODateEnable = false;
                bool dtpLODateMandatory = false;
                dtpLODateVisible = _clsGlobal.GetFieldVisible("PF-105001", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpLODateEnable = _clsGlobal.GetFieldEnabled("PF-105001", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpLODateMandatory = _clsGlobal.GetFieldMandatory("PF-105001", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = dtpLODateVisible, IsEnable = dtpLODateEnable, IsMandatory = dtpLODateMandatory, ColumnName = "dtpLODate" });

                //Agreement Date
                bool dtpAgreementDateVisible = false;
                bool dtpAgreementDateEnable = false;
                bool dtpAgreementDateMandatory = false;
                dtpAgreementDateVisible = _clsGlobal.GetFieldVisible("PF-105002", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpAgreementDateEnable = _clsGlobal.GetFieldEnabled("PF-105002", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpAgreementDateMandatory = _clsGlobal.GetFieldMandatory("PF-105002", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = dtpAgreementDateVisible, IsEnable = dtpAgreementDateEnable, IsMandatory = dtpAgreementDateMandatory, ColumnName = "dtpAgreementDate" });

                //Offer Till Date
                bool dtpOfferTillDateVisible = false;
                bool dtpOfferTillDateEnable = false;
                bool dtpOfferTillDateMandatory = false;
                dtpOfferTillDateVisible = _clsGlobal.GetFieldVisible("PF-105003", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpOfferTillDateEnable = _clsGlobal.GetFieldEnabled("PF-105003", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpOfferTillDateMandatory = _clsGlobal.GetFieldMandatory("PF-105003", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = dtpOfferTillDateVisible, IsEnable = dtpOfferTillDateEnable, IsMandatory = dtpOfferTillDateMandatory, ColumnName = "dtpOfferTillDate" });

                //Posting Date
                bool dtpPostingDateVisible = false;
                bool dtpPostingDateEnable = false;
                bool dtpPostingDateMandatory = false;
                dtpPostingDateVisible = _clsGlobal.GetFieldVisible("PF-105004", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpPostingDateEnable = _clsGlobal.GetFieldEnabled("PF-105004", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpPostingDateMandatory = _clsGlobal.GetFieldMandatory("PF-105004", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = dtpPostingDateVisible, IsEnable = dtpPostingDateEnable, IsMandatory = dtpPostingDateMandatory, ColumnName = "dtpPostingDate" });

                //Font Size
                bool txtFontSizeVisible = false;
                bool txtFontSizeEnable = false;
                bool txtFontSizeMandatory = false;
                txtFontSizeVisible = _clsGlobal.GetFieldVisible("PF-102007", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtFontSizeEnable = _clsGlobal.GetFieldEnabled("PF-102007", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtFontSizeMandatory = _clsGlobal.GetFieldMandatory("PF-102007", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtFontSizeVisible, IsEnable = txtFontSizeEnable, IsMandatory = txtFontSizeMandatory, ColumnName = "txtFontSize" });

                //Comm %
                bool commPerVisible = false;
                bool commPerEnable = false;
                bool commPerMandatory = false;
                commPerVisible = _clsGlobal.GetFieldVisible("PF-104003", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                commPerEnable = _clsGlobal.GetFieldEnabled("PF-104003", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                commPerMandatory = _clsGlobal.GetFieldMandatory("PF-104003", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = commPerVisible, IsEnable = commPerEnable, IsMandatory = commPerMandatory, ColumnName = "commPer" });
                #endregion

                #region Instalment
                //Frequency of Instalment 
                bool txtFOIVisible = false;
                bool txtFOIEnable = false;
                bool txtFOIMandatory = false;
                txtFOIVisible = _clsGlobal.GetFieldVisible("PF-102014", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtFOIEnable = _clsGlobal.GetFieldEnabled("PF-102014", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtFOIMandatory = _clsGlobal.GetFieldMandatory("PF-102014", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtFOIVisible, IsEnable = txtFOIEnable, IsMandatory = txtFOIMandatory, ColumnName = "txtFOI" });

                //LEFS Interest Code
                bool intCodeVisible = false;
                bool intCodeEnable = false;
                bool intCodeMandatory = false;
                intCodeVisible = _clsGlobal.GetFieldVisible("PF-100020", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                intCodeEnable = _clsGlobal.GetFieldEnabled("PF-100020", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                intCodeMandatory = _clsGlobal.GetFieldMandatory("PF-100020", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = intCodeVisible, IsEnable = intCodeEnable, IsMandatory = intCodeMandatory, ColumnName = "intCode" });
                #endregion

                #region Additional Info
                //Credit Term 
                bool txtCreditTermVisible = false;
                bool txtCreditTermEnable = false;
                bool txtCreditTermMandatory = false;
                txtCreditTermVisible = _clsGlobal.GetFieldVisible("PF-102016", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtCreditTermEnable = _clsGlobal.GetFieldEnabled("PF-102016", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtCreditTermMandatory = _clsGlobal.GetFieldMandatory("PF-102016", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtCreditTermVisible, IsEnable = txtCreditTermEnable, IsMandatory = txtCreditTermMandatory, ColumnName = "txtCreditTerm" });

                //Internal Credit Term 
                bool txtIntCreditTermVisible = false;
                bool txtIntCreditTermEnable = false;
                bool txtIntCreditTermMandatory = false;
                txtIntCreditTermVisible = _clsGlobal.GetFieldVisible("PF-102017", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtIntCreditTermEnable = _clsGlobal.GetFieldEnabled("PF-102017", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtIntCreditTermMandatory = _clsGlobal.GetFieldMandatory("PF-102017", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtIntCreditTermVisible, IsEnable = txtIntCreditTermEnable, IsMandatory = txtIntCreditTermMandatory, ColumnName = "txtIntCreditTerm" });

                //Late Payment Interest (%)
                bool txtLatePaytIntVisible = false;
                bool txtLatePaytIntEnable = false;
                bool txtLatePaytIntMandatory = false;
                txtLatePaytIntVisible = _clsGlobal.GetFieldVisible("PF-102018", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtLatePaytIntEnable = _clsGlobal.GetFieldEnabled("PF-102018", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtLatePaytIntMandatory = _clsGlobal.GetFieldMandatory("PF-102018", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtLatePaytIntVisible, IsEnable = txtLatePaytIntEnable, IsMandatory = txtLatePaytIntMandatory, ColumnName = "txtLatePaytInt" });

                //Minimum Late Payment Interest ($)
                bool txtMinLatePaytIntVisible = false;
                bool txtMinLatePaytIntEnable = false;
                bool txtMinLatePaytIntMandatory = false;
                txtMinLatePaytIntVisible = _clsGlobal.GetFieldVisible("PF-102019", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtMinLatePaytIntEnable = _clsGlobal.GetFieldEnabled("PF-102019", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtMinLatePaytIntMandatory = _clsGlobal.GetFieldMandatory("PF-102019", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtMinLatePaytIntVisible, IsEnable = txtMinLatePaytIntEnable, IsMandatory = txtMinLatePaytIntMandatory, ColumnName = "txtMinLatePaytInt" });

                //Facility Fee ($)
                bool txtFacilityFeeVisible = false;
                bool txtFacilityFeeEnable = false;
                bool txtFacilityFeeMandatory = false;
                txtFacilityFeeVisible = _clsGlobal.GetFieldVisible("PF-102021", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtFacilityFeeEnable = _clsGlobal.GetFieldEnabled("PF-102021", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtFacilityFeeMandatory = _clsGlobal.GetFieldMandatory("PF-102021", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtFacilityFeeVisible, IsEnable = txtFacilityFeeEnable, IsMandatory = txtFacilityFeeMandatory, ColumnName = "txtFacilityFee" });

                //Mode of Payment
                bool cboModeofPaytVisible = false;
                bool cboModeofPaytEnable = false;
                bool cboModeofPaytMandatory = false;
                cboModeofPaytVisible = _clsGlobal.GetFieldVisible("PF-100022", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                cboModeofPaytEnable = _clsGlobal.GetFieldEnabled("PF-100022", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                cboModeofPaytMandatory = _clsGlobal.GetFieldMandatory("PF-100022", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = cboModeofPaytVisible, IsEnable = cboModeofPaytEnable, IsMandatory = cboModeofPaytMandatory, ColumnName = "cboModeofPayt" });

                //DOA Group
                bool grpDOAVisible = false;
                grpDOAVisible = _clsGlobal.GetFieldVisible("PF-190000", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = grpDOAVisible, ColumnName = "grpDOA" });
                #endregion

                #region Additional Info 2
                //Prepayment (%)
                bool txtPrepVisible = false;
                bool txtPrepEnable = false;
                bool txtPrepMandatory = false;
                txtPrepVisible = _clsGlobal.GetFieldVisible("PF-104010", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtPrepEnable = _clsGlobal.GetFieldEnabled("PF-104010", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtPrepMandatory = _clsGlobal.GetFieldMandatory("PF-104010", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtPrepVisible, IsEnable = txtPrepEnable, IsMandatory = txtPrepMandatory, ColumnName = "txtPrep" });

                //Prepayment Black Out Period (Month)
                bool txtPrepBOPdVisible = false;
                bool txtPrepBOPdEnable = false;
                bool txtPrepBOPdMandatory = false;
                txtPrepBOPdVisible = _clsGlobal.GetFieldVisible("PF-104011", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtPrepBOPdEnable = _clsGlobal.GetFieldEnabled("PF-104011", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtPrepBOPdMandatory = _clsGlobal.GetFieldMandatory("PF-104011", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtPrepBOPdVisible, IsEnable = txtPrepBOPdEnable, IsMandatory = txtPrepBOPdMandatory, ColumnName = "txtPrepBOPd" });

                //Prepayment Notice Period (Month)
                bool txtPrepNoticePdVisible = false;
                bool txtPrepNoticePdEnable = false;
                bool txtPrepNoticePdMandatory = false;
                txtPrepNoticePdVisible = _clsGlobal.GetFieldVisible("PF-104012", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtPrepNoticePdEnable = _clsGlobal.GetFieldEnabled("PF-104012", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtPrepNoticePdMandatory = _clsGlobal.GetFieldMandatory("PF-104012", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtPrepNoticePdVisible, IsEnable = txtPrepNoticePdEnable, IsMandatory = txtPrepNoticePdMandatory, ColumnName = "txtPrepNoticePd" });

                //Cancelation Fee (%)
                bool txtCancelationFeeVisible = false;
                bool txtCancelationFeeEnable = false;
                bool txtCancelationFeeMandatory = false;
                txtCancelationFeeVisible = _clsGlobal.GetFieldVisible("PF-104013", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtCancelationFeeEnable = _clsGlobal.GetFieldEnabled("PF-104013", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtCancelationFeeMandatory = _clsGlobal.GetFieldMandatory("PF-104013", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtCancelationFeeVisible, IsEnable = txtCancelationFeeEnable, IsMandatory = txtCancelationFeeMandatory, ColumnName = "txtCancelationFee" });

                //Maturity Date
                bool dtpMaturityDateVisible = false;
                bool dtpMaturityDateEnable = false;
                bool dtpMaturityDateMandatory = false;
                dtpMaturityDateVisible = _clsGlobal.GetFieldVisible("PF-105005", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpMaturityDateEnable = _clsGlobal.GetFieldEnabled("PF-105005", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                dtpMaturityDateMandatory = _clsGlobal.GetFieldMandatory("PF-105005", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = dtpMaturityDateVisible, IsEnable = dtpMaturityDateEnable, IsMandatory = dtpMaturityDateMandatory, ColumnName = "dtpMaturityDate" });

                //Renewal Months
                bool txtRenewMthsVisible = false;
                bool txtRenewMthsEnable = false;
                bool txtRenewMthsMandatory = false;
                txtRenewMthsVisible = _clsGlobal.GetFieldVisible("PF-104014", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtRenewMthsEnable = _clsGlobal.GetFieldEnabled("PF-104014", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                txtRenewMthsMandatory = _clsGlobal.GetFieldMandatory("PF-104014", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtRenewMthsVisible, IsEnable = txtRenewMthsEnable, IsMandatory = txtRenewMthsMandatory, ColumnName = "txtRenewMths" });
                #endregion

                #region Schedule
                //Collection Fee Option Table
                bool grpCollectionFeeVisible = false;
                bool grpCollectionFeeEnable = false;
                bool grpCollectionFeeMandatory = false;
                grpCollectionFeeVisible = _clsGlobal.GetFieldVisible("PF-190001", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                grpCollectionFeeEnable = _clsGlobal.GetFieldEnabled("PF-190001", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                grpCollectionFeeMandatory = _clsGlobal.GetFieldMandatory("PF-190001", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = grpCollectionFeeVisible, IsEnable = grpCollectionFeeEnable, IsMandatory = grpCollectionFeeMandatory, ColumnName = "grpCollectionFee" });

                //Collection Fee in Schedule Table
                bool txtCollectionFeeVisible = false;
                txtCollectionFeeVisible = _clsGlobal.GetFieldVisible("PF-104015", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = txtCollectionFeeVisible, ColumnName = "txtCollectionFee" });

                #endregion
            }
        }

        //Load Field Properties for Equipment List (Enabled, Visible, Mandatory, Checkbox default tick) by Sub Ctr Type 
        public void LoadFieldProperties_EquipmentTbl(List<FieldPropertiesModel> models, string SubConGroupCode)
        {
            using (var db = new MainDbContext())
            {
                #region Equipment List
                bool brand = false;
                brand = _clsGlobal.GetFieldVisible("PF-104005", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = brand, ColumnIndex = 3, ColumnName = "Brand" });

                bool modelCol = false;
                modelCol = _clsGlobal.GetFieldVisible("PF-104006", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = modelCol, ColumnIndex = 4, ColumnName = "Model" });

                bool vehicleMake = false;
                vehicleMake = _clsGlobal.GetFieldVisible("PF-104017", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = vehicleMake, ColumnIndex = 5, ColumnName = "VehicleMake" });

                bool vehicleModel = false;
                vehicleModel = _clsGlobal.GetFieldVisible("PF-104007", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = vehicleModel, ColumnIndex = 6, ColumnName = "VehicleModel" });

                bool unitPrice = true;
                unitPrice = _clsGlobal.GetFieldVisible("PF-104016", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = unitPrice, ColumnIndex = 8, ColumnName = "UnitPrice" });

                bool downPaymentECOBOS = false;
                downPaymentECOBOS = _clsGlobal.GetFieldVisible("PF-100023", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = downPaymentECOBOS, ColumnIndex = 12, ColumnName = "DownPaymentSupplierECOBOS" });

                bool downPaymentSupplier = false;
                downPaymentSupplier = _clsGlobal.GetFieldVisible("PF-104008", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = downPaymentSupplier, ColumnIndex = 13, ColumnName = "DownPaymentSupplier" });

                bool Supplier = false;
                Supplier = _clsGlobal.GetFieldVisible("PF-104009", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = Supplier, ColumnIndex = 16, ColumnName = "Supplier" });

                bool gstCheck = true;
                gstCheck = _clsGlobal.GetFieldDefaultCheck("PF-104018", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                models.Add(new FieldPropertiesModel() { IsVisible = true, IsChecked = gstCheck, ColumnIndex = 10, ColumnName = "GST" });




                #endregion
            }
        }

        //Load Field Default Value from Parameter table OR by Sub Ctr Type 
        public PreConViewModel LoadDefaultValue(string SubConGroupCode)
        {
            PreConViewModel model = new PreConViewModel();
            using (var db = new MainDbContext())
            {
                #region GST Percentage = 7
                string GSTPer = _clsGlobal.GetDefaultValue("P-10002", DateTime.UtcNow);
                try
                {
                    model.GSTPer =Convert.ToDecimal(GSTPer);
                }
                catch
                {
                    model.GSTPer = 0;
                }
                #endregion

                #region Offer Till Days = 14 days
                string OfferTillDays = _clsGlobal.GetDefaultValue("P-10010", DateTime.UtcNow);
                try
                {
                    DateTime OfferTillDate = DateTime.Today.AddDays(Convert.ToInt16(OfferTillDays));
                    model.OfferTillDate = OfferTillDate.ToShortDateString();
                }
                catch
                {
                    model.OfferTillDate = DateTime.Today.ToShortDateString();
                }
                #endregion

                #region Font Size = 9
                string FontSize = _clsGlobal.GetDefaultValue("P-10003", DateTime.UtcNow);
                try
                {
                    model.FontSize = Convert.ToInt32(FontSize);
                }
                catch
                {
                    model.FontSize = 0;
                }
                #endregion

                #region Period of Lease = 0
                string PeriodofLease = _clsGlobal.GetDefaultValue("P-10004", DateTime.UtcNow);
                try
                {
                    model.PeriodofLease = Convert.ToInt32(PeriodofLease);
                }
                catch
                {
                    model.PeriodofLease = 0;
                }
                #endregion

                #region Frequency of Instalment = 0
                string FreqofInst = _clsGlobal.GetDefaultValue("P-10006", DateTime.UtcNow);
                try
                {
                    model.FreqofInst = Convert.ToInt32(FreqofInst);
                }
                catch
                {
                    model.FreqofInst = 0;
                }
                #endregion

                #region Upfront Payment Months = 1
                string UpfrontPaymentMth = _clsGlobal.GetDefaultValue("P-10011", DateTime.UtcNow);
                try
                {
                    model.UpfrontPaymentMth = Convert.ToInt32(UpfrontPaymentMth);
                }
                catch
                {
                    model.UpfrontPaymentMth = 0;
                }
                #endregion

                #region External Credit Term (Days) = 0
                string CreditTerm = _clsGlobal.GetDefaultValue("P-10008", DateTime.UtcNow);
                try
                {
                    model.CreditTerm = Convert.ToInt32(CreditTerm);
                }
                catch
                {
                    model.CreditTerm = 0;
                }
                #endregion

                #region Internal Credit Term (Days) = 7
                string IntCreditTerm = _clsGlobal.GetDefaultValue("P-10009", DateTime.UtcNow);
                try
                {
                    model.IntCreditTerm = Convert.ToInt32(IntCreditTerm);
                }
                catch
                {
                    model.IntCreditTerm = 0;
                }
                #endregion

                #region LPI % by Sub Ctr Type
                string LatePaytIntPer = _clsGlobal.GetDefaultValueMatrix("PM-20003", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                try
                {
                    model.LatePaytIntPer =Convert.ToDecimal(LatePaytIntPer);
                }
                catch
                {
                    model.LatePaytIntPer = 0;
                }
                #endregion

                #region Min. LPI Amount by Sub Ctr Type
                string MinLatePaytAmt = _clsGlobal.GetDefaultValueMatrix("PM-20004", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                try
                {
                    model.MinLatePaytAmt = Convert.ToDecimal(MinLatePaytAmt);
                }
                catch
                {
                    model.MinLatePaytAmt = 0;
                }
                #endregion

                #region Processing/Commitment Fee by Sub Ctr Type
                string ProCommFee = _clsGlobal.GetDefaultValueMatrix("PM-20005", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                try
                {
                    model.ProCommFee = Convert.ToDecimal(ProCommFee);
                }
                catch
                {
                    model.ProCommFee = 0;
                }
                #endregion

                #region Prepayment % by Sub Ctr Type
                string PrepaymentPer = _clsGlobal.GetDefaultValueMatrix("PM-20007", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                try
                {
                    model.PrepaymentPer = Convert.ToDecimal(PrepaymentPer);
                }
                catch
                {
                    model.PrepaymentPer = 0;
                }
                #endregion

                #region Prepayment Blackout Period (Months) by Sub Ctr Type
                string PrepBlackoutPeriod = _clsGlobal.GetDefaultValue("P-20008", DateTime.UtcNow);
                try
                {
                    model.PrepBlackoutPeriod = Convert.ToInt32(PrepBlackoutPeriod);
                }
                catch
                {
                    model.PrepBlackoutPeriod = 0;
                }
                #endregion

                #region Prepayment Notice Period (Months) by Sub Ctr Type
                string PrepNoticePeriod = _clsGlobal.GetDefaultValue("P-20009", DateTime.UtcNow);
                try
                {
                    model.PrepNoticePeriod = Convert.ToInt32(PrepNoticePeriod);
                }
                catch
                {
                    model.PrepNoticePeriod = 0;
                }
                #endregion

                #region Cancelation Fee by Sub Ctr Type
                string CancelationFee = _clsGlobal.GetDefaultValueMatrix("PM-20010", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                try
                {
                    model.CancelationFee = Convert.ToDecimal(CancelationFee);
                }
                catch
                {
                    model.CancelationFee = 0;
                }
                #endregion
            }
            return model;
        }

        //Load Function Logic ID from Matrix table OR by Sub Ctr Type 
        public LogicModel LoadLogicID(string SubConGroupCode)
        {
            LogicModel model = new LogicModel();
            using (var db = new MainDbContext())
            {
                //Contract Schedule Calculation
                model.logic_CtrSchCalc = _clsGlobal.GetLogicCode("F-10000", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);

                //Contract Maturity Date Default Value
                model.logic_CtrMaturityDate = _clsGlobal.GetLogicCode("F-10001", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);

                //Contract Renewal Months Default Value
                model.logic_CtrRenewalMths = _clsGlobal.GetLogicCode("F-10002", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
            }
            return model;
        }

        //Load List Of Value for drop down
        public DropDownModel LoadDropDownData(string ConGroupCode, string SubConGroupCode)
        {
            DropDownModel model = new DropDownModel();
            using (var db = new MainDbContext())
            {
                List<SelectListItem> lstType = _clsGlobal.GetListOfValue("CONTRACT_TYPE", "", "O", "", ConGroupCode);
                model.lstCtrType = lstType;

                lstType = _clsGlobal.GetListOfValue("SUB_CONTRACT_TYPE", ConGroupCode, "O", "", SubConGroupCode);
                model.lstSubCtrType = lstType;

                lstType = _clsGlobal.GetListOfValue("PRODUCT_TYPE", "", "O", "", "");
                model.lstProdType = lstType;

                //Instalment Option set to "Straight" by default
                string InsOptionDef = _clsGlobal.GetDefaultValue("P-10000", DateTime.UtcNow);
                lstType = _clsGlobal.GetListOfValue("INSTALMENT_OPTION", "", "O", "", InsOptionDef);
                model.lstInsOption = lstType;

                //Rate Option set to "Flat" by default
                string RateOptionDef = _clsGlobal.GetDefaultValue("P-10001", DateTime.UtcNow);
                lstType = _clsGlobal.GetListOfValue("RATE_OPTION", "", "O", "", RateOptionDef);
                model.lstRateOption = lstType;

                //Frequency of Instalment Option set to "Monthly" by default
                string FOIODef = _clsGlobal.GetDefaultValue("P-10005", DateTime.UtcNow);
                lstType = _clsGlobal.GetListOfValue("FREQ_OF_INSTALMENT", "", "O", "", FOIODef);
                model.lstFOIO = lstType;

                //Default to "GIRO" if "Spring" is selected.
                string ModeOfPaymentDefVal = _clsGlobal.GetDefaultValueMatrix("PM-20011", SubConGroupCode, clsGlobal.MatrixSubCtrTypeCode, DateTime.UtcNow);
                lstType = _clsGlobal.GetListOfValue("PAY_MODE", "", "O", "", ModeOfPaymentDefVal);
                model.lstModeofPayt = lstType;

                lstType = _clsGlobal.GetListOfValue("BANK_LIST", "", "O", "", "");
                model.lstBank = lstType;

                lstType = _clsGlobal.GetListOfValue("PAYMENT_OPTION", "", "O", "", "");
                model.lstPaymentOption = lstType;

                lstType = _clsGlobal.GetListOfValue("FINANCIAL_SERVICES_SALES_DEPARTMENT", "", "O", "", "");
                model.lstSalesExecutive = lstType;

                model.lstLEFSIntCode = _clsContractGeneral.GetLEFSInterestCode(SubConGroupCode);

                lstType = _clsGlobal.GetListOfValue("SECURITY_LIST_LEVEL_1", "", "O", "", "");
                model.lstSecurityLevel1 = lstType;

								lstType = _clsGlobal.GetListOfValue("BUY_BACK_PERCENTAGE_TYPE", "", "O", "", "");
								model.lstBuybackPercentageType = lstType;

            }
            return model;
        }

        //Load drop down data in equipment list
        public void LoadEquipmentDropDownData(CommonDropDownModel model)
        {
            using (var db = new OrixDBEntities())
            {
                //Brand
                model.BrandList = _clsAsset.GetBrand(true);

                //Vehicle Make                
                model.VehicleMake = _clsAsset.GetVehicleMake(true);

                //Supplier                
                model.SupplierList = _clsCRM.GetSupplierFromCRM();
            }
        }
        #endregion

        #region Other Functions
        public bool checkContractExistByCtrNo(string contNo)
        {
            bool flag = false;
            using (var db = new MainDbContext())
            {
                flag = db.Cfstb_ctr_mas.AsNoTracking().Where(f => f.Cm_ctr_num == contNo).Any();
            }
            return flag;
        }

        public bool checkSerialNumberExistByCtrNo(string contNo)
        {
            bool flag = false;
            using (var db = new MainDbContext())
            {
                flag = db.Cfstb_serial_num.AsNoTracking().Where(f => f.cs_ctr_num == contNo).Any();
            }
            return flag;
        }

        public string insertPreContract_Master(PreConSaveModel model, SysAutoGenerateReturn refNumber, SysAutoGenerateReturn contractNumber)
        {
            string res = "";
            glog.Debug("insertPreContract_Master: Entry");
            using (var db = new MainDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var cretedBy = db.Sys_Users.FirstOrDefault(x => x.Name == model.CreatedBy).Email;
                        var objPreCon = new PreContract_Master()
                        {
                            //ContractNumber = model.ContractNumber,
                            RefNumber = refNumber.NewId,  
                            ContractNumber = contractNumber.NewId,                        
                            CreatedBy = cretedBy,
                            CreatedDate = model.CreatedDate,
                            Customer = model.Customer,
                            CustomerAddress = model.CustomerAddress,
                            CustomerConPerson = model.CustomerConPerson,
                            CustomerDept = model.CustomerDept,
                            CustomerType = model.CustomerType,
                            LEFSInterestCode = model.LEFSInterestCode,
                            RolloverNumber = model.RolloverNumber,
                            Status = model.Status,
                            ContractType = model.ContractType,
                            SubContractType = model.SubContractType,
                            ProductType = model.ProductType,
                            SubProductType = model.SubProductType,
                            UpdatedBy = model.UpdatedBy,
							UpdatedDate = model.UpdatedDate == null ? null : model.UpdatedDate,
							BuyBackInd = model.BuybackInd,
							RecourseInd = model.RecourseInd,
                            PropertyTotLTVPer = model.PropertyTotLTVPer,
                            VesselTotLTVPer = model.VesselTotLTVPer                            
                        };
                        db.PreContract_Master.Add(objPreCon);

                        //Primary Key table need to save first
                        db.SaveChanges();

                        var existSecurityCount = db.PreContract_SecurityList
                               .Where(x => x.RolloverNumber == model.RolloverNumber && x.ContractNumber == contractNumber.NewId).Count();
                        foreach (var item in model.SecurityList)
                        {
                            existSecurityCount = existSecurityCount + 1;
                            var objPreConSecurityList = new PreContract_SecurityList()
                            {
                                //ContractNumber = model.ContractNumber,
                                RefNumber = refNumber.NewId,  
                                ContractNumber = contractNumber.NewId,
                                RolloverNumber = model.RolloverNumber,
                                ItemNumber = existSecurityCount,
                                SecurityLevel1 = item.SecurityLevel1,
                                SecurityLevel2 = item.SecurityLevel2,
                                Status = "O",
                                CreatedBy = cretedBy,
                                CreatedDate = model.CreatedDate,
                                UpdatedBy = model.UpdatedBy,
								UpdatedDate = model.UpdatedDate == null ? null : model.UpdatedDate
                            };
                            db.PreContract_SecurityList.Add(objPreConSecurityList);
                        }


                        var existSecurityItemCount = db.PreContract_SecurityItem
                              .Where(x => x.RolloverNumber == model.RolloverNumber && x.ContractNumber == contractNumber.NewId).Count();

                        var guarantor = model.IndividualGuarantorList.Union(model.CorparateGuarantorList);

                        foreach (var item in guarantor)
                        {
                            existSecurityItemCount = existSecurityItemCount + 1;
                            var objPreConSecurityList = new PreContract_SecurityItem()
                            {
                                //ContractNumber = model.ContractNumber,
                                RefNumber = refNumber.NewId,  
                                ContractNumber = contractNumber.NewId,
                                RolloverNumber = model.RolloverNumber,
                                ItemNumber = existSecurityItemCount,
                                SecurityListLevel2 =clsVariables.ConstGuarantor, //"SLL2-1000"
                                GuarantorType=item.GuarantorType,
                                SecurityID = item.SecurityID,
                                GuarantorAddress = item.GuarantorAddress,
                                GuarantorDept = item.GuarantorDept,
                                GuarantorConPerson = item.GuarantorConPerson,
                                LetterType = item.LetterType,
                                Status = item.Status,
                                CreatedBy = cretedBy,
                                CreatedDate = model.CreatedDate,
                                UpdatedBy = model.UpdatedBy,
								UpdatedDate = model.UpdatedDate == null ? null : model.UpdatedDate
                            };
                            db.PreContract_SecurityItem.Add(objPreConSecurityList);
                        }

                        //existSecurityCount = db.PreContract_SecurityItem
                        //       .Where(x => x.RolloverNumber == model.RolloverNumber && x.ContractNumber == model.ContractNumber).Count();
                        foreach (var item in model.MortgagorPropertyAndVesselList)
                        {
                            existSecurityItemCount = existSecurityItemCount + 1;
                            var objPreConSecurityList = new PreContract_SecurityItem()
                            {
                                //ContractNumber = model.ContractNumber,
                                RefNumber = refNumber.NewId,  
                                ContractNumber = contractNumber.NewId,
                                RolloverNumber = model.RolloverNumber,
                                ItemNumber = existSecurityItemCount,
                                SecurityListLevel2 = item.SecurityListLevel2,
                                CustomerType = item.CustomerType,
                                SecurityID = item.SecurityID,
                                Customer = item.Customer,
                                IndicativeValuationAmt = item.IndicativeValuationAmt,
                                LoanAmtProportion = item.LoanAmtProportion,
                                LTVPercentage = item.LTVPercentage,
                                Status = item.Status,
                                CreatedBy = cretedBy,
                                CreatedDate = model.CreatedDate,
                                UpdatedBy = model.UpdatedBy,
								UpdatedDate = model.UpdatedDate == null ? null : model.UpdatedDate
                            };
                            db.PreContract_SecurityItem.Add(objPreConSecurityList);
                        }

						//existSecurityCount = db.PreContract_SecurityItem
						//	   .Where(x => x.RolloverNumber == model.RolloverNumber && x.ContractNumber == model.ContractNumber).Count();
						foreach (var item in model.DebentureList)
						{
							existSecurityItemCount = existSecurityItemCount + 1;
							var objPreConSecurityList = new PreContract_SecurityItem()
							{                                
                                //ContractNumber = model.ContractNumber,
                                RefNumber = refNumber.NewId,
                                ContractNumber = contractNumber.NewId,    
								RolloverNumber = model.RolloverNumber,
								ItemNumber = existSecurityItemCount,
								SecurityListLevel2 = item.SecurityListLevel2,
								CustomerType = item.CustomerType,
								SecurityID = item.SecurityID,
								Customer = item.Customer,							
								Status = item.Status,
								CreatedBy = cretedBy,
								CreatedDate = model.CreatedDate
							};
							db.PreContract_SecurityItem.Add(objPreConSecurityList);
						}

						#region -- Buy Back --
                        var existBuyBackCount = db.PreContract_BuyBackGuarantor
                               .Where(x => x.RolloverNumber == model.RolloverNumber && x.ContractNumber == contractNumber.NewId).Count();
                        var existBuyBackAmountCount = db.PreContract_BuyBackGuarantor_Amount
                               .Where(x => x.RolloverNumber == model.RolloverNumber && x.ContractNumber == contractNumber.NewId && x.ItemNumber == existBuyBackCount).Count();

						var preGuarantor = "";
                        foreach (var item in model.BuyBackList)
                        {
							if (preGuarantor != item.GuarantorCode)
							{
                            existBuyBackCount = existBuyBackCount + 1;
								existBuyBackAmountCount = 0;
                            var objPreConBuyBackList = new PreContract_BuyBackGuarantor()
                            {
                                //ContractNumber = model.ContractNumber,
                                RefNumber = refNumber.NewId,
								ContractNumber = contractNumber.NewId,    
                                RolloverNumber = model.RolloverNumber,
                                ItemNumber = existBuyBackCount,
                                GuarantorType = item.Status,
                                Guarantor = item.GuarantorCode,
                                GuarantorAddress = item.GuarantorAddress,
                                GuarantorConPerson = item.GuarantorConPerson,
                                GuarantorDept = item.GuarantorDept,
                                LetterType = item.LetterType,
                                Status = "O",
                                CreatedBy = cretedBy,
                                CreatedDate = model.CreatedDate,
                            };
                            db.PreContract_BuyBackGuarantor.Add(objPreConBuyBackList);
							}
							preGuarantor = item.GuarantorCode;

                            //Buy Back Ammount
                            existBuyBackAmountCount = existBuyBackAmountCount + 1;
                            var objPreConBuyBackAmountList = new PreContract_BuyBackGuarantor_Amount()
                            {
                                //ContractNumber = model.ContractNumber,
                                RefNumber = refNumber.NewId,
								ContractNumber = contractNumber.NewId,
                                RolloverNumber = model.RolloverNumber,
                                ItemNumber = existBuyBackCount,
                                LineNumber = existBuyBackAmountCount,
                                PeriodFrom = item.PeriodFrom,
                                PeriodTo = item.PeriodTo,
                                BuyBackAmount = item.BuyBackAmount,
                                BuyBackPercentage = item.BuyBackPercentage,
                                BuyBackType = item.BuyBackType,
                                Status = "O",
                                CreatedBy = cretedBy,
                                CreatedDate = model.CreatedDate
                            };
                            db.PreContract_BuyBackGuarantor_Amount.Add(objPreConBuyBackAmountList);
                        }
                        #endregion

                        #region -- Recourse --
                        var existRecourseCount = db.PreContract_RecourseGuarantor
                               .Where(x => x.RolloverNumber == model.RolloverNumber && x.ContractNumber == contractNumber.NewId).Count();
                        var existRecourseAmountCount = db.PreContract_RecourseGuarantor_Amount
                               .Where(x => x.RolloverNumber == model.RolloverNumber && x.ContractNumber == contractNumber.NewId && x.ItemNumber == existBuyBackCount).Count();
						preGuarantor = "";
                        foreach (var item in model.RecourseList)
                        {
							if (preGuarantor != item.GuarantorCode)
							{
                            existRecourseCount = existRecourseCount + 1;
								existRecourseAmountCount = 0;
                            var objPreConRecourseList = new PreContract_RecourseGuarantor()
                            {
                                //ContractNumber = model.ContractNumber,
                                RefNumber = refNumber.NewId,
                                ContractNumber = contractNumber.NewId,
                                RolloverNumber = model.RolloverNumber,
                                ItemNumber = existRecourseCount,
                                GuarantorType = item.Status,
                                Guarantor = item.GuarantorCode,
                                GuarantorAddress = item.GuarantorAddress,
                                GuarantorConPerson = item.GuarantorConPerson,
                                GuarantorDept = item.GuarantorDept,
                                LetterType = item.LetterType,
                                Status = "O",
                                CreatedBy = cretedBy,
                                CreatedDate = model.CreatedDate,
                            };
                            db.PreContract_RecourseGuarantor.Add(objPreConRecourseList);
							}

							preGuarantor = item.GuarantorCode;
                            //Recourse Ammount
                            existRecourseAmountCount = existRecourseAmountCount + 1;
                            var objPreConBuyBackAmountList = new PreContract_RecourseGuarantor_Amount()
                            {
                                //ContractNumber = model.ContractNumber,
                                RefNumber = refNumber.NewId,
                                ContractNumber = contractNumber.NewId,
                                RolloverNumber = model.RolloverNumber,
                                ItemNumber = existRecourseCount,
                                LineNumber = existRecourseAmountCount,
                                PeriodFrom = item.PeriodFrom,
                                PeriodTo = item.PeriodTo,
                                RecourseAmount = item.RecourseAmount,
                                RecoursePercentage = item.RecoursePercentage,
                                RecourseType = item.RecourseType,
                                Status = "O",
                                CreatedBy = cretedBy,
                                CreatedDate = model.CreatedDate
                            };
                            db.PreContract_RecourseGuarantor_Amount.Add(objPreConBuyBackAmountList);
                        }
                        #endregion
                        
                        db.SaveChanges();
                        res = objPreCon.ContractNumber;

                        //Update System ID last number used
                        if (clsGlobal.UpdateSystemIDLastNum(contractNumber, cretedBy, db) == false)
                            throw new Exception("Failed to update Pre-Contract Number last number used.");

                        if (clsGlobal.UpdateSystemIDLastNum(refNumber, cretedBy, db) == false)
                            throw new Exception("Failed to update Pre-Contract Number last number used.");

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        glog.Error("insertPreContract_Master Exception: " + ex.Message + ex.InnerException.InnerException);
                        transaction.Rollback();
                        return res;
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                    glog.Debug("insertPreContract_Master: Exit");
                    return res;
                }
            }
        }

		#endregion
	}
}
