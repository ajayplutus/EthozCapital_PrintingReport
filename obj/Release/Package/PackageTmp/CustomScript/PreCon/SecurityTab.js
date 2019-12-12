$(document).ready(function () {

    window.dropdownLetterType = "";
    $.ajax({
        url: varSitePath + "/PreCon/GetLetterType",
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            dropdownLetterType = json.data;
        }
    });


    window.manualMask = function (value) {
        value = value.replace(/,/g, '');
        var maskValue = value.toString().split(".")[0];
        var numberOfdigit = maskValue.length;
        var afterMaskStart = numberOfdigit % 3;
        var result = "";
        var isSecond = false;
        var newlength = numberOfdigit;
        for (var i = 0; i < numberOfdigit; i++) {
            if (i === afterMaskStart && !isSecond && i !== 0) {
                isSecond = true;
                result = result + ",";
            }
            if (newlength % 3 === 0 && i !== afterMaskStart)
                result = result + ",";
            result = result + maskValue.charAt(i);
            newlength = newlength - 1;
        }
        var decimalPlaces = value.toString().split(".")[1];
        if (decimalPlaces)
            result = result + "." + decimalPlaces;
        return result;
    };

});

$(document).on('click', function (event) {
    //GetLetterTypeForGuarantor();
    targetRowEventSecurity(event);
    targetRowEventIndividualGuarantorList(event);
    targetRowEventCorporateGuarantorList(event);
    targetRowEventMortgagePropertyList(event);
    targetRowEventMortgageVesselList(event);
    targetRowEventDebentureVehicleList(event);
    targetRowEventDebentureConstructionEquipmentList(event);
    targetRowEventDebentureIndustrialEquipmentList(event);
    targetRowEventDebentureInventoryList(event);
    targetRowEventDebentureReceivableList(event);
    targetRowEventDebentureCashAndEquivalentIndividualList(event);
    targetRowEventDebentureCashAndEquivalentCompanyList(event);
    targetRowEventSecuritiesFinancialInstrumentsList(event);
    targetRowEventSecurityDepositList(event);
    targetRowEventGuarantorList(event, "BuyBackIndividualGuarantorList");
    targetRowEventGuarantorList(event, "BuyBackCorporateGuarantorList");
    targetRowEventGuarantorList(event, "RecourseIndividualGuarantorList");
    targetRowEventGuarantorList(event, "RecourseCorporateGuarantorList");
    targetRowEventBuyBackAmountList(event, "BuyBackAmount");
    targetRowEventBuyBackAmountList(event, "RecourseAmount");
});
var dropdownListValue = [];
var dropdownIndividualGuarantornameListValue = [];
var Individual;
var Corporate;
var dropdownCorporateGuarantornameListValue = [];

var iSExistGuarantor = false;
var iSExistMortgageProperty = false;
var iSExistMortgageVessel = false;
var isExistDebentureVehicle = false;
var isExistDebentureConstructionEquipment = false;
var isExistDebentureIndustrialEquipment = false;
var isExistDebentureInventory = false;
var isExistDebentureReceivable = false;
var isExistDebentureEquivalentIndividual = false;
var isExistDebentureEquivalentCompany = false;
var isExistSecuritiesFinancialInstruments = false;
var isExistSecurityDeposit = false;

$(function () {
    //dropdownListValue = '@{ @Html.Raw(Json.Encode(dropdownModel.lstSecurityLevel1)); }';


    var appendSecurityRow = {
        "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
        "SecurityList1stLevel": "", "SecurityList2ndLevel": ""
    };
    function initializeNewRowSecurityList() {
        //appendSecurityRow.SecurityList1stLevel = "<input style='width:200px !important' class='editor securityList1stLevel' type='text' tabindex='1' value=''><span class='edited securityList1stLevel_edited'></span>";
        //appendSecurityRow.SecurityList2ndLevel = "<input style='width:200px !important' class='editor securityList2ndLevel' type='text' tabindex='2' value=''><span class='edited securityList2ndLevel_edited'></span>";
        if (dropdownListValue) {
            var strSecurityLevel1 = "<select id='selectSecurityLevel1' class='editor securitylevel1 js-example-basic-sec1' tabindex='1' onchange='setSecurityLevel2(this)'>";
            var jsonParseData = JSON.parse(dropdownListValue);//.Data;
            strSecurityLevel1 += "<option selected value> -- Select -- </option>";
            $.each(jsonParseData, function (i, v) {
                strSecurityLevel1 += "<option value = " + v.Value + ">" + v.Text + "</option>";
            });
            strSecurityLevel1 += "</select><span class='edited'></span><input class='securityvalue1' type='hidden' value=''/>";
            appendSecurityRow.SecurityList1stLevel = strSecurityLevel1;

            var strSecurityLevel2 = "<select id='selectSecurityLevel2' tabindex='2' class='editor securitylevel2 js-example-basic-sec1' onchange='checkSecurityLevel2(this)'>";
            strSecurityLevel2 += "<option selected value> -- Select -- </option>";
            strSecurityLevel2 += "</select><span class='edited'></span><input class='securityvalue2' type='hidden' value=''/>";
            appendSecurityRow.SecurityList2ndLevel = strSecurityLevel2;
            $("#selectSecurityLevel2").change();

        }
        else {
            appendSecurityRow.SecurityList1stLevel = "<select id='selectSecurityLevel1' class='editor' tabindex='1'></select><span class='edited'></span>";
        }
    }
    window.tblSecurityList = $("#tblSecurityList").DataTable({
        "paging": false,
        "info": false,
        "searching": false,
        "language": {
            "zeroRecords": "No Data Available.",
            "infoEmpty": "No Data Available."
        },
        "dom": '<"top"i>rt<"bottom"flp><"clear">'
    });
    $("#btnAddSecurityList").on("click", function () {
        //$('.select2-container').hide();
        if (tblSecurityList.rows().count() == 0) {
            initializeNewRowSecurityList();
        }
        if (isSecurityValidRow(this)) {
            var _row = tblSecurityList.rows.add([[appendSecurityRow.Action, appendSecurityRow.SecurityList1stLevel, appendSecurityRow.SecurityList2ndLevel]]);
            _row.draw();
            setActiveRowSecurityList($("#tblSecurityList tr:last"));
        }
    });
    $('#tblSecurityList tbody').on('click', 'button.deleting', function () {
        tblSecurityList.row($(this).parents("tr")).remove().draw();
        $('#fldSecurity').removeClass('error-div');
        enableDisableSecurityTable("");
        checkFieldValid();
    });
    $("#tblSecurityList tbody").on('click', 'tr', function (event) {
        targetRowEventSecurity(event);
        if (!($(this).hasClass("activeRow"))) {
            if (isSecurityValidRow(this))
                setActiveRowSecurityList(this);
        }
    });
    window.targetRowEventSecurity = function (event) {
        //function targetRowEventSecurity(event) {
        var currentTab = $('.nav-tabs').find('li.active a').text();
        if (
			currentTab == 'Security' &&
			!$(event.target).closest('#tblSecurityList tbody tr.activeRow').length &&
			!$(event.target).closest('#tblSecurityList thead').length &&
			!$(event.target).closest('.sweet-alert').length &&
			!$(event.target).closest('#btnAddSecurityList').length &&
			!$(event.target).closest('.select2-container').length &&
			!$(event.target).closest('#tblSecurityList').length &&
			!$(event.target).closest('button.deleting').length) {
            //if (!isRowChanged()) {
            //	$('#tblSecurityList tr.activeRow').find('button.deleting').click();
            //}
            //else
            if (isSecurityValidRow(this)) {
                setActiveRowSecurityList(null);
            }
        }
    };
    function isSecurityExist(event) {
        var isDuplicate = false;
        var security = $(event).find(".securitylevel1").val();
        var adjacentSecurity = $(event).find(".securitylevel2").val();
        $("#tblSecurityList tbody tr").each(function () {
            var sval1 = $(this).find(".securityvalue1").val();
            var sval2 = $(this).find(".securityvalue2").val();
            var isactiverow = $(this).hasClass('activeRow');
            if (security === sval1 && adjacentSecurity === sval2 && !isactiverow) {
                //swal(swalGlobal.SwalTitle_Error, "Security List 1st Level selected - Security List 2nd Level selected already exist!", swalGlobal.SwalType_Error);
                isDuplicate = true;
            }
        });
        if (isDuplicate) {
            $(event).find('.select2-selection').addClass("selection-danger");
        }
        else {
            $(event).find('.select2-selection').removeClass("selection-danger");
        }
        checkFieldValid();
        return isDuplicate;
    }

    window.isSecurityValidRow = function (el) {
        var activeRow = $("#tblSecurityList tr.activeRow");
        var isValid = false;
        if (activeRow && activeRow.length > 0) {
            var isDuplicate = isSecurityExist(activeRow);
            if (isDuplicate) {
                if (el !== null && ($(el).attr('id') === 'btnAddSecurityList' || $(el).attr('id') === 'SaveContract'))
					swal(swalGlobal.SwalTitle_Error, "Security list selected already exist!", swalGlobal.SwalType_Error);

                return false;
            }
            $(activeRow[0]).find("select").each(function () {
                if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {

                    if (!$(this).val()) {
                        $(this).addClass("alert-danger");
                        if ($(this).hasClass("js-example-basic-sec1")) {
                            $(this).next().children().children().addClass("selection-danger");
                        }
                    }
                    else {
                        $(this).removeClass("alert-danger");
                        if ($(this).hasClass('js-example-basic-sec1')) {
                            $(this).next().children('.select2-selection').removeClass("selection-danger");
                        }
                    }

                }
                //}
            });
        }
        else { return true; }
        isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;
        if (!isValid) {
            $('#fldSecurity').addClass('error-div');
            //swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
        }
        else {
            $('#fldSecurity').removeClass('error-div');
        }
        checkFieldValid();
        return isValid;
    };
    function setActiveRowSecurityList(row) {
        $("#tblSecurityList tr").each(function () {
            if ($(this).hasClass('activeRow')) {
                $(this).find(":input").each(function () {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec1')) {
                        DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text()
                        $(this).next().remove();
                    }
                    if ($(this).val() == "") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec1')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                    //if ($(this).val() == "") { //isValidRow=false;
                    //	$(this).next('span').empty();
                    //}
                    //else {
                    //	$(this).next('span').html($(this).val());
                    //}
                });
                $(this).removeClass("activeRow");
            }
        });
        if (row)
            $(row).addClass('activeRow');
        $('#tblSecurityList .activeRow .js-example-basic-sec1').select2();
    }
    window.setSecurityLevel2 = function (e) {
        var parentId = $(e).val();
        var currentSecurityLevel2 = $(e).closest("tr").find('.securitylevel2');
        var options = "";
        if (parentId) {
            $.ajax({
                url: varSitePath + '/PreCon/GetSecurityLevel2BySecurityLevel1',
                data: { parentId: parentId },
                type: 'Get',
                success: function (Result) {
                    $.each(Result, function (i, cat) {
                        options += "<option value = '" + cat.Value + "'>" + cat.Text + "</option>";
                    });
                    if (e) {
                        currentSecurityLevel2.html(options);
                        $(e).closest("tr").find('.securityvalue1').val(parentId);
                        $(e).closest("tr").find('.securityvalue2').val($(e).closest("tr").find('.securitylevel2').val());

                        var securityLevel2Text = $(e).closest("tr").find('.securitylevel2 option:selected').text();
                        enableDisableSecurityTable(securityLevel2Text);
                    }
                }, complete: function () {
                    checkSecurityLevel2(currentSecurityLevel2);
                },
                error: function () {
                }
            });
        }
        else {
            currentSecurityLevel2.html("<option value=''>--Select--</option>");
        }
    };


    window.enableDisableSecurityTable = function (val) {
        iSExistGuarantor = false;
        iSExistMortgageProperty = false;
        iSExistMortgageVessel = false;
        isExistDebentureVehicle = false;
        isExistDebentureConstructionEquipment = false;
        isExistDebentureIndustrialEquipment = false;
        isExistDebentureInventory = false;
        isExistDebentureReceivable = false;
        isExistDebentureEquivalentIndividual = false;
        isExistDebentureEquivalentCompany = false;
        isExistSecuritiesFinancialInstruments = false;
        isExistSecurityDeposit = false;
        $("#tblSecurityList tbody tr").each(function () {
            var securityvalue2 = $(this).find(".securityvalue2").val();
            if (securityvalue2 === ConstGuarantor) {
                iSExistGuarantor = true;
            }
            if (securityvalue2 === ConstMortgageProperty) {
                iSExistMortgageProperty = true;
            }
            if (securityvalue2 === ConstMortgageVessel) {
                iSExistMortgageVessel = true;
            }
            if (securityvalue2 === ConstDebentureVehicle) {
                isExistDebentureVehicle = true;
            }
            if (securityvalue2 === ConstDebentureConstructionEquipment) {
                isExistDebentureConstructionEquipment = true;
            }
            if (securityvalue2 === ConstDebentureIndustrialEquipment) {
                isExistDebentureIndustrialEquipment = true;
            }
            if (securityvalue2 === ConstDebentureInventories) {
                isExistDebentureInventory = true;
            }
            if (securityvalue2 === ConstDebentureReceivables) {
                isExistDebentureReceivable = true;
            }
            if (securityvalue2 === CashEquivalentIndividual) {
                isExistDebentureEquivalentIndividual = true;
            }
            if (securityvalue2 === CashEquivalentCompany) {
                isExistDebentureEquivalentCompany = true;
            }
            if (securityvalue2 === SecFinInstruments) {
                isExistSecuritiesFinancialInstruments = true;
            }
            if (securityvalue2 === SecurityDeposit) {
                isExistSecurityDeposit = true;
            }
        });
        if (iSExistGuarantor || val === "Guarantor") {
            $("#btnAddIndividualGuarantorList").removeClass("hide");
            $("#btnAddCorporateGuarantorList").removeClass("hide");
        }
        else {
            $("#btnAddIndividualGuarantorList").addClass("hide");
            $("#btnAddCorporateGuarantorList").addClass("hide");
            $('#tblIndividualGuarantorList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
            $('#tblCorporateGuarantorList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
        }

        if (iSExistMortgageProperty || val === "Property") {
            $("#btnAddMortgagePropertyList").removeClass("hide");
        }
        else {
            $("#btnAddMortgagePropertyList").addClass("hide");

            $('#tblMortgagePropertyList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
            calcTotalLTV('tblMortgagePropertyList');
        }

        if (iSExistMortgageVessel || val === "Vessel") {
            $("#btnAddMortgageVesselList").removeClass("hide");
        }
        else {
            $("#btnAddMortgageVesselList").addClass("hide");

            $('#tblMortgageVesselList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
            calcTotalLTV('tblMortgageVesselList');
        }

        if (isExistDebentureVehicle || val === "Vehicle") {
            $("#btnAddDebentureVehicleList").removeClass("hide");
        }
        else {
            $("#btnAddDebentureVehicleList").addClass("hide");

            $('#tblDebentureVehicleList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
        }

        if (isExistDebentureConstructionEquipment || val === "Construction Equipment") {
            $("#btnAddDebentureConstructionEquipmentList").removeClass("hide");
        }
        else {
            $("#btnAddDebentureConstructionEquipmentList").addClass("hide");

            $('#tblDebentureConstructionEquipmentList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
        }
        if (isExistDebentureIndustrialEquipment || val === "Industrial Equipment") {
            $("#btnAddDebentureIndustrialEquipmentList").removeClass("hide");
        }
        else {
            $("#btnAddDebentureIndustrialEquipmentList").addClass("hide");

            $('#tblDebentureIndustrialEquipmentList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
        }

        if (isExistDebentureInventory || val === "Inventories") {
            $("#btnAddDebentureInventoryList").removeClass("hide");
        }
        else {
            $("#btnAddDebentureInventoryList").addClass("hide");

            $('#tblDebentureInventoryList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
        }

        if (isExistDebentureReceivable || val === "Receivables") {
            $("#btnAddDebentureReceivableList").removeClass("hide");
        }
        else {
            $("#btnAddDebentureReceivableList").addClass("hide");

            $('#tblDebentureReceivableList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
        }

        if (isExistDebentureEquivalentIndividual || val === "Cash & Equivalent (Individual)") {
            $("#btnAddDebentureCashAndEquivalentIndividualList").removeClass("hide");
        }
        else {
            $("#btnAddDebentureCashAndEquivalentIndividualList").addClass("hide");

            $('#tblDebentureCashAndEquivalentIndividualList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
        }

        if (isExistDebentureEquivalentCompany || val === "Cash & Equivalent (Company)") {
            $("#btnAddDebentureCashAndEquivalentCompanyList").removeClass("hide");
        }
        else {
            $("#btnAddDebentureCashAndEquivalentCompanyList").addClass("hide");

            $('#tblDebentureCashAndEquivalentCompanyList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
        }

        if (isExistSecuritiesFinancialInstruments || val === "Securities/ Financial Instruments") {
            $("#btnAddSecuritiesFinancialInstruments").removeClass("hide");
        }
        else {
            $("#btnAddSecuritiesFinancialInstruments").addClass("hide");

            $('#tblSecuritiesFinancialInstruments tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
        }

        if (isExistSecurityDeposit || val === "Security Deposit") {
            $("#btnAddSecurityDepositList").removeClass("hide");
        }
        else {
            $("#btnAddSecurityDepositList").addClass("hide");

            $('#tblSecurityDepositList tbody tr').each(function () {
                $(this).find('.deleting').click();
            });
        }
    };
    window.checkSecurityLevel2 = function (e) {
        var value = $(e).val();
        $(e).closest("tr").find('.securityvalue2').val(value);
        enableDisableSecurityTable("");
        var SubConGroupCode = (new URL(location.href)).searchParams.get('SubConGroupCode');
        if (value !== '' && value !== undefined) {
            $.ajax({
                url: varSitePath + '/PreCon/CheckSecurityLevel2IsValid',
                data: { value: value, SubConGroupCode: SubConGroupCode },
                type: 'Get',
                success: function (Result) {
                    if (Result !== "") {
                        swal(swalGlobal.SwalTitle_Error, Result, swalGlobal.SwalType_Error);
                        //$(e).prop("disabled", true);
                        $(e).val("");
                        isSecurityValidRow(event);
                    }
                    else {
                        //$(e).prop("disabled", false);
                    }
                },
                error: function (data) {
                }
            });
        }
    };
    //Individual Guarantor List	
    window.tblIndividualGuarantorList = $("#tblIndividualGuarantorList").DataTable({
        "paging": false,
        "info": false,
        "searching": false,
        "language": {
            "zeroRecords": "No Data Available.",
            "infoEmpty": "No Data Available."
        },
        "dom": '<"top"i>rt<"bottom"flp><"clear">'
    });
    var appendIndividualGuarantorRow = {
        "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
        "Name": "", "Address": "", "Department": "", "ContactPerson": "",
        "LetterType": "", "NRICFINPASSPORT": "",
        "MobileNo": "", "Email": "", "OfficeNo": "", "HomeNo": "", "FaxNo": "", "PagerNo": ""
    };
    //$("#tblIndividualGuarantorList tbody").on('click', 'tr', function (event) {
    //    setGuarantorListActiveRow(this);
    //});
    function isGuarantorExist(table, selectedValue) {
        var isDuplicate = false;
        if (selectedValue) {
            $("#" + table + " tbody tr").each(function () {
                var currentRowValue = $(this).find(".name").attr("data-id");
                if (currentRowValue) {
                    if (currentRowValue === selectedValue && !$(this).hasClass('activeRow')) {
                        swal(swalGlobal.SwalTitle_Error, "Guarantor Name already selected!", swalGlobal.SwalType_Error);
                        isDuplicate = true;
                    }
                }
            });
        }
        return isDuplicate;
    }

    window.ResetGuarantorList = function (IndividualOrCorporate) {
        var activeRow = $("#tbl" + IndividualOrCorporate + "GuarantorList tr.activeRow");
        activeRow.find('.address').html("<option disabled selected value> -- Select -- </option>");
        activeRow.find('.department').html("<option disabled selected value> -- Select -- </option>");
        activeRow.find('.contactPerson').html("<option disabled selected value> -- Select -- </option>");
        activeRow.find('.NRICFINPASSPORT').val('');
        activeRow.find('.RocUenType').val('');
        activeRow.find('.mobile').val('');
        activeRow.find('.email').val('');
        activeRow.find('.officeno').val('');
        activeRow.find('.homeno').val('');
        activeRow.find('.faxno').val('');
        activeRow.find('.pagerno').val('');
    };
    window.setGuarantorListActiveRow = function (row) {
        if (row !== null) {
            $(row).addClass('activeRow');
            //Guatentor autocomplete
            $('.activeRow .name').autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: varSitePath + '/PreCon/GetGuarantorNameAutoComplete',
                        data: { textFilter: request.term, IndividualorCorporate: "Individual" },
                        type: 'GET',
                        cache: false,
                        async: false,
                        dataType: 'JSON',
                        global: false,
                        success: function (json) {
                            var res = JSON.parse(json.data);
                            $(res).each(function (i, v) {
                                v.label = v.label.replace("&#39", '\'').replace("&#34", "\"").replace("&#12296", "<").replace("&#12297", ">");
                            });
                            response(res);
                        }
                    });
                },
                minLength: 1,
                scroll: true,
                autoFocus: false,
                autoFill: false,
                focus: function (event, ui) {
                    return false;
                },
                select: function (event, ui) {
                    var el = $(row).find('.name');
                    el.val(ui.item.label);
                    el.attr("data-id", ui.item.value);
                    return false;
                },
                change: function (event, ui) {
                    var el = $(row).find('.name');
                    if (ui.item) {
                        el.val(ui.item.label);
                        el.attr("data-id", ui.item.value);
                    }
                    else {
                        //$('.name').val('');
                        //el.attr("data-id", '');
                        //$(this).removeClass('alert-danger');
                    }
                    return false;
                }
            });
        }
    };
    function initializeNewRowIndividualGuarantorList() {
        //appendSecurityRow.SecurityList1stLevel = "<input style='width:200px !important' class='editor securityList1stLevel' type='text' tabindex='1' value=''><span class='edited securityList1stLevel_edited'></span>";
        //appendSecurityRow.SecurityList2ndLevel = "<input style='width:200px !important' class='editor securityList2ndLevel' type='text' tabindex='2' value=''><span class='edited securityList2ndLevel_edited'></span>";
        if (dropdownIndividualGuarantornameListValue) {
            //var name = "<select class='editor name js-example-basic-sec2' tabindex='1' onchange='setGuarantorAddress(this,\"@ViewBag.Individual\")'>";
            var name = "<input data-id='' class='editor name unrequired ui-autocomplete-input' type='text' autocomplete='off'><span class='snpname edited'></span>";
            var address = "<select name='Guarantoraddress' class='editor address js-example-basic-sec2' tabindex='2' onchange='setGuarantorDepartment(this)'>";
            var department = "<select name='Guarantordepartment' class='editor department js-example-basic-sec2' tabindex='3' onchange='setGuarantorContactPerson(this)'>";
            var contactPerson = "<select name='GuarantorcontactPerson' class='editor contactPerson js-example-basic-sec2' tabindex='4'>";
            var letterType = "<select class='editor letterType js-example-basic-sec2' tabindex='5'>";
            var NRICFINPASSPORT = "<input class='editor NRICFINPASSPORT unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var mobile = "<input class='editor mobile unrequired unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var email = "<input class='editor email' type='text' disabled='disabled'><span class='edited'></span>";
            var officeno = "<input class='editor officeno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var homeno = "<input class='editor homeno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var faxno = "<input class='editor faxno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var pagerno = "<input class='editor pagerno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            appendIndividualGuarantorRow.Name = name;
            address += "<option disabled selected value> -- Select -- </option>";
            address += "</select><span class='edited'></span>";
            appendIndividualGuarantorRow.Address = address;
            department += "<option disabled selected value> -- Select -- </option>";
            department += "</select><span class='edited'></span>";
            appendIndividualGuarantorRow.Department = department;
            contactPerson += "<option disabled selected value> -- Select -- </option>";
            contactPerson += "</select><span class='edited'></span>";
            appendIndividualGuarantorRow.ContactPerson = contactPerson;
            letterType += "<option value> -- Select -- </option>";
            $.each(dropdownLetterType, function (i, v) {
                letterType += "<option value = " + v.Value + ">" + v.Text + "</option>";
            })
            letterType += "</select><span class='edited'></span>";
            appendIndividualGuarantorRow.LetterType = letterType;
            appendIndividualGuarantorRow.NRICFINPASSPORT = NRICFINPASSPORT;
            appendIndividualGuarantorRow.MobileNo = mobile;
            appendIndividualGuarantorRow.Email = email;
            appendIndividualGuarantorRow.OfficeNo = officeno;
            appendIndividualGuarantorRow.HomeNo = homeno;
            appendIndividualGuarantorRow.FaxNo = faxno;
            appendIndividualGuarantorRow.PagerNo = pagerno;
        }
        else {
            //appendIndividualGuarantorRow.Name = "<select id='selectName' class='editor' tabindex='1'></select><span class='edited'></span>";
            appendIndividualGuarantorRow.Name = "<input id='selectName' class='editor name unrequired' type='text'></input><span class='edited'></span>";
        }
        setActiveRowIndividualGuarantorList($("#tblIndividualGuarantorList tr:last"));
    }

    $("#btnAddIndividualGuarantorList").on("click", function () {
        if (tblIndividualGuarantorList.rows().count() == 0) {
            initializeNewRowIndividualGuarantorList();
        }
        if (isIndividualGuarantorValidRow()) {
            var _row = tblIndividualGuarantorList.rows.add([[
				appendIndividualGuarantorRow.Action,
				appendIndividualGuarantorRow.Name,
				appendIndividualGuarantorRow.Address,
				appendIndividualGuarantorRow.Department,
				appendIndividualGuarantorRow.ContactPerson,
				appendIndividualGuarantorRow.LetterType,
				appendIndividualGuarantorRow.NRICFINPASSPORT,
				appendIndividualGuarantorRow.MobileNo,
				appendIndividualGuarantorRow.Email,
				appendIndividualGuarantorRow.OfficeNo,
				appendIndividualGuarantorRow.HomeNo,
				appendIndividualGuarantorRow.FaxNo,
				appendIndividualGuarantorRow.PagerNo
            ]]);
            _row.draw();
            setletterType($("#tblIndividualGuarantorList tr:last"));
            setActiveRowIndividualGuarantorList($("#tblIndividualGuarantorList tr:last"));

        }
    });
    $('#tblIndividualGuarantorList tbody').on('click', 'button.deleting', function () {
        tblIndividualGuarantorList.row($(this).parents("tr")).remove().draw();
        $('#fldIndividualGuarantorList').removeClass('error-div');
    });
    $("#tblIndividualGuarantorList tbody").on('click', 'tr', function (event) {
        //targetRowEventIndividualGuarantorList(event);
        if (!($(this).hasClass("activeRow"))) {
            if (isIndividualGuarantorValidRow())
                setActiveRowIndividualGuarantorList(this);
        }
    });
    window.targetRowEventIndividualGuarantorList = function (event) {
        var currentTab = $('.nav-tabs').find('li.active a').text();
        if (
			currentTab == 'Security' &&
			!$(event.target).closest('#tblIndividualGuarantorList tbody tr.activeRow').length &&
			!$(event.target).closest('#tblIndividualGuarantorList thead').length &&
			!$(event.target).closest('.sweet-alert').length &&
			!$(event.target).closest('.ui-autocomplete').length &&
			!$(event.target).closest('#btnAddIndividualGuarantorList').length &&
			!$(event.target).closest('#tblIndividualGuarantorList').length &&
			!$(event.target).closest('button.deleting').length) {
            //if (!isRowChanged()) {
            //	$('#tblSecurityList tr.activeRow').find('button.deleting').click();
            //}
            //else if (true) {
            if (isIndividualGuarantorValidRow())
                setActiveRowIndividualGuarantorList(null);
            //}
            //setletterType(null);
        }
    };
    window.isIndividualGuarantorValidRow = function () {

        var activeRow = $("#tblIndividualGuarantorList tr.activeRow");
        var isValid = false;
        if (activeRow && activeRow.length > 0) {
            $(activeRow[0]).find("select").each(function () {
                if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                    if (!$(this).val() || $(this).val() == "--Select--") {
                        $(this).addClass("alert-danger");
                        if ($(this).hasClass("js-example-basic-sec2")) {
                            $(this).next().find('.select2-selection').addClass("selection-danger");
                        }
                    }
                    else {
                        $(this).removeClass("alert-danger");
                        if ($(this).hasClass('js-example-basic-sec2')) {
                            $(this).next().find('.select2-selection').removeClass("selection-danger");
                        }
                    }
                }
            });
        }
        else { return true; }
        isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;
				if (!isValid) {
            $('#fldIndividualGuarantorList').addClass('error-div');
            //swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
        }
				else {			
					$('#fldIndividualGuarantorList').removeClass('error-div');
				}
        checkFieldValid();
        return isValid;
    }
    window.setActiveRowIndividualGuarantorList = function (row) {
        $("#tblIndividualGuarantorList tr").each(function () {
            if ($(this).hasClass('activeRow')) {
                $(this).find(":input").each(function () {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec2')) {
                        DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text()
                        $(this).next().remove();
                    }
                    if ($(this).val() == "") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec2')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                });
                $(this).removeClass("activeRow");
            }
        });
        if (row) {
            $(row).addClass('activeRow');
            //setletterType(row);
            $('.activeRow .name').autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: varSitePath + '/PreCon/GetGuarantorNameAutoComplete',
                        data: { textFilter: request.term, IndividualorCorporate: "Individual" },
                        type: 'GET',
                        cache: false,
                        async: false,
                        dataType: 'JSON',
                        global: false,
                        success: function (json) {
                            var res = JSON.parse(json.data);
                            $(res).each(function (i, v) {
                                v.label = v.label.replace("&#39", '\'').replace("&#34", "\"").replace("&#12296", "<").replace("&#12297", ">");
                            });
                            response(res);
                        }
                    });
                },
                minLength: 1,
                scroll: true,
                autoFocus: false,
                autoFill: false,
                focus: function (event, ui) {
                    return false;
                },
                select: function (event, ui) {
                    var el = $(row).find('.name');
                    el.attr("data-id", ui.item.value);
                    el.val(ui.item.label);
                    setGuarantorAddress(this, "Individual");
                    return false;
                },
                change: function (event, ui) {
                    //var el = $(row).find('.name');
                    if (!ui.item) {
                        //el.val(ui.item.label);
                        //el.attr("data-id", ui.item.value);
                        //setGuarantorAddress(this, "Individual");
                        ResetGuarantorList("Individual");
                    }
                    //else {
                    //	//$('.name').val('');
                    //	//el.attr("data-id", '');
                    //	//$(this).removeClass('alert-danger');
                    //}
                    return false;
                }
            });
        }
        $('#tblIndividualGuarantorList .activeRow .js-example-basic-sec2').select2();
    };




    window.setletterType = function (e) {
        var indVal = $('#ddlIndividualCorporate').val();
        var indValCode = "";
        var corValCode = "";
        if (indVal === "Individual") {
            indValCode = "LT-1000";
            corValCode = "LT-1001";
        }
        else if (indVal === "Corporate") {
            indValCode = "LT-1002";
            corValCode = "LT-1003";
        }
        if (e) {
            var table = $(e).closest('table').attr('id');
            var currentLetterType = $(e).find('.letterType');
            //if (table === 'tblIndividualGuarantorList') {
            //	currentLetterType.val(indValCode);
            //}
            //if (table === 'tblCorporateGuarantorList') {
            //	currentLetterType.val(corValCode);
            //}
            if (table.includes("Individual")) {
                currentLetterType.val(indValCode);
            }
            if (table.includes("Corporate")) {
                currentLetterType.val(corValCode);
            }
            $(e).find(".select2-selection__rendered").val(currentLetterType.text());
        }
        else {
            $("#tblIndividualGuarantorList tbody tr").each(function () {
                var txt = $(this).find(".letterType option[value='" + indValCode + "']").text();
                $(this).find('.letterType').val(indValCode);
                $(this).find('.letterType').next('.edited').html(txt);
            });
            $("#tblCorporateGuarantorList tbody tr").each(function () {
                var txt = $(this).find(".letterType option[value='" + corValCode + "']").text();
                $(this).find('.letterType').val(corValCode);
                $(this).find('.letterType').next('.edited').html(txt);
            });

            $("#tblBuyBackIndividualGuarantorList tbody tr").each(function () {
                var txt = $(this).find(".letterType option[value='" + indValCode + "']").text();
                $(this).find('.letterType').val(indValCode);
                $(this).find('.letterType').next('.edited').html(txt);
            });
            $("#tblBuyBackCorporateGuarantorList tbody tr").each(function () {
                var txt = $(this).find(".letterType option[value='" + corValCode + "']").text();
                $(this).find('.letterType').val(corValCode);
                $(this).find('.letterType').next('.edited').html(txt);
            });
            $("#tblRecourseIndividualGuarantorList tbody tr").each(function () {
                var txt = $(this).find(".letterType option[value='" + indValCode + "']").text();
                $(this).find('.letterType').val(indValCode);
                $(this).find('.letterType').next('.edited').html(txt);
            });
            $("#tblRecourseCorporateGuarantorList tbody tr").each(function () {
                var txt = $(this).find(".letterType option[value='" + corValCode + "']").text();
                $(this).find('.letterType').val(corValCode);
                $(this).find('.letterType').next('.edited').html(txt);
            });
        }
    };
    window.setGuarantorAddress = function (e, Individual_Corporate) {

        var table = $(e).closest('table').attr('id');
        var selectedNameCode = $(e).attr("data-id");
        if (!isGuarantorExist(table, selectedNameCode)) {
            var IndividualCorporate = Individual_Corporate;
            var options = "";
            if (selectedNameCode != '' && selectedNameCode != undefined) {
                $.ajax({
				    url: varSitePath+'/Security/getAddress',
                    data: { selected: selectedNameCode, IndividualCorporate: IndividualCorporate },
                    type: 'Get',
                    async: false,
                    global: false,
                    success: function (Result) {
                        //var res = JSON.parse(Result.data);
                        $.each(Result.data, function (i, cat) {
                            options += "<option value = '" + cat.AddressId + "'>" + cat.Address + "</option>";
                        });
                        if (e) {
                            var currentAddress = $(e).closest("tr").find('.address');
                            currentAddress.html(options);
                            setGuarantorDepartment(currentAddress);
                            GetNricFinPassportForGuarantor(e);
                            GetRocUenTypeForGuarantor(e);
                        }
                    },
                    error: function (data) {
                    }
                });
            }
        }
        else {
            $(e).val('');
            $(e).attr("data-id", '');
            ResetGuarantorList(Individual_Corporate);
        }
    };
    window.setGuarantorDepartment = function (e) {
        var selectedAddress = $(e).val();
        var options = "";
        if (selectedAddress != '' && selectedAddress != undefined) {
            $.ajax({
			    url: varSitePath+'/Security/getDepartmentList',
                data: { selectedAddress: selectedAddress },
                type: 'Get',
                async: false,
                global: false,
                success: function (Result) {
                    $.each(Result.data, function (i, cat) {
                        options += "<option value = '" + cat.cd_ref_num + "'>" + cat.cd_dept_desc + "</option>";
                    });
                    if (e) {
                        var currentDepartment = $(e).closest("tr").find('.department');
                        currentDepartment.html(options);
                        setGuarantorContactPerson(currentDepartment);
                    }
                },
                error: function (data) {
                }
            });
        }
    }
    window.setGuarantorContactPerson = function (e) {
        var selectedDepartment = $(e).val();
        var options = "";
        if (selectedDepartment != '' && selectedDepartment != undefined) {
            $.ajax({
			    url: varSitePath+'/Security/getContactPerson',
                data: { selectedDepartment: selectedDepartment },
                type: 'Get',
                success: function (Result) {
                    $.each(Result.data, function (i, cat) {
                        options += "<option value = '" + cat.Value + "'>" + cat.Contact + "</option>";
                    });
                    if (e) {
                        var currentContactPerson = $(e).closest("tr").find('.contactPerson');
                        currentContactPerson.html(options);
											GetContactPersonDetails(e);

											if ($(e).closest("table").attr("Id") == "tblRecourseCorporateGuarantorList"
												|| $(e).closest("table").attr("Id") == "tblRecourseIndividualGuarantorList"
												|| $(e).closest("table").attr("Id") == "tblBuyBackIndividualGuarantorList"
												|| $(e).closest("table").attr("Id") == "tblBuyBackCorporateGuarantorList") {
												AddUpdateGuarantor();
												}
                        //GetMobileNumberForGuarantor(e);
                        //GetEmailForGuarantor(e);
                        //GetOfficeNumberForGuarantor(e);
                        //GetHomeNumberForGuarantor(e);
                        //GetFaxNumberForGuarantor(e);
                        //GetPagerNumberForGuarantor(e);
                    }
                },
                error: function (data) {
                }
            });
        }
    };
    //window.GetLetterTypeForGuarantor = function () {
    //	var options = "";
    //	$.ajax({
    //		url: "/PreCon/GetLetterType",
    //		type: 'GET',
    //		cache: false,
    //		dataType: 'json',
    //		global: false,
    //		success: function (json) {
    //			dropdownLetterType = json.data;
    //		}
    //	});
    //};
    window.GetContactPersonDetails = function (e) {
        var currentRow = $(e).closest("tr");
        var currentContactPerson = $(e).closest("tr").find('.contactPerson').val();
        $.ajax({
            url: varSitePath + "/PreCon/GetContactPersonDetails",
            data: { selectedContactPerson: currentContactPerson },
            type: 'GET',
            cache: false,
            dataType: 'json',
            global: false,
            success: function (json) {
                //currentMobile.val(json.data);
                var res = json.data;
                currentRow.find('.mobile').val(res.Mobile);
                currentRow.find('.email').val(res.Email);
                currentRow.find('.officeno').val(res.OfficeNumber);
                currentRow.find('.homeno').val(res.HomeNumber);
                currentRow.find('.faxno').val(res.FaxNumber);
                currentRow.find('.pagerno').val(res.PagerNumber);
            }
        });
    };
    function GetNricFinPassportForGuarantor(e) {
        var currentNricFinPassport = $(e).closest("tr").find('.NRICFINPASSPORT');
        var currentName = $(e).closest("tr").find('.name').attr("data-id");
        $.ajax({
            url: varSitePath + '/PreCon/getNricFinPassportType',
            data: { selected: currentName },
            type: 'GET',
            cache: false,
            async: false,
            dataType: 'json',
            global: false,
            success: function (json) {
                currentNricFinPassport.val(json.data);
            }
        });
    }
    function GetRocUenTypeForGuarantor(e) {
        var currentRocUenType = $(e).closest("tr").find('.RocUenType');
        var currentNameCode = $(e).closest("tr").find('.name').attr("data-id");
        $.ajax({
            url: varSitePath + '/PreCon/getRocUenType',
            data: { selected: currentNameCode },
            type: 'GET',
            cache: false,
            async: false,
            dataType: 'json',
            global: false,
            success: function (json) {
                currentRocUenType.val(json.data);
            }
        });
    }
    window.tblCorporateGuarantorList = $("#tblCorporateGuarantorList").DataTable({
        "paging": false,
        "info": false,
        "searching": false,
        "language": {
            "zeroRecords": "No Data Available.",
            "infoEmpty": "No Data Available."
        },
        "dom": '<"top"i>rt<"bottom"flp><"clear">'
    });
    var appendCorporateGuarantorRow = {
        "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
        "Name": "", "Address": "", "Department": "", "ContactPerson": "",
        "LetterType": "", "RocUenType": "",
        "MobileNo": "", "Email": "", "OfficeNo": "", "HomeNo": "", "FaxNo": "", "PagerNo": ""
    };
    function initializeNewRowCorporateGuarantorList() {
        if (dropdownCorporateGuarantornameListValue) {
            //var name = "<select id='selectName' class='editor name js-example-basic-sec3' tabindex='1'>";
            var name = "<input data-id='' class='editor name unrequired ui-autocomplete-input' type='text' autocomplete='off'><span class='snpname edited'></span>";
            var address = "<select id='selectAddress' class='editor address js-example-basic-sec3' tabindex='2' onchange='setGuarantorDepartment(this)'>";
            var department = "<select id='selectDepartment' class='editor department js-example-basic-sec3' tabindex='3' onchange='setGuarantorContactPerson(this)'>";
            var contactPerson = "<select id='selectContactPerson' class='editor contactPerson js-example-basic-sec3' tabindex='4'>";
            var letterType = "<select id='selectLetterType' class='editor letterType js-example-basic-sec3' tabindex='5'>";
            var RocUenType = "<input class='editor RocUenType unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var mobile = "<input class='editor mobile unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var email = "<input class='editor email unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var officeno = "<input class='editor officeno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var homeno = "<input class='editor homeno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var faxno = "<input class='editor faxno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            var pagerno = "<input class='editor pagerno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
            //var jsonParseData = JSON.parse(dropdownCorporateGuarantornameListValue);//.Data;
            //name += "<option disabled selected value> -- Select -- </option>";
            //$.each(jsonParseData, function (i, v) {
            //    name += "<option value = " + v.value + ">" + v.label + "</option>";
            //})
            //name += "</select><span class='edited'></span>";
            appendCorporateGuarantorRow.Name = name;
            address += "<option disabled selected value> -- Select -- </option>";
            address += "</select><span class='edited'></span>";
            appendCorporateGuarantorRow.Address = address;
            department += "<option disabled selected value> -- Select -- </option>";
            department += "</select><span class='edited'></span>";
            appendCorporateGuarantorRow.Department = department;
            contactPerson += "<option disabled selected value> -- Select -- </option>";
            contactPerson += "</select><span class='edited'></span>";
            appendCorporateGuarantorRow.ContactPerson = contactPerson;
            letterType += "<option value> -- Select -- </option>";
            $.each(dropdownLetterType, function (i, v) {
                letterType += "<option value = " + v.Value + ">" + v.Text + "</option>";
            })
            letterType += "</select><span class='edited'></span>";
            appendCorporateGuarantorRow.LetterType = letterType;
            appendCorporateGuarantorRow.RocUenType = RocUenType;
            appendCorporateGuarantorRow.MobileNo = mobile;
            appendCorporateGuarantorRow.Email = email;
            appendCorporateGuarantorRow.OfficeNo = officeno;
            appendCorporateGuarantorRow.HomeNo = homeno;
            appendCorporateGuarantorRow.FaxNo = faxno;
            appendCorporateGuarantorRow.PagerNo = pagerno;
        }
        else {
            appendCorporateGuarantorRow.Name = "<input id='selectName' class='editor name unrequired' type='text'></input><span class='edited'></span>";
        }
        setActiveRowCorporateGuarantorList($("#tblCorporateGuarantorList tr:last"));
    }
    $("#btnAddCorporateGuarantorList").on("click", function () {
        if (tblCorporateGuarantorList.rows().count() == 0) {
            initializeNewRowCorporateGuarantorList();
        }
        if (isCorporateGuarantorValidRow()) {
            var _row = tblCorporateGuarantorList.rows.add([[
				appendCorporateGuarantorRow.Action,
				appendCorporateGuarantorRow.Name,
				appendCorporateGuarantorRow.Address,
				appendCorporateGuarantorRow.Department,
				appendCorporateGuarantorRow.ContactPerson,
				appendCorporateGuarantorRow.LetterType,
				appendCorporateGuarantorRow.RocUenType,
				appendCorporateGuarantorRow.MobileNo,
				appendIndividualGuarantorRow.Email,
				appendCorporateGuarantorRow.OfficeNo,
				appendCorporateGuarantorRow.HomeNo,
				appendCorporateGuarantorRow.FaxNo,
				appendCorporateGuarantorRow.PagerNo
            ]]);
            _row.draw();
            setletterType($("#tblCorporateGuarantorList tr:last"));
            setActiveRowCorporateGuarantorList($("#tblCorporateGuarantorList tr:last"));
        }
    });
    $('#tblCorporateGuarantorList tbody').on('click', 'button.deleting', function () {
        tblCorporateGuarantorList.row($(this).parents("tr")).remove().draw();
        $('#fldCorporateGuarantorList').removeClass('error-div');
    });
    $("#tblCorporateGuarantorList tbody").on('click', 'tr', function (event) {
        targetRowEventCorporateGuarantorList(event);
        if (!($(this).hasClass("activeRow"))) {
            if (isCorporateGuarantorValidRow())
                setActiveRowCorporateGuarantorList(this);
        }
    });
    window.targetRowEventCorporateGuarantorList = function (event) {
        var currentTab = $('.nav-tabs').find('li.active a').text();
        if (
			currentTab === 'Security' &&
			!$(event.target).closest('#tblCorporateGuarantorList tbody tr.activeRow').length &&
			!$(event.target).closest('#tblCorporateGuarantorList thead').length &&
			!$(event.target).closest('.ui-autocomplete').length &&
			!$(event.target).closest('.sweet-alert').length &&
			!$(event.target).closest('#btnAddCorporateGuarantorList').length &&
			!$(event.target).closest('#tblCorporateGuarantorList').length &&
			!$(event.target).closest('button.deleting').length) {
            if (isCorporateGuarantorValidRow())
                setActiveRowCorporateGuarantorList(null);
            //setletterType(null);
        }
    };

    function isCorporateGuarantorValidRow() {
        var activeRow = $("#tblCorporateGuarantorList tr.activeRow");
        var isValid = false;
        if (activeRow && activeRow.length > 0) {
            $(activeRow[0]).find("select").each(function () {
                if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {

                    if (!$(this).val() || $(this).val() == "--Select--") {
                        $(this).addClass("alert-danger");
                        if ($(this).hasClass("js-example-basic-sec3")) {
                            $(this).next().find('.select2-selection').addClass("selection-danger");
                        }
                    }
                    else {
                        $(this).removeClass("alert-danger");
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next().find('.select2-selection').removeClass("selection-danger");
                        }
                    }

                }
            });
        }
        else { return true; }
        isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;
        if (!isValid) {
            $('#fldCorporateGuarantorList').addClass('error-div');
            //swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
        }
        else {
            $('#fldCorporateGuarantorList').removeClass('error-div');
        }
        checkFieldValid();
        return isValid;
    }

    window.setActiveRowCorporateGuarantorList = function (row) {
        $("#tblCorporateGuarantorList tr").each(function () {
            if ($(this).hasClass('activeRow')) {
                $(this).find(":input").each(function () {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text()
                        $(this).next().remove();
                    }
                    if ($(this).val() === "") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                });
                $(this).removeClass("activeRow");
            }
        });
        if (row) {
            $(row).addClass('activeRow');
            //setletterType(row);
            $('.activeRow .name').autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: varSitePath + '/PreCon/GetGuarantorNameAutoComplete',
                        data: { textFilter: request.term, IndividualorCorporate: "Corporate" },
                        type: 'GET',
                        cache: false,
                        async: false,
                        dataType: 'JSON',
                        global: false,
                        success: function (json) {
                            var res = JSON.parse(json.data);
                            $(res).each(function (i, v) {
                                v.label = v.label.replace("&#39", '\'').replace("&#34", "\"").replace("&#12296", "<").replace("&#12297", ">");
                            });
                            response(res);
                        }
                    });
                },
                minLength: 1,
                scroll: true,
                autoFocus: false,
                autoFill: false,
                focus: function (event, ui) {
                    return false;
                },
                select: function (event, ui) {
                    var el = $(row).find('.name');
                    el.val(ui.item.label);
                    el.attr("data-id", ui.item.value);
                    setGuarantorAddress(this, "Corporate");
                    return false;
                },
                change: function (event, ui) {
                    //var el = $(row).find('.name');
                    if (!ui.item) {
                        //el.val(ui.item.label);
                        //el.attr("data-id", ui.item.value);
                        //setGuarantorAddress(this, "Corporate");
                        ResetGuarantorList("Corporate");
                    }
                    //else {
                    //	//$('.name').val('');
                    //	//el.attr("data-id", '');
                    //	//$(this).removeClass('alert-danger');
                    //}					
                    return false;
                }
            });
        }
        $('#tblCorporateGuarantorList .activeRow .js-example-basic-sec3').select2();
    }

});


window.tblMortgagePropertyList = $("#tblMortgagePropertyList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">',
    "destroy": true
});

var appendMortgagePropertyRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
    "ExpandCollapse": "<img class='iconexpand' src='" + varSitePath + "/images/Plus.png' onclick='MortgageProperty(this)'/>",
    "IndividualCorporate": "",
    "CustomerThirdParty": "", "PropetyAddress": "", "CreditLimit": "", "IndicativeValuationAmount": "",
    "LoanAmountProportion": "", "LTV": "", "FirstThirdParty": "", "PropertyTypeLevel1": "", "PropertyTypeLevel2": "",
    "FormalValuation": "", "TitleNumber": "", "MortgageNumber": "",
    "ChargeNumber": "", "ChargeDate": "", "Status": ""
};

function initializeNewRowMortgagePropertyList() {
    var ExpandCollapse = "<img src='" + varSitePath + "/images/Plus.png' class='iconexpand' onclick='BindMortgagePropertySubGrid(this)'/><span class='edited'></span>";
    //var IndividualCorporate = "<select class='editor individualcorporate js-example-basic-sec3' onchange='setCustomerThirdParty(\"tblMortgagePropertyList\")' tabindex='3'>";
    var IndividualCorporate = "<select class='editor individualcorporate js-example-basic-sec3' onchange='setCustomerThirdParty(\"tblMortgagePropertyList\")' tabindex='3'>";
    var CustomerThirdParty = "<input data-id='' onfocus='setTableNameForCustomerThirdParty(this)' class='editor customerthirdparty unrequired ui-autocomplete-input' type='text' autocomplete='off'><span class='ctparty edited'></span>";
    //var CustomerThirdParty = "<select class='editor customerthirdparty js-example-basic-sec3' onchange='setPropetyAddress()' tabindex='4'>";
    var PropetyAddress = "<select class='editor propetyaddress unrequired js-example-basic-sec3' onchange=setValueChangeOfPropetyAddress('propetyaddress') tabindex='4'>";
    var CreditLimit = "<input class='editor creditlimit unrequired decimal money' type='text' disabled='disabled'><span class='edited'></span>";
    var IndicativeValuationAmount = "<input class='editor indicativevaluationamount unrequired decimal money' value='0.00' min='1' type='text' disabled='disabled'><span class='edited'></span>";
    var LoanAmountProportion = "<input class='editor loanamountproportion decimal money' type='text' value='0.00' min='1' onblur='calcLTV(\"tblMortgagePropertyList\")'><span class='edited'></span>";
    var LTV = "<input class='editor ltv decimal percentage' type='text' value='0.0000'><span class='edited'></span>";
    var FirstThirdParty = "<input class='editor firstthirdparty unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var PropertyTypeLevel1 = "<input class='editor propertytypelevel1 unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var PropertyTypeLevel2 = "<input class='editor propertytypelevel2 unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var FormalValuation = "<input class='editor formalvaluation unrequired decimal money' type='text' disabled='disabled'><span class='edited'></span>";
    var TitleNumber = "<input class='editor titlenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var MortgageNumber = "<input class='editor mortgagenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeNumber = "<input class='editor chargenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeDate = "<input class='editor chargedate unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var Status = "<input class='editor status unrequired' type='text' disabled='disabled'><span class='edited'></span>";

    appendMortgagePropertyRow.ExpandCollapse = ExpandCollapse;        

    IndividualCorporate += "<option>Individual</option><option>Corporate</option>";
    IndividualCorporate += "</select><span class='edited'></span>";
    appendMortgagePropertyRow.IndividualCorporate = IndividualCorporate;

    //CustomerThirdParty += "<option disabled selected> -- Select -- </option></select>";
    CustomerThirdParty += "<span class='edited'></span>";
    appendMortgagePropertyRow.CustomerThirdParty = CustomerThirdParty;

    PropetyAddress += "<option disabled selected> -- Select -- </option>";
    PropetyAddress += "</select><span class='edited'></span>";
    appendMortgagePropertyRow.PropetyAddress = PropetyAddress;

    appendMortgagePropertyRow.CreditLimit = CreditLimit;

    appendMortgagePropertyRow.IndicativeValuationAmount = IndicativeValuationAmount;
    appendMortgagePropertyRow.LoanAmountProportion = LoanAmountProportion;
    appendMortgagePropertyRow.LTV = LTV;
    appendMortgagePropertyRow.FirstThirdParty = FirstThirdParty;
    appendMortgagePropertyRow.PropertyTypeLevel1 = PropertyTypeLevel1;
    appendMortgagePropertyRow.PropertyTypeLevel2 = PropertyTypeLevel2;
    appendMortgagePropertyRow.FormalValuation = FormalValuation;

    appendMortgagePropertyRow.TitleNumber = TitleNumber;
    appendMortgagePropertyRow.MortgageNumber = MortgageNumber;
    appendMortgagePropertyRow.ChargeNumber = ChargeNumber;
    appendMortgagePropertyRow.ChargeDate = ChargeDate;
    //}
    //else {
    //    appendMortgagePropertyRow.Name = "<select id='selectName' class='editor' tabindex='1'></select><span class='edited'></span>";
    //}
}

$("#btnAddMortgagePropertyList").on("click", function () {
    if (tblMortgagePropertyList.rows().count() === 0) {
        initializeNewRowMortgagePropertyList();
    }
    else {
        isMortgagePropertyValidRow(this);
    }
    if (isMortgagePropertyValidRow(this)) {
        var _row = tblMortgagePropertyList.rows.add([[
			appendMortgagePropertyRow.Action,
			appendMortgagePropertyRow.ExpandCollapse,			
			appendMortgagePropertyRow.IndividualCorporate,
			appendMortgagePropertyRow.CustomerThirdParty,
			appendMortgagePropertyRow.PropetyAddress,
			appendMortgagePropertyRow.CreditLimit,
			appendMortgagePropertyRow.IndicativeValuationAmount,
			appendMortgagePropertyRow.LoanAmountProportion,
			appendMortgagePropertyRow.LTV,
			appendMortgagePropertyRow.FirstThirdParty,
			appendMortgagePropertyRow.PropertyTypeLevel1,
			appendMortgagePropertyRow.PropertyTypeLevel2,
			appendMortgagePropertyRow.FormalValuation,
			appendMortgagePropertyRow.TitleNumber,
			appendMortgagePropertyRow.MortgageNumber,
			appendMortgagePropertyRow.ChargeNumber,
			appendMortgagePropertyRow.ChargeDate

        ]]);
        _row.draw();
        setActiveRowMortgagePropertyList($("#tblMortgagePropertyList tr:last"));
        setCustomerThirdParty("tblMortgagePropertyList");
    }
    ClearMortgagePropertySubGrid();
    BindMortgagePropertySubGrid();
});
$('#tblMortgagePropertyList tbody').on('click', 'button.deleting', function () {
    tblMortgagePropertyList.row($(this).parents("tr")).remove().draw();
    $('#fldMortgageProperty').removeClass('error-div');
    calcTotalLTV('tblMortgagePropertyList');
    ClearMortgagePropertySubGrid();
    BindMortgagePropertySubGrid();
});

window.showHideMortgagePropertyListField = function () {
    var activeRow = $("#tblMortgagePropertyList tr.activeRow");    
    activeRow.find(".individualcorporate").attr("disabled", false);
    activeRow.find(".customerthirdparty").attr("disabled", false);
    activeRow.find(".propetyaddress").attr("disabled", false);

    activeRow.find(".individualcorporate").removeClass("unrequired");
    activeRow.find(".customerthirdparty").removeClass("unrequired");
    activeRow.find(".propetyaddress").removeClass("unrequired");

    activeRow.find(".individualcorporate option[value='select']").remove();    
};

$("#tblMortgagePropertyList tbody").on('click', 'tr', function (event) {
    targetRowEventMortgagePropertyList(event);
    if (!($(this).hasClass("activeRow"))) {
        if (isMortgagePropertyValidRow(this))
            setActiveRowMortgagePropertyList(this);
    }
});
window.targetRowEventMortgagePropertyList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblMortgagePropertyList tbody tr.activeRow').length &&
		!$(event.target).closest('#tblMortgagePropertyList thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('#btnAddMortgagePropertyList').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#tblMortgagePropertyList').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isMortgagePropertyValidRow(event))
            setActiveRowMortgagePropertyList(null);

    }
};


window.isPropertyAddressExist = function (event, el) {
    var isDuplicate = false;
    var add = $(event).find(".propetyaddress").val();
    if (add && add !== "--Select--") {
        $("#tblMortgagePropertyList tbody tr").each(function () {
            var add1 = $(this).find(".propetyaddress").val();
            var isactiverow = $(this).hasClass('activeRow');
            if (add === add1 && !isactiverow) {
                if (el !== null && ($(el).attr('id') === 'btnAddMortgagePropertyList' || $(el.target).attr('id') === 'SaveContract'))
                    swal(swalGlobal.SwalTitle_Error, "Property address selected already exist!", swalGlobal.SwalType_Error);
                isDuplicate = true;
            }
        });
    }
    return isDuplicate;
};


function isMortgagePropertyValidRow(el) {
    var activeRow = $("#tblMortgagePropertyList tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {

        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).next().find('.select2-selection').removeClass("selection-danger");
                }
            }


            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if ($(this).val() === "" || $(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else if ($(this).hasClass("loanamountproportion")) {
                    if (parseInt($(this).val().replace(/,/g, '')) === 0)
                        $(this).addClass("alert-danger");
                    else
                        $(this).removeClass("alert-danger");
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }
            }
        });
    }
    else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isPropertyAddressExist(activeRow, el);
    if (isDuplicate) {
        $(activeRow).find(".propetyaddress").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".propetyaddress").next().find('.select2-selection').removeClass("selection-danger");

    if (!isValid) {
        $('#fldMortgageProperty').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddMortgagePropertyList' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldMortgageProperty').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setActiveRowMortgagePropertyList = function (row) {
    $("#tblMortgagePropertyList tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                if ($(this).is('input:checkbox')) {
                    if ($(this).is(':checked')) {
                        $(this).next('span').html('<i class="fa fa-check"></i>');
                    }
                    else {
                        $(this).next('span').html('');
                    }
                }
                else {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        if ($(this).val() !== "")
                            DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                        $(this).next().remove();
                    }
                    if ($(this).val() === "" || $(this).val() === "select" || $(this).val() === "--Select--") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
        showHideMortgagePropertyListField();
    }
    calcTotalLTV('tblMortgagePropertyList');
    $('#tblMortgagePropertyList .activeRow .js-example-basic-sec3').select2();
};

var tableName = "";
window.setTableNameForCustomerThirdParty = function (el) {
    tableName = $(el).closest('table').attr('id');
    setCustomerThirdParty(tableName);
}

window.setCustomerThirdParty = function (table) {
    var options = "";

    if (table === "tblDebentureConstructionEquipmentList") {
        ResetConstructionEquipment();
        $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
        $('#' + table + ' tr.activeRow').find('.equipment').html('<option>--Select--</option>');
    }
    if (table === "tblDebentureIndustrialEquipmentList") {
        ResetIndustrialEquipment();
        $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
        $('#' + table + ' tr.activeRow').find('.industrial').html('<option>--Select--</option>');
    }
    if (table === "tblSecuritiesFinancialInstruments") {
        ResetSecuritiesFinancialInstrument();
        $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
        //$('#' + table + ' tr.activeRow').find('.industrial').html('<option>--Select--</option>');
    }
    if (table === "tblSecurityDepositList") {
        ResetSecurityDepositList();
        $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
        //$('#' + table + ' tr.activeRow').find('.industrial').html('<option>--Select--</option>');
    }
    if (table === "tblDebentureInventoryList") {
        ResetDebentureInventoryList();
        $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
        //$('#' + table + ' tr.activeRow').find('.industrial').html('<option>--Select--</option>');
    }
    if (table === "tblDebentureIndustrialEquipmentList") {
        ResetIndustrialEquipment();
        $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
    }
    if (table === "tblDebentureConstructionEquipmentList") {
        ResetConstructionEquipment();
        $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
    }
    if (table === "tblDebentureReceivableList") {
        ResetDebentureReceivableList();
        $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
    }
    if (table === "tblMortgagePropertyList") {
        ResetMortgagePropertyList();
    }
    if (table === "tblMortgageVesselList") {
        ResetMortgageVesselList();
    }
    if (table === "tblSecuritiesFinancialInstruments") {
        ResetSecuritiesFinancialInstrument();
    }
    if (table === "tblSecurityDepositList") {
        ResetSecurityDepositList();
    }
    if (table === "tblDebentureVehicleList") {
        ResetDebentureVehicleList();
        $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
        $('#' + table + ' tr.activeRow').find('.vehiclechassisandregnumber').html('<option>--Select--</option>');
    }

    var currentcustomerthirdparty = $('#' + table + ' tr.activeRow').find('.customerthirdparty');
    var IndividualorCorporate = $('#' + table + ' tr.activeRow').find('.individualcorporate').val();
    if (table === "tblDebentureCashAndEquivalentIndividualList" || IndividualorCorporate == "select")
        IndividualorCorporate = "Individual";
    if (table === "tblDebentureCashAndEquivalentCompanyList")
        IndividualorCorporate = "Corporate";

    $('.activeRow .customerthirdparty').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: varSitePath + '/PreCon/GetCustomerAutoComplete',
                data: { textFilter: request.term, IndividualCorporate: IndividualorCorporate },
                type: 'GET',
                cache: false,
                async: false,
                dataType: 'JSON',
                global: false,
                success: function (json) {
                    var res = JSON.parse(json.data);
                    $(res).each(function (i, v) {
                        v.label = v.label.replace("&#39", '\'').replace("&#34", "\"").replace("&#12296", "<").replace("&#12297", ">");
                    });
                    response(res);
                }
            });
        },
        minLength: 1,
        scroll: true,
        autoFocus: false,
        autoFill: false,
        focus: function (event, ui) {
            return false;
        },
        select: function (event, ui) {
            // var el = $(this).find('.customerthirdparty');				
            currentcustomerthirdparty.val(ui.item.label);
            currentcustomerthirdparty.attr("data-id", ui.item.value);
            if (table === "tblMortgagePropertyList") {
                ClearMortgagePropertySubGrid();
                setPropetyAddress();
            }
            if (table === "tblMortgageVesselList") {
                ClearMortgageVesselSubGrid();
                setHullNumberVesselName();
            }
            if (table === "tblDebentureVehicleList")
                setVehicleChassisAndRegNumber();
            if (table === "tblDebentureConstructionEquipmentList")
                setConstructionEquip();
            if (table === "tblDebentureIndustrialEquipmentList")
                setIndustrialEquip();
            if (table === "tblDebentureInventoryList")
                setTypeDescription();
            if (table === "tblDebentureReceivableList")
                setReceivableAmount();
            if (table === "tblDebentureCashAndEquivalentIndividualList")
                setCashEquivalentInd();
            if (table === "tblDebentureCashAndEquivalentCompanyList")
                setCashEquivalentCom();
            if (table === "tblSecuritiesFinancialInstruments")
                setAmountAndDocumentNumber();
            if (table === "tblSecurityDepositList")
                setSecurityDepositBillToAmount();

            return false;
        },
        change: function (event, ui) {
            //var el = $(this).find('.customerthirdparty');
            if (ui.item) {
                currentcustomerthirdparty.val(ui.item.label);
                currentcustomerthirdparty.attr("data-id", ui.item.value);
            }
            else {
                //$('.name').val('');
                //el.attr("data-id", '');
                //$(this).removeClass('alert-danger');
                if (table === "tblMortgagePropertyList") {
                    ResetMortgagePropertyList();
                }
                if (table === "tblMortgageVesselList") {
                    ResetMortgageVesselList();
                }
                if (table === "tblDebentureCashAndEquivalentCompanyList") {
                    ResetDebentureCashAndEquivalentList("tblDebentureCashAndEquivalentCompanyList");
                    $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
                }
                if (table === "tblDebentureCashAndEquivalentIndividualList") {
                    ResetDebentureCashAndEquivalentList("tblDebentureCashAndEquivalentIndividualList");
                    $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
                }
                if (table === "tblDebentureReceivableList") {
                    ResetDebentureReceivableList();
                    $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
                }
                if (table === "tblDebentureInventoryList") {
                    ResetDebentureInventoryList();
                    $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
                    $('#' + table + ' tr.activeRow').find('.typedescription').html('<option>--Select--</option>');
                }
                if (table === "tblDebentureIndustrialEquipmentList") {
                    ResetIndustrialEquipment();
                    $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
                    $('#' + table + ' tr.activeRow').find('.industrial').html('<option>--Select--</option>');
                }
                if (table === "tblDebentureConstructionEquipmentList") {
                    ResetConstructionEquipment();
                    $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
                    $('#' + table + ' tr.activeRow').find('.equipment').html('<option>--Select--</option>');
                }
                if (table === "tblSecuritiesFinancialInstruments") {
                    $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
                    ResetSecuritiesFinancialInstrument();
                }
                if (table === "tblSecurityDepositList") {
                    $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
                    ResetSecurityDepositList();
                }
                if (table === "tblDebentureVehicleList") {
                    ResetDebentureVehicleList();
                    $('#' + table + ' tr.activeRow').find('.customerthirdparty').val('');
                    $('#' + table + ' tr.activeRow').find('.vehiclechassisandregnumber').html('<option>--Select--</option>');
                }
            }
            return false;
        }
    });
};

window.setPropetyAddress = function () {
    var options = "";
    var activeRow = $("#tblMortgagePropertyList tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getPropertyAddress',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    var firstThirdtParty = v.FirstThirdParty === "F" ? "First Party" : "Third Party";
                    var chargedate = isNullOrEmpty(v.ChargeDate);
                    options += "<option creditlimit='" + v.CreditLimit + "' indicativevaluation=" + v.IndicativeValuation +
						" firstthirdparty='" + firstThirdtParty + "' propertytypelevel1='" + v.PropertyTypeLevel1 +
						"' propertytypelevel2='" + v.PropertyTypeLevel2 +
						"' formalvaluation='" + v.FormalValuation +
						"' titlenumber='" + v.TitleNumber +
						"' mortgagenumber='" + v.MortgageNumber +
						"' chargenumber='" + v.ChargeNumber +
						"' chargedate='" + chargedate +
						"' value='" + v.ID + "'>" + v.PropertyAddress + "</option>";
                });
                activeRow.find('.propetyaddress').html(options);
                setValueChangeOfPropetyAddress('propetyaddress');
                RebindMortgagePropertySubGrid(activeRow);
            }
            else {
                ResetMortgagePropertyList(true);

            }
        }
    });
};

window.setValueChangeOfPropetyAddress = function (column) {
    var activeRow = $("#tblMortgagePropertyList tr.activeRow");
    var selectedValue = activeRow.find('.' + column + ' option:selected');
    activeRow.find('.creditlimit').val(selectedValue.attr('creditlimit'));
    activeRow.find('.indicativevaluationamount').val(selectedValue.attr('indicativevaluation'));
    activeRow.find('.firstthirdparty').val(selectedValue.attr('firstthirdparty'));
    activeRow.find('.propertytypelevel1').val(selectedValue.attr('propertytypelevel1'));
    activeRow.find('.propertytypelevel2').val(selectedValue.attr('propertytypelevel2'));
    activeRow.find('.formalvaluation').val(manualMask(selectedValue.attr('formalvaluation')));
    activeRow.find('.titlenumber').val(selectedValue.attr('titlenumber'));
    activeRow.find('.mortgagenumber').val(selectedValue.attr('mortgagenumber'));
    activeRow.find('.chargenumber').val(selectedValue.attr('chargenumber'));
    activeRow.find('.chargedate').val(selectedValue.attr('chargedate'));

    calcLTV("tblMortgagePropertyList");
    manualMaskActiveRowMoney(activeRow);
    ClearMortgagePropertySubGrid();
};
window.ResetMortgagePropertyList = function (thirdparty) {
    var activeRow = $("#tblMortgagePropertyList tr.activeRow");    
    activeRow.find('.loanamountproportion').val('0.00');
    activeRow.find('.ltv').val('0.0000');
    if (!thirdparty)
        activeRow.find('.customerthirdparty').val('');
    activeRow.find('.propetyaddress').html('<option>--Select--</option>');
    activeRow.find('.creditlimit').val('');
    activeRow.find('.indicativevaluationamount').val('0.00');
    activeRow.find('.firstthirdparty').val('');
    activeRow.find('.propertytypelevel1').val('');
    activeRow.find('.propertytypelevel2').val('');
    activeRow.find('.formalvaluation').val('0.00');
    activeRow.find('.titlenumber').val('');
    activeRow.find('.mortgagenumber').val('');
    activeRow.find('.chargenumber').val('');
    activeRow.find('.chargedate').val('');

    calcLTV("tblMortgagePropertyList");
    ClearMortgagePropertySubGrid();
};

window.isNullOrEmpty = function (el) {
    if (el)
        return el;
    else
        return "";
};
window.calcLTV = function (table) {
    var activeRow = $("#" + table + " tr.activeRow");    
    var loanAmt = parseFloat(activeRow.find('.loanamountproportion').val().replace(/,/g, '')) || 0;
    var indicativeValueAmt = parseFloat(activeRow.find('.indicativevaluationamount').val().replace(/,/g, '')) || 0.00;
    if (indicativeValueAmt > 0) {
        var ltvPer = parseFloat(loanAmt / indicativeValueAmt * 100) || 0.00;
        var isDecimal = ltvPer.toString().split(".").length;

        var numberOfdecimal = isDecimal > 1 ? ltvPer.toString().split(".")[1].length : 0;
        if (numberOfdecimal > 2)
            ltvPer = parseFloat(ltvPer.toString().split(".")[0] + "." + ltvPer.toString().split(".")[1].substring(0, 2)) + 0.01;
        ltvPer = parseFloat(ltvPer).toFixed(4);
        if (isNaN(ltvPer))
            ltvPer = 0;
        activeRow.find('.ltv').val(manualMask(ltvPer));
    }
    calcTotalLTV(table);
};

window.manualMaskActiveRowMoney = function (activeRow) {
    $(activeRow).find(".money").each(function () {
        $(this).val(manualMask($(this).val()));
    });
};



window.calcTotalLTV = function (table) {

    //var numberOfRow = table === "tblMortgagePropertyList" ? tblMortgagePropertyList.rows().count() : tblMortgageVesselList.rows().count();
    var ltvPercent = parseFloat("0.0000");
    $("#" + table + " tbody tr").each(function () {
        if ($(this).hasClass('activeRow'))
            ltvPercent = parseFloat(parseFloat(ltvPercent) + parseFloat($(this).find('td .ltv').val().replace(/,/g, '') || 0));
        else
            ltvPercent = parseFloat(parseFloat(ltvPercent) + parseFloat($(this).find('td .ltv').next('.edited').text().replace(/,/g, '') || 0));
    });
    //var finalltvPercent = parseFloat(ltvPercent / numberOfRow).toFixed(4);
    var finalltvPercent = parseFloat(ltvPercent).toFixed(4);
    if (isNaN(finalltvPercent))
        finalltvPercent = "0.0000";
    if (table === "tblMortgagePropertyList") {
        $("#ltvPerMortgagePropertyList").val(manualMask(finalltvPercent));
    }
    else
        $("#ltvPerMortgageVesselList").val(manualMask(finalltvPercent));
};

//Vessel List
window.tblMortgageVesselList = $("#tblMortgageVesselList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

var appendMortgageVesselRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
    "ExpandCollapse": "",
    "IndividualCorporate": "",
    "CustomerThirdParty": "", "HullNumberAndVesselName": "", "CreditLimit": "", "IndicativeValuationAmount": "",
    "LoanAmountProportion": "", "LTV": "", "InsuranceList": "", "CountryofRegistration": "",
    "FormalValuation": "", "MortgageNumber": "",
    "ChargeNumber": "", "ChargeDate": "", "Status": ""
};

function initializeNewRowMortgageVesselList() {
    // if (dropdownMortgagePropertynameListValue) {
    var ExpandCollapse = "<img src='" + varSitePath + "/images/Plus.png' class='iconexpand' onclick='BindMortgageVesselSubGrid(this)'>";    
    var IndividualCorporate = "<select class='editor individualcorporate js-example-basic-sec3' onchange='setCustomerThirdParty(\"tblMortgageVesselList\")' tabindex='3'>";
    //var CustomerThirdParty = "<select class='editor customerthirdparty js-example-basic-sec3' onchange='setHullNumberVesselName()' tabindex='4'>";
    var CustomerThirdParty = "<input data-id='' onfocus='setTableNameForCustomerThirdParty(this)' class='editor customerthirdparty unrequired ui-autocomplete-input' type='text' autocomplete='off'><span class='ctparty edited'></span>";


    var HullNumberAndVesselName = "<select class='editor hullnumberandvesselname js-example-basic-sec3' onchange=setValueChangeOfCrossHullNumberAndVesselName('hullnumberandvesselname') tabindex='4'>";
    var CreditLimit = "<input class='editor creditlimit unrequired decimal money' type='text' disabled='disabled'><span class='edited'></span>";
    var IndicativeValuationAmount = "<input class='editor indicativevaluationamount unrequired decimal money'  value='0.00' min='1' type='text' disabled='disabled'><span class='edited'></span>";
    var LoanAmountProportion = "<input class='editor loanamountproportion decimal money' type='text' value='0.00' min='1' onblur='calcLTV(\"tblMortgageVesselList\")'><span class='edited'></span>";
    var LTV = "<input class='editor ltv decimal percentage' type='text' value='0.0000'><span class='edited'></span>";
    var InsuranceList = "<a href='javascript:void(0)' class='insurancelist unrequired' onclick='getSecurityVesselInsurance(this,event)' value=''>Insurance List</a><span class='edited'></span>";
    var CountryofRegistration = "<input class='editor countryofregistration unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var FormalValuation = "<input class='editor formalvaluation unrequired decimal money' type='text' disabled='disabled'><span class='edited'></span>";
    var MortgageNumber = "<input class='editor mortgagenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeNumber = "<input class='editor chargenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeDate = "<input class='editor chargedate unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var Status = "<input class='editor status unrequired' type='text' disabled='disabled'><span class='edited'></span>";

    appendMortgageVesselRow.ExpandCollapse = ExpandCollapse;
    
    IndividualCorporate += "<option>Individual</option><option>Corporate</option>";
    IndividualCorporate += "</select><span class='edited'></span>";
    appendMortgageVesselRow.IndividualCorporate = IndividualCorporate;

    //CustomerThirdParty += "<option disabled value='' selected> -- Select -- </option>";
    CustomerThirdParty += "</select><span class='edited'></span>";
    appendMortgageVesselRow.CustomerThirdParty = CustomerThirdParty;

    HullNumberAndVesselName += "<option disabled value='' selected> -- Select -- </option></select>";
    HullNumberAndVesselName += "<span class='edited'></span>";
    appendMortgageVesselRow.HullNumberAndVesselName = HullNumberAndVesselName;

    appendMortgageVesselRow.CreditLimit = CreditLimit;

    appendMortgageVesselRow.IndicativeValuationAmount = IndicativeValuationAmount;
    appendMortgageVesselRow.LoanAmountProportion = LoanAmountProportion;
    appendMortgageVesselRow.LTV = LTV;
    appendMortgageVesselRow.InsuranceList = InsuranceList;
    appendMortgageVesselRow.CountryofRegistration = CountryofRegistration;
    appendMortgageVesselRow.FormalValuation = FormalValuation;

    appendMortgageVesselRow.MortgageNumber = MortgageNumber;
    appendMortgageVesselRow.ChargeNumber = ChargeNumber;
    appendMortgageVesselRow.ChargeDate = ChargeDate;
    //}
    //else {
    //    appendMortgageVesselRow.Name = "<select id='selectName' class='editor' tabindex='1'></select><span class='edited'></span>";
    //}
}

$("#btnAddMortgageVesselList").on("click", function () {
    if (tblMortgageVesselList.rows().count() === 0) {
        initializeNewRowMortgageVesselList();
    }
    else {
        isMortgageVesselValidRow(this);
    }
    if (isMortgageVesselValidRow(this)) {
        var _row = tblMortgageVesselList.rows.add([[
			appendMortgageVesselRow.Action,
			appendMortgageVesselRow.ExpandCollapse,			
			appendMortgageVesselRow.IndividualCorporate,
			appendMortgageVesselRow.CustomerThirdParty,
			appendMortgageVesselRow.HullNumberAndVesselName,
			appendMortgageVesselRow.CreditLimit,
			appendMortgageVesselRow.IndicativeValuationAmount,
			appendMortgageVesselRow.LoanAmountProportion,
			appendMortgageVesselRow.LTV,
			appendMortgageVesselRow.InsuranceList,
			appendMortgageVesselRow.CountryofRegistration,
			appendMortgageVesselRow.FormalValuation,
			appendMortgageVesselRow.MortgageNumber,
			appendMortgageVesselRow.ChargeNumber,
			appendMortgageVesselRow.ChargeDate

        ]]);
        _row.draw();
        setActiveRowMortgageVesselList($("#tblMortgageVesselList tr:last"));
        setCustomerThirdParty("tblMortgageVesselList");
    }
    ClearMortgageVesselSubGrid();
    BindMortgageVesselSubGrid();
});
$('#tblMortgageVesselList tbody').on('click', 'button.deleting', function () {
    tblMortgageVesselList.row($(this).parents("tr")).remove().draw();
    $('#fldMortgageVessel').removeClass('error-div');
    calcTotalLTV('tblMortgageVesselList');
    ClearMortgageVesselSubGrid();
    BindMortgageVesselSubGrid();
});

window.showHideMortgageVesselListField = function () {
    var activeRow = $("#tblMortgageVesselList tr.activeRow");    
    
    activeRow.find(".individualcorporate").removeClass("unrequired");
    activeRow.find(".customerthirdparty").removeClass("unrequired");
    activeRow.find(".hullnumberandvesselname").removeClass("unrequired");

    activeRow.find(".individualcorporate").attr("disabled", false);
    activeRow.find(".customerthirdparty").attr("disabled", false);
    activeRow.find(".hullnumberandvesselname").attr("disabled", false);

    activeRow.find(".individualcorporate option[value='select']").remove();
};

$("#tblMortgageVesselList tbody").on('click', 'tr', function (event) {
    targetRowEventMortgageVesselList(event);
    if (!($(this).hasClass("activeRow"))) {
        if (isMortgageVesselValidRow(this))
            setActiveRowMortgageVesselList(this);
    }
});
window.targetRowEventMortgageVesselList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblMortgageVesselList tbody tr.activeRow').length &&
		!$(event.target).closest('#tblMortgageVesselList thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAddMortgageVesselList').length &&
		!$(event.target).closest('#tblMortgageVesselList').length &&
		!$(event.target).closest('#InsuranceModal').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isMortgageVesselValidRow(this))
            setActiveRowMortgageVesselList(null);

    }
};

window.isHullNumberExist = function (event, el) {
    var isDuplicate = false;
    var hullnum = $(event).find(".hullnumberandvesselname").val();
    if (hullnum && hullnum !== "--Select--") {
        $("#tblMortgageVesselList tbody tr").each(function () {
            var hullnum1 = $(this).find(".hullnumberandvesselname").val();
            var isactiverow = $(this).hasClass('activeRow');
            if (hullnum === hullnum1 && !isactiverow) {
                if (el !== null && ($(el).attr('id') === 'btnAddMortgageVesselList' || $(el.target).attr('id') === 'SaveContract'))
                    swal(swalGlobal.SwalTitle_Error, "Hull number selected already exist!", swalGlobal.SwalType_Error);
                isDuplicate = true;
            }
        });
    }
    return isDuplicate;
};


function isMortgageVesselValidRow(el) {
    var activeRow = $("#tblMortgageVesselList tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).next().find('.select2-selection').removeClass("selection-danger");
                }
            }


            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if ($(this).val() === "" || $(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else if ($(this).hasClass("loanamountproportion")) {
                    if (parseInt($(this).val().replace(/,/g, '')) === 0)
                        $(this).addClass("alert-danger");
                    else
                        $(this).removeClass("alert-danger");
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }
            }
        });
    }
    else { return true; }

    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isHullNumberExist(activeRow, el);
    if (isDuplicate) {
        $(activeRow).find(".hullnumberandvesselname").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".hullnumberandvesselname").next().find('.select2-selection').removeClass("selection-danger");


    if (!isValid) {
        $('#fldMortgageVessel').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddMortgageVesselList' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldMortgageVessel').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setActiveRowMortgageVesselList = function (row) {
    $("#tblMortgageVesselList tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                if ($(this).is('input:checkbox')) {
                    if ($(this).is(':checked')) {
                        $(this).next('span').html('<i class="fa fa-check"></i>');
                    }
                    else {
                        $(this).next('span').html('');
                    }
                }
                else {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        if ($(this).val() !== "" || $(this).val() === "select" || $(this).val() === "--Select--")
                            DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                        $(this).next().remove();
                    }
                    if ($(this).val() === "" || $(this).val() === "select" || $(this).val() === "--Select--") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
        showHideMortgageVesselListField();
    }
    calcTotalLTV('tblMortgageVesselList');
    $('#tblMortgageVesselList .activeRow .js-example-basic-sec3').select2();
};

window.setHullNumberVesselName = function () {
    var options = "";
    var activeRow = $("#tblMortgageVesselList tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getHullNumberAndVesselName',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    var chargedate = isNullOrEmpty(v.ChargeDate);
                    options += "<option creditlimit='" + v.CreditLimit +
						"' indicativevaluation='" + v.IndicativeValuation +
						"' formalvaluation='" + v.FormalValuation +
						"' countryofregistration='" + v.CountryOfReg +
						"' mortgagenumber='" + v.MortgageNumber +
						"' chargenumber='" + v.ChargeNumber +
						"' chargedate='" + chargedate +
						"' value='" + v.ID + "'>" + v.HullNumber + " - " + v.VesselName + "</option>";
                });
                activeRow.find('.hullnumberandvesselname').html(options);
                setValueChangeOfCrossHullNumberAndVesselName('hullnumberandvesselname');
                RebindMortgageVesselSubGrid(activeRow);
            }
            else {
                ResetMortgageVesselList(true);
            }
        }
    });
};

window.setValueChangeOfCrossHullNumberAndVesselName = function (column) {
    var activeRow = $("#tblMortgageVesselList tr.activeRow");
    var selectedValue = activeRow.find('.' + column + ' option:selected');
    activeRow.find('.creditlimit').val(selectedValue.attr('creditlimit'));
    activeRow.find('.indicativevaluationamount').val(selectedValue.attr('indicativevaluation'));
    activeRow.find('.formalvaluation').val(selectedValue.attr('formalvaluation'));
    activeRow.find('.countryofregistration').val(selectedValue.attr('countryofregistration'));
    activeRow.find('.mortgagenumber').val(selectedValue.attr('mortgagenumber'));
    activeRow.find('.chargenumber').val(selectedValue.attr('chargenumber'));
    activeRow.find('.chargedate').val(selectedValue.attr('chargedate'));

    calcLTV("tblMortgageVesselList");
    manualMaskActiveRowMoney(activeRow);
    ClearMortgageVesselSubGrid();
};

window.ResetMortgageVesselList = function (thirdparty) {
    var activeRow = $("#tblMortgageVesselList tr.activeRow");   
    activeRow.find('.loanamountproportion').val('0.00');
    activeRow.find('.ltv').val('0.0000');
    if (!thirdparty)
        activeRow.find('.customerthirdparty').val('');
    activeRow.find('.hullnumberandvesselname').html('<option>--Select--</option>');
    activeRow.find('.creditlimit').val('');
    activeRow.find('.indicativevaluationamount').val('');
    activeRow.find('.formalvaluation').val('');
    activeRow.find('.countryofregistration').val('');
    activeRow.find('.mortgagenumber').val('');
    activeRow.find('.chargenumber').val('');
    activeRow.find('.chargedate').val('');

    calcLTV("tblMortgageVesselList");
    ClearMortgageVesselSubGrid();
};

window.getSecurityVesselInsurance = function (e, event) {
    var disabled = true;
    if ($("#SaveContract").hasClass('disabled'))
        disabled = false;

    event.stopPropagation();    
    var hullNumberAndVesselName = $(e).closest('tr').find('.hullnumberandvesselname option:selected').val();

    $.ajax({
        url: varSitePath + '/PreCon/GetSecurityVesselInsurance',
        data: { HullNumber: hullNumberAndVesselName },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.length > 0) {
                var data = "<table class='dataTable table table-bordered'><thead><tr><th>Insurance Type</th><th>Expiry Date</th></tr></thead><tbody>";
                $(json).each(function (i, v) {
                    data = data + '<tr><td>' + v.InsuranceType + '</td>' +
						'<td>' + v.ExpiryDate + '</td></tr></tbody>';
                });
                $("#InsuranceModalBody").html(data);
                $("#InsuranceModal").modal('show');
                if (disabled) {
                    $("#SaveContract").removeClass("disabled");
                    $("#SaveContract").prop("disabled", false);
                }
            }

        }
    });
};


// Sub Grids
var oTable; var bTable;
var iTableCounter = 1;
var oInnerTable;
var tblMortgagePropertyListDetailsHtml;
var tblMortgageVesselListDetailsHtml;
var nCloneTh = document.createElement('th');
var nCloneTd = document.createElement('td');
function RebindMortgagePropertySubGrid(object) {
    if ($(object).parent().parent().next().hasClass('subgridrow')) {
        /* This row is already exist remove it */
        $(object).parent().parent().next().remove();
        BindMortgagePropertySubGrid(object);
    }
}
function RebindMortgageVesselSubGrid(object) {
    if ($(object).parent().parent().next().hasClass('subgridrow')) {
        /* This row is already exist remove it */
        $(object).parent().parent().next().remove();
        BindMortgageVesselSubGrid(object);
    }
}
function ClearMortgagePropertySubGrid() {
    $("#tblMortgagePropertyList tr.subgridrow").each(function (item, v) {
        $(this).remove();
    });
    $("#tblMortgagePropertyList tr td img").each(function (item, v) {
        $(this).attr('src', varSitePath + '/images/Plus.png');
        $(this).removeClass('iconremove'); $(this).addClass('iconexpand');
    });
}
function ClearMortgageVesselSubGrid() {
    $("#tblMortgageVesselList tr.subgridrow").each(function (item, v) {
        $(this).remove();
    });
    $("#tblMortgageVesselList tr td img").each(function (item, v) {
        $(this).attr('src', varSitePath + '/images/Plus.png');
        $(this).removeClass('iconremove'); $(this).addClass('iconexpand');
    });
}
function BindMortgagePropertySubGrid(object) {
    if ($(object).hasClass('iconexpand')) {
        ClearMortgagePropertySubGrid();
    }
    tblMortgagePropertyListDetailsHtml = $("#tblMortgagePropertyListDetails").html();
    nCloneTd.innerHTML = "<img src='" + varSitePath + "/images/Plus.png'>";
    nCloneTd.className = "center";
    var nTr = $(object).parent().parent();
    var nTds = $(object).parent();
    var activeRow = $("#tblMortgagePropertyList tr.activeRow");    
    var propertyAddressID = nTr.find('.propetyaddress option:selected').val();
    if (propertyAddressID !== '--Select--' && propertyAddressID) {
        if (nTr.next().hasClass('subgridrow')) {
            /* This row is already open - close it */
            $(object).attr('src', varSitePath + '/images/Plus.png');
            $(object).removeClass('iconremove');
            $(object).addClass('iconexpand');
            nTr.next().remove();
        }
        else {
            $(object).attr('src', varSitePath + '/images/Minus.png');
            $(object).removeClass('iconexpand'); $(object).addClass('iconremove');
            var newInnertTable = fnFormatDetails(iTableCounter, tblMortgagePropertyListDetailsHtml);
            $(object).parent().parent().after("<tr class='subgridrow hide'><td colspan='23'>" + newInnertTable + "</td></tr>");
            //oTable.fnOpen(nTr, fnFormatDetails(iTableCounter, tblMortgagePropertyListDetailsHtml));
            oInnerTable = $("#tblMortgagePropertyList_" + iTableCounter).dataTable({
                "columnDefs": [{ width: 15, targets: [0, 1, 2] },
				{ width: 10, targets: [3, 5, 6, 7, 8, 9, 10, 11, 12] },
				{ width: 5, targets: [4] }],
                "searching": false,
                "serverSide": true,
                "autoWidth": false,
                "paging": false,
                "info": false,
                "ajax":
				{
				    "url": varSitePath + '/PreCon/getSecurityPropertyMortgagorDetails',
				    "data": { propertyAddressID: propertyAddressID},
				    "async": true,
				    "type": "POST",
				    "dataType": "JSON",
				    "global": false,
				    "complete": function (data) {
				        nTr.next().removeClass('hide');
				    }
				},
                "columns": [
					{
					    "data": "ClientName"
					},
					{
					    "data": "MainMortgagor"
					},
					{
					    "data": "NRICFINPASSPORT"
					},
					{
					    "data": "ROCUEN"
					},
					{
					    "data": "MortgagorAddress"
					},
					{
					    "data": "Department"
					},
					{
					    "data": "ContactPerson"
					},
					{
					    "data": "MobileNumber"
					}, {
					    "data": "Email"
					}, {
					    "data": "OfficeNumber"
					}, {
					    "data": "HomeNumber"
					}, {
					    "data": "FaxNumber"
					}, {
					    "data": "PagerNumber"
					}]
            });
            iTableCounter = iTableCounter + 1;
        }
    }
}
function BindMortgageVesselSubGrid(object) {
    if ($(object).hasClass('iconexpand')) {
        ClearMortgageVesselSubGrid();
    }
    tblMortgageVesselListDetailsHtml = $("#tblMortgageVesselListDetails").html();
    nCloneTd.innerHTML = "<img src=" + varSitePath + "'/images/Plus.png'>";
    nCloneTd.className = "center";
    var nTr = $(object).parent().parent();
    var nTds = $(object).parent();
    var activeRow = $("#tblMortgageVesselList tr.activeRow");    
    var hullVessel = nTr.find('.hullnumberandvesselname option:selected').val();
    if (hullVessel !== "--Select--" && hullVessel) {

        if (nTr.next().hasClass('subgridrow')) {
            /* This row is already open - close it */
            $(object).attr('src', varSitePath + '/images/Plus.png');
            $(object).removeClass('iconremove'); $(object).addClass('iconexpand');
            nTr.next().remove();
        }
        else {
            $(object).attr('src', varSitePath + '/images/Minus.png');
            $(object).removeClass('iconexpand'); $(object).addClass('iconremove');
            var newInnertTable = fnFormatVesselDetails(iTableCounter, tblMortgageVesselListDetailsHtml);
            $(object).parent().parent().after("<tr class='subgridrow hide'><td colspan='23'>" + newInnertTable + "</td></tr>");
            //oTable.fnOpen(nTr, fnFormatDetails(iTableCounter, tblMortgagePropertyListDetailsHtml));
            oInnerTable = $("#tblMortgageVesselList_" + iTableCounter).dataTable({
                "columnDefs": [{ width: 15, targets: [0, 1, 2] },
				{ width: 10, targets: [3, 5, 6, 7, 8, 9, 10, 11, 12] },
				{ width: 5, targets: [4] }],
                "searching": false,
                "serverSide": true,
                "autoWidth": false,
                "paging": false,
                "info": false,
                "ajax":
				{
				    "url": varSitePath + '/PreCon/getSecurityPropertyVesselDetails',
				    "data": { selectedHullVessel: hullVessel },
				    "async": true,
				    "type": "POST",
				    "dataType": "JSON",
				    "global": false,
				    "complete": function (data) {
				        nTr.next().removeClass('hide');
				    }
				},
                "columns": [
					{
					    "data": "MortgagorName"
					},
					{
					    "data": "Main_SecondaryMortgagor"
					},
					{
					    "data": "NRIC_FIN_PASSPORT"
					},
					{
					    "data": "ROCUEN"
					},
					{
					    "data": "Address"
					},
					{
					    "data": "Department"
					},
					{
					    "data": "ContactPerson"
					},
					{
					    "data": "MobileNumber"
					}, {
					    "data": "Email"
					}, {
					    "data": "OfficeNumber"
					}, {
					    "data": "HomeNumber"
					}, {
					    "data": "FaxNumber"
					}, {
					    "data": "PagerNumber"
					}]
            });
            iTableCounter = iTableCounter + 1;
        }
    }
}
function fnFormatDetails(table_id, html) {
    var sOut = "<table id=\"tblMortgagePropertyList_" + table_id + "\">";
    sOut += html;
    sOut += "</table>";
    return sOut;
}
function fnFormatVesselDetails(table_id, html) {
    var sOut = "<table id=\"tblMortgageVesselList_" + table_id + "\">";
    sOut += html;
    sOut += "</table>";
    return sOut;
}
//$('#tblMortgagePropertyList thead tr').each(function () {
//    this.insertBefore(nCloneTh, this.childNodes[0]);
//});
//$('#tblMortgagePropertyList tbody tr').each(function () {
//    this.insertBefore(nCloneTd.cloneNode(true), this.childNodes[0]);
//});
//$('#tblMortgageVesselList thead tr').each(function () {
//    this.insertBefore(nCloneTh, this.childNodes[0]);
//});
//$('#tblMortgageVesselList tbody tr').each(function () {
//    this.insertBefore(nCloneTd.cloneNode(true), this.childNodes[0]);
//});
//End Shivam


















// Debenture Vehicle

window.tblDebentureVehicleList = $("#tblDebentureVehicleList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

var appendDebentureVehicleRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
    "CrossCollateralization": "", "CrossCollateralizationContractNumber": "", "CrossCollateralizationRolloverNumber": "",
    "CrossCollateraizationVehicleChassisAndRegNumber": "", "IndividualCorporate": "",
    "CustomerThirdParty": "", "VehicleChassisAndRegNumber": "", "VehicleType": "", "VehicleMake": "",
    "VehicleModel": "", "COEExpiryDate": "", "EngineNumber": "", "Value": "",
    "ChargeNumber": "", "ChargeDate": "", "Status": ""
};

function initializeNewRowDebentureVehicleList() {
    var CrossCollateralization = "<input class='editor crosscollateralization unrequired checkbox-center' type='checkbox' checked><span class='edited checkbox-center'></span>";
    var CrossCollateralizationContractNumber = "<input class='editor unrequired crosscollateralizationcontractnumber' onblur='setCrossCollateralizationDebentureVehicle(this)' type='text'><span class='edited'></span>";
    var CrossCollateralizationRolloverNumber = "<input class='editor unrequired crosscollateralizationrollovernumber' onblur='setCrossCollateralizationDebentureVehicle(this)' type='number' value='0'><span class='edited'></span>";
    var CrossCollateraizationVehicleChassisAndRegNumber = "<select class='editor crosscollateraizationvehiclechassisandregnumber unrequired js-example-basic-sec3' onchange=setValueChangeOfCrossVehicleChassisAndRegNumber('crosscollateraizationvehiclechassisandregnumber') tabindex='2'>";
    var IndividualCorporate = "<select class='editor individualcorporate js-example-basic-sec3' onchange='setCustomerThirdParty(\"tblDebentureVehicleList\")' tabindex='3'>";
    var CustomerThirdParty = "<input data-id='' onfocus='setTableNameForCustomerThirdParty(this)' class='editor customerthirdparty ui-autocomplete-input' type='text' autocomplete='off'><span class='ctparty edited'></span>";

    var VehicleChassisAndRegNumber = "<select class='editor vehiclechassisandregnumber js-example-basic-sec3'  onchange=setValueChangeOfCrossVehicleChassisAndRegNumber('vehiclechassisandregnumber') tabindex='4'>";
    var VehicleType = "<input class='editor vehicletype unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var VehicleMake = "<input class='editor vehiclemake unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var VehicleModel = "<input class='editor vehiclemodel unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var COEExpiryDate = "<input class='editor coeexpirydate unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var EngineNumber = "<input class='editor enginenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeNumber = "<input class='editor chargenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var Value = "<input class='editor value unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeDate = "<input class='editor chargedate unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var Status = "<input class='editor status unrequired' type='text' disabled='disabled'><span class='edited'></span>";


    appendDebentureVehicleRow.CrossCollateralization = CrossCollateralization;
    appendDebentureVehicleRow.CrossCollateralizationContractNumber = CrossCollateralizationContractNumber;
    appendDebentureVehicleRow.CrossCollateralizationRolloverNumber = CrossCollateralizationRolloverNumber;

    CrossCollateraizationVehicleChassisAndRegNumber += "<option disabled selected> -- Select -- </option>";
    CrossCollateraizationVehicleChassisAndRegNumber += "</select><span class='edited'></span>";
    appendDebentureVehicleRow.CrossCollateraizationVehicleChassisAndRegNumber = CrossCollateraizationVehicleChassisAndRegNumber;

    IndividualCorporate += "<option>Individual</option><option>Corporate</option>";
    IndividualCorporate += "</select><span class='edited'></span>";
    appendDebentureVehicleRow.IndividualCorporate = IndividualCorporate;

    CustomerThirdParty += "</select><span class='edited'></span>";
    appendDebentureVehicleRow.CustomerThirdParty = CustomerThirdParty;

    VehicleChassisAndRegNumber += "<option disabled value='' selected> -- Select -- </option></select>";
    VehicleChassisAndRegNumber += "<span class='edited'></span>";
    appendDebentureVehicleRow.VehicleChassisAndRegNumber = VehicleChassisAndRegNumber;

    appendDebentureVehicleRow.VehicleType = VehicleType;
    appendDebentureVehicleRow.VehicleMake = VehicleMake;
    appendDebentureVehicleRow.VehicleModel = VehicleModel;
    appendDebentureVehicleRow.COEExpiryDate = COEExpiryDate;
    appendDebentureVehicleRow.EngineNumber = EngineNumber;
    appendDebentureVehicleRow.Value = Value;
    appendDebentureVehicleRow.ChargeNumber = ChargeNumber;
    appendDebentureVehicleRow.ChargeDate = ChargeDate;
}

$("#btnAddDebentureVehicleList").on("click", function () {
    if (tblDebentureVehicleList.rows().count() === 0) {
        initializeNewRowDebentureVehicleList();
    }
    else {
        isDebentureVehicleValidRow(this);
    }
    if (isDebentureVehicleValidRow(this)) {
        var _row = tblDebentureVehicleList.rows.add([[
			appendDebentureVehicleRow.Action,
			appendDebentureVehicleRow.CrossCollateralization,
			appendDebentureVehicleRow.CrossCollateralizationContractNumber,
			appendDebentureVehicleRow.CrossCollateralizationRolloverNumber,
			appendDebentureVehicleRow.CrossCollateraizationVehicleChassisAndRegNumber,
			appendDebentureVehicleRow.IndividualCorporate,
			appendDebentureVehicleRow.CustomerThirdParty,
			appendDebentureVehicleRow.VehicleChassisAndRegNumber,
			appendDebentureVehicleRow.VehicleType,
			appendDebentureVehicleRow.VehicleMake,
			appendDebentureVehicleRow.VehicleModel,
			appendDebentureVehicleRow.COEExpiryDate,
			appendDebentureVehicleRow.ChargeNumber,
			appendDebentureVehicleRow.ChargeDate,
			appendDebentureVehicleRow.EngineNumber,
			appendDebentureVehicleRow.Value
        ]]);
        _row.draw();
        setActiveRowDebentureVehicleList($("#tblDebentureVehicleList tr:last"));
        setCustomerThirdParty("tblDebentureVehicleList");
    }
});
$('#tblDebentureVehicleList tbody').on('click', 'button.deleting', function () {
    tblDebentureVehicleList.row($(this).parents("tr")).remove().draw();
    $('#fldDebentureVehicle').removeClass('error-div');
});

$('#tblDebentureVehicleList tbody').on('click', '.crosscollateralization', function () {
    showHideDebentureVehicleListField();
});

window.showHideDebentureVehicleListField = function () {
    var activeRow = $("#tblDebentureVehicleList tr.activeRow");
    if (activeRow.find('.crosscollateralization').is(':checked')) {
        activeRow.find(".crosscollateralizationcontractnumber").attr("disabled", false);
        activeRow.find(".crosscollateralizationrollovernumber").attr("disabled", false);
        activeRow.find(".crosscollateraizationvehiclechassisandregnumber").attr("disabled", false);

        activeRow.find(".individualcorporate").prepend($('<option value="select">' + "--Select--" + '</option>'));
        activeRow.find(".individualcorporate").find('option:first').prop('selected', true);

        activeRow.find(".customerthirdparty").val('');
        activeRow.find(".vehiclechassisandregnumber").html('');
        activeRow.find(".vehiclechassisandregnumber").append($('<option value="">' + "--Select--" + '</option>'));


        activeRow.find(".individualcorporate").attr("disabled", true);
        activeRow.find(".customerthirdparty").attr("disabled", true);
        activeRow.find(".vehiclechassisandregnumber").attr("disabled", true);

        activeRow.find(".crosscollateralizationcontractnumber").removeClass("unrequired");
        activeRow.find(".crosscollateralizationrollovernumber").removeClass("unrequired");
        activeRow.find(".crosscollateraizationhullnumberandvesselname").removeClass("unrequired");

        activeRow.find(".individualcorporate").addClass("unrequired").removeClass("alert-danger");
        activeRow.find(".customerthirdparty").addClass("unrequired").removeClass("alert-danger");
        activeRow.find(".vehiclechassisandregnumber").addClass("unrequired").removeClass("alert-danger");
        activeRow.find(".vehiclechassisandregnumber").next('span').find('.selection-danger').removeClass('selection-danger');
    }
    else {
        activeRow.find(".crosscollateralizationcontractnumber").addClass("unrequired").removeClass("alert-danger");
        activeRow.find(".crosscollateralizationrollovernumber").addClass("unrequired").removeClass("alert-danger");
        activeRow.find(".crosscollateraizationvehiclechassisandregnumber").addClass("unrequired").removeClass("alert-danger");
        activeRow.find(".crosscollateraizationvehiclechassisandregnumber").next('span').find('.selection-danger').removeClass('selection-danger');

        activeRow.find(".individualcorporate").removeClass("unrequired");
        activeRow.find(".customerthirdparty").removeClass("unrequired");
        activeRow.find(".vehiclechassisandregnumber").removeClass("unrequired");

        activeRow.find(".crosscollateralizationcontractnumber").val('');
        activeRow.find(".crosscollateralizationrollovernumber").val('0');
        activeRow.find(".crosscollateraizationvehiclechassisandregnumber").html('');
        activeRow.find(".crosscollateraizationvehiclechassisandregnumber").append($('<option value="">' + "--Select--" + '</option>'));



        activeRow.find(".crosscollateralizationcontractnumber").attr("disabled", true);
        activeRow.find(".crosscollateralizationrollovernumber").attr("disabled", true);
        activeRow.find(".crosscollateraizationvehiclechassisandregnumber").attr("disabled", true);

        activeRow.find(".individualcorporate").attr("disabled", false);
        activeRow.find(".customerthirdparty").attr("disabled", false);
        activeRow.find(".vehiclechassisandregnumber").attr("disabled", false);

        activeRow.find(".individualcorporate option[value='select']").remove();
    }
};

$("#tblDebentureVehicleList tbody").on('click', 'tr', function (event) {
    targetRowEventDebentureVehicleList(event);
    if (!($(this).hasClass("activeRow"))) {
        if (isDebentureVehicleValidRow(this))
            setActiveRowDebentureVehicleList(this);
    }
});
window.targetRowEventDebentureVehicleList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblDebentureVehicleList tbody tr.activeRow').length &&
		!$(event.target).closest('#tblDebentureVehicleList thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAddDebentureVehicleList').length &&
		!$(event.target).closest('#tblDebentureVehicleList').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isDebentureVehicleValidRow(event))
            setActiveRowDebentureVehicleList(null);

    }
};

window.isChassisNumberExist = function (event, el) {
    var isDuplicate = false;
    var hullnum = $(event).find(".vehiclechassisandregnumber").val();
    if (hullnum) {
        $("#tblDebentureVehicleList tbody tr").each(function () {
            var hullnum1 = $(this).find(".vehiclechassisandregnumber").val();
            var isactiverow = $(this).hasClass('activeRow');
            if (hullnum === hullnum1 && !isactiverow) {
                if (el !== null && ($(el).attr('id') === 'btnAddDebentureVehicleList' || $(el.target).attr('id') === 'SaveContract'))
                    swal(swalGlobal.SwalTitle_Error, "Chassis number selected already exist!", swalGlobal.SwalType_Error);
                isDuplicate = true;
            }
        });
    }
    return isDuplicate;
};


function isDebentureVehicleValidRow(el) {
    var activeRow = $("#tblDebentureVehicleList tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).next().find('.select2-selection').removeClass("selection-danger");
                }
            }
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if ($(this).val() === "" || $(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }

            }
        });
    }
    else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isChassisNumberExist(activeRow, el);
    if (isDuplicate) {
        $(activeRow).find(".vehiclechassisandregnumber").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".vehiclechassisandregnumber").next().find('.select2-selection').removeClass("selection-danger");

    if (!isValid) {
        $('#fldDebentureVehicle').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddDebentureVehicleList' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldDebentureVehicle').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setActiveRowDebentureVehicleList = function (row) {
    $("#tblDebentureVehicleList tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                if ($(this).is('input:checkbox')) {
                    if ($(this).is(':checked')) {
                        $(this).next('span').html('<i class="fa fa-check"></i>');
                    }
                    else {
                        $(this).next('span').html('');
                    }
                }
                else {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        if ($(this).val() !== "")
                            DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                        $(this).next().remove();
                    }
                    if ($(this).val() === "" || $(this).val() === "select") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
        showHideDebentureVehicleListField();
    }
    $('#tblDebentureVehicleList .activeRow .js-example-basic-sec3').select2();
};

window.setCrossCollateralizationDebentureVehicle = function (object) {
    var options = "";
    var SubConGroupCode = (new URL(location.href)).searchParams.get('SubConGroupCode');
    var activeRow = $("#tblDebentureVehicleList tr.activeRow");
    var cr_coll_cont_num = activeRow.find('.crosscollateralizationcontractnumber').val();
    var cr_coll_roll_num = activeRow.find('.crosscollateralizationrollovernumber').val() || 0;
    $.ajax({
        url: varSitePath + '/PreCon/getCrossCollateralizationVehicle',
        data: { SubConGroupCode: SubConGroupCode, crossCollateralizationContactNumber: cr_coll_cont_num, crossCollateralizationRolloverNumber: cr_coll_roll_num },
        type: 'GET',
        cache: false,
        async: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    var chargedate = isNullOrEmpty(v.ChargeDate);
                    options += "<option vehicletype ='" + v.ChassisNumber +
						"' coeexpirydate='" + v.COE_ExpiryDate +
						"' value1='" + v.Value +
						"' vehiclemake='" + v.VehicleMake +
						"' vehiclemodel='" + v.VehicleModel +
						"' enginenumber='" + v.EngineNumber +
						"' chargenumber='" + v.ChargeNumber +
						"' chargedate='" + chargedate +
						"' value='" + v.ID + "'>" + v.ChassisNumber + " - " + v.RegNumber + "</option>";
                });
                activeRow.find('.crosscollateraizationvehiclechassisandregnumber').html(options);
                setValueChangeOfCrossVehicleChassisAndRegNumber('crosscollateraizationvehiclechassisandregnumber');
            }
            else {
                swal(swalGlobal.SwalTitle_Error, "Invalid Contract Number!", swalGlobal.SwalType_Error);
                ResetDebentureVehicleList();
            }
        }
    });
};

window.setVehicleChassisAndRegNumber = function () {
    var options = "";
    var activeRow = $("#tblDebentureVehicleList tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getVehicleChassisAndRegNumber',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    var chargedate = isNullOrEmpty(v.ChargeDate);
                    options += "<option vehicletype ='" + v.ChassisNumber +
						"' coeexpirydate='" + v.COE_ExpiryDate +
						"' vehiclemake='" + v.VehicleMake +
						"' vehiclemodel='" + v.VehicleModel +
						"' value1='" + v.Value +
						"' enginenumber='" + v.EngineNumber +
						"' chargenumber='" + v.ChargeNumber +
						"' chargedate='" + chargedate +
						"' value='" + v.ID + "'>" + v.ChassisNumber + " - " + v.RegNumber + "</option>";
                });
                activeRow.find('.vehiclechassisandregnumber').html(options);
                setValueChangeOfCrossVehicleChassisAndRegNumber('vehiclechassisandregnumber');
            }
            else {
                ResetDebentureVehicleList();
            }
        }
    });
};

window.setValueChangeOfCrossVehicleChassisAndRegNumber = function (column) {
    var activeRow = $("#tblDebentureVehicleList tr.activeRow");
    var selectedValue = activeRow.find('.' + column + ' option:selected');
    activeRow.find('.vehicletype').val(selectedValue.attr('vehicletype'));
    activeRow.find('.vehiclemake').val(selectedValue.attr('vehiclemake'));
    activeRow.find('.vehiclemodel').val(selectedValue.attr('vehiclemodel'));
    activeRow.find('.value').val(manualMask(selectedValue.attr('value1')));
    activeRow.find('.enginenumber').val(selectedValue.attr('enginenumber'));
    activeRow.find('.chargenumber').val(selectedValue.attr('chargenumber'));
    activeRow.find('.chargedate').val(selectedValue.attr('chargedate'));
    activeRow.find('.coeexpirydate').val(selectedValue.attr('coeexpirydate'));
};

window.ResetDebentureVehicleList = function () {
    var activeRow = $("#tblDebentureVehicleList tr.activeRow");
    activeRow.find('.crosscollateraizationvehiclechassisandregnumber').html('');
    activeRow.find('.crosscollateralizationcontractnumber').val('');
    activeRow.find('.vehicletype').val('');
    activeRow.find('.vehiclemake').val('');
    activeRow.find('.vehiclemodel').val('');
    activeRow.find('.value').val('');
    activeRow.find('.enginenumber').val('');
    activeRow.find('.chargenumber').val('');
    activeRow.find('.chargedate').val('');
    activeRow.find('.coeexpirydate').val('');
};




// Debenture Construction Equipment

window.tblDebentureConstructionEquipmentList = $("#tblDebentureConstructionEquipmentList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

var appendDebentureConstructionEquipmentRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
    "IndividualCorporate": "",
    "CustomerThirdParty": "", "Equipment": "", "SerialNumber": "", "SecuredAmount": "",
    "YearofManufacture": "", "EngineNumber": "",
    "ChargeNumber": "", "ChargeDate": "", "Status": ""
};

function initializeNewRowDebentureConstructionEquipmentList() {
    var IndividualCorporate = "<select class='editor individualcorporate js-example-basic-sec3' onchange='setCustomerThirdParty(\"tblDebentureConstructionEquipmentList\")' tabindex='3'>";
    var CustomerThirdParty = "<input data-id='' onfocus='setTableNameForCustomerThirdParty(this)' class='editor customerthirdparty ui-autocomplete-input' type='text' autocomplete='off'><span class='ctparty edited'></span>";

    var Equipment = "<select class='editor equipment js-example-basic-sec3' tabindex='4'>";
    var SerialNumber = "<input class='editor serialnumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var SecuredAmount = "<input class='editor securedamount unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var YearofManufacture = "<input class='editor yearofmanufacture unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var EngineNumber = "<input class='editor enginenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeNumber = "<input class='editor chargenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeDate = "<input class='editor chargedate unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var Status = "<input class='editor status unrequired' type='text' disabled='disabled'><span class='edited'></span>";

    IndividualCorporate += "<option>Individual</option><option>Corporate</option>";
    IndividualCorporate += "</select><span class='edited'></span>";
    appendDebentureConstructionEquipmentRow.IndividualCorporate = IndividualCorporate;

    CustomerThirdParty += "</select><span class='edited'></span>";
    appendDebentureConstructionEquipmentRow.CustomerThirdParty = CustomerThirdParty;

    Equipment += "<option disabled value='' selected> -- Select -- </option></select>";
    Equipment += "<span class='edited'></span>";
    appendDebentureConstructionEquipmentRow.Equipment = Equipment;

    appendDebentureConstructionEquipmentRow.SerialNumber = SerialNumber;
    appendDebentureConstructionEquipmentRow.SecuredAmount = SecuredAmount;
    appendDebentureConstructionEquipmentRow.YearofManufacture = YearofManufacture;
    appendDebentureConstructionEquipmentRow.EngineNumber = EngineNumber;
    appendDebentureConstructionEquipmentRow.ChargeNumber = ChargeNumber;
    appendDebentureConstructionEquipmentRow.ChargeDate = ChargeDate;
}

$("#btnAddDebentureConstructionEquipmentList").on("click", function () {
    if (tblDebentureConstructionEquipmentList.rows().count() === 0) {
        initializeNewRowDebentureConstructionEquipmentList();
    }
    if (isDebentureConstructionEquipmentValidRow(this)) {
        var _row = tblDebentureConstructionEquipmentList.rows.add([[
			appendDebentureConstructionEquipmentRow.Action,
			appendDebentureConstructionEquipmentRow.IndividualCorporate,
			appendDebentureConstructionEquipmentRow.CustomerThirdParty,
			appendDebentureConstructionEquipmentRow.Equipment,
			appendDebentureConstructionEquipmentRow.SerialNumber,
			appendDebentureConstructionEquipmentRow.SecuredAmount,
			appendDebentureConstructionEquipmentRow.ChargeNumber,
			appendDebentureConstructionEquipmentRow.ChargeDate,
			appendDebentureConstructionEquipmentRow.EngineNumber,
			appendDebentureConstructionEquipmentRow.YearofManufacture
        ]]);
        _row.draw();
        setActiveRowDebentureConstructionEquipmentList($("#tblDebentureConstructionEquipmentList tr:last"));
        setCustomerThirdParty("tblDebentureConstructionEquipmentList");
    }
});
$('#tblDebentureConstructionEquipmentList tbody').on('click', 'button.deleting', function () {
    tblDebentureConstructionEquipmentList.row($(this).parents("tr")).remove().draw();
    $('#fldDebentureConstructionEquipment').removeClass('error-div');
});

$("#tblDebentureConstructionEquipmentList tbody").on('click', 'tr', function (event) {
    if (!($(this).hasClass("activeRow"))) {
        if (isDebentureConstructionEquipmentValidRow(this))
            setActiveRowDebentureConstructionEquipmentList(this);
    }
});
window.targetRowEventDebentureConstructionEquipmentList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblDebentureConstructionEquipmentList tbody tr.activeRow').length &&
		!$(event.target).closest('#tblDebentureConstructionEquipmentList thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.select2-container').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAddDebentureConstructionEquipmentList').length &&
		!$(event.target).closest('#tblDebentureConstructionEquipmentList').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isDebentureConstructionEquipmentValidRow(event))
            setActiveRowDebentureConstructionEquipmentList(null);

    }
};

window.isEquipmentExist = function (event, el) {
    var isDuplicate = false;
    var equipment = $(event).find(".equipment").val();
    if (equipment) {
        $("#tblDebentureConstructionEquipmentList tbody tr").each(function () {
            var equipment1 = $(this).find(".equipment").val();
            var isactiverow = $(this).hasClass('activeRow');
            if (equipment === equipment1 && !isactiverow) {
                if (el !== null && ($(el).attr('id') === 'btnAddDebentureConstructionEquipmentList' || $(el.target).attr('id') === 'SaveContract'))
                    swal(swalGlobal.SwalTitle_Error, "Equipment selected already exist!", swalGlobal.SwalType_Error);
                isDuplicate = true;
            }
        });
    }
    return isDuplicate;
};


function isDebentureConstructionEquipmentValidRow(el) {
    var activeRow = $("#tblDebentureConstructionEquipmentList tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).next().find('.select2-selection').removeClass("selection-danger");
                }
            }
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if ($(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    if (!$(this).val()) {
                        $(this).addClass("alert-danger");
                        if ($(this).hasClass("js-example-basic-sec3")) {
                            $(this).next().find('.select2-selection').addClass("selection-danger");
                        }
                    }
                    else {
                        $(this).removeClass("alert-danger");
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next().find('.select2-selection').removeClass("selection-danger");
                        }
                    }
                }
            }
        });
    }
    else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isEquipmentExist(activeRow, el);
    if (isDuplicate) {
        $(activeRow).find(".equipment").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".equipment").next().find('.select2-selection').removeClass("selection-danger");

    if (!isValid) {
        $('#fldDebentureConstructionEquipment').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddDebentureConstructionEquipmentList' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldDebentureConstructionEquipment').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setActiveRowDebentureConstructionEquipmentList = function (row) {
    $("#tblDebentureConstructionEquipmentList tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                if ($(this).is('input:checkbox')) {
                    if ($(this).is(':checked')) {
                        $(this).next('span').html('<i class="fa fa-check"></i>');
                    }
                    else {
                        $(this).next('span').html('');
                    }
                }
                else {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        if ($(this).val() !== "")
                            DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                        $(this).next().remove();
                    }
                    if ($(this).val() === "") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
    }
    $('#tblDebentureConstructionEquipmentList .activeRow .js-example-basic-sec3').select2();
};


window.setConstructionEquip = function () {
    var options = "";
    var activeRow = $("#tblDebentureConstructionEquipmentList tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getConstructionEquip',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    options += "<option serialnumber ='" + v.SerialNumber +
						"' securedamount ='" + v.SecuredAmount +
						"' enginenumber ='" + v.EngineNumber +
						"' yearofmanufacture ='" + v.YearOfManufacture +
						"' enginenumber='" + v.EngineNumber +
						"' chargenumber='" + v.ChargeNumber +
						"' chargedate='" + v.ChargeDate +
						"' value='" + v.ID + "'>" + v.EquipBrand + " - " + v.EquipModel + "</option>";
                });
                activeRow.find('.equipment').html(options);
                setValueChangeOfConstructionEquipment();
            }
            else {
                ResetConstructionEquipment();
            }
        }
    });
};

window.setValueChangeOfConstructionEquipment = function () {
    var activeRow = $("#tblDebentureConstructionEquipmentList tr.activeRow");
    var selectedValue = activeRow.find('.equipment option:selected');
    activeRow.find('.serialnumber').val(selectedValue.attr('serialnumber'));
    activeRow.find('.securedamount').val(manualMask(selectedValue.attr('securedamount')));
    activeRow.find('.yearofmanufacture').val(selectedValue.attr('yearofmanufacture'));
    activeRow.find('.chargenumber').val(selectedValue.attr('chargenumber'));
    activeRow.find('.enginenumber').val(selectedValue.attr('enginenumber'));
    activeRow.find('.chargedate').val(selectedValue.attr('chargedate'));
};

window.ResetConstructionEquipment = function () {
    var activeRow = $("#tblDebentureConstructionEquipmentList tr.activeRow");
    activeRow.find('.equipment').html('<option>--Select--</option>');
    activeRow.find('.serialnumber').val('');
    activeRow.find('.securedamount').val('');
    activeRow.find('.yearofmanufacture').val('');
    activeRow.find('.chargenumber').val('');
    activeRow.find('.enginenumber').val('');
    activeRow.find('.chargedate').val('');
};






// Debenture Industrial Equipment

window.tblDebentureIndustrialEquipmentList = $("#tblDebentureIndustrialEquipmentList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

var appendDebentureIndustrialEquipmentRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
    "IndividualCorporate": "",
    "CustomerThirdParty": "", "Equipment": "", "SerialNumber": "", "SecuredAmount": "",
    "YearofManufacture": "", "EngineNumber": "",
    "ChargeNumber": "", "ChargeDate": "", "Status": ""
};

function initializeNewRowDebentureIndustrialEquipmentList() {
    var IndividualCorporate = "<select class='editor individualcorporate js-example-basic-sec3' onchange='setCustomerThirdParty(\"tblDebentureIndustrialEquipmentList\")' tabindex='3'>";
    var CustomerThirdParty = "<input data-id='' onfocus='setTableNameForCustomerThirdParty(this)' class='editor customerthirdparty ui-autocomplete-input' type='text' autocomplete='off'><span class='ctparty edited'></span>";

    var Equipment = "<select class='editor industrial js-example-basic-sec3' tabindex='4'>";
    var SerialNumber = "<input class='editor serialnumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var SecuredAmount = "<input class='editor securedamount unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var YearofManufacture = "<input class='editor yearofmanufacture unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var EngineNumber = "<input class='editor enginenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeNumber = "<input class='editor chargenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeDate = "<input class='editor chargedate unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var Status = "<input class='editor status unrequired' type='text' disabled='disabled'><span class='edited'></span>";

    IndividualCorporate += "<option>Individual</option><option>Corporate</option>";
    IndividualCorporate += "</select><span class='edited'></span>";
    appendDebentureIndustrialEquipmentRow.IndividualCorporate = IndividualCorporate;

    CustomerThirdParty += "</select><span class='edited'></span>";
    appendDebentureIndustrialEquipmentRow.CustomerThirdParty = CustomerThirdParty;

    Equipment += "<option disabled value='' selected> -- Select -- </option></select>";
    Equipment += "<span class='edited'></span>";
    appendDebentureIndustrialEquipmentRow.Equipment = Equipment;

    appendDebentureIndustrialEquipmentRow.SerialNumber = SerialNumber;
    appendDebentureIndustrialEquipmentRow.SecuredAmount = SecuredAmount;
    appendDebentureIndustrialEquipmentRow.YearofManufacture = YearofManufacture;
    appendDebentureIndustrialEquipmentRow.EngineNumber = EngineNumber;
    appendDebentureIndustrialEquipmentRow.ChargeNumber = ChargeNumber;
    appendDebentureIndustrialEquipmentRow.ChargeDate = ChargeDate;
}

$("#btnAddDebentureIndustrialEquipmentList").on("click", function () {
    if (tblDebentureIndustrialEquipmentList.rows().count() === 0) {
        initializeNewRowDebentureIndustrialEquipmentList();
    }
    if (isDebentureIndustrialEquipmentValidRow(this)) {
        var _row = tblDebentureIndustrialEquipmentList.rows.add([[
			appendDebentureIndustrialEquipmentRow.Action,
			appendDebentureIndustrialEquipmentRow.IndividualCorporate,
			appendDebentureIndustrialEquipmentRow.CustomerThirdParty,
			appendDebentureIndustrialEquipmentRow.Equipment,
			appendDebentureIndustrialEquipmentRow.SerialNumber,
			appendDebentureIndustrialEquipmentRow.SecuredAmount,
			appendDebentureIndustrialEquipmentRow.ChargeNumber,
			appendDebentureIndustrialEquipmentRow.ChargeDate,
			appendDebentureIndustrialEquipmentRow.EngineNumber,
			appendDebentureIndustrialEquipmentRow.YearofManufacture
        ]]);
        _row.draw();
        setActiveRowDebentureIndustrialEquipmentList($("#tblDebentureIndustrialEquipmentList tr:last"));
        setCustomerThirdParty("tblDebentureIndustrialEquipmentList");
    }
});
$('#tblDebentureIndustrialEquipmentList tbody').on('click', 'button.deleting', function () {
    tblDebentureIndustrialEquipmentList.row($(this).parents("tr")).remove().draw();
    $('#fldDebentureIndustrialEquipment').removeClass('error-div');
});

$("#tblDebentureIndustrialEquipmentList tbody").on('click', 'tr', function (event) {
    if (!($(this).hasClass("activeRow"))) {
        if (isDebentureIndustrialEquipmentValidRow(this))
            setActiveRowDebentureIndustrialEquipmentList(this);
    }
});
window.targetRowEventDebentureIndustrialEquipmentList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblDebentureIndustrialEquipmentList tbody tr.activeRow').length &&
		!$(event.target).closest('#tblDebentureIndustrialEquipmentList thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.select2-container').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAddDebentureIndustrialEquipmentList').length &&
		!$(event.target).closest('#tblDebentureIndustrialEquipmentList').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isDebentureIndustrialEquipmentValidRow(event))
            setActiveRowDebentureIndustrialEquipmentList(null);

    }
};

window.isIndustrialExist = function (event, el) {
    var isDuplicate = false;
    var equipment = $(event).find(".industrial").val();
    if (equipment) {
        $("#tblDebentureIndustrialEquipmentList tbody tr").each(function () {
            var equipment1 = $(this).find(".industrial").val();
            var isactiverow = $(this).hasClass('activeRow');
            if (equipment === equipment1 && !isactiverow) {
                if (el !== null && ($(el).attr('id') === 'btnAddDebentureIndustrialEquipmentList' || $(el.target).attr('id') === 'SaveContract'))
                    swal(swalGlobal.SwalTitle_Error, "Equipment selected already exist!", swalGlobal.SwalType_Error);
                isDuplicate = true;
            }
        });
    }
    return isDuplicate;
};


function isDebentureIndustrialEquipmentValidRow(el) {
    var activeRow = $("#tblDebentureIndustrialEquipmentList tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).find('.select2-selection').removeClass("selection-danger");
                }
            }
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if ($(this).val() === "" || $(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }
            }
        });
    }
    else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isIndustrialExist(activeRow, el);
    if (isDuplicate) {
        $(activeRow).find(".industrial").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".industrial").next().find('.select2-selection').removeClass("selection-danger");

    if (!isValid) {
        $('#fldDebentureIndustrialEquipment').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddDebentureIndustrialEquipmentList' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldDebentureIndustrialEquipment').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setActiveRowDebentureIndustrialEquipmentList = function (row) {
    $("#tblDebentureIndustrialEquipmentList tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                if ($(this).is('input:checkbox')) {
                    if ($(this).is(':checked')) {
                        $(this).next('span').html('<i class="fa fa-check"></i>');
                    }
                    else {
                        $(this).next('span').html('');
                    }
                }
                else {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        if ($(this).val() !== "")
                            DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                        $(this).next().remove();
                    }
                    if ($(this).val() === "") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
    }
    $('#tblDebentureIndustrialEquipmentList .activeRow .js-example-basic-sec3').select2();
};


window.setIndustrialEquip = function () {
    var options = "";
    var activeRow = $("#tblDebentureIndustrialEquipmentList tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getIndustrialEquip',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    options += "<option serialnumber ='" + v.SerialNumber +
						"' securedamount ='" + v.SecuredAmount +
						"' enginenumber ='" + v.EngineNumber +
						"' yearofmanufacture ='" + v.YearOfManufacture +
						"' enginenumber='" + v.EngineNumber +
						"' chargenumber='" + v.ChargeNumber +
						"' chargedate='" + v.ChargeDate +
						"' value='" + v.ID + "'>" + v.EquipBrand + " - " + v.EquipModel + "</option>";
                });
                activeRow.find('.industrial').html(options);
                setValueChangeOfIndustrialEquipment();
            }
            else {
                ResetIndustrialEquipment();
            }
        }
    });
};

window.setValueChangeOfIndustrialEquipment = function () {
    var activeRow = $("#tblDebentureIndustrialEquipmentList tr.activeRow");
    var selectedValue = activeRow.find('.industrial option:selected');
    activeRow.find('.serialnumber').val(selectedValue.attr('serialnumber'));
    activeRow.find('.securedamount').val(manualMask(selectedValue.attr('securedamount')));
    activeRow.find('.yearofmanufacture').val(selectedValue.attr('yearofmanufacture'));
    activeRow.find('.chargenumber').val(selectedValue.attr('chargenumber'));
    activeRow.find('.enginenumber').val(selectedValue.attr('enginenumber'));
    activeRow.find('.chargedate').val(selectedValue.attr('chargedate'));
};

window.ResetIndustrialEquipment = function () {
    var activeRow = $("#tblDebentureIndustrialEquipmentList tr.activeRow");
    activeRow.find('.industrial').html('<option>--Select--</option>');
    activeRow.find('.serialnumber').val('');
    activeRow.find('.securedamount').val('');
    activeRow.find('.yearofmanufacture').val('');
    activeRow.find('.chargenumber').val('');
    activeRow.find('.enginenumber').val('');
    activeRow.find('.chargedate').val('');
};






// Inventory List

window.tblDebentureInventoryList = $("#tblDebentureInventoryList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

var appendDebentureInventoryRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
    "IndividualCorporate": "",
    "CustomerThirdParty": "", "TypeDescription": "", "Value": "",
    "ChargeNumber": "", "ChargeDate": "", "Status": ""
};

function initializeNewRowDebentureInventoryList() {
    var IndividualCorporate = "<select class='editor individualcorporate js-example-basic-sec3' onchange='setCustomerThirdParty(\"tblDebentureInventoryList\")' tabindex='3'>";
    var CustomerThirdParty = "<input data-id='' onfocus='setTableNameForCustomerThirdParty(this)' class='editor customerthirdparty ui-autocomplete-input' type='text' autocomplete='off'><span class='ctparty edited'></span>";

    var TypeDescription = "<select class='editor typedescription js-example-basic-sec3' tabindex='4'>";
    var ChargeNumber = "<input class='editor chargenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var Value = "<input class='editor value unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeDate = "<input class='editor chargedate unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var Status = "<input class='editor status unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    
    IndividualCorporate += "<option>Individual</option><option>Corporate</option>";
    IndividualCorporate += "</select><span class='edited'></span>";
    appendDebentureInventoryRow.IndividualCorporate = IndividualCorporate;

    CustomerThirdParty += "</select><span class='edited'></span>";
    appendDebentureInventoryRow.CustomerThirdParty = CustomerThirdParty;

    TypeDescription += "<option disabled value='' selected> -- Select -- </option></select>";
    TypeDescription += "<span class='edited'></span>";
    appendDebentureInventoryRow.TypeDescription = TypeDescription;

    appendDebentureInventoryRow.Value = Value;
    appendDebentureInventoryRow.ChargeNumber = ChargeNumber;
    appendDebentureInventoryRow.ChargeDate = ChargeDate;
}

$("#btnAddDebentureInventoryList").on("click", function () {
    if (tblDebentureInventoryList.rows().count() === 0) {
        initializeNewRowDebentureInventoryList();
    }
    else {
        isDebentureInventoryValidRow(this);
    }
    if (isDebentureInventoryValidRow(this)) {
        var _row = tblDebentureInventoryList.rows.add([[
			appendDebentureInventoryRow.Action,			
			appendDebentureInventoryRow.IndividualCorporate,
			appendDebentureInventoryRow.CustomerThirdParty,
			appendDebentureInventoryRow.TypeDescription,
			appendDebentureInventoryRow.Value,
			appendDebentureInventoryRow.ChargeNumber,
			appendDebentureInventoryRow.ChargeDate
        ]]);
        _row.draw();
        setActiveRowDebentureInventoryList($("#tblDebentureInventoryList tr:last"));
        setCustomerThirdParty("tblDebentureInventoryList");
    }
});
$('#tblDebentureInventoryList tbody').on('click', 'button.deleting', function () {
    tblDebentureInventoryList.row($(this).parents("tr")).remove().draw();
    $('#fldDebentureInventory').removeClass('error-div');
});

window.showHideDebentureInventoryListField = function () {
    var activeRow = $("#tblDebentureInventoryList tr.activeRow");        
    activeRow.find(".individualcorporate").removeClass("unrequired");
    activeRow.find(".customerthirdparty").removeClass("unrequired");
    activeRow.find(".typedescription").removeClass("unrequired");

    activeRow.find(".individualcorporate").attr("disabled", false);
    activeRow.find(".customerthirdparty").attr("disabled", false);
    activeRow.find(".typedescription").attr("disabled", false);

    activeRow.find(".individualcorporate option[value='select']").remove();
};

$("#tblDebentureInventoryList tbody").on('click', 'tr', function (event) {
    targetRowEventDebentureInventoryList(event);
    if (!($(this).hasClass("activeRow"))) {
        if (isDebentureInventoryValidRow(this))
            setActiveRowDebentureInventoryList(this);
    }
});
window.targetRowEventDebentureInventoryList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblDebentureInventoryList tbody tr.activeRow').length &&
		!$(event.target).closest('#tblDebentureInventoryList thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAddDebentureInventoryList').length &&
		!$(event.target).closest('#tblDebentureInventoryList').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isDebentureInventoryValidRow(event))
            setActiveRowDebentureInventoryList(null);

    }
};

window.isInventoryExist = function (event, el) {
    var isDuplicate = false;
    var hullnum = $(event).find(".typedescription").val();
    if (hullnum) {
        $("#tblDebentureInventoryList tbody tr").each(function () {
            var hullnum1 = $(this).find(".typedescription").val();
            var isactiverow = $(this).hasClass('activeRow');
            if (hullnum === hullnum1 && !isactiverow) {
                if (el !== null && ($(el).attr('id') === 'btnAddDebentureInventoryList' || $(el.target).attr('id') === 'SaveContract'))
                    swal(swalGlobal.SwalTitle_Error, "Inventory selected already exist!", swalGlobal.SwalType_Error);
                isDuplicate = true;
            }
        });
    }
    return isDuplicate;
};


function isDebentureInventoryValidRow(el) {
    var activeRow = $("#tblDebentureInventoryList tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).next().find('.select2-selection').removeClass("selection-danger");
                }
            }
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if ($(this).val() === "" || $(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }
            }
        });
    }
    else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isInventoryExist(activeRow, el);
    if (isDuplicate) {
        $(activeRow).find(".typedescription").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".typedescription").next().find('.select2-selection').removeClass("selection-danger");

    if (!isValid) {
        $('#fldDebentureInventory').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddDebentureInventoryList' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldDebentureInventory').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setActiveRowDebentureInventoryList = function (row) {
    $("#tblDebentureInventoryList tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                if ($(this).is('input:checkbox')) {
                    if ($(this).is(':checked')) {
                        $(this).next('span').html('<i class="fa fa-check"></i>');
                    }
                    else {
                        $(this).next('span').html('');
                    }
                }
                else {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        if ($(this).val() !== "")
                            DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                        $(this).next().remove();
                    }
                    if ($(this).val() === "" || $(this).val() === "select") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
        showHideDebentureInventoryListField();
    }
    $('#tblDebentureInventoryList .activeRow .js-example-basic-sec3').select2();
};

window.setTypeDescription = function () {
    var options = "";
    var activeRow = $("#tblDebentureInventoryList tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getInventoryTypeDescription',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    var chargedate = isNullOrEmpty(v.ChargeDate);
                    options += "<option value1='" + v.Value +
						"' chargenumber='" + v.ChargeNumber +
						"' chargedate='" + chargedate +
						"' value='" + v.ID + "'>" + v.Type + "</option>";
                });
                activeRow.find('.typedescription').html(options);
                setValueChangeOfCrossTypeDescription('typedescription');
            }
            else {
                ResetDebentureInventoryList();
            }
        }
    });
};

window.setValueChangeOfCrossTypeDescription = function (column) {
    var activeRow = $("#tblDebentureInventoryList tr.activeRow");
    var selectedValue = activeRow.find('.' + column + ' option:selected');
    activeRow.find('.value').val(manualMask(selectedValue.attr('value1')));
    activeRow.find('.chargenumber').val(selectedValue.attr('chargenumber'));
    activeRow.find('.chargedate').val(selectedValue.attr('chargedate'));
};

window.ResetDebentureInventoryList = function () {
    var activeRow = $("#tblDebentureInventoryList tr.activeRow");    
    activeRow.find('.value').val('');
    activeRow.find('.chargenumber').val('');
    activeRow.find('.chargedate').val('');
};



// Receivable List

window.tblDebentureReceivableList = $("#tblDebentureReceivableList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

var appendDebentureReceivableRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
    "IndividualCorporate": "",
    "CustomerThirdParty": "", "Amount": "",
    "ChargeNumber": "", "ChargeDate": "", "Status": ""
};

function initializeNewRowDebentureReceivableList() {
    var IndividualCorporate = "<select class='editor individualcorporate js-example-basic-sec3' onchange='setCustomerThirdParty(\"tblDebentureReceivableList\")' tabindex='3'>";
    var CustomerThirdParty = "<input data-id='' onfocus='setTableNameForCustomerThirdParty(this)' class='editor customerthirdparty ui-autocomplete-input' type='text' autocomplete='off'><span class='ctparty edited'></span>";

    var Amount = "<select class='editor receivableamount js-example-basic-sec3' tabindex='4'>";
    var ChargeNumber = "<input class='editor chargenumber unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeDate = "<input class='editor chargedate unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var Status = "<input class='editor status unrequired' type='text' disabled='disabled'><span class='edited'></span>";

    IndividualCorporate += "<option>Individual</option><option>Corporate</option>";
    IndividualCorporate += "</select><span class='edited'></span>";
    appendDebentureReceivableRow.IndividualCorporate = IndividualCorporate;

    CustomerThirdParty += "</select><span class='edited'></span>";
    appendDebentureReceivableRow.CustomerThirdParty = CustomerThirdParty;

    Amount += "<option disabled value='' selected> -- Select -- </option></select>";
    Amount += "<span class='edited'></span>";
    appendDebentureReceivableRow.Amount = Amount;

    appendDebentureReceivableRow.ChargeNumber = ChargeNumber;
    appendDebentureReceivableRow.ChargeDate = ChargeDate;
}

$("#btnAddDebentureReceivableList").on("click", function () {
    if (tblDebentureReceivableList.rows().count() === 0) {
        initializeNewRowDebentureReceivableList();
    }
    else {
        isDebentureReceivableValidRow(this);
    }
    if (isDebentureReceivableValidRow(this)) {
        var _row = tblDebentureReceivableList.rows.add([[
			appendDebentureReceivableRow.Action,			
			appendDebentureReceivableRow.IndividualCorporate,
			appendDebentureReceivableRow.CustomerThirdParty,
			appendDebentureReceivableRow.Amount,
			appendDebentureReceivableRow.ChargeNumber,
			appendDebentureReceivableRow.ChargeDate
        ]]);
        _row.draw();
        setActiveRowDebentureReceivableList($("#tblDebentureReceivableList tr:last"));
        setCustomerThirdParty("tblDebentureReceivableList");
    }
});
$('#tblDebentureReceivableList tbody').on('click', 'button.deleting', function () {
    tblDebentureReceivableList.row($(this).parents("tr")).remove().draw();
    $('#fldDebentureReceivable').removeClass('error-div');
});

window.showHideDebentureReceivableListField = function () {
    var activeRow = $("#tblDebentureReceivableList tr.activeRow");    
    
    activeRow.find(".individualcorporate").removeClass("unrequired");
    activeRow.find(".customerthirdparty").removeClass("unrequired");
    activeRow.find(".receivableamount").removeClass("unrequired");

    activeRow.find(".individualcorporate").attr("disabled", false);
    activeRow.find(".customerthirdparty").attr("disabled", false);
    activeRow.find(".receivableamount").attr("disabled", false);

    activeRow.find(".individualcorporate option[value='select']").remove();
};

$("#tblDebentureReceivableList tbody").on('click', 'tr', function (event) {
    targetRowEventDebentureReceivableList(event);
    if (!($(this).hasClass("activeRow"))) {
        if (isDebentureReceivableValidRow(this))
            setActiveRowDebentureReceivableList(this);
    }
});
window.targetRowEventDebentureReceivableList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblDebentureReceivableList tbody tr.activeRow').length &&
		!$(event.target).closest('#tblDebentureReceivableList thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAddDebentureReceivableList').length &&
		!$(event.target).closest('.select2-container').length &&
		!$(event.target).closest('#tblDebentureReceivableList').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isDebentureReceivableValidRow(event))
            setActiveRowDebentureReceivableList(null);

    }
};

window.isReceivableExist = function (event, el) {
    var isDuplicate = false;
    var amount = $(event).find(".receivableamount").val();
    if (amount) {
        $("#tblDebentureReceivableList tbody tr").each(function () {
            var amount1 = $(this).find(".receivableamount").val();
            var isactiverow = $(this).hasClass('activeRow');
            if (amount === amount1 && !isactiverow) {
                if (el !== null && ($(el).attr('id') === 'btnAddDebentureReceivableList' || $(el.target).attr('id') === 'SaveContract'))
                    swal(swalGlobal.SwalTitle_Error, "Amount selected already exist!", swalGlobal.SwalType_Error);
                isDuplicate = true;
            }
        });
    }
    return isDuplicate;
};

function isDebentureReceivableValidRow(el) {
    var activeRow = $("#tblDebentureReceivableList tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).next().find('.select2-selection').removeClass("selection-danger");
                }
            }
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if ($(this).val() === "" || $(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    if (!$(this).val()) {
                        $(this).addClass("alert-danger");
                        if ($(this).hasClass("js-example-basic-sec3")) {
                            $(this).next().find('.select2-selection').addClass("selection-danger");
                        }
                    }
                    else {
                        $(this).removeClass("alert-danger");
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next().find('.select2-selection').removeClass("selection-danger");
                        }
                    }
                }
            }
        });
    }
    else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isReceivableExist(activeRow, el);
    if (isDuplicate) {
        $(activeRow).find(".receivableamount").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".receivableamount").next().find('.select2-selection').removeClass("selection-danger");



    if (!isValid) {
        $('#fldDebentureReceivable').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddDebentureReceivableList' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldDebentureReceivable').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setActiveRowDebentureReceivableList = function (row) {
    $("#tblDebentureReceivableList tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                if ($(this).is('input:checkbox')) {
                    if ($(this).is(':checked')) {
                        $(this).next('span').html('<i class="fa fa-check"></i>');
                    }
                    else {
                        $(this).next('span').html('');
                    }
                }
                else {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        if ($(this).val() !== "")
                            DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                        $(this).next().remove();
                    }
                    if ($(this).val() === "" || $(this).val() === "select") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
        showHideDebentureReceivableListField();
    }
    $('#tblDebentureReceivableList .activeRow .js-example-basic-sec3').select2();
};

window.setReceivableAmount = function () {
    var options = "";
    var activeRow = $("#tblDebentureReceivableList tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getReceivableAmount',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    var chargedate = isNullOrEmpty(v.ChargeDate);
                    options += "<option chargenumber='" + v.ChargeNumber +
						"' chargedate='" + chargedate +
						"' value='" + v.ID + "'>" + manualMask(v.Amount) + "</option>";
                });
                activeRow.find('.receivableamount').html(options);
                setValueChangeOfCrossReceivableAmount('receivableamount');
            }
            else {
                ResetDebentureReceivableList();
            }
        }
    });
};

window.setValueChangeOfCrossReceivableAmount = function (column) {
    var activeRow = $("#tblDebentureReceivableList tr.activeRow");
    var selectedValue = activeRow.find('.' + column + ' option:selected');
    activeRow.find('.chargenumber').val(selectedValue.attr('chargenumber'));
    activeRow.find('.chargedate').val(selectedValue.attr('chargedate'));
};

window.ResetDebentureReceivableList = function () {
    var activeRow = $("#tblDebentureReceivableList tr.activeRow");    
    activeRow.find('.receivableamount').html('<option>--Select--</option>');
    activeRow.find('.chargenumber').val('');
    activeRow.find('.chargedate').val('');
};

//End Receivable List


//Common for 'Cash & Equivalent' and 'Security Deposit'

var appendDebentureCashAndEquivalentRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",    
    "CustomerThirdParty": "", "IndividualCorporate": "",
    "BillToAndAmount": "", "Refundable": "", "GuaranteeBondsType": "",
    "BillToNRIFINPASSPORT": "", "BillToROCUEN": "", "BillToAddress": "", "BillToDepartment": "",
    "BillToContactPerson": "", "BillToMobileNo": "", "BillToEmail": "", "BillToOfficeNo": "",
    "BillToHomeNo": "", "BillToFaxNo": "", "BillToPagerNo": "", "Status": ""
};

function initializeNewRowDebentureCashAndEquivalentList(btn) {       
    var CustomerThirdParty = "<input data-id='' onfocus='setTableNameForCustomerThirdParty(this)' class='editor customerthirdparty ui-autocomplete-input' type='text' autocomplete='off'><span class='ctparty edited'></span>";
    var IndividualCorporate = "<select class='editor individualcorporate js-example-basic-sec3' onchange='setCustomerThirdParty(\"tblSecurityDepositList\")' tabindex='3'>";

    var BillToAndAmount = "<select class='editor billtoandamount js-example-basic-sec3' tabindex='4'>";
    var Refundable = "<input class='editor refundable unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var GuaranteeBondsType = "<input class='editor guaranteebondstype unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToNRIFINPASSPORT = "<input class='editor billtonrifinpassport unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToROCUEN = "<input class='editor billtorocuen unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToAddress = "<input class='editor billtoaddress unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToDepartment = "<input class='editor billtodepartment unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToContactPerson = "<input class='editor billtocontactperson unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToMobileNo = "<input class='editor billtomobileno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToEmail = "<input class='editor billtoemail unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToOfficeNo = "<input class='editor billtoofficeno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToHomeNo = "<input class='editor billtohomeno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToFaxNo = "<input class='editor billtofaxno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var BillToPagerNo = "<input class='editor billtopagerno unrequired' type='text' disabled='disabled'><span class='edited'></span>";

    CustomerThirdParty += "</select><span class='edited'></span>";
    appendDebentureCashAndEquivalentRow.CustomerThirdParty = CustomerThirdParty;

    IndividualCorporate += "<option>Individual</option><option>Corporate</option>";
    IndividualCorporate += "</select><span class='edited'></span>";
    appendDebentureCashAndEquivalentRow.IndividualCorporate = IndividualCorporate;

    BillToAndAmount += "<option disabled value='' selected> -- Select-- </option></select>";
    BillToAndAmount += "<span class='edited'></span>";
    appendDebentureCashAndEquivalentRow.BillToAndAmount = BillToAndAmount;

    appendDebentureCashAndEquivalentRow.IndividualCorporate = IndividualCorporate;
    appendDebentureCashAndEquivalentRow.Refundable = Refundable;
    appendDebentureCashAndEquivalentRow.GuaranteeBondsType = GuaranteeBondsType;
    appendDebentureCashAndEquivalentRow.BillToNRIFINPASSPORT = BillToNRIFINPASSPORT;
    appendDebentureCashAndEquivalentRow.BillToROCUEN = BillToROCUEN;
    appendDebentureCashAndEquivalentRow.BillToAddress = BillToAddress;
    appendDebentureCashAndEquivalentRow.BillToDepartment = BillToDepartment;
    appendDebentureCashAndEquivalentRow.BillToContactPerson = BillToContactPerson;
    appendDebentureCashAndEquivalentRow.BillToMobileNo = BillToMobileNo;
    appendDebentureCashAndEquivalentRow.BillToEmail = BillToEmail;
    appendDebentureCashAndEquivalentRow.BillToOfficeNo = BillToOfficeNo;
    appendDebentureCashAndEquivalentRow.BillToHomeNo = BillToHomeNo;
    appendDebentureCashAndEquivalentRow.BillToFaxNo = BillToFaxNo;
    appendDebentureCashAndEquivalentRow.BillToPagerNo = BillToPagerNo;
}

window.showHideDebentureCashAndEquivalentListField = function (table) {
    var activeRow = $("#" + table + " tr.activeRow");
    
    activeRow.find(".individualcorporate").removeClass("unrequired");
    activeRow.find(".customerthirdparty").removeClass("unrequired");
    activeRow.find(".billtoandamount").removeClass("unrequired");

    activeRow.find(".individualcorporate").attr("disabled", false);
    activeRow.find(".customerthirdparty").attr("disabled", false);
    activeRow.find(".billtoandamount").attr("disabled", false);

    activeRow.find(".individualcorporate option[value='select']").remove();
};

window.isAmountExist = function (event, table, el) {
    var isDuplicate = false;
    var amount = $(event).find(".billtoandamount").val();
    if (amount) {
        $("#" + table + " tbody tr").each(function () {
            var amount1 = $(this).find(".billtoandamount").val();
            var isactiverow = $(this).hasClass('activeRow');
            if (amount === amount1 && !isactiverow) {
                if (el !== null && ($(el).attr('id') === 'btnAddSecurityDepositList' ||
					$(el).attr('id') === 'btnAddDebentureCashAndEquivalentCompanyList' ||
					$(el).attr('id') === 'btnAddDebentureCashAndEquivalentIndividualList' ||
					$(el.target).attr('id') === 'SaveContract'))
                    swal(swalGlobal.SwalTitle_Error, "Amount selected already exist!", swalGlobal.SwalType_Error);
                isDuplicate = true;
            }
        });
    }
    return isDuplicate;
};

window.setValueChangeOfCrossCashAndEquivalent = function (table, column) {
    var activeRow = $("#" + table + " tr.activeRow");
    var selectedValue = activeRow.find('.' + column + ' option:selected');
    activeRow.find('.refundable').val(selectedValue.attr('refundable'));
    activeRow.find('.guaranteebondstype').val(selectedValue.attr('guaranteebondstype'));
    activeRow.find('.billtoaddress').val(selectedValue.attr('billtoaddress'));
    activeRow.find('.billtocontactperson').val(selectedValue.attr('billtocontactperson'));
    activeRow.find('.billtodepartment').val(selectedValue.attr('billtodepartment'));
    activeRow.find('.billtoemail').val(selectedValue.attr('billtoemail'));
    activeRow.find('.billtofaxno').val(selectedValue.attr('billtofaxno'));
    activeRow.find('.billtohomeno').val(selectedValue.attr('billtohomeno'));
    activeRow.find('.billtomobileno').val(selectedValue.attr('billtomobileno'));
    activeRow.find('.billtonrifinpassport').val(selectedValue.attr('billtonrifinpassport'));
    activeRow.find('.billtorocuen').val(selectedValue.attr('billtorocuen'));
    activeRow.find('.billtoofficeno').val(selectedValue.attr('billtoofficeno'));
    activeRow.find('.billtopagerno').val(selectedValue.attr('billtopagerno'));
};

window.ResetDebentureCashAndEquivalentList = function (table) {
    var activeRow = $("#" + table + " tr.activeRow");    
    activeRow.find('.refundable').val('');
    activeRow.find('.guaranteebondstype').val('');    
    activeRow.find('.billtoandamount').html('<option>--Select--</option>');
    activeRow.find('.billtoaddress').val('');
    activeRow.find('.billtocontactperson').val('');
    activeRow.find('.billtodepartment').val('');
    activeRow.find('.billtoemail').val('');
    activeRow.find('.billtofaxno').val('');
    activeRow.find('.billtohomeno').val('');
    activeRow.find('.billtomobileno').val('');
    activeRow.find('.billtonrifinpassport').val('');
    activeRow.find('.billtorocuen').val('');
    activeRow.find('.billtoofficeno').val('');
    activeRow.find('.billtopagerno').val('');
};

window.setActiveRowDebentureCashAndEquivalentList = function (row, table) {
    $("#" + table + " tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                if ($(this).is('input:checkbox')) {
                    if ($(this).is(':checked')) {
                        $(this).next('span').html('<i class="fa fa-check"></i>');
                    }
                    else {
                        $(this).next('span').html('');
                    }
                }
                else {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        if ($(this).val() !== "")
                            DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                        $(this).next().remove();
                    }
                    if ($(this).val() === "" || $(this).val() === "select") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
        showHideDebentureCashAndEquivalentListField(table);
    }
    $('#' + table + ' .activeRow .js-example-basic-sec3').select2();
};

//End Common for 'Cash & Equivalent' and 'Security Deposit'


// Cash & Equivalent(Individual) List

window.tblDebentureCashAndEquivalentIndividualList = $("#tblDebentureCashAndEquivalentIndividualList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

$("#btnAddDebentureCashAndEquivalentIndividualList").on("click", function () {
    if (tblDebentureCashAndEquivalentIndividualList.rows().count() === 0) {
        initializeNewRowDebentureCashAndEquivalentList('btnAddDebentureCashAndEquivalentIndividualList');
    }
    else {
        isDebentureCashAndEquivalentIndividualValidRow(this);
    }
    if (isDebentureCashAndEquivalentIndividualValidRow(this)) {
        var _row = tblDebentureCashAndEquivalentIndividualList.rows.add([[
			appendDebentureCashAndEquivalentRow.Action,			
			appendDebentureCashAndEquivalentRow.CustomerThirdParty,
			appendDebentureCashAndEquivalentRow.BillToAndAmount,
			appendDebentureCashAndEquivalentRow.Refundable,
			appendDebentureCashAndEquivalentRow.GuaranteeBondsType,
			appendDebentureCashAndEquivalentRow.BillToNRIFINPASSPORT,
			appendDebentureCashAndEquivalentRow.BillToAddress,
			appendDebentureCashAndEquivalentRow.BillToDepartment,
			appendDebentureCashAndEquivalentRow.BillToContactPerson,
			appendDebentureCashAndEquivalentRow.BillToMobileNo,
			appendDebentureCashAndEquivalentRow.BillToEmail,
			appendDebentureCashAndEquivalentRow.BillToOfficeNo,
			appendDebentureCashAndEquivalentRow.BillToHomeNo,
			appendDebentureCashAndEquivalentRow.BillToFaxNo,
			appendDebentureCashAndEquivalentRow.BillToPagerNo
        ]]);
        _row.draw();
        setActiveRowDebentureCashAndEquivalentList($("#tblDebentureCashAndEquivalentIndividualList tr:last"), 'tblDebentureCashAndEquivalentIndividualList');
        setCustomerThirdParty("tblDebentureCashAndEquivalentIndividualList");
    }
});

$('#tblDebentureCashAndEquivalentIndividualList tbody').on('click', 'button.deleting', function () {
    tblDebentureCashAndEquivalentIndividualList.row($(this).parents("tr")).remove().draw();
    $('#fldDebentureCashAndEquivalentIndividual').removeClass('error-div');
});

$("#tblDebentureCashAndEquivalentIndividualList tbody").on('click', 'tr', function (event) {
    if (!($(this).hasClass("activeRow"))) {
        if (isDebentureCashAndEquivalentIndividualValidRow(this))
            setActiveRowDebentureCashAndEquivalentList(this, 'tblDebentureCashAndEquivalentIndividualList');
    }
});

window.targetRowEventDebentureCashAndEquivalentIndividualList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblDebentureCashAndEquivalentIndividualList tbody tr.activeRow').length &&
		!$(event.target).closest('#tblDebentureCashAndEquivalentIndividualList thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAddDebentureCashAndEquivalentIndividualList').length &&
		!$(event.target).closest('.select2-container').length &&
		!$(event.target).closest('#tblDebentureCashAndEquivalentIndividualList').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isDebentureCashAndEquivalentIndividualValidRow(event))
            setActiveRowDebentureCashAndEquivalentList(null, 'tblDebentureCashAndEquivalentIndividualList');

    }
};

function isDebentureCashAndEquivalentIndividualValidRow(el) {
    var activeRow = $("#tblDebentureCashAndEquivalentIndividualList tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).next().find('.select2-selection').removeClass("selection-danger");
                }
            }
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {

                if (!$(this).val() || $(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }
            }
        });
    }
    else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isAmountExist(activeRow, 'tblDebentureCashAndEquivalentIndividualList', el);
    if (isDuplicate) {
        $(activeRow).find(".billtoandamount").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".billtoandamount").next().find('.select2-selection').removeClass("selection-danger");

    if (!isValid) {
        $('#fldDebentureCashAndEquivalentIndividual').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddDebentureCashAndEquivalentIndividualList' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldDebentureCashAndEquivalentIndividual').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setCashEquivalentInd = function () {
    var options = "";
    var activeRow = $("#tblDebentureCashAndEquivalentIndividualList tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getCashEquivalentInd',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    options += "<option billtoaddress='" + v.BillToAddress +
						"' refundable='" + v.Refundable +
						"' guaranteebondstype='" + v.GuaranteeBondsType +
						"' billtocontactperson='" + v.BillToConPerson +
						"' billtodepartment='" + v.BillToDept +
						"' billtoemail='" + v.BillToEmail +
						"' billtofaxno='" + v.BillToFaxNumber +
						"' billtohomeno='" + v.BillToHomeNumber +
						"' billtomobileno='" + v.BillToMobileNumber +
						"' billtonrifinpassport='" + v.BillToNRIC_FIN_PASSPORT +
						"' billtoofficeno='" + v.BillToOfficeNumber +
						"' billtopagerno='" + v.BillToPagerNumber +
						"' value='" + v.ID + "'>" + v.BillToCustomer + " - " + manualMask(v.Amount) + "</option>";
                });
                activeRow.find('.billtoandamount').html(options);
                setValueChangeOfCrossCashAndEquivalent('tblDebentureCashAndEquivalentIndividualList', 'billtoandamount');
            }
            else {
                ResetDebentureCashAndEquivalentList('tblDebentureCashAndEquivalentIndividualList');
            }
        }
    });
};

//End Cash & Equivalent(Individual) List



// Cash & Equivalent(Company) List

window.tblDebentureCashAndEquivalentCompanyList = $("#tblDebentureCashAndEquivalentCompanyList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

$("#btnAddDebentureCashAndEquivalentCompanyList").on("click", function () {
    if (tblDebentureCashAndEquivalentCompanyList.rows().count() === 0) {
        initializeNewRowDebentureCashAndEquivalentList('btnAddDebentureCashAndEquivalentCompanyList');
    }
    else {
        isDebentureCashAndEquivalentCompanyValidRow(this);
    }
    if (isDebentureCashAndEquivalentCompanyValidRow(this)) {
        var _row = tblDebentureCashAndEquivalentCompanyList.rows.add([[
			appendDebentureCashAndEquivalentRow.Action,			
			appendDebentureCashAndEquivalentRow.CustomerThirdParty,
			appendDebentureCashAndEquivalentRow.BillToAndAmount,
			appendDebentureCashAndEquivalentRow.Refundable,
			appendDebentureCashAndEquivalentRow.GuaranteeBondsType,
			appendDebentureCashAndEquivalentRow.BillToROCUEN,
			appendDebentureCashAndEquivalentRow.BillToAddress,
			appendDebentureCashAndEquivalentRow.BillToDepartment,
			appendDebentureCashAndEquivalentRow.BillToContactPerson,
			appendDebentureCashAndEquivalentRow.BillToMobileNo,
			appendDebentureCashAndEquivalentRow.BillToEmail,
			appendDebentureCashAndEquivalentRow.BillToOfficeNo,
			appendDebentureCashAndEquivalentRow.BillToHomeNo,
			appendDebentureCashAndEquivalentRow.BillToFaxNo,
			appendDebentureCashAndEquivalentRow.BillToPagerNo
        ]]);
        _row.draw();
        setActiveRowDebentureCashAndEquivalentList($("#tblDebentureCashAndEquivalentCompanyList tr:last"), 'tblDebentureCashAndEquivalentCompanyList');
        setCustomerThirdParty("tblDebentureCashAndEquivalentCompanyList");
    }
});

$('#tblDebentureCashAndEquivalentCompanyList tbody').on('click', 'button.deleting', function () {
    tblDebentureCashAndEquivalentCompanyList.row($(this).parents("tr")).remove().draw();
    $('#fldDebentureCashAndEquivalentCompany').removeClass('error-div');
});

$("#tblDebentureCashAndEquivalentCompanyList tbody").on('click', 'tr', function (event) {
    if (!($(this).hasClass("activeRow"))) {
        if (isDebentureCashAndEquivalentCompanyValidRow(this))
            setActiveRowDebentureCashAndEquivalentList(this, 'tblDebentureCashAndEquivalentCompanyList');
    }
});

window.targetRowEventDebentureCashAndEquivalentCompanyList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblDebentureCashAndEquivalentCompanyList tbody tr.activeRow').length &&
		!$(event.target).closest('#tblDebentureCashAndEquivalentCompanyList thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAddDebentureCashAndEquivalentCompanyList').length &&
		!$(event.target).closest('.select2-container').length &&
		!$(event.target).closest('#tblDebentureCashAndEquivalentCompanyList').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isDebentureCashAndEquivalentCompanyValidRow(event))
            setActiveRowDebentureCashAndEquivalentList(null, 'tblDebentureCashAndEquivalentCompanyList');

    }
};

function isDebentureCashAndEquivalentCompanyValidRow(el) {
    var activeRow = $("#tblDebentureCashAndEquivalentCompanyList tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).next().find('.select2-selection').removeClass("selection-danger");
                }
            }
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {

                if (!$(this).val() || $(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }
            }
        });
    }
    else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isAmountExist(activeRow, 'tblDebentureCashAndEquivalentCompanyList', el);
    if (isDuplicate) {
        $(activeRow).find(".billtoandamount").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".billtoandamount").next().find('.select2-selection').removeClass("selection-danger");


    if (!isValid) {
        $('#fldDebentureCashAndEquivalentCompany').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddDebentureCashAndEquivalentCompanyList' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldDebentureCashAndEquivalentCompany').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setCashEquivalentCom = function () {
    var options = "";
    var activeRow = $("#tblDebentureCashAndEquivalentCompanyList tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getCashEquivalentCom',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    options += "<option billtoaddress='" + v.BillToAddress +
						"' refundable='" + v.Refundable +
						"' guaranteebondstype='" + v.GuaranteeBondsType +
						"' billtocontactperson='" + v.BillToConPerson +
						"' billtodepartment='" + v.BillToDept +
						"' billtoemail='" + v.BillToEmail +
						"' billtofaxno='" + v.BillToFaxNumber +
						"' billtohomeno='" + v.BillToHomeNumber +
						"' billtomobileno='" + v.BillToMobileNumber +
						"' billtorocuen='" + v.BillToROCUEN +
						"' billtoofficeno='" + v.BillToOfficeNumber +
						"' billtopagerno='" + v.BillToPagerNumber +
						"' value='" + v.ID + "'>" + v.BillToCustomer + " - " + manualMask(v.Amount) + "</option>";
                });
                activeRow.find('.billtoandamount').html(options);
                setValueChangeOfCrossCashAndEquivalent('tblDebentureCashAndEquivalentCompanyList', 'billtoandamount');
            }
            else {
                ResetDebentureCashAndEquivalentList('tblDebentureCashAndEquivalentCompanyList');
            }
        }
    });
};

//End Cash & Equivalent(Company) List


//Securities Financial Instruments

window.tblSecuritiesFinancialInstruments = $("#tblSecuritiesFinancialInstruments").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

var appendSecuritiesFinancialInstrumentsRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
    "IndividualCorporate": "",
    "CustomerThirdParty": "", "AmountDocumentNumber": "", "FinancialInstrumentType": "",
    "BankNameFinancial": "", "ChargeDate": "", "Status": ""
};

function initializeNewRowSecuritiesFinancialInstruments() {
    var IndividualCorporate = "<select class='editor individualcorporate js-example-basic-sec3' onchange='setCustomerThirdParty(\"tblSecuritiesFinancialInstruments\")' tabindex='3'>";
    var CustomerThirdParty = "<input data-id='' onfocus='setTableNameForCustomerThirdParty(this)' class='editor customerthirdparty ui-autocomplete-input' type='text' autocomplete='off'><span class='ctparty edited'></span>";
    var FinancialInstrumentType = "<input class='editor financialinstrumenttype unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var AmountDocumentNumber = "<select class='editor amountdocumentnumber js-example-basic-sec3' tabindex='4'>";
    var BankNameFinancial = "<input class='editor banknamefinancial unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var ChargeDate = "<input class='editor chargedate unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var Status = "<input class='editor status unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    
    IndividualCorporate += "<option>Individual</option><option>Corporate</option>";
    IndividualCorporate += "</select><span class='edited'></span>";
    appendSecuritiesFinancialInstrumentsRow.IndividualCorporate = IndividualCorporate;
    CustomerThirdParty += "</select><span class='edited'></span>";
    appendSecuritiesFinancialInstrumentsRow.CustomerThirdParty = CustomerThirdParty;
    appendSecuritiesFinancialInstrumentsRow.FinancialInstrumentType = FinancialInstrumentType;
    AmountDocumentNumber += "<option disabled value='' selected> --Select-- </option></select>";
    AmountDocumentNumber += "<span class='edited'></span>";
    appendSecuritiesFinancialInstrumentsRow.BankNameFinancial = BankNameFinancial;
    appendSecuritiesFinancialInstrumentsRow.AmountDocumentNumber = AmountDocumentNumber;
    appendSecuritiesFinancialInstrumentsRow.ChargeDate = ChargeDate;
}

$("#btnAddSecuritiesFinancialInstruments").on("click", function () {
    if (tblSecuritiesFinancialInstruments.rows().count() === 0) {
        initializeNewRowSecuritiesFinancialInstruments();
    }
    if (isSecuritiesFinancialInstrumentsValidRow(this)) {
        var _row = tblSecuritiesFinancialInstruments.rows.add([[
			appendSecuritiesFinancialInstrumentsRow.Action,			
			appendSecuritiesFinancialInstrumentsRow.IndividualCorporate,
			appendSecuritiesFinancialInstrumentsRow.CustomerThirdParty,
			appendSecuritiesFinancialInstrumentsRow.AmountDocumentNumber,
			appendSecuritiesFinancialInstrumentsRow.FinancialInstrumentType,
			appendSecuritiesFinancialInstrumentsRow.BankNameFinancial,
			appendSecuritiesFinancialInstrumentsRow.ChargeDate
        ]]);
        _row.draw();
        setActiveRowSecuritiesFinancialInstrumentsList($("#tblSecuritiesFinancialInstruments tr:last"));
        setCustomerThirdParty("tblSecuritiesFinancialInstruments");
    }
});
$('#tblSecuritiesFinancialInstruments tbody').on('click', 'button.deleting', function () {
    tblSecuritiesFinancialInstruments.row($(this).parents("tr")).remove().draw();
    $('#fldSecuritiesFinancialInstruments').removeClass('error-div');
});

window.showHideSecuritiesFinancialInstrumentsListField = function () {
    var activeRow = $("#tblSecuritiesFinancialInstruments tr.activeRow");    
    
    activeRow.find(".individualcorporate").removeClass("unrequired");
    activeRow.find(".customerthirdparty").removeClass("unrequired");
    activeRow.find(".amountdocumentnumber").removeClass("unrequired");

    activeRow.find(".individualcorporate").attr("disabled", false);
    activeRow.find(".customerthirdparty").attr("disabled", false);
    activeRow.find(".amountdocumentnumber").attr("disabled", false);

    activeRow.find(".individualcorporate option[value='select']").remove();
};

$("#tblSecuritiesFinancialInstruments tbody").on('click', 'tr', function (event) {
    targetRowEventSecuritiesFinancialInstrumentsList(event);
    if (!($(this).hasClass("activeRow"))) {
        if (isSecuritiesFinancialInstrumentsValidRow(event))
            setActiveRowSecuritiesFinancialInstrumentsList(this);
    }
});
window.targetRowEventSecuritiesFinancialInstrumentsList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblSecuritiesFinancialInstruments tbody tr.activeRow').length &&
		!$(event.target).closest('#tblSecuritiesFinancialInstruments thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAddSecuritiesFinancialInstruments').length &&
		!$(event.target).closest('.select2-container').length &&
		!$(event.target).closest('#tblSecuritiesFinancialInstruments').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isSecuritiesFinancialInstrumentsValidRow(event))
            setActiveRowSecuritiesFinancialInstrumentsList(null);

    }
};

window.isSecuritiesFinancialInstrumentsExist = function (event, el) {
    var isDuplicate = false;
    var amountdocumentnumber = $(event).find(".amountdocumentnumber").val();
    if (amountdocumentnumber) {
        $("#tblSecuritiesFinancialInstruments tbody tr").each(function () {
            var amountdocumentnumber1 = $(this).find(".amountdocumentnumber").val();
            var isactiverow = $(this).hasClass('activeRow');
            if (amountdocumentnumber === amountdocumentnumber1 && !isactiverow) {
                if (el !== null && ($(el).attr('id') === 'btnAddSecuritiesFinancialInstruments' || $(el.target).attr('id') === 'SaveContract'))
                    swal(swalGlobal.SwalTitle_Error, "Amount selected already exist!", swalGlobal.SwalType_Error);
                isDuplicate = true;
            }
        });
    }
    return isDuplicate;
};

function isSecuritiesFinancialInstrumentsValidRow(el) {
    var activeRow = $("#tblSecuritiesFinancialInstruments tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).next().find('.select2-selection').removeClass("selection-danger");
                }
            }
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if ($(this).val() === "" || $(this).val() === "select" || $(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }
            }
        });
    }
    else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isSecuritiesFinancialInstrumentsExist(activeRow, el);
    if (isDuplicate) {
        $(activeRow).find(".amountdocumentnumber").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".amountdocumentnumber").next().find('.select2-selection').removeClass("selection-danger");

    if (!isValid) {
        $('#fldSecuritiesFinancialInstruments').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddSecuritiesFinancialInstruments' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldSecuritiesFinancialInstruments').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setActiveRowSecuritiesFinancialInstrumentsList = function (row) {
    $("#tblSecuritiesFinancialInstruments tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                if ($(this).is('input:checkbox')) {
                    if ($(this).is(':checked')) {
                        $(this).next('span').html('<i class="fa fa-check"></i>');
                    }
                    else {
                        $(this).next('span').html('');
                    }
                }
                else {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        if ($(this).val() !== "")
                            DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                        $(this).next().remove();
                    }
                    if ($(this).val() === "" || $(this).val() === "select") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
        showHideSecuritiesFinancialInstrumentsListField();
    }
    $('#tblSecuritiesFinancialInstruments .activeRow .js-example-basic-sec3').select2();
};

window.setAmountAndDocumentNumber = function () {
    var options = "";
    var activeRow = $("#tblSecuritiesFinancialInstruments tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getAmountAndDocumentNumber',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    options += "<option financialinstrumenttype='" + v.Type +
						"' banknamefinancial='" + v.BankFinancialCom +
						"' chargedate='" + v.ChargeDate +
						"' value='" + v.ID + "'>" + manualMask(v.Amount) + " " + v.DocumentNumber + "</option>";
                });
                activeRow.find('.amountdocumentnumber').html(options);
                setValueChangeOfCrossSecuritiesFinancialInstrument('amountdocumentnumber');
            }
            else {
                ResetSecuritiesFinancialInstrument();
            }
        }
    });
};

window.setValueChangeOfCrossSecuritiesFinancialInstrument = function (column) {
    var activeRow = $("#tblSecuritiesFinancialInstruments tr.activeRow");
    var selectedValue = activeRow.find('.' + column + ' option:selected');
    activeRow.find('.financialinstrumenttype').val(selectedValue.attr('financialinstrumenttype'));
    activeRow.find('.banknamefinancial').val(selectedValue.attr('banknamefinancial'));
    activeRow.find('.chargedate').val(selectedValue.attr('chargedate'));
};

window.ResetSecuritiesFinancialInstrument = function () {
    var activeRow = $("#tblSecuritiesFinancialInstruments tr.activeRow");    
    activeRow.find('.amountdocumentnumber').html('<option>--Select--</option>');
    activeRow.find('.financialinstrumenttype').val('');
    activeRow.find('.banknamefinancial').val('');
    activeRow.find('.chargedate').val('');
};

// End of Securities Financial Instruments



//Security Deposit List
window.tblSecurityDepositList = $("#tblSecurityDepositList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

$("#btnAddSecurityDepositList").on("click", function () {
    if (tblSecurityDepositList.rows().count() === 0) {
        initializeNewRowDebentureCashAndEquivalentList('btnAddSecurityDepositList');
    }
    if (isSecurityDepositListValidRow(this)) {
        var _row = tblSecurityDepositList.rows.add([[
			appendDebentureCashAndEquivalentRow.Action,			
			appendDebentureCashAndEquivalentRow.IndividualCorporate,
			appendDebentureCashAndEquivalentRow.CustomerThirdParty,
			appendDebentureCashAndEquivalentRow.BillToAndAmount,
			appendDebentureCashAndEquivalentRow.Refundable,
			appendDebentureCashAndEquivalentRow.BillToNRIFINPASSPORT,
			appendDebentureCashAndEquivalentRow.BillToROCUEN,
			appendDebentureCashAndEquivalentRow.BillToAddress,
			appendDebentureCashAndEquivalentRow.BillToDepartment,
			appendDebentureCashAndEquivalentRow.BillToContactPerson,
			appendDebentureCashAndEquivalentRow.BillToMobileNo,
			appendDebentureCashAndEquivalentRow.BillToEmail,
			appendDebentureCashAndEquivalentRow.BillToOfficeNo,
			appendDebentureCashAndEquivalentRow.BillToHomeNo,
			appendDebentureCashAndEquivalentRow.BillToFaxNo,
			appendDebentureCashAndEquivalentRow.BillToPagerNo
        ]]);
        _row.draw();
        setActiveRowSecurityDepositList($("#tblSecurityDepositList tr:last"));
        setCustomerThirdParty("tblSecurityDepositList");
    }
});

$('#tblSecurityDepositList tbody').on('click', 'button.deleting', function () {
    tblSecurityDepositList.row($(this).parents("tr")).remove().draw();
    $('#fldSecurityDepositList').removeClass('error-div');
});

$("#tblSecurityDepositList tbody").on('click', 'tr', function (event) {
    targetRowEventSecurityDepositList(event);
    if (!($(this).hasClass("activeRow"))) {
        if (isSecurityDepositListValidRow(event))
            setActiveRowSecurityDepositList(this);
    }
});

function isSecurityDepositListValidRow(el) {
    var activeRow = $("#tblSecurityDepositList tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass("unrequired")) {
                $(this).removeClass("alert-danger");
                if ($(this).hasClass('js-example-basic-sec3')) {
                    $(this).next().find('.select2-selection').removeClass("selection-danger");
                }
            }
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if ($(this).val() === "" || $(this).val() === "select" || $(this).val() === "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec3")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }
            }
        });
    }
    else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;

    var isDuplicate = isAmountExist(activeRow, 'tblSecurityDepositList', el);
    if (isDuplicate) {
        $(activeRow).find(".billtoandamount").next().find('.select2-selection').addClass("selection-danger");
        return false;
    }
    else if (isValid)
        $(activeRow).find(".billtoandamount").next().find('.select2-selection').removeClass("selection-danger");

    if (!isValid) {
        $('#fldSecurityDepositList').addClass('error-div');
        if (el !== null && ($(el).attr('id') === 'btnAddSecurityDepositList' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, swalGlobal.Error_InvalidData, swalGlobal.SwalType_Error);
    }
    else {
        $('#fldSecurityDepositList').removeClass('error-div');
    }
    checkFieldValid();
    return isValid;
}

window.setActiveRowSecurityDepositList = function (row) {
    $("#tblSecurityDepositList tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                if ($(this).is('input:checkbox')) {
                    if ($(this).is(':checked')) {
                        $(this).next('span').html('<i class="fa fa-check"></i>');
                    }
                    else {
                        $(this).next('span').html('');
                    }
                }
                else {
                    var DropText = '';
                    if ($(this).hasClass('js-example-basic-sec3')) {
                        if ($(this).val() !== "")
                            DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                        $(this).next().remove();
                    }
                    if ($(this).val() === "" || $(this).val() === "select") {
                        $(this).next('span').empty();
                    }
                    else {
                        if ($(this).hasClass('js-example-basic-sec3')) {
                            $(this).next('span').html(DropText);
                        }
                        else {
                            $(this).next('span').html($(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
        showHideDebentureCashAndEquivalentListField('tblSecurityDepositList');
    }
    $('#tblSecurityDepositList .activeRow .js-example-basic-sec3').select2();
};

window.targetRowEventSecurityDepositList = function (event) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab === 'Security' &&
		!$(event.target).closest('#tblSecurityDepositList tbody tr.activeRow').length &&
		!$(event.target).closest('#tblSecurityDepositList thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAddSecurityDepositList').length &&
		!$(event.target).closest('.select2-container').length &&
		!$(event.target).closest('#tblSecurityDepositList').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isSecurityDepositListValidRow(event))
            setActiveRowSecurityDepositList(null);

    }
};

window.setSecurityDepositBillToAmount = function () {
    var options = "";
    var activeRow = $("#tblSecurityDepositList tr.activeRow");
    var cm_client_cod = activeRow.find('.customerthirdparty').attr("data-id");
    $.ajax({
        url: varSitePath + '/PreCon/getSecurityDepositBillToAmount',
        data: { cm_client_cod: cm_client_cod },
        type: 'GET',
        cache: false,
        dataType: 'json',
        global: false,
        success: function (json) {
            if (json.data.length > 0) {
                $(json.data).each(function (i, v) {
                    options += "<option billtoaddress='" + v.BillToAddress +
						"' refundable='" + v.Refundable +
						"' guaranteebondstype='" + v.GuaranteeBondsType +
						"' billtorocuen ='" + v.BillToROCUEN +
						"' billtonrifinpassport='" + v.BillToNRIC_FIN_PASSPORT +
						"' billtocontactperson='" + v.BillToConPerson +
						"' billtodepartment='" + v.BillToDept +
						"' billtoemail='" + v.BillToEmail +
						"' billtofaxno='" + v.BillToFaxNumber +
						"' billtohomeno='" + v.BillToHomeNumber +
						"' billtomobileno='" + v.BillToMobileNumber +
						"' billtoofficeno='" + v.BillToOfficeNumber +
						"' billtopagerno='" + v.BillToPagerNumber +
						"' value='" + v.ID + "'>" + v.BillToCustomer + " - " + manualMask(v.Amount) + "</option>";
                });
                activeRow.find('.billtoandamount').html(options);
                setValueChangeOfCrossCashAndEquivalent('tblSecurityDepositList', 'billtoandamount');
            }
            else {
                ResetSecurityDepositList();
            }
        }
    });
};

window.ResetSecurityDepositList = function () {
    var activeRow = $("#tblSecurityDepositList tr.activeRow");    
    activeRow.find('.refundable').val('');
    activeRow.find('.guaranteebondstype').val('');    
    activeRow.find('.billtoandamount').html('<option>--Select--</option>');
    activeRow.find('.billtoaddress').val('');
    activeRow.find('.billtocontactperson').val('');
    activeRow.find('.billtodepartment').val('');
    activeRow.find('.billtoemail').val('');
    activeRow.find('.billtofaxno').val('');
    activeRow.find('.billtohomeno').val('');
    activeRow.find('.billtomobileno').val('');
    activeRow.find('.billtonrifinpassport').val('');
    activeRow.find('.billtoofficeno').val('');
    activeRow.find('.billtopagerno').val('');
};

//End of Security Deposit List





function ShowHideAllRecourseGuarantorList() {
    if ($("#ddlResourseGuaranter").val() == "1") {
        $("#btnAddRecourseIndividualGuarantorList").removeClass("hide");
        $("#btnAddRecourseCorporateGuarantorList").removeClass("hide");
        $("#btnAddRecourseAmount").removeClass("hide");
    }
    else {
        $("#btnAddRecourseIndividualGuarantorList").addClass("hide");
        $("#btnAddRecourseCorporateGuarantorList").addClass("hide");
        $("#btnAddRecourseAmount").addClass("hide");
				$('#RecourseGuarantor').removeClass('error-div');
        $('#fldRecourseAmount').removeClass('error-div');

        RecourseguarantorList = [];

        $('#tblRecourseIndividualGuarantorList tbody tr').each(function () {
            $(this).find('.deleting').click();
        });
        $('#tblRecourseCorporateGuarantorList tbody tr').each(function () {
            $(this).find('.deleting').click();
        });
        $('#tblRecourseAmount tbody tr').each(function () {
            $(this).find('.deleting').click();
        });
    }

}
function ShowHideAllBuyBackGuarantorList() {
    if ($("#ddlBuyBackGuaranter").val() == "1") {
        $("#btnAddBuyBackIndividualGuarantorList").removeClass("hide");
        $("#btnAddBuyBackCorporateGuarantorList").removeClass("hide");
        $("#btnAddBuyBackAmount").removeClass("hide");
    }
    else {
        $("#btnAddBuyBackIndividualGuarantorList").addClass("hide");
        $("#btnAddBuyBackCorporateGuarantorList").addClass("hide");
        $("#btnAddBuyBackAmount").addClass("hide");
				$('#BuyBackGuarantor').removeClass('error-div');
        $('#fldBuyBackAmount').removeClass('error-div');
        guarantorList = [];

        $('#tblBuyBackIndividualGuarantorList tbody tr').each(function () {
            $(this).find('.deleting').click();
        });
        $('#tblBuyBackCorporateGuarantorList tbody tr').each(function () {
            $(this).find('.deleting').click();
        });
        $('#tblBuyBackAmount tbody tr').each(function () {
            $(this).find('.deleting').click();
        });
    }
}


//Common For Gurantor

var appendGuarantorRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
    "Name": "", "Address": "", "Department": "", "ContactPerson": "",
    "LetterType": "", "NRICFINPASSPORT": "", "RocUenType": "",
    "MobileNo": "", "Email": "", "OfficeNo": "", "HomeNo": "", "FaxNo": "", "PagerNo": ""
};

function initializeNewRowGuarantorList() {
    var name = "<input data-id='' class='editor name unrequired ui-autocomplete-input' type='text' autocomplete='off'><span class='snpname edited'></span>";
    var address = "<select name='Guarantoraddress' class='editor address js-example-basic-sec4' tabindex='2' onchange='setGuarantorDepartment(this)'>";
    var department = "<select name='Guarantordepartment' class='editor department js-example-basic-sec4' tabindex='3' onchange='setGuarantorContactPerson(this)'>";
    var contactPerson = "<select name='GuarantorcontactPerson' class='editor contactPerson js-example-basic-sec4' tabindex='4'>";
    var letterType = "<select class='editor letterType js-example-basic-sec4' tabindex='5'>";
    var RocUenType = "<input class='editor RocUenType unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var NRICFINPASSPORT = "<input class='editor NRICFINPASSPORT unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var mobile = "<input class='editor mobile unrequired unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var email = "<input class='editor email' type='text' disabled='disabled'><span class='edited'></span>";
    var officeno = "<input class='editor officeno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var homeno = "<input class='editor homeno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var faxno = "<input class='editor faxno unrequired' type='text' disabled='disabled'><span class='edited'></span>";
    var pagerno = "<input class='editor pagerno unrequired' type='text' disabled='disabled'><span class='edited'></span>";

    appendGuarantorRow.Name = name;

    address += "<option disabled selected value> -- Select -- </option>";
    address += "</select><span class='edited'></span>";

    appendGuarantorRow.Address = address;
    department += "<option disabled selected value> -- Select -- </option>";
    department += "</select><span class='edited'></span>";
    appendGuarantorRow.Department = department;

    contactPerson += "<option disabled selected value> -- Select -- </option>";
    contactPerson += "</select><span class='edited'></span>";
    appendGuarantorRow.ContactPerson = contactPerson;

    letterType += "<option value> -- Select -- </option>";
    $.each(dropdownLetterType, function (i, v) {
        letterType += "<option value = " + v.Value + ">" + v.Text + "</option>";
    })
    letterType += "</select><span class='edited'></span>";
    appendGuarantorRow.LetterType = letterType;
    appendGuarantorRow.RocUenType = RocUenType;
    appendGuarantorRow.NRICFINPASSPORT = NRICFINPASSPORT;
    appendGuarantorRow.MobileNo = mobile;
    appendGuarantorRow.Email = email;
    appendGuarantorRow.OfficeNo = officeno;
    appendGuarantorRow.HomeNo = homeno;
    appendGuarantorRow.FaxNo = faxno;
    appendGuarantorRow.PagerNo = pagerno;
}

window.targetRowEventGuarantorList = function (event, table) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab == 'Security' &&
		!$(event.target).closest('#tbl' + table + ' tbody tr.activeRow').length &&
		!$(event.target).closest('#tbl' + table + ' thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAdd' + table).length &&
		!$(event.target).closest('#tbl' + table).length &&
		!$(event.target).closest('button.deleting').length) {
        if (isGuarantorValidRow(table))
            setActiveRowGuarantorList(null, table);
    }
};

window.isGuarantorValidRow = function (table) {
    var activeRow = $("#tbl" + table + " tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find("select").each(function () {
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if (!$(this).val() || $(this).val() == "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec4")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec4')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }
            }
        });
    }
    //else { return true; }
    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;
	if (!isValid) {		
        $('#fld' + table).addClass('error-div');
    }
	else {
		$('#fld' + table).removeClass('error-div');
	}
    checkFieldValid();
    return isValid;
}

window.ResetGuarantor = function (table) {
    var activeRow = $("#tbl" + table + " tr.activeRow");
    activeRow.find('.address').html("<option disabled selected value> -- Select -- </option>");
    activeRow.find('.department').html("<option disabled selected value> -- Select -- </option>");
    activeRow.find('.contactPerson').html("<option disabled selected value> -- Select -- </option>");
    activeRow.find('.RocUenType').val('');
    activeRow.find('.NRICFINPASSPORT').val('');
    activeRow.find('.RocUenType').val('');
    activeRow.find('.mobile').val('');
    activeRow.find('.email').val('');
    activeRow.find('.officeno').val('');
    activeRow.find('.homeno').val('');
    activeRow.find('.faxno').val('');
    activeRow.find('.pagerno').val('');
};

window.setActiveRowGuarantorList = function (row, table) {
    $("#tbl" + table + " tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                var DropText = '';
                if ($(this).hasClass('js-example-basic-sec4')) {
                    DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text()
                    $(this).next().remove();
                }
                if ($(this).val() == "") {
                    $(this).next('span').empty();
                }
                else {
                    if ($(this).hasClass('js-example-basic-sec4')) {
                        $(this).next('span').html(DropText);
                    }
                    else {
                        $(this).next('span').html($(this).val());
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row) {
        $(row).addClass('activeRow');
        var IndividualCorporate = table.includes("Individual") ? "Individual" : table.includes("Corporate") ? "Corporate" : "";
        var guarantorName = $('#tbl' + table + ' tr.activeRow').find('.name');
        $('.activeRow .name').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: varSitePath + '/PreCon/GetGuarantorNameAutoComplete',
                    data: { textFilter: request.term, IndividualorCorporate: IndividualCorporate },
                    type: 'GET',
                    cache: false,
                    async: false,
                    dataType: 'JSON',
                    global: false,
                    success: function (json) {
                        var res = JSON.parse(json.data);
                        $(res).each(function (i, v) {
                            v.label = v.label.replace("&#39", '\'').replace("&#34", "\"").replace("&#12296", "<").replace("&#12297", ">");
                        });
                        response(res);
                    }
                });
            },
            minLength: 1,
            scroll: true,
            autoFocus: false,
            autoFill: false,
            focus: function (event, ui) {
                return false;
            },
            select: function (event, ui) {
                guarantorName.attr("data-id", ui.item.value);
                guarantorName.val(ui.item.label);
                setGuarantorAddress(this, IndividualCorporate);
                //AddUpdateGuarantor();
                return false;
            },
            change: function (event, ui) {
                if (!ui.item) {
                    guarantorName.attr("data-id", '');
                    guarantorName.val('');
                    ResetGuarantor(table);
                }
                return false;
            }
        });
        $('#tbl' + table + ' .activeRow .js-example-basic-sec4').select2();
    }
};

//Common For Gurantor End


//Buy Back Individual Guarantor
window.tblBuyBackIndividualGuarantorList = $("#tblBuyBackIndividualGuarantorList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

$("#btnAddBuyBackIndividualGuarantorList").on("click", function () {
    if (tblBuyBackIndividualGuarantorList.rows().count() == 0) {
        initializeNewRowGuarantorList();
    }
    if (isGuarantorValidRow("BuyBackIndividualGuarantorList")) {
        var _row = tblBuyBackIndividualGuarantorList.rows.add([[
			appendGuarantorRow.Action,
			appendGuarantorRow.Name,
			appendGuarantorRow.Address,
			appendGuarantorRow.Department,
			appendGuarantorRow.ContactPerson,
			appendGuarantorRow.LetterType,
			appendGuarantorRow.NRICFINPASSPORT,
			appendGuarantorRow.MobileNo,
			appendGuarantorRow.Email,
			appendGuarantorRow.OfficeNo,
			appendGuarantorRow.HomeNo,
			appendGuarantorRow.FaxNo,
			appendGuarantorRow.PagerNo
        ]]);
        _row.draw();
        setletterType($("#tblBuyBackIndividualGuarantorList tr:last"));
        setActiveRowGuarantorList($("#tblBuyBackIndividualGuarantorList tr:last"), "BuyBackIndividualGuarantorList");

    }
});

$('#tblBuyBackIndividualGuarantorList tbody').on('click', 'button.deleting', function () {
    tblBuyBackIndividualGuarantorList.row($(this).parents("tr")).remove().draw();
    $('#fldBuyBackIndividualGuarantorList').removeClass('error-div');

    DeleteGuarantor($(this).closest('tr').find('.name').attr('data-id'));

    //console.log($(this).closest('tr').find('.name').attr('data-id'));
});

$("#tblBuyBackIndividualGuarantorList tbody").on('click', 'tr', function (event) {
    if (!($(this).hasClass("activeRow"))) {
        if (isGuarantorValidRow("BuyBackIndividualGuarantorList"))
            setActiveRowGuarantorList(this, "BuyBackIndividualGuarantorList");
    }
});


//Buy Back Corporate Guarantor
window.tblBuyBackCorporateGuarantorList = $("#tblBuyBackCorporateGuarantorList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

$("#btnAddBuyBackCorporateGuarantorList").on("click", function () {
    if (tblBuyBackCorporateGuarantorList.rows().count() == 0) {
        initializeNewRowGuarantorList();
    }
    if (isGuarantorValidRow("BuyBackCorporateGuarantorList")) {
        var _row = tblBuyBackCorporateGuarantorList.rows.add([[
			appendGuarantorRow.Action,
			appendGuarantorRow.Name,
			appendGuarantorRow.Address,
			appendGuarantorRow.Department,
			appendGuarantorRow.ContactPerson,
			appendGuarantorRow.LetterType,
			appendGuarantorRow.RocUenType,
			appendGuarantorRow.MobileNo,
			appendGuarantorRow.Email,
			appendGuarantorRow.OfficeNo,
			appendGuarantorRow.HomeNo,
			appendGuarantorRow.FaxNo,
			appendGuarantorRow.PagerNo
        ]]);
        _row.draw();
        setletterType($("#tblBuyBackCorporateGuarantorList tr:last"));
        setActiveRowGuarantorList($("#tblBuyBackCorporateGuarantorList tr:last"), "BuyBackCorporateGuarantorList");

    }
});

$('#tblBuyBackCorporateGuarantorList tbody').on('click', 'button.deleting', function () {
    tblBuyBackCorporateGuarantorList.row($(this).parents("tr")).remove().draw();
    $('#fldBuyBackCorporateGuarantorList').removeClass('error-div');
    DeleteGuarantor($(this).closest('tr').find('.name').attr('data-id'));
});

$("#tblBuyBackCorporateGuarantorList tbody").on('click', 'tr', function (event) {
    if (!($(this).hasClass("activeRow"))) {
        if (isGuarantorValidRow("BuyBackCorporateGuarantorList"))
            setActiveRowGuarantorList(this, "BuyBackCorporateGuarantorList");
    }
});




//Recourse Individual Guarantor
window.tblRecourseIndividualGuarantorList = $("#tblRecourseIndividualGuarantorList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

$("#btnAddRecourseIndividualGuarantorList").on("click", function () {
    if (tblRecourseIndividualGuarantorList.rows().count() == 0) {
        initializeNewRowGuarantorList();
    }
    if (isGuarantorValidRow("RecourseIndividualGuarantorList")) {
        var _row = tblRecourseIndividualGuarantorList.rows.add([[
			appendGuarantorRow.Action,
			appendGuarantorRow.Name,
			appendGuarantorRow.Address,
			appendGuarantorRow.Department,
			appendGuarantorRow.ContactPerson,
			appendGuarantorRow.LetterType,
			appendGuarantorRow.NRICFINPASSPORT,
			appendGuarantorRow.MobileNo,
			appendGuarantorRow.Email,
			appendGuarantorRow.OfficeNo,
			appendGuarantorRow.HomeNo,
			appendGuarantorRow.FaxNo,
			appendGuarantorRow.PagerNo
        ]]);
        _row.draw();
        setletterType($("#tblRecourseIndividualGuarantorList tr:last"));
        setActiveRowGuarantorList($("#tblRecourseIndividualGuarantorList tr:last"), "RecourseIndividualGuarantorList");

    }
});

$('#tblRecourseIndividualGuarantorList tbody').on('click', 'button.deleting', function () {
    tblRecourseIndividualGuarantorList.row($(this).parents("tr")).remove().draw();
    $('#fldRecourseIndividualGuarantorList').removeClass('error-div');
    DeleteRecourseGuarantor($(this).closest('tr').find('.name').attr('data-id'));
});

$("#tblRecourseIndividualGuarantorList tbody").on('click', 'tr', function (event) {
    if (!($(this).hasClass("activeRow"))) {
        if (isGuarantorValidRow("RecourseIndividualGuarantorList"))
            setActiveRowGuarantorList(this, "RecourseIndividualGuarantorList");
    }
});


//Recourse Corporate Guarantor
window.tblRecourseCorporateGuarantorList = $("#tblRecourseCorporateGuarantorList").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

$("#btnAddRecourseCorporateGuarantorList").on("click", function () {
    if (tblRecourseCorporateGuarantorList.rows().count() == 0) {
        initializeNewRowGuarantorList();
    }
    if (isGuarantorValidRow("RecourseCorporateGuarantorList")) {
        var _row = tblRecourseCorporateGuarantorList.rows.add([[
			appendGuarantorRow.Action,
			appendGuarantorRow.Name,
			appendGuarantorRow.Address,
			appendGuarantorRow.Department,
			appendGuarantorRow.ContactPerson,
			appendGuarantorRow.LetterType,
			appendGuarantorRow.RocUenType,
			appendGuarantorRow.MobileNo,
			appendGuarantorRow.Email,
			appendGuarantorRow.OfficeNo,
			appendGuarantorRow.HomeNo,
			appendGuarantorRow.FaxNo,
			appendGuarantorRow.PagerNo
        ]]);
        _row.draw();
        setletterType($("#tblRecourseCorporateGuarantorList tr:last"));
        setActiveRowGuarantorList($("#tblRecourseCorporateGuarantorList tr:last"), "RecourseCorporateGuarantorList");

    }
});

$('#tblRecourseCorporateGuarantorList tbody').on('click', 'button.deleting', function () {
    tblRecourseCorporateGuarantorList.row($(this).parents("tr")).remove().draw();
    $('#fldRecourseCorporateGuarantorList').removeClass('error-div');
    DeleteRecourseGuarantor($(this).closest('tr').find('.name').attr('data-id'));
});

$("#tblRecourseCorporateGuarantorList tbody").on('click', 'tr', function (event) {
    if (!($(this).hasClass("activeRow"))) {
        if (isGuarantorValidRow("RecourseCorporateGuarantorList"))
            setActiveRowGuarantorList(this, "RecourseCorporateGuarantorList");
    }
});



//Common Amount

var appendAmountRow = {
    "Action": "<button type='button' class='btn deleting common-del-btn' >Delete</button>",
    "Guarantor": "", "PeriodForm": "", "PeriodTo": "", "BuyBackPerType": "",
    "BuyBackPer": "", "BuyBackAmount": "", "RecoursePerType": "", "RecoursePer": "", "RecourseAmount": ""
};

function initializeNewAmountList() {
    var Guarantor = "<select class='editor guarantor js-example-basic-sec4' tabindex='2'>";
    var PeriodForm = "<input onchange='checkPeriodto(this);InputLimitForm(this)' class='editor periodform unrequired integer' min='1' value='1' type='number'><span class='edited'></span>";
    var PeriodTo = "<input onchange='InputLimitTo(this)' class='editor periodto unrequired integer' min='1' value='1' type='number'><span class='edited'></span>";
	var BuyBackPer = "<input class='editor buybackper unrequired percentage' value='0.0000' type='text'><span class='edited'></span>";
	var BuyBackAmount = "<input class='editor buybackamount money' value='0.00' type='text'><span class='edited'></span>";
    var BuyBackPerType = "<select class='editor buybackpertype js-example-basic-sec4'value='0.00' tabindex='3'>";

	var RecoursePer = "<input class='editor recourseper unrequired percentage' value='0.0000' type='text'><span class='edited'></span>";
    var RecourseAmount = "<input class='editor recourseamount money' value='0.00' type='text'><span class='edited'></span>";
    var RecoursePerType = "<select class='editor recoursepertype js-example-basic-sec4' tabindex='3'>";

    Guarantor += "<option disabled selected value> -- Select -- </option>";
    $(guarantorList).each(function (i, v) {
        Guarantor += v;
    });
    Guarantor += "</select><span class='edited'></span>";
    appendAmountRow.Guarantor = Guarantor;


    var jsonParseData = JSON.parse(BuyBackPercentage);
    BuyBackPerType += "<option selected value> --Select-- </option>";
    $(jsonParseData).each(function (i, v) {
        BuyBackPerType += "<option value = " + v.Value + ">" + v.Text + "</option>";
    });
    BuyBackPerType += "</select><span class='edited'></span>";
    appendAmountRow.BuyBackPerType = BuyBackPerType;

    RecoursePerType += "<option selected value> --Select-- </option>";
    $(jsonParseData).each(function (i, v) {
        RecoursePerType += "<option value = " + v.Value + ">" + v.Text + "</option>";
    });
    RecoursePerType += "</select><span class='edited'></span>";
    appendAmountRow.RecoursePerType = RecoursePerType;

    appendAmountRow.PeriodForm = PeriodForm;
    appendAmountRow.PeriodTo = PeriodTo;
    appendAmountRow.BuyBackPer = BuyBackPer;
    appendAmountRow.BuyBackAmount = BuyBackAmount;
    appendAmountRow.RecoursePer = RecoursePer;
    appendAmountRow.RecourseAmount = RecourseAmount;
}

window.targetRowEventBuyBackAmountList = function (event, table) {
    var currentTab = $('.nav-tabs').find('li.active a').text();
    if (
		currentTab == 'Security' &&
		!$(event.target).closest('#tbl' + table + ' tbody tr.activeRow').length &&
		!$(event.target).closest('#tbl' + table + ' thead').length &&
		!$(event.target).closest('.sweet-alert').length &&
		!$(event.target).closest('.ui-autocomplete').length &&
		!$(event.target).closest('#btnAdd' + table).length &&
		!$(event.target).closest('#tbl' + table).length &&
		!$(event.target).closest('.select2-container').length &&
		!$(event.target).closest('button.deleting').length) {
        if (isAmountValidRow(table, event))
            setActiveRowAmountList(null, table);
    }
};

window.isAmountValidRow = function (table, el) {
    var activeRow = $("#tbl" + table + " tr.activeRow");
    var isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find("select").each(function () {
            if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
                if (!$(this).val() || $(this).val() == "--Select--") {
                    $(this).addClass("alert-danger");
                    if ($(this).hasClass("js-example-basic-sec4")) {
                        $(this).next().find('.select2-selection').addClass("selection-danger");
                    }
                }
                else {
                    $(this).removeClass("alert-danger");
                    if ($(this).hasClass('js-example-basic-sec4')) {
                        $(this).next().find('.select2-selection').removeClass("selection-danger");
                    }
                }
            }
        });

        //Buy Back
			var buybackAmount = parseFloat($(activeRow[0]).find(".buybackamount").val() || 0);
			var buybackPer = parseFloat($(activeRow[0]).find(".buybackper").val() || 0);

			if (buybackAmount == 0 && buybackPer == 0) {
            if (el !== null && ($(el).attr('id') === 'btnAddBuyBackAmount' || $(el).attr('id') === 'SaveContract'))
                swal(swalGlobal.SwalTitle_Error, "Please enter Buy Back % or Buy Back Amount!", swalGlobal.SwalType_Error);
        }

        if ($(activeRow[0]).find(".buybackpertype option:selected").val() !== '') {
					if (buybackPer == 0) {
                $(activeRow[0]).find(".buybackper").addClass("alert-danger");
            }
            else
                $(activeRow[0]).find(".buybackper").removeClass("alert-danger");
            $(activeRow[0]).find(".buybackamount").removeClass("alert-danger");
        }
				else if (buybackAmount == 0) {
            $(activeRow[0]).find(".buybackamount").addClass("alert-danger");
        }
        else {
            $(activeRow[0]).find(".buybackamount").removeClass("alert-danger");
            $(activeRow[0]).find(".buybackper").removeClass("alert-danger");
            $(activeRow[0]).find(".buybackpertype").removeClass("alert-danger");
            $(activeRow[0]).find(".buybackpertype").next().find('.select2-selection').removeClass("selection-danger");
        }


			var recourseAmount = parseFloat($(activeRow[0]).find(".recourseamount").val()||0);
			var recoursePer = parseFloat($(activeRow[0]).find(".recourseper").val()||0);

        //Rcourse
			if (recourseAmount==0 && recoursePer==0) {
            if (el !== null && ($(el).attr('id') === 'btnAddRecourseAmount' || $(el).attr('id') === 'SaveContract'))
                swal(swalGlobal.SwalTitle_Error, "Please enter Recourse % or Recourse Amount!", swalGlobal.SwalType_Error);
        }

        if ($(activeRow[0]).find(".recoursepertype option:selected").val() !== '') {
					if (recoursePer == 0) {
                $(activeRow[0]).find(".recourseper").addClass("alert-danger");
            }
            else
                $(activeRow[0]).find(".recourseper").removeClass("alert-danger");
            $(activeRow[0]).find(".recourseamount").removeClass("alert-danger");
        }
				else if (recourseAmount == 0) {
            $(activeRow[0]).find(".recourseamount").addClass("alert-danger");
        }
        else {
            $(activeRow[0]).find(".recourseamount").removeClass("alert-danger");
            $(activeRow[0]).find(".recourseper").removeClass("alert-danger");
            $(activeRow[0]).find(".recoursepertype").removeClass("alert-danger");
            $(activeRow[0]).find(".recoursepertype").next().find('.select2-selection').removeClass("selection-danger");
        }

    }
    else { return true; }
    isCompleteBuyBackDetails(activeRow[0], el);

    isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ||
		($("#tbl" + table + " tr").find(".alert-row") && $("#tbl" + table + " tr").find(".alert-row").length > 0) ? false : true;


    if (!isValid) {
        //if (el !== null && ($(el).attr('id') === 'btnAddBuyBackAmount' || $(el).attr('id') === 'btnAddRecourseAmount' || $(el).attr('id') === 'SaveContract'))
        //swal(swalGlobal.SwalTitle_Error, "Invalid Data Input!", swalGlobal.SwalType_Error);
        if (!$('#fld' + table).hasClass('error-manual'))
					$('#fld' + table).addClass('error-div');

    }
    else {
        if (!$('#fld' + table).hasClass('error-manual'))
				$('#fld' + table).removeClass('error-div');
			if (tblBuyBackAmount.rows().count() > 0) {
				$('#BuyBackGuarantor').removeClass('error-div');
				$('#fld' + table).removeClass('error-div');
			}
			if (tblRecourseAmount.rows().count() > 0) {
				$('#RecourseGuarantor').removeClass('error-div');
				$('#fld' + table).removeClass('error-div');
			}
    }
    checkFieldValid();
    return isValid;
}

window.ResetAmount = function (table) {
    var activeRow = $("#tbl" + table + " tr.activeRow");
    activeRow.find('.address').html("<option disabled selected value> -- Select -- </option>");
    activeRow.find('.department').html("<option disabled selected value> -- Select -- </option>");
    activeRow.find('.contactPerson').html("<option disabled selected value> -- Select -- </option>");
    activeRow.find('.RocUenType').val('');
    activeRow.find('.NRICFINPASSPORT').val('');
    activeRow.find('.RocUenType').val('');
    activeRow.find('.mobile').val('');
    activeRow.find('.email').val('');
    activeRow.find('.officeno').val('');
    activeRow.find('.homeno').val('');
    activeRow.find('.faxno').val('');
    activeRow.find('.pagerno').val('');
};

window.setActiveRowAmountList = function (row, table) {
	enableDisable(table);
	$("#tbl" + table + " tr").each(function () {
		if ($(this).hasClass('activeRow')) {
			$(this).find(":input").each(function () {
				var DropText = '';
				if ($(this).hasClass('js-example-basic-sec4')) {
					DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text()
					$(this).next().remove();
				}
				if ($(this).val() == "") {
					$(this).next('span').empty();
				}
				else {
					if ($(this).hasClass('js-example-basic-sec4')) {
						$(this).next('span').html(DropText);
					}
					else {
						$(this).next('span').html($(this).val());
					}
				}
			});
			$(this).removeClass("activeRow");
		}
	});
	if (row) {
		$(row).addClass('activeRow');
		var IndividualCorporate = table.includes("Individual") ? "Individual" : table.includes("Corporate") ? "Corporate" : "";
		var guarantorName = $('#tbl' + table + ' tr.activeRow').find('.name');
		$('.activeRow .name').autocomplete({
			source: function (request, response) {
				$.ajax({
					url: varSitePath+'/PreCon/GetGuarantorNameAutoComplete',
					data: { textFilter: request.term, IndividualorCorporate: IndividualCorporate },
					type: 'GET',
					cache: false,
					async: false,
					dataType: 'JSON',
					global: false,
					success: function (json) {
						var res = JSON.parse(json.data);
						$(res).each(function (i, v) {
							v.label = v.label.replace("&#39", '\'').replace("&#34", "\"").replace("&#12296", "<").replace("&#12297", ">");
						});
						response(res);
					}
				});
			},
			minLength: 1,
			scroll: true,
			autoFocus: false,
			autoFill: false,
			focus: function (event, ui) {
				return false;
			},
			select: function (event, ui) {
				guarantorName.attr("data-id", ui.item.value);
				guarantorName.val(ui.item.label);
				//setGuarantorAddress(this, IndividualCorporate);
				//isCompleteBuyBackDetails(ui.item.value);
				return false;
			},
			change: function (event, ui) {
				if (!ui.item) {
					guarantorName.attr("data-id", '');
					guarantorName.val('');
					ResetGuarantor(table);
				}
				return false;
			}
		});
		$('#tbl' + table + ' .activeRow .js-example-basic-sec4').select2();
	}
};

window.isCompleteBuyBackDetails = function (e,el) {
	var PeriodOflease = parseInt($('#txtPeriodofLease').val());
	var PrevGuarantor = $(e).closest('tr').prev().find('.guarantor option:selected').val();
	var PrevGuarantorName = $(e).closest('tr').prev().find('.guarantor option:selected').text();
	var PrePeriodto = parseInt($(e).closest('tr').prev().find('.periodto').val());

    var selectedGuarantor = $(e).closest('tr').find(".guarantor option:selected").val();

	if (selectedGuarantor == PrevGuarantor) {
		if (PeriodOflease == PrePeriodto) {
			//$(e).val($(e).find("option:first").val());
			//$(e).next().find(".select2-selection__rendered").text("-- Select --");
			//$(e).closest('tr').find(".guarantor option:contains(" + selectedGuarantor + ")").attr("disabled", true);

            $(e).closest('tr').find('.guarantor').addClass("alert-danger");
            $(e).closest('tr').find('.guarantor').next().find('.select2-selection').addClass("selection-danger");

            if (el !== null && ($(el).attr('id') === 'btnAddBuyBackAmount' || $(el).attr('id') === 'SaveContract'))
                swal(swalGlobal.SwalTitle_Error, "Buy back details of selected Guarantor already completed!", swalGlobal.SwalType_Error);

            if (el !== null && ($(el).attr('id') === 'btnAddRecourseAmount' || $(el).attr('id') === 'SaveContract'))
                swal(swalGlobal.SwalTitle_Error, "Recourse details of selected Guarantor already completed!", swalGlobal.SwalType_Error);

		}
		else if (PeriodOflease > PrePeriodto) {
			$(e).closest('tr').find('.periodform').val(PrePeriodto + 1);

			var Periodto = parseInt($(e).closest('tr').find('.periodto').val());
			var Periodform = parseInt($(e).closest('tr').find('.periodform').val());
			if (Periodform > Periodto)
				$(e).closest('tr').find('.periodto').val(Periodform + 1);
		}
	}
	else if (PrePeriodto && PeriodOflease !== PrePeriodto) {
		$(e).closest('tr').find('.guarantor').addClass("alert-danger");
		$(e).closest('tr').find('.guarantor').next().find('.select2-selection').addClass("selection-danger");

        if (el !== null && ($(el).attr('id') === 'btnAddBuyBackAmount' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, "Please complete buy back details for " + PrevGuarantorName, swalGlobal.SwalType_Error);

        if (el !== null && ($(el).attr('id') === 'btnAddRecourseAmount' || $(el).attr('id') === 'SaveContract'))
            swal(swalGlobal.SwalTitle_Error, "Please complete recourse details for " + PrevGuarantorName, swalGlobal.SwalType_Error);
    }
};


function enableDisable(table) {
	var periodOflease = parseInt($('#txtPeriodofLease').val());
	var arr = [];
	$("#tbl" + table + " tbody tr").each(function (ii, vv) {
		var currentGuarantor = $(this).find(".guarantor option:selected").val();
		if ($(this).find(".periodto").val() == periodOflease) {
			arr.push(currentGuarantor);			
		}		
	});

	$.each(arr, function (i, v) {
		$("#tbl" + table + " tbody tr .guarantor option").each(function (ii, vv) {
			if ($(vv).val() == v && $(this).closest('tr').find(".guarantor option:selected").val() !== v
				//&& $(this).closest('tr').find(".periodto").val() == periodOflease
			) {
				$(vv).attr('disabled', 'disabled');
			}			
		});
	});
}





window.guarantorList = [];
window.RecourseguarantorList = [];
//window.hiddenguarantorId = '';

window.AddUpdateGuarantor = function () {
    guarantorList = [];
    RecourseguarantorList = [];
    var option = '';
    var BuyBackmap = new Map(); var Recoursemap = new Map();
    if (tblBuyBackIndividualGuarantorList.rows().count() > 0) {
        $("#tblBuyBackIndividualGuarantorList tbody tr").each(function () {
            var model = {
                Status: "I",
                GuarantorCode: $(this).find(".name").attr("data-id"),
                GuarantorAddress: $(this).find('.address option:selected').val(),
                GuarantorDept: $(this).find('.department option:selected').val(),
                GuarantorConPerson: $(this).find('.contactPerson option:selected').val(),
                LetterType: $(this).find('.letterType option:selected').val(),
            }
            var guarantorText = $(this).find(".name").val();
            BuyBackmap.set(guarantorText, model);
        });
    }
    if (tblBuyBackCorporateGuarantorList.rows().count() > 0) {
        $("#tblBuyBackCorporateGuarantorList tbody tr").each(function () {
            var model = {
                Status: "C",
                GuarantorCode: $(this).find(".name").attr("data-id"),
                GuarantorAddress: $(this).find('.address option:selected').val(),
                GuarantorDept: $(this).find('.department option:selected').val(),
                GuarantorConPerson: $(this).find('.contactPerson option:selected').val(),
                LetterType: $(this).find('.letterType option:selected').val(),
            }
            var guarantorText = $(this).find(".name").val();
            BuyBackmap.set(guarantorText, model);
        });
    }

	if (tblRecourseIndividualGuarantorList.rows().count() > 0) {
		$("#tblRecourseIndividualGuarantorList tbody tr").each(function () {
			var model = {
				Status: "I",
				GuarantorCode: $(this).find(".name").attr("data-id"),
				GuarantorAddress: $(this).find('.address option:selected').val(),
				GuarantorDept: $(this).find('.department option:selected').val(),
				GuarantorConPerson: $(this).find('.contactPerson option:selected').val(),
				LetterType: $(this).find('.letterType option:selected').val(),
			}
			var guarantorText = $(this).find(".name").val();
			Recoursemap.set(guarantorText, model);
		});
	}
	if (tblRecourseCorporateGuarantorList.rows().count() > 0) {
		$("#tblRecourseCorporateGuarantorList tbody tr").each(function () {
			var model = {
				Status: "C",
				GuarantorCode: $(this).find(".name").attr("data-id"),
				GuarantorAddress: $(this).find('.address option:selected').val(),
				GuarantorDept: $(this).find('.department option:selected').val(),
				GuarantorConPerson: $(this).find('.contactPerson option:selected').val(),
				LetterType: $(this).find('.letterType option:selected').val(),
			}
			var guarantorText = $(this).find(".name").val();
			Recoursemap.set(guarantorText, model);
		});
	}
	//sorting list 
	const sortedBuyBackItems = new Map([...BuyBackmap.entries()].sort());
	for (let element of sortedBuyBackItems) {
		var option = "<option disabled data-Status='" + element[1].Status + "' data-GuarantorAddress='" + element[1].GuarantorAddress + "' data-GuarantorDept='" + element[1].GuarantorDept + "'data-GuarantorConPerson='" + element[1].GuarantorConPerson + "' data-LetterType='" + element[1].LetterType + "'  value='" + element[1].GuarantorCode + "'>" + element[0] + "</option>";
		guarantorList.push(option);
	}

	const sortedRecourseItems = new Map([...Recoursemap.entries()].sort());
	for (let element of sortedRecourseItems) {
		var option = "<option disabled data-Status='" + element[1].Status + "' data-GuarantorAddress='" + element[1].GuarantorAddress + "' data-GuarantorDept='" + element[1].GuarantorDept + "'data-GuarantorConPerson='" + element[1].GuarantorConPerson + "' data-LetterType='" + element[1].LetterType + "'  value='" + element[1].GuarantorCode + "'>" + element[0] + "</option>";
		RecourseguarantorList.push(option);
	}

	if (tblBuyBackAmount.rows().count() > 0) {
		$("#tblBuyBackAmount tbody tr").each(function (i, v) {
			$(guarantorList).each(function (ii, vv) {
				var options = $(v).find(".guarantor option[value = '" + $(vv).val() + "']").length > 0;
				if (!options) {
					//$(v).find('.guarantor').html(guarantorList);
					$(v).find('.guarantor').append(vv);
				}				
			});
		});
	}
	if (tblRecourseAmount.rows().count() > 0) {
		$("#tblRecourseAmount tbody tr").each(function (i, v) {
			$(RecourseguarantorList).each(function (ii, vv) {
				var options = $(v).find(".guarantor option[value = '" + $(vv).val() + "']").length > 0;
				if (!options) {
					//$(v).find('.guarantor').html(RecourseguarantorList);
					$(v).find('.guarantor').append(vv);
				}
			});
		});
	}
};

window.DeleteGuarantor = function (optionValue) {
	var optionsTodelete = [];
	$.each(guarantorList, function (i, v) {
		if (v) {
			if (v.includes(optionValue))
				guarantorList.splice(i, 1);
		}
			//optionsTodelete.push(v);
	});

	//$.each(optionsTodelete, function (i, v) {
	//	if (v.includes(optionValue))
	//		guarantorList.splice(i,1);		
	//});
	$("#tblBuyBackAmount tbody tr").each(function (ii,vv) {	
		var currentGuarantor = $(this).find(".guarantor option:selected").val();
		if (!currentGuarantor)
			currentGuarantor = $(this).find(".guarantor option:eq(" + ii + ")").val();
		if (currentGuarantor === optionValue)
			tblBuyBackAmount.row($(this)).remove().draw();		
	});
	$("#tblBuyBackAmount tbody tr").find(".guarantor option[value='" + optionValue + "']").remove();

	//if (tblBuyBackAmount.rows().count() == 0)
	//	AddUpdateGuarantor();
};

window.DeleteRecourseGuarantor = function (optionValue) {
	$.each(RecourseguarantorList, function (i, v) {
		if (v) {
			if (v.includes(optionValue))
				RecourseguarantorList.splice(i, 1);
		}	
	});

	$("#tblRecourseAmount tbody tr").each(function (ii, vv) {
		var currentGuarantor = $(this).find(".guarantor option:selected").val();
		if (!currentGuarantor)
			currentGuarantor = $(this).find(".guarantor option:eq(" + ii + ")").val();
		if (currentGuarantor === optionValue)
			tblRecourseAmount.row($(this)).remove().draw();
	});
	$("#tblRecourseAmount tbody tr").find(".guarantor option[value='" + optionValue + "']").remove();
};




$('#txtPeriodofLease').on('focusin', function () {	
	$(this).data('val', $(this).val());
});
//$('#txtPeriodofLease').change(function () {
//    var ifBuyBackAmountrows = tblBuyBackAmount.rows().count();
//    var ifRecourseAmountrows = tblRecourseAmount.rows().count();
//		var prevVal = $(this).data('val');

//    if (ifBuyBackAmountrows > 0 || ifRecourseAmountrows > 0) {
//			swal({
//				title: swalGlobal.SwalTitle_Confirm,
//				text: "Buy Back Amount details and Recourse Amount details will be removed.",
//				type: swalGlobal.SwalType_Warning,
//				showCancelButton: true,
//				confirmButtonColor: "#DD6B55",
//				confirmButtonText: "Yes",
//				cancelButtonText: "No"
//			}, function (isConfirm) {
//				if (isConfirm) {
//					$('#tblBuyBackAmount tbody tr').each(function () {
//						$(this).find('.deleting').click();
//					});
//					$('#tblRecourseAmount tbody tr').each(function () {
//						$(this).find('.deleting').click();
//					});
//				}
//				else {
//					$('#txtPeriodofLease').val(prevVal);
//				}
//			});
//    }
//});


function checkBeginEndPeriod(tableName,el) {

    var PeriodOflease = parseInt($('#txtPeriodofLease').val());

    var periodTo = parseInt($(el).closest('tr').find('.periodto').val());
    var periodFrom = parseInt($(el).closest('tr').find('.periodform').val());

    var Prevperiodto = parseInt($(el).closest('tr').prev().find('.periodto').val());


	var Currentg = $(el).closest('tr').find('.guarantor option:selected').val();
	var Prevg = $(el).closest('tr').prev().find('.guarantor option:selected').val();
	var Nextg = $(el).closest('tr').next().find('.guarantor option:selected').val();

	if (Nextg && Nextg != Currentg && periodTo !== PeriodOflease)
		$(el).closest('tr').find('.periodto').val(PeriodOflease);

	if (!Prevperiodto || Currentg !== Prevg)
		$(el).closest('tr').find('.periodform').val(1);

    if (periodTo > PeriodOflease) {
        $(el).closest('tr').find('.periodto').val(PeriodOflease);
    }
		else if (periodTo < periodFrom) {
			if (periodFrom >= PeriodOflease)
				periodFrom = PeriodOflease;
        $(el).closest('tr').find('.periodto').val(periodFrom)
    }


	var beginclass = "periodform";
	var	endclass = "periodto";
	
	$('#' + tableName + ' tbody tr').each(function (i,v) {
		var end = parseInt($(this).find("." + endclass).val()) || 0;
		var begin = parseInt($(this).find("." + beginclass).val()) || 1;

		var currentguarantor = $(this).closest('tr').find('.guarantor option:selected').val() || $(this).closest('tr').find(".guarantor option:eq(" + i + ")").val();
		var preIndex = parseInt(i - 1) || 0;
		var Prevguarantor = $(this).closest('tr').prev().find('.guarantor option:selected').val() || $(this).closest('tr').find(".guarantor option:eq(" + preIndex + ")").val();

        var preEnd = parseInt($(this).closest("tr").prev().find("." + endclass).val());
        if (isNaN(preEnd))
            preEnd = 0;
        preEnd = preEnd + 1;
        var tds = $(this).find('td');
        if (((begin > end && begin < preEnd) || begin != preEnd) && currentguarantor == Prevguarantor) {
            if (!$(this).hasClass('activeRow'))
                tds.addClass('alert-row');
            tds.closest("fieldset").addClass('error-div');
        }
        else {
            tds.removeClass('alert-row');
            tds.closest("fieldset").removeClass('error-div');
        }
    });
}


//Buy Back Amount
window.tblBuyBackAmount = $("#tblBuyBackAmount").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

$("#btnAddBuyBackAmount").on("click", function () {
    var periodOflease = parseInt($('#txtPeriodofLease').val());
    var rowCount = tblBuyBackAmount.rows().count();
    var previousRow = (rowCount > 1) ? rowCount : 1;
    var periodtoOfpreviousRow = 0;
    

	if (periodOflease > 0) {
		if (rowCount == 0) {
			initializeNewAmountList();
		}
		if (isAmountValidRow("BuyBackAmount", this)) {
			if (rowCount != 0) {
				periodtoOfpreviousRow = parseInt($('#tblBuyBackAmount tbody tr:nth-child(' + previousRow + ')').find('.periodto').val());
			}
			if (periodtoOfpreviousRow == periodOflease)
				periodtoOfpreviousRow = 0;

			periodtoOfpreviousRow = periodtoOfpreviousRow + 1;
			var PeriodForm = "<input onchange='checkBeginEndPeriod(\"tblBuyBackAmount\",this)' class='editor periodform unrequired integer' min='1' value='" + periodtoOfpreviousRow + "' type='number'><span class='edited'></span>";
			periodtoOfpreviousRow = periodtoOfpreviousRow + 1;
			if (periodtoOfpreviousRow > periodOflease)
				periodtoOfpreviousRow = periodOflease;
			var PeriodTo = "<input onchange='checkBeginEndPeriod(\"tblBuyBackAmount\",this)' class='editor periodto unrequired integer' min='1' value='" + periodtoOfpreviousRow++ + "' type='number'><span class='edited'></span>";

            appendAmountRow.PeriodForm = PeriodForm;
            appendAmountRow.PeriodTo = PeriodTo;

		var Guarantor = "<select class='editor guarantor js-example-basic-sec4' tabindex='2'>";
				Guarantor += "<option disabled selected value> -- Select -- </option>";
			$(guarantorList).each(function (i, v) {
				var isComplete = false;
				$("#tblBuyBackAmount tbody tr").each(function (ii, vv) {	
					var currentGuarantor = $(this).find(".guarantor option:selected").val();
					if (!currentGuarantor)
						currentGuarantor = $(this).find(".guarantor option:eq(" + ii + ")").val();

					if ($(this).find(".periodto").val() == periodOflease && $(v).val() == currentGuarantor) {
						isComplete = true;
					}					
				});				
				if (isComplete)
					Guarantor += v;
				else
					Guarantor += v.replace("disabled", "");
				});
				Guarantor += "</select><span class='edited'></span>";
				appendAmountRow.Guarantor = Guarantor;
				var _row = tblBuyBackAmount.rows.add([[
					appendAmountRow.Action,
					appendAmountRow.Guarantor,
					appendAmountRow.PeriodForm,
					appendAmountRow.PeriodTo,
					appendAmountRow.BuyBackPerType,
					appendAmountRow.BuyBackPer,
					appendAmountRow.BuyBackAmount
				]]);
				_row.draw();
				setActiveRowAmountList($("#tblBuyBackAmount tr:last"), "BuyBackAmount");			
		}
	}
	else {
		swal(swalGlobal.SwalTitle_Error, "Please enter Period of Lease (Months)!", swalGlobal.SwalType_Error);
	}
});

$('#tblBuyBackAmount tbody').on('click', 'button.deleting', function () {
	var CurrentGuaruntorId = $(this).closest('tr').find('.guarantor option:selected').val();
	if ($('#tblBuyBackAmount tbody tr .guarantor').length == 1 || !CurrentGuaruntorId) {
		tblBuyBackAmount.row($(this).parents("tr")).remove().draw();
	}
	else {
		var ths = $(this);
		swal({
			title: swalGlobal.SwalTitle_Confirm,
			text: "All lines of the selected Guarantor will be removed.",
			type: swalGlobal.SwalType_Warning,
			showCancelButton: true,
			confirmButtonColor: "#DD6B55",
			confirmButtonText: "Yes",
			cancelButtonText: "No"
		}, function (isConfirm) {
			if (isConfirm) {
				if (CurrentGuaruntorId != '') {
					$('#tblBuyBackAmount tbody tr .guarantor').each(function () {
						if ($(this).val() == CurrentGuaruntorId) {
							tblBuyBackAmount.row($(this).parents("tr")).remove().draw();
						}
					});
				}
				else {
					tblBuyBackAmount.row(ths.parents("tr")).remove().draw();
				}
			}
		});
    }
    $('#fldBuyBackAmount').removeClass('error-div');
});

$("#tblBuyBackAmount tbody").on('click', 'tr', function (event) {
    if (!($(this).hasClass("activeRow"))) {
        if (isAmountValidRow("BuyBackAmount"))
            setActiveRowAmountList(this, "BuyBackAmount");
    }
});



//Recourse Amount
window.tblRecourseAmount = $("#tblRecourseAmount").DataTable({
    "paging": false,
    "info": false,
    "searching": false,
    "language": {
        "zeroRecords": "No Data Available.",
        "infoEmpty": "No Data Available."
    },
    "dom": '<"top"i>rt<"bottom"flp><"clear">'
});

$("#btnAddRecourseAmount").on("click", function () {
    var periodOflease = parseInt($('#txtPeriodofLease').val());
    var rowCount = tblRecourseAmount.rows().count();
    var previousRow = (rowCount > 1) ? rowCount : 1;
    var periodtoOfpreviousRow = 0; 

    if (periodOflease > 0) {
        if (rowCount == 0) {
            initializeNewAmountList();
        }
        if (isAmountValidRow("RecourseAmount", this)) {

					if (rowCount != 0) {
						periodtoOfpreviousRow = parseInt($('#tblRecourseAmount tbody tr:nth-child(' + previousRow + ')').find('.periodto').val());
					}
					if (periodtoOfpreviousRow >= periodOflease)
						periodtoOfpreviousRow = 0;

			periodtoOfpreviousRow = periodtoOfpreviousRow + 1;
			var PeriodForm = "<input onchange='checkBeginEndPeriod(\"tblRecourseAmount\",this)' class='editor periodform unrequired integer' min='1' value='" + periodtoOfpreviousRow+"' type='number'><span class='edited'></span>";
			periodtoOfpreviousRow = periodtoOfpreviousRow + 1;
			if (periodtoOfpreviousRow > periodOflease)
				periodtoOfpreviousRow = periodOflease;
			var PeriodTo = "<input onchange='checkBeginEndPeriod(\"tblRecourseAmount\",this)' class='editor periodto unrequired integer' min='1' value='" + periodtoOfpreviousRow++ + "' type='number'><span class='edited'></span>";

            appendAmountRow.PeriodForm = PeriodForm;
            appendAmountRow.PeriodTo = PeriodTo;

				var Guarantor = "<select class='editor guarantor js-example-basic-sec4' tabindex='2'>";
				Guarantor += "<option disabled selected value> -- Select -- </option>";	
			

			$(RecourseguarantorList).each(function (i, v) {
				var isComplete = false;
				$("#tblRecourseAmount tbody tr").each(function (ii, vv) {
					var currentGuarantor = $(this).find(".guarantor option:selected").val();
					if (!currentGuarantor)
						currentGuarantor = $(this).find(".guarantor option:eq(" + ii + ")").val();

					if ($(this).find(".periodto").val() == periodOflease && $(v).val() == currentGuarantor) {
						isComplete = true;
					}
				});
				if (isComplete)
					Guarantor += v;
				else
					Guarantor += v.replace("disabled", "");
			});




				Guarantor += "</select><span class='edited'></span>";
			appendAmountRow.Guarantor = Guarantor;

            var _row = tblRecourseAmount.rows.add([[
                appendAmountRow.Action,
                appendAmountRow.Guarantor,
                appendAmountRow.PeriodForm,
                appendAmountRow.PeriodTo,
                appendAmountRow.RecoursePerType,
                appendAmountRow.RecoursePer,
                appendAmountRow.RecourseAmount
            ]]);
            _row.draw();
            setActiveRowAmountList($("#tblRecourseAmount tr:last"), "RecourseAmount");
        }
    }
    else {
        swal(swalGlobal.SwalTitle_Error, "Please enter Period of Lease (Months)!", swalGlobal.SwalType_Error);
    }
});

$('#tblRecourseAmount tbody').on('click', 'button.deleting', function () {
	var CurrentGuaruntorId = $(this).closest('tr').find('.guarantor option:selected').val();
	if ($('#tblRecourseAmount tbody tr .guarantor').length == 1 || !CurrentGuaruntorId) {
		tblRecourseAmount.row($(this).parents("tr")).remove().draw();
	}
	else {
		var ths = $(this);
		swal({
			title: swalGlobal.SwalTitle_Confirm,
			text: "All lines of the selected Guarantor will be removed.",
			type: swalGlobal.SwalType_Warning,
			showCancelButton: true,
			confirmButtonColor: "#DD6B55",
			confirmButtonText: "Yes",
			cancelButtonText: "No"
		},
			function (isConfirm) {
			    if (isConfirm) {
			        if (CurrentGuaruntorId != '') {
			            $('#tblRecourseAmount tbody tr .guarantor').each(function () {
			                if ($(this).val() == CurrentGuaruntorId) {
			                    tblRecourseAmount.row($(this).parents("tr")).remove().draw();
			                }
			            });
			        }
			        else {
			            tblRecourseAmount.row(ths.parents("tr")).remove().draw();
			        }
			    }
			});
    }
    $('#fldRecourseAmount').removeClass('error-div');
});

$("#tblRecourseAmount tbody").on('click', 'tr', function (event) {
    if (!($(this).hasClass("activeRow"))) {
        if (isAmountValidRow("RecourseAmount"))
            setActiveRowAmountList(this, "RecourseAmount");
    }
});