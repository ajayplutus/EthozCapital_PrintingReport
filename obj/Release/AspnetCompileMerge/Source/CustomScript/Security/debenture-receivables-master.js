﻿//$('.money').mask("#,##0.00", { reverse: true });
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
        $(this).val(val + ".00");
    }
    else
        $(this).val(val);
});
$(document).ready(function () {

    $("#Reset").on('click', function () {
        $("#txtAmount").val('0.00');
        $("#txtChargeNumber").val('');
        $("#txtChargeDate").val('');
        $("#tblCustomerToAccessList tbody tr").each(function () {
            $(this).find("button.deleting").click();
        });
    });

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
        }
    });
});

var model = {
    Id: "",
    Amount: "",
    ChargeNumber: "",
    ChargeDate: "",
    CreatedBy: "",
    CreatedDate: "",
    Status: "",
    CustomerToAccess: [{
        IndividualCorporate: "",
        Customer: ""
    }]
};

function SaveNewReceivable(url) {
    if (Validation()) {
        $('#myModal').modal({
            backdrop: 'static',
            keyboard: false
        });
        var model = {
            Id: "",
            Amount: "",
            ChargeNumber: "",
            ChargeDate: "",
            CreatedBy: "",
            CreatedDate: "",
            Status :"",
            CustomerToAccess: []
        };
        model.Id = $("#Id").val();
        model.Amount = $("#txtAmount").val();
        model.ChargeNumber = $("#txtChargeNumber").val();
        model.ChargeDate = $("#txtChargeDate").val();
        model.CreatedBy = $("#CreatedBy").val();
        model.CreatedDate = $("#dtpCreatedDate").val();
        model.Status = $("#Status").val();
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

        $.post(varSitePath +"/Security/InsertReceivableDetails", { json: JSON.stringify(model) }, function (response) {
            $('#myModal').modal("hide");
            if (response.Status === 1) {
                swal(swalGlobal.SwalTitle_Success, response.Message, swalGlobal.SwalType_Success);
                $("#Reset").click();
                $("#MainView").load(url);
            }
            else {
                if (response.Status === 0);
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
    $.post(varSitePath +'/Security/UpdateReceivableStatus', { Id: Id, Status: Status }).done(function (response) {
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
        if ($(this).val() === "" ||$(this).val() === "0.00" || $(this).val() === null) {
            $(this).addClass('alert-danger');
        }
        else {
            $(this).removeClass('alert-danger');
        }
    });

    if ($('#tblCustomerToAccessList tbody').find('td.dataTables_empty').length > 0) {
        $("#tblCustomerToAccessList_wrapper").addClass('alert-danger-table');
    }
    else {
        $("#tblCustomerToAccessList_wrapper").removeClass('alert-danger-table');
    }

    if ($('.alert-danger').length > 0 || $(".alert-danger-table").length > 0) {
        swal(swalGlobal.SwalTitle_Error, "Please enter required fields.", swalGlobal.SwalType_Error);
        return false;
    }
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
    $("#MainView").load(url);
}