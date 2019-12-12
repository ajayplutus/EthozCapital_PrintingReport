var Insurance_Type = [];
var newRowInsaurance = {
    "Action": "<button type='button' tabindex='1' class='btn deleting btnAction' >Delete</button>", "Insurance_Type": "", "Expiry_Date": ""
};
//$('.money').mask("#,##0.00", { reverse: true });
$(document).on('blur', '.money', function () {
    let val = $(this).val();
    if (val === '' || val === '0' || val === '00' || val === '.00') val = '0.00';
    while (val.length > 0) {
        if (val.charAt(0) == '0' && val.charAt(1) !== '.') {
            val = val.substr(1);
        }
        else
            break;
    }

    if (val.indexOf(',') === 0) {
        val = val.substr(1);
        if (val.charAt(0) === '0' && val.charAt(1) !== '.') {
            val = '0.00';
        }
    }
    if (val.indexOf(".") === -1 && val !== '') {
        $(this).val(val + ".00");
    }
    else
        $(this).val(val);
});

function getInsuranceType() {

    $.get(varSitePath + '/Security/GetInsuranceType', function (response) {
        Insurance_Type = response;
    })
        .fail(function (ex) {
            $('#myModal').modal("hide");
            swal(swalGlobal.SwalTitle_Error, "Error Occured whiling loading Insurance Type", swalGlobal.SwalType_Error);
            return [];
        });
};

function securityInitializeNewRow() {
    $.get(varSitePath + '/Security/GetInsuranceType', function (response) {
        Insurance_Type = response;
    });
    if (Insurance_Type) {
        var strnsurance_Type = "<select tabindex='2' style='width:100% !important; font-size:14px;' class='editor select Insurance_Type js-example-basic-single1 ddlInsuranceType'>";
        $.each(Insurance_Type, function (i, v) {
            strnsurance_Type += "<option value='" + v.Value + "'>" + v.Text + "</option>";
        });
        strnsurance_Type += "</select><span data-value='' class='edited Insurance_Type_edited' style='width:100% !important;'>test</span>";
        newRowInsaurance.Insurance_Type = strnsurance_Type;
    }
    else {
        newRowInsaurance.Insurance_Type = "<select class='editor select Insurance_Type_edited js-example-basic-single1 Insurance_Type' style='width:100% !important; font-size:14px;' tabindex='2'></select><span class='edited Insurance_Type_edited'></span>";
    }
    newRowInsaurance.Expiry_Date = "<input type='text' tabindex='3' name='charge_date' class='editor expiry_date' readonly='true' style='font-size:14px;'/><span class='edited expiry_edited'></span><span class='InsItemNumber' style='display:none'>0</span>";
};

function InitializeInsauranceRowUpdate(InsauranceDtl) {
    $.get(varSitePath + '/Security/GetInsuranceType', function (response) {
        Insurance_Type = response;
    });

    if (InsauranceDtl.length > 0) {
        $.each(InsauranceDtl, function (key, value) {
            if (Insurance_Type) {
                var strnsurance_Type = "<select tabindex='2' style='width:100% !important; font-size:14px;' class='editor select Insurance_Type js-example-basic-single1 ddlInsuranceType'>";
                $.each(value.Insurance, function (i, v) {
                    if (v.Value === value.InsuranceType) {
                        strnsurance_Type += "<option value='" + v.Value + "' selected='true'>" + v.Text + "</option>";
                    }
                    else {
                        strnsurance_Type += "<option value='" + v.Value + "'>" + v.Text + "</option>";
                    }
                });
                strnsurance_Type += "</select><span data-value='" + value.InsuranceType + "' class='edited Insurance_Type_edited' style='width:100% !important;'>" + value.InsuranceTypeDisplay + "</span>";
                newRowInsaurance.Insurance_Type = strnsurance_Type;
            }
            else {
                newRowInsaurance.Insurance_Type = "<select class='editor select Insurance_Type_edited js-example-basic-single1 Insurance_Type' style='width:100% !important; font-size:14px;' tabindex='2'></select><span data-value='" + value.InsuranceType + "' class='edited Insurance_Type_edited'>" + value.InsuranceTypeDisplay + "</span>";
            }
            newRowInsaurance.Expiry_Date = "<input type='text' tabindex='3' name='charge_date' class='editor expiry_date' readonly='true' style='font-size:14px;' value='" + value.ExpiryDate + "'/><span class='edited expiry_edited'>" + value.ExpiryDate + "</span><span class='InsItemNumber' style='display:none'>" + value.ItemNumber+"</span>";

            var _row1 = tblInsurance.rows.add([[newRowInsaurance.Action, newRowInsaurance.Insurance_Type, newRowInsaurance.Expiry_Date]]);

            _row1.draw();
        });
    }
    var date = new Date();
    var m = date.getMonth(), d = date.getDate(), y = date.getFullYear();
    $("#tblInsurance").find('.expiry_date').datepicker({
        minDate: new Date(y, m, d + 1),
        changeMonth: true,
        changeYear: true,
        dateFormat: 'dd/mm/yy',
        showButtonPanel: true,
        closeText: 'Clear',
        onClose: function () {
            var event = arguments.callee.caller.caller.arguments[0];
            if ($(event.delegateTarget).hasClass('ui-datepicker-close')) {
                $(this).val('');
            }
        }
    });
}

function targetInsuranceRowEvent(event) {
    if (!$(event.target).closest('#tblInsurance tbody tr.activeRow').length &&
        !$(event.target).closest('#tblInsurance thead').length &&
        !$(event.target).closest('.sweet-alert').length &&
        !$(event.target).closest('#SaveInventory').length &&
        !$(event.target).closest('.ui-autocomplete').length &&
        !$(event.target).closest('.ui-datepicker-header').length &&
        !$(event.target).closest('#btnAddMortgagorList').length &&
        !$(event.target).closest('#btnAddInsurance').length &&
        !$(event.target).closest('#btnCustomerToAccessList').length &&
        !$(event.target).closest('button.deleting').length) {
        if (!isInsuranceRowChanged()) {
            $('#tblInsurance tr.activeRow').find('button.deleting').click();
        }
        else if (!IsDuplicateInsuranceType() && isValidInsuranceRow()) {
            setInsuranceActiveRow(null);
        }
    }
}

function isInsuranceRowChanged() {
    let activeRow = $("#tblInsurance tr.activeRow");
    let isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass('unrequired')) {
                if ($(this).val() !== '')
                    isValid = true;
            }
        });
        if (!isValid) {
            if ($(activeRow[0]).find('.Insurance_Type').val() === ''
                && $(activeRow[0]).find('.charge_date').val() === ''
            )
                isValid = false;
            else
                isValid = true;

        }
    }
    else {
        return true;
    }
    return isValid;
}

function setInsuranceActiveRow(row) {
    $("#tblInsurance tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                $(this).next('span').html('');

                var DropText = '';
                if ($(this).hasClass('js-example-basic-single1')) {
                    DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                    $(this).next().remove();
                }
                if (!($(this).hasClass("deleting")) && $(this).val() === "") {
                    $(this).addClass("alert-danger");
                    $(this).next('span').empty();
                }
                else {
                    if ($(this).hasClass('js-example-basic-single1')) {
                        $(this).next('span').html($(this).find("option:selected").text());
                        $(this).next('span').data("value", $(this).val());
                    }
                    else {
                        $(this).next('span').html($(this).val());
                        if ($(this).hasClass('expiry_date')) {
                            $(this).next('span').data("value", $(this).val());
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });
    if (row != null)
        $(row).addClass('activeRow');
    $('.activeRow .js-example-basic-single1').select2();
};

$(document).ready(function () {
    $('.js-example-basic-single3').select2();
    var j = 0;
    getInsuranceType();

    securityInitializeNewRow();

    $(document).on('click', function (event) {
        targetInsuranceRowEvent(event);
    });

    $("#tblInsurance tbody").on('click', 'tr', function (event) {
        if (!($(this).hasClass("activeRow")) && isValidInsuranceRow()) {
            if (!IsDuplicateInsuranceType()) {
                setInsuranceActiveRow(this);
            }
        }
    });

    $("#Reset").on('click', function () {
        $("#txtVesselName").val("");
        $("#txtHullNumber").val("");
        $("#Country_Registration").parent().find("span.select2-selection__rendered").text("--Select--");
        $("select#Country_Registration").prop('selectedIndex', 0);
        $("#txtformalvaluation").val("0.00");
        $("#txtmortgagenumber").val("");
        $("#txtchargenumber").val("");
        $("#chargedate").val("");
        $("#txtCreditLimit").val("0.00");
        $("#txtIndicativeValuation").val("0.00");
        $("#tblMortgagorList tbody tr").each(function () {
            $(this).find('button.deleting').click();
        });
        $("#tblInsurance tbody tr").each(function () {
            $(this).find('button.deleting').click();
        });
        $("#tblCustomerToAccessList tbody tr").each(function () {
            $(this).find('button.deleting').click();
        });
    });

    var IsInsuranceType = false;

    $("#btnAddInsurance ").on("click", function () {
        if (!IsDuplicateInsuranceType() && ($.isFunction(isValidRow) ? isValidRow() : true) && ($.isFunction(isValidCustomerRow) ? isValidCustomerRow() : true)) {
            j++;
            if (tblInsurance.rows().count() === 0) {
                securityInitializeNewRow();
            }

            if (tblInsurance.rows().count() === 0 || (!IsInsuranceType && isValidInsuranceRow())) {

                securityInitializeNewRow();
                var _row1 = tblInsurance.rows.add([[newRowInsaurance.Action, newRowInsaurance.Insurance_Type, newRowInsaurance.Expiry_Date]]);

                _row1.draw();
                setInsuranceActiveRow($("#tblInsurance tr:last"));
                $("#insurance").removeClass("alert-danger-table");
                var date = new Date();
                var m = date.getMonth(), d = date.getDate(), y = date.getFullYear();
                $(".activeRow").find('.expiry_date').datepicker({
                    minDate: new Date(y, m, d + 1),
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: 'dd/mm/yy',
                    showButtonPanel: true,
                    closeText: 'Clear',
                    onClose: function () {
                        var event = arguments.callee.caller.caller.arguments[0];
                        if ($(event.delegateTarget).hasClass('ui-datepicker-close')) {
                            $(this).val('');
                        }
                    }
                });
            }
        }
    });

    $('#tblInsurance tbody').on('click', 'button.deleting', function () {
        IsInsuranceType = false;
        tblInsurance.row($(this).parents("tr")).remove().draw();
    });

    targetInsuranceRowEvent(event);

    isInsuranceRowChanged();

    $('body').on('click', '.ui-state-default', function () {
        event.stopPropagation();
    });

    setInsuranceActiveRow();

    var d = new Date();
    var yrRange = (d.getFullYear() - 20) + ":" + (d.getFullYear() + 20);
    $(".chargedate").datepicker({
        changeMonth: true,
        changeYear: true,
        yearRange: yrRange,
        dateFormat: 'dd/mm/yy',
        showButtonPanel: true,
        closeText: 'Clear',
        onClose: function () {
            var event = arguments.callee.caller.caller.arguments[0];
            if ($(event.delegateTarget).hasClass('ui-datepicker-close')) {
                $(this).val('');
            }
        },
    });

});

var model = {
    Id: "",
    HullNumber: "",
    VesselName: "",
    CreatedBy: "",
    CreatedDate: "",
    Status: "",
    Mortgagor: [{
        IndividualCorporate: "",
        Mortgagor: "",
        MainType: "",
        NRICType: "",
        ROCType: "",
        Address: "",
        Department: "",
        ContactPerson: "",

    }],
    VesselDetails: {
        CountryofRegistration: "",
        MortgageNumber: "",
        ChargeNumber: "",
        FormalValuation: "",
        ChargeDate: "",
        CreditLimit: "",
        IndicativeValuation: "",
        FormalValuationOld: "",
        CreditLimitOld: "",
        IndicativeValuationOld: ""
    },
    InsuranceDetail: [{
        InsuranceType: "",
        ExpiryDate: ""
    }],
    CustomerToSccess: [{
        IndividualCorporate: "",
        Customer: ""
    }]
};

function SaveNewVessel(url) {

    if (Validation() && !IsDuplicateCustomer() && !IsDuplicateInsuranceType() && isValidRow() && ValidateExpiryDate()) {
        $('#myModal').modal({
            backdrop: 'static',
            keyboard: false
        });
        var model = {
            Id: "",
            HullNumber: "",
            VesselName: "",
            CreatedBy: "",
            CreatedDate: "",
            Status: "",
            Mortgagor: [],
            VesselDetails: {
                CountryofRegistration: "",
                MortgageNumber: "",
                ChargeNumber: "",
                FormalValuation: "",
                ChargeDate: "",
                CreditLimit: "",
                IndicativeValuation: "",
                FormalValuationOld: "",
                CreditLimitOld: "",
                IndicativeValuationOld: ""
            },
            InsuranceDetail: [],
            CustomerToAccess: []
        };
        model.Id = $("#Id").val();
        model.HullNumber = $("#txtHullNumber").val();
        model.VesselName = $("#txtVesselName").val();
        model.CreatedBy = $("#CreatedBy").val();
        model.CreatedDate = $("#dtpCreatedDate").val();
        model.Status = $("#Status").val();
        $("#tblMortgagorList tbody tr").each(function () {
            if (!$(this).hasClass('activeRow')) {
                var mortgagor1 = {
                    IndividualCorporate: $(this).find('.Individual_Corporate_edited').text(),
                    Mortgagor: $(this).find('.mortgagorName').data("value"),
                    MainType: $(this).find('.checkMain').text(),
                    NRICType: $(this).find('.NricType_edited').text(),
                    ROCType: $(this).find('.RocUen_edited').text(),
                    Address: $(this).find('.Address_edited').data("value"),
                    Department: $(this).find('.Department_edited').data("value"),
                    ContactPerson: $(this).find('.ContactPerson_edited').data("value"),
                    ItemNumber: $(this).find('.ItemNumber').text()
                };
                model.Mortgagor.push(mortgagor1);
            }
            else {
                var mortgagor1 = {
                    IndividualCorporate: $(this).find('select.Individual_Corporate').find('option:selected').val(),
                    Mortgagor: $(this).find('.mortgagor').data('id'),
                    MainType: $(this).find('select.Main_Secondary').find('option:selected').text(),
                    NRICType: $(this).find('input.NricFinPassport').val(),
                    ROCType: $(this).find('input.RocUen').text(),
                    Address: $(this).find('select.ddlAddress').find('option:selected').val(),
                    Department: $(this).find('.ddlDepartment').find('option:selected').val(),
                    ContactPerson: $(this).find('select.contact_Person').find('option:selected').val(),
                    ItemNumber: $(this).find('.ItemNumber').text()
                };
                model.Mortgagor.push(mortgagor1);
            }
        });
        $("#tblCustomerToAccessList tbody tr").each(function () {
            if (!$(this).hasClass('activeRow')) {
                var customerToAccess1 = {
                    IndividualCorporate: $(this).find('.Individual_Corporate_Customer_edited').text(),
                    Customer: $(this).find('.customer_edited').data("value"),
                };
                model.CustomerToAccess.push(customerToAccess1);
            }
            else {
                var customerToAccess1 = {
                    IndividualCorporate: $(this).find(".Individual_Corporate_Customer").find("option:selected").val(),
                    Customer: $(this).find('.customer').data('id'),
                };
                model.CustomerToAccess.push(customerToAccess1);
            }
        });
        $("#tblInsurance tbody tr").each(function () {
            if (!$(this).hasClass('activeRow')) {
                var insurance1 = {
                    InsuranceType: $(this).find('.Insurance_Type_edited').data("value"),
                    ExpiryDate: $(this).find('.expiry_edited').text(),
                    ItemNumber: $(this).find('.InsItemNumber').text()
                };
                model.InsuranceDetail.push(insurance1);
            }
            else {
                var insurance1 = {
                    InsuranceType: $(this).find(".ddlInsuranceType").find("option:selected").val(),
                    ExpiryDate: $(this).find('.expiry_date').val(),
                    ItemNumber: $(this).find('.InsItemNumber').text()
                };
                model.InsuranceDetail.push(insurance1);
            }
        });
        var vessel1 = {
            CountryofRegistration: $('.CountryofRegistration').val(),
            FormalValuation: $('.formal_valuation').val(),
            MortgageNumber: $('.mortgage_number').val(),
            ChargeNumber: $('.charge_number').val(),
            ChargeDate: $('.charge_date').val(),
            CreditLimit: $("#txtCreditLimit").val(),
            IndicativeValuation: $("#txtIndicativeValuation").val(),
            FormalValuationOld: $("#FormalValuationOld").val(),
            CreditLimitOld: $("#CreditLimitOld").val(),
            IndicativeValuationOld: $("#IndicativeValuationOld").val()
        };
        model.VesselDetails = vessel1;

        $.post(varSitePath + "/Security/InsertVesselDetails", { json: JSON.stringify(model) }, function (response) {
            $('#myModal').modal("hide");
            if (response.Status === 1) {
                swal(swalGlobal.SwalTitle_Success, response.Message, swalGlobal.SwalType_Success);
                $("#Reset").click();
                $("#MainView").load(url);
            }
            else {
                if (response.Status === 2) $("#txtHullNumber").addClass('alert-danger');
                swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
            }
        })
            .fail(function (ex) {
                $('#myModal').modal("hide");
                swal(swalGlobal.SwalTitle_Error, "Error", swalGlobal.SwalType_Error);
            });
    }
    else
        $('.sweet-overlay').attr("tabindex", 1000);

}

function IsDuplicateInsuranceType() {
    var i = 0;
    IsInsuranceType = false;
    var Insurance_Type = $("#tblInsurance tbody tr.activeRow").find("select.Insurance_Type").find("option:selected").text();
    $(".Insurance_Type_edited").each(function () {
        if (Insurance_Type === $(this).text() && !$(this).closest('tr.activeRow').length) {
            swal(swalGlobal.SwalTitle_Error, "Insurance Type already selected!", swalGlobal.SwalType_Error);
            IsInsuranceType = true;
        }
    });
    return IsInsuranceType;
}

function ValidateExpiryDate() {
    var i = 0;
    IsValidExpiryDate = true;
    var date = $.datepicker.formatDate('dd/mm/yy', new Date());
    $(".expiry_edited").each(function () {
        var expiryDate = $(this).text();
        if (splitdate(date) > splitdate(expiryDate)) {
            swal(swalGlobal.SwalTitle_Error, "Expiry Date must be greater than current date", swalGlobal.SwalType_Error);
            IsValidExpiryDate = false;
        }
    });
    return IsValidExpiryDate;
}

function splitdate(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

function UpdateStatus(url) {
    event.stopPropagation();
    swal({
        title: swalGlobal.SwalTitle_Warning,
        text: "Are you sure to discharge/cancel the security item?",
        type: swalGlobal.SwalType_Warning,
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes",
        cancelButtonText: "No"
    }, function (isConfirm) {
        if (isConfirm) {
            setTimeout(function () {
                DischargeCancelStatus(url);
            }, 1000);
        }
    });
}

function DischargeCancelStatus(url) {
    var Status = "";
    var Id = $("#Id").val();
    if ($("#txtChargeDate").val() === "") {
        Status = "C";
    }
    else {
        Status = "D";
    }
    $.post(varSitePath + '/Security/UpdateVesselStatus', { Id: Id, Status: Status }).done(function (response) {
        if (response.Status === 1) {
            swal({
                title: swalGlobal.SwalTitle_Success,
                text: response.Message,
                type: swalGlobal.SwalType_Success
            }, function () {
                $("#MainView").load(url);
            });
        }
        else {
            if (response.Status === 2)
                swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
            $("#MainView").load(url);
        }
    });
}

function Validation() {
    $(".required").each(function () {
        if ($(this).val() === "" || $(this).val() === "0.00" || $(this).val() === null) {
            $(this).addClass('alert-danger');
            if ($(this).is("select")) {
                $(this).parent().find("span.select2-selection__rendered").addClass('alert-danger');
            }
        }
        else {
            $(this).removeClass('alert-danger');
            if ($(this).is("select")) {
                $(this).parent().find("span.select2-selection__rendered").removeClass('alert-danger');
            }
        }

    });
    if ($('#tblMortgagorList tbody').find('td.dataTables_empty').length > 0) {
        $(this).parent().find("span.select2-selection__rendered").addClass('alert-danger');
        $("#mortgatorDetails").addClass("alert-danger-table");
    }
    else {
        $(this).parent().find("span.select2-selection__rendered").removeClass('alert-danger');
        $("#mortgatorDetails").removeClass("alert-danger-table");
    }
    if ($('#tblCustomerToAccessList tbody').find('td.dataTables_empty').length > 0)
        $("#tblCustomerToAccessList_wrapper").addClass("alert-danger-table");
    else
        $("#tblCustomerToAccessList_wrapper").removeClass("alert-danger-table");

    if ($('#tblInsurance tbody').find('td.dataTables_empty').length > 0)
        $("#insurance").addClass("alert-danger-table");
    else
        $("#insurance").removeClass("alert-danger-table");

    if ($(".alert-danger").length > 0 || $('.alert-danger-table').length > 0) {
        swal(swalGlobal.SwalTitle_Error, "Please enter required fields.", swalGlobal.SwalType_Error);

        return false;
    }
    else if (!CheckMainMortgagor())
        return false;
    else
        return true;
}
function onViewClick(path) {
    $('#myModal').modal({
        backdrop: 'static',
        keyboard: false
    });
    startTimer();
    window.location.href = path;
}

function ResetView(url) {
    $('#myModal').modal({
        backdrop: 'static',
        keyboard: false
    });
    startTimer();
    resetTimer();
    $("#MainView").load(url);
}