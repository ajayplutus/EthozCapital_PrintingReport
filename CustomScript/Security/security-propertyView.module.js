//$('.money').mask("#,##0.00", { reverse: true });

$(document).ready(function () {
   
    $('.js-example-basic-single3').select2();
    
    $("#Reset").on('click', function () {
        $("#Property_Address").val("");
        document.getElementById("First_third_Party").selectedIndex = "0";
        $("#Property_Level_1").val("");
        $("#Property_Level_2").val("");
        $("#Property_Level_1").parent().find("span.select2-selection__rendered").text("--Select--");
        $("#Property_Level_2").parent().find("span.select2-selection__rendered").text("--Select--");
        $("#formal_valuation").val("0.00");
        $("#title_number").val("");
        $("#mortgage_number").val("");
        $(".ddlAddress").html('');
        $("#charge_number").val("");
        $("#chargedate").val("");
        $("#txtCreditLimit").val("0.00");
        $("#txtIndicativeValuation").val("0.00");
        $("#tblMortgagorList tbody tr").each(function () {
            $(this).find('button.deleting').click();
        });
        $("#tblCustomerToAccessList tbody tr").each(function () {
            $(this).find('button.deleting').click();
        });
    });

    var d = new Date();
    var yrRange = (d.getFullYear() - 20) + ":" + (d.getFullYear() + 20);
    $(".chargedate").datepicker({
        dateFormat: 'dd/mm/yy',
        changeMonth: true,
        changeYear: true,
        yearRange: yrRange,
        showButtonPanel: true,
        closeText: 'Clear',
        onClose: function () {
            var event = arguments.callee.caller.caller.arguments[0];
            if ($(event.delegateTarget).hasClass('ui-datepicker-close')) {
                $(this).val('');

            }
        },
    });

    $(document).on('blur', '.money', function () {
        let val = $(this).val();
        if (val === '' || val === '0' || val === '00' || val === '.00') val = '0.00';
        while (val.length > 0) {
            if (val.charAt(0) === '0' && val.charAt(1) !== '.') {
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
            $(this).val(val + '.00');
        }
        else
            $(this).val(val);
    });
});
var model = {
    Id: "",
    PropertyAddress: "",
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
        ContactPerson: ""

    }],
    PropertyDetails: {
        PropertyTypeLevel1: "",
        Propertytypelevel2: "",
        PartyType: "",
        FormalValuation: "",
        TitleNumber: "",
        MortgagorNumber: "",
        ChangeNumber: "",
        ChargeDate: "",
        CreditLimit: "",
        IndicativeValuation: "",
        FormalValuationOld: "",
        CreditLimitOld: "",
        IndicativeValuationOld: ""
    },
    CustomerToSccess: [{
        IndividualCorporate: "",
        Customer: ""

    }]
};

function SaveNewProperty(url) {
    if (Validation() && !IsDuplicateCustomer() && isValidRow()) {
        $('#myModal').modal({
            backdrop: 'static',
            keyboard: false
        });
        var model = {
            Id: "",
            PropertyAddress: "",
            CreatedBy: "",
            CreatedDate: "",
            Status: "",
            Mortgagor: [],
            PropertyDetails: {
                PropertyTypeLevel1: "",
                Propertytypelevel2: "",
                PartyType: "",
                FormalValuation: "",
                TitleNumber: "",
                MortgagorNumber: "",
                ChangeNumber: "",
                ChargeDate: "",
                CreditLimit: "",
                IndicativeValuation: "",
                 FormalValuationOld: "",
                CreditLimitOld: "",
                IndicativeValuationOld: ""
            },
            CustomerToAccess: []
        };
        model.Id = $("#Id").val();
        model.PropertyAddress = $("#Property_Address").val();
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
                    ItemNumber: $(this).find('.ItemNumber').text()
                };
                model.CustomerToAccess.push(customerToAccess1);
            }
            else {
                var customerToAccess1 = {
                    IndividualCorporate: $(this).find(".Individual_Corporate_Customer").find("option:selected").val(),
                    Customer: $(this).find('.customer').data('id'),
                    ItemNumber: $(this).find('.ItemNumber').text()
                };
                model.CustomerToAccess.push(customerToAccess1);
            }
        });
        var property1 = {
            PropertyTypeLevel1: $('.Property_Level_1').val(),
            Propertytypelevel2: $('.Property_Level_2').val(),
            PartyType: $('.First_third_Party').val(),
            FormalValuation: $('.formal_valuation').val(),
            TitleNumber: $('.title_number').val(),
            MortgagorNumber: $('.mortgage_number').val(),
            ChangeNumber: $('.charge_number').val(),
            ChargeDate: $('.charge_date').val(),
            CreditLimit: $("#txtCreditLimit").val(),
            IndicativeValuation: $("#txtIndicativeValuation").val(),
            FormalValuationOld: $("#FormalValuationOld").val(),
            CreditLimitOld: $("#CreditLimitOld").val(),
            IndicativeValuationOld: $("#IndicativeValuationOld").val()
        };
        model.PropertyDetails = property1;

        $.post(varSitePath +"/Security/InsertPropertyDetails", { json: JSON.stringify(model) }, function (response) {
            $('#myModal').modal("hide");
            if (response.Status === 1) {
                swal(swalGlobal.SwalTitle_Success, response.Message, swalGlobal.SwalType_Success);
                $("#MainView").load(url);
            }
            else {
                if (response.Status === 2) $("#Property_Address").addClass('alert-danger');
                swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
            }
        })
            .fail(function (ex) {
                $('#myModal').modal("hide");
                swal(swalGlobal.SwalTitle_Error, "Error", swalGlobal.SwalType_Error);
            });
    }
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
    $.post(varSitePath +'/Security/UpdatePropertyStatus', { Id: Id, Status: Status }).done(function (response) {
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
                $(this).parent().find("span.select2-selection__rendered").addClass('alert-danger');
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