var IsEditView = false;

$(document).ready(function () {
    var d = new Date();
    var yrRange = (d.getFullYear() - 20) + ":" + (d.getFullYear() + 20);
    $(".dtpPaymentDate").datepicker({
        changeMonth: true,
        changeYear: true,
        yearRange: yrRange,
        dateFormat: 'dd/mm/yy',
        showButtonPanel: true,
        closeText: 'Clear',
        gotocurrent: true,
        onClose: function () {
            var event = arguments.callee.caller.caller.arguments[0];
            if ($(event.delegateTarget).hasClass('ui-datepicker-close')) {
                $(this).val('');
            }
        }
    });

    $('.dtpPaymentDate').datepicker('setDate', new Date());
    if (!IsEditView) {
        PopulateSpotterFee();
    }
    FnCalculateSummary();
});

function onSearchSpotterDetails() {
    $('#divContractDetails').html('');
    $("#divSummary").removeClass("alert-danger-table");
    if (!IsEditView) {
        if ($('#dtpAgrementDate').val() === '' || $('#dtpAgrementDate').val() === null) {
            swal(swalGlobal.SwalTitle_Error, "Please input a valid As at Date.", swalGlobal.SwalType_Error);
            $("#dtpAgrementDate").removeClass('readonly-date-bg');
            $("#dtpAgrementDate").addClass('alert-danger');
            return false;
        }
        else {
            $("#dtpAgrementDate").addClass('readonly-date-bg');
            $("#dtpAgrementDate").removeClass('alert-danger');
            PopulateSpotterFee();
        }
    }
    else {
        if ($("#spotterRefNumber").val() === "") {
            swal(swalGlobal.SwalTitle_Error, "Please select Spotter Ref Number", swalGlobal.SwalType_Error);
            return;
        }
        else {
            PopulateSpotterFee();
        }
    }
}

function onResetSpotterDetails() {
    $('#divContractDetails').html('');
    if (!IsEditView) {
        $('.dtpPaymentDate').datepicker('setDate', new Date());
        $("#dtpAgrementDate").addClass('readonly-date-bg');
        $("#dtpAgrementDate").removeClass('alert-danger');
        PopulateSpotterFee();
    }
    else {
        FnUnlockRecord($("#spotterRefNumber").find('option:selected').val());
        $("#spotterRefNumber").prop('selectedIndex', 0);
        $('#tblContracDetails tbody').empty();
        $('#tblSpotterDetails tbody').empty();
        $('#dtpPreparationDate').val('');
    }
    $("#divSummary").removeClass("alert-danger-table");
    FnCalculateSummary();
}

function SaveSpotterFee() {
    if (Validation()) {
        $('#myModal').modal({
            backdrop: 'static',
            keyboard: false
        });
        model = {
            SpotterId: '',
            SpotterRefNumber: '',
            PreparationDate: '',
            Amount: '',
            SpotterContractDetails: []
        };
        model.SpotterId = $("#SpotterId").val(),
            model.SpotterRefNumber = $("#spotterRefNumber").find('option:selected').val(),
            model.PreparationDate = $("#dtpPreparationDate").val(),
            model.Status = $("#Status").val(),
            model.CreatedBy = $("#CreatedBy").val(),
            model.CreatedDate = $("#CreatedDate").val(),

            $("#tblSummary tbody tr").each(function () {
                model.Amount = $(this).find('.txtAmount').text()
            });
        $("#tblContracDetails tbody tr").each(function () {
            var spotterDetails = {
                SpotterDetailId: $(this).find('.spotterDetailId').text(),
                ContractNumber: $(this).find('.contractNumber').text(),
                RolloverNumber: $(this).find('.rolloverNumber').text(),
                ItemNumber: $(this).find('.itemNumber').text()
            };
            model.SpotterContractDetails.push(spotterDetails);
        });
        $.post(varSitePath + "/Payment/InsertSpotterData", { json: JSON.stringify(model) }, function (response) {
            $('#myModal').modal("hide");
            if (response.Status === 1) {
                swal({
                    title: swalGlobal.SwalTitle_Success,
                    text: response.Message,
                    type: swalGlobal.SwalType_Success
                }, function () {
                    if (!IsEditView) {
                        PopulateSpotterFee();
                    }
                    else {
                        $("#spotterRefNumber").prop('selectedIndex', 0);
                        $('#divContractDetails').html('');
                        $('#tblSpotterDetails tbody').empty();
                        $("#dtpPreparationDate").val("");
                        FnCalculateSummary();
                    }
                });
                
                $('#myModal').modal("hide");
            }
            else {
                if (response.Status === 0) {
                    swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
                }
                $('#myModal').modal("hide");
            }
        });
    }
}

function PopulateSpotterFee() {
    $('#divContractDetails').html('');
    if (!IsEditView) {

        var dtpPreparationDate = $('#dtpAgrementDate').val();
        $.post(varSitePath + '/Payment/RetrieveOutstandingSpotterFee', { dtPreparationDate: dtpPreparationDate }, function (data) {
            if (data.length === 0) {
                swal(swalGlobal.SwalTitle_Error, "No outstanding spotter fee found!", swalGlobal.SwalType_Error);
                $('#tblSpotterDetails tbody').empty();
                FnCalculateSummary();
                return;
            }
            else {
                var refNum = '';
                GetSpotterDetails(refNum);
            }
            $('#myModal').modal("hide");
        });
    }
    else {
        var refNum = $("#spotterRefNumber").find('option:selected').val();
        $.post(varSitePath + '/Payment/FnCheckRecordLockStatus', { refNumber: refNum }, function (response) {
            if (response.Status === 1) {
                swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
                $('#divContractDetails').html('');
                $('#tblSpotterDetails tbody').empty();
                FnCalculateSummary();
                return;
            }
            else {
                var SpotterRefNum = $("#spotterRefNumber").find('option:selected').val();
                $.post(varSitePath + '/Payment/RetrieveSpotterFee', { refNumber: SpotterRefNum ,strStatus :'P'}, function (data) {
                    if (data.length === 0) {
                        swal(swalGlobal.SwalTitle_Error, "No Record(s) found!", swalGlobal.SwalType_Error);
                        $('#divContractDetails').html('');
                        $('#tblSpotterDetails tbody').empty();
                        return;
                    }
                    else {
                        GetSpotterDetails(refNum,data);
                    }
                    $('#myModal').modal("hide");
                });
            }

            $('#myModal').modal("hide");
        });
    }

}

function GetSpotterDetails(refNumber,response) {
    $.post(varSitePath + '/Payment/GetSpotterDetails', { spotterRefNum: refNumber }, function (data) {
        console.info("In method");
        $("#divSpotterDetail").html(data);
        if (IsEditView) {
            var SpotterRefNum = $("#spotterRefNumber").find('option:selected').val();
            GetSpotterMaster(SpotterRefNum);
            FnLockRecord(SpotterRefNum);
            FnCalculateSummary(response);
        }
        else {
            FnCalculateSummary();
        }
        $('#myModal').modal("hide");
    });
}

function GetSpotterMaster(SpotterRefNum) {
    $.post(varSitePath + '/Payment/GetSpotterMaster', { spotterRefNum: SpotterRefNum }, function (data) {
        if (data !== null) {
            $("#SpotterId").val(data.SpotterID);
            $("#dtpPreparationDate").val(data.PreparationDate);
            $("#Status").val(data.Status);
            $("#CreatedBy").val(data.CreatedBy);
            $("#CreatedDate").val(data.CreatedDate);
        }
        $('#myModal').modal("hide");
    });
}

function FnLockRecord(refNumber) {
    $.post(varSitePath + '/Payment/LockRecord', { refNumber: refNumber }, function (data) {
        $('#myModal').modal("hide");
    });
}

function FnUnlockRecord(refNumber) {
    $.post(varSitePath + '/Payment/RemoveLockRecord', { refNumber: refNumber }, function (data) {
        $('#myModal').modal("hide");
    });
}

function FnPopulateContractDetails(refferId) {
    $('#tblContracDetails tbody').empty();
    $.post(varSitePath + '/Payment/PopulateContractDetails', { refferId: refferId }, function (data) {
        $("#divContractDetails").html(data);
        $('#myModal').modal("hide");
    });
}

function FnCalculateSummary(data) {
    $('#myModal').modal("hide");
    var count = 0;
    var amount = "0.00";
    if (data !== undefined) {
        $.each(data, function (item, value) {
            if (value.PostInd === 'Y') {
                amount = parseFloat(amount) + parseFloat(value.SpotterAmt);
            }
        });
        amount = parseFloat(amount);
        count = $("#tblSpotterDetails tbody tr").find("input[type=checkbox]:checked").length;
    }
    else {
        $("#tblSpotterDetails tbody tr").each(function () {
            var isChecked = $(this).find("[type = checkbox]").prop("checked");
            if (isChecked) {
                var amt = $(this).find(".detailsAmount").text().replace(/,/g, "");
                amount = parseFloat(amount) + parseFloat(amt);
            }
        });
        count = $("#tblSpotterDetails tbody tr").find("input[type=checkbox]:checked").length;
    }
    var amt = parseFloat(amount).toFixed(2);
    $tr = $('<tr>').append(
        $('<td class="align-center">').text(count),
        $('<td>').html('$<span class="txtAmount float-right">' + amt.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + '</span>')
    );
    $('#tblSummary tbody').html($tr);
}

function Validation() {
    $(".required").each(function () {
        if ($(this).val() === "" || $(this).val() === "0.00" || $(this).val() === null || $(this).val() === "--Select--") {
            $(this).addClass('alert-danger');
            if ($(this).is("select")) {
                $(this).parent().find("span.select2-selection__rendered").addClass('alert-danger');
            }
            if ($(this).attr('id') === "dtpAgrementDate") {
                $(this).removeClass('readonly-date-bg');
                $(this).addClass('alert-danger');
            }
        }
        else {
            $(this).removeClass('alert-danger');
            if ($(this).is("select")) {
                $(this).parent().find("span.select2-selection__rendered").removeClass('alert-danger');
            }
            if ($(this).attr('id') === "dtpAgrementDate") {
                $(this).addClass('readonly-date-bg');
                $(this).removeClass('alert-danger');
            }
        }

    });

    $("#tblSummary tbody tr").each(function () {
        var amt = $(this).find('.txtAmount').text();
        if (amt === "0.00") {
            $("#divSummary").addClass("alert-danger-table");
        }
        else {
            $("#divSummary").removeClass("alert-danger-table");
        }
    });

    if ($(".alert-danger").length > 0 || $('.alert-danger-table').length > 0) {
        swal(swalGlobal.SwalTitle_Error, "Please fill in mandatory fields!", swalGlobal.SwalType_Error);
        return false;
    }
    else
        return true;
}

function fnIncludeTransaction(event, referalnumber, postind) {
    var postInd = "";
    var isChecked = event.checked;
    if (isChecked) {
        postInd = "Y";
    }
    else {
        postInd = "N";
    }
    $.post(varSitePath + '/Payment/IncludeTransaction', { PostInd: postInd, Refereal: referalnumber }, function (data) {
        if (data !== undefined) {
            FnCalculateSummary(data);
            $('#myModal').modal("hide");
        }
    });
    FnPopulateContractDetails(referalnumber);
}

function fnIncludeTransactionCon(event, referalnumber, cotractNum, amt) {
    var postInd = "";
    var isChecked = event.checked;
    if (isChecked) {
        postInd = "Y";
    }
    else {
        postInd = "N";
    }
    $.post(varSitePath + '/Payment/IncludeTransaction', { PostInd: postInd, Refereal: referalnumber, CotractNum: cotractNum }, function (data) {
        if (data !== undefined) {
            FnCalculateSummary(data);
            $('#myModal').modal("hide");
        }
    });


    if ($('[spotterId=' + referalnumber + ']').length >= 1 && $('[spotterId=' + referalnumber + ']:checked').length === 0) {
        $('tr[id^="' + referalnumber + '"]').find("input[type=checkbox]").prop('checked', false);
    }
    else {
        $('tr[id^="' + referalnumber + '"]').find("input[type=checkbox]").prop('checked', true);
    }
}