var optDtlDialog = {
    autoOpen: false,
    dialogClass: 'ModalWindowDisplayMeOnTop',
    modal: true,
    width: 600,
    open: function () {
        $('.ui-dialog-titlebar').addClass('fm-button-icon-closethick');
    }
};

var optRsnDialog = {
    autoOpen: false,
    dialogClass: 'ReasonModalWindowDisplayMeOnTop',
    modal: true,
    open: function () {
        $('.ui-dialog-titlebar').addClass('fm-button-icon-closethick');
        $("#divDisableBackground").show();
    }
    ,
    close: function () {
        $("#divDisableBackground").hide();
    }
};

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

    var d = new Date();
    var yrRange = (d.getFullYear() - 20) + ":" + (d.getFullYear() + 20);
    $(".dtpApprovalDate").datepicker({
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

    var firstDay = new Date(new Date().getFullYear(), 0, 1);
    $('#dtpApprovedDateFrom').datepicker('setDate', firstDay);

    var lastDay = new Date(new Date().getFullYear(), 11, 31);
    $('#dtpApprovedDateTo').datepicker('setDate', lastDay);

    $("#txtReassignBatchNo").autocomplete({
        source: function (request, response) {
            var sourcePage = $("#ApprovalPage").val();
            $.ajax({
                url: "GetBatchNoAutoComplete",
                data: { textFilter: request.term, sourcePage: sourcePage },
                type: 'GET',
                cache: false,
                dataType: 'json',
                global: false,
                success: function (json) {
                    response(JSON.parse(json.data));
                }
            });
        },
        minLength: 1,
        scroll: true,
        autoFocus: false,
        autoFill: false,
        select: function (event, ui) {
            $('#txtReassignBatchNo').val(ui.item.label);
            $('#ReassignBatchNoId').text(ui.item.value);
            return false;
        },
        change: function (event, ui) {
            if (!ui.item) {
                $('#txtReassignBatchNo').val('');
                $('#ReassignBatchNoId').text('');
            }
            return false;

        }
    });

    RejectApprovalEvent();


});

function onDetailClick(CompositeKey1, CompositeKey2, ApprovalProcessDetailID, ApprovalHeaderID, ApprovalProcessID, Status) {
    $.post(varSitePath + '/Approval/ApprovalSpotterDetails',
        { SpotterRefNo: CompositeKey1, ApprovalProcessDetailID: ApprovalProcessDetailID, ApprovalHeaderID: ApprovalHeaderID, ApprovalProcessID: ApprovalProcessID, strStatus: Status }, function (data) {
            $('#modalDialog').html(data);
            $("#modalDialog").dialog(optDtlDialog).dialog("open");
        });
}

function onReassignClick(ApprovalProcessDetailID, preparationDate, BatchNo, ) {
    $.post(varSitePath + '/Approval/ReassignApprovalDetails',
        { SpotterRefNo: BatchNo, PreparationDate: preparationDate, ApprovalProcessDetailID: ApprovalProcessDetailID }, function (data) {
            $('#ReassignApprovalPopUp').html(data);
            $("#ReassignApprovalPopUp").dialog(optDtlDialog).dialog("open");
        });
}

function onApprovedViewSearchClick() {

    var batchNo = $("#txtReassignBatchNo").val();
    var dateFrom = $('#dtpApprovedDateFrom').val();
    var dateTo = $("#dtpApprovedDateTo").val();
    var type = $("#sysApprovalName").find('option:selected').val();

    $("#AprrovalSearch .required").each(function () {
        if ($(this).val() === "" || $(this).val() === "0.0000" || $(this).val() === null) {
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

    if ($("#AprrovalSearch .alert-danger").length > 0) {
        swal(swalGlobal.SwalTitle_Error, "Please enter required fields", swalGlobal.SwalType_Error);
        return;
    }

    if ($("#dtpApprovedDateFrom").val() !== '' && $("#dtpApprovedDateTo").val() !== '') {
        if ($("#dtpApprovedDateFrom").val() > $("#dtpApprovedDateTo").val()) {
            swal(swalGlobal.SwalTitle_Error, "Approved Date From must be earlier or equal to Approved Date To!", swalGlobal.SwalType_Error);
            return;
        }
    }

    $.post(varSitePath + '/Approval/OnSearchApprovedView', { AssignTo: '', BatchNo: batchNo, DateFrom: dateFrom, DateTo: dateTo, Type: type, ApprovalPage: 'IsHistory' }, function (data) {
        console.info("data", data);
        $("#ApprovedView").html(data);

    });
}

function onResetClick() {

    $("#txtReassignBatchNo").val('');
    var firstDay = new Date(new Date().getFullYear(), 0, 1);
    $('#dtpApprovedDateFrom').datepicker('setDate', firstDay);

    var lastDay = new Date(new Date().getFullYear(), 11, 31);
    $('#dtpApprovedDateTo').datepicker('setDate', lastDay);
    $("#sysApprovalName").prop('selectedIndex', 0);
    $('#tblSpotterDetails tbody').empty();
}

function onReassignViewSearchClick() {

    var batchNo = $("#ReassignBatchNoId").text();
    var assignTo = $("#sysApprovalName").find('option:selected').val();

    $("#ReassignSearch .required").each(function () {
        if ($(this).val() === "" || $(this).val() === "0.0000" || $(this).val() === null) {
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

    if ($("#ReassignSearch .alert-danger").length > 0) {
        swal(swalGlobal.SwalTitle_Error, "Please enter required fields", swalGlobal.SwalType_Error);
        return;
    }

    $.post(varSitePath + '/Approval/OnSearchApprovedView', { AssignTo: assignTo, BatchNo: batchNo, DateFrom: '', DateTo: '', Type: '', ApprovalPage: 'IsReassign' }, function (data) {
        $("#ReassignView").html(data);
    });
}

function onApproveSpotter(ApprovalProcessDetailID) {
    console.info("onApproveSpotter");
    if ($("#tblContracDetails tbody tr").find("input[type=checkbox]:checked").length === 0) {
        swal(swalGlobal.SwalTitle_Error, "Please at least select one contract to approve!", swalGlobal.SwalType_Error);
        return;
    }
    else {
        fnApproveRejectSpotter(ApprovalProcessDetailID);
    }
}

function onRejectSpotter(ApprovalProcessDetailID) {
    console.info("onRejectSpotter");
    if ($("#tblContracDetails tbody tr").find("input[type=checkbox]:checked").length === 0) {
        swal(swalGlobal.SwalTitle_Error, "Please at least select one contract to reject!", swalGlobal.SwalType_Error);
        return;
    }
    else {
        $("#RejectApprovalReasonPopUp").dialog(optRsnDialog).dialog("open");
        $("#hdnApprovalProcessDetailID").val(ApprovalProcessDetailID);
    }
}

function onCancelReasonPopup() {
    $("#RejectApprovalReasonPopUp").dialog(optRsnDialog).dialog("close");
    $("#hdnApprovalProcessDetailID").val('');
    $("#RejectReason").val('');
}

function RejectApprovalEvent() {
    $("#btnRejectApproval").click(function () {

        $("#RejectApprovalReasonPopUp .required").each(function () {
            if ($(this).val().trim() === "" || $(this).val() === "0.0000" || $(this).val() === null) {
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
        if ($("#RejectApprovalReasonPopUp .alert-danger").length > 0) {
            swal(swalGlobal.SwalTitle_Error, "Reason cannot leave empty!", swalGlobal.SwalType_Error);
            return;
        }
        if ($("#frmRejectApprovalReasonPopUp")[0].checkValidity()) {

            var ApprovalProcessDetailID = $("#hdnApprovalProcessDetailID").val();
            var Reason = $("#RejectReason").val();
            fnApproveRejectSpotter(ApprovalProcessDetailID, Reason);
        }
    });
}

function fnApproveRejectSpotter(ApprovalProcessDetailID, Reason) {
    model = {
        ApprovalProcessDetailID: '',
        ApprovalHeaderID: '',
        RejectReason: '',
        IsAllCheck: '',
        SpotterSummary:
            { SpotterRefNumber: '' }
        ,
        OutstandingFee: []
    }
    if ($("#tblContracDetails tbody tr").find("input[type=checkbox]:checked").length === $("#tblContracDetails tbody tr").length) {
        model.IsAllCheck = true;
    }
    else {
        model.IsAllCheck = false;
    }
    model.ApprovalProcessDetailID = ApprovalProcessDetailID;
    model.ApprovalHeaderID = $("#ApprovalHeaderID").val();
    model.ApprovalProcessID = $("#ApprovalProcessID").val();
    model.RejectReason = Reason;
    $("#tblSummary tbody tr").each(function () {
        model.SpotterSummary.Amount = $(this).find('.txtAmount').text();
    });
    model.SpotterSummary.SpotterRefNumber = $("#SpotterSummary_SpotterRefNumber").val();
    model.SpotterSummary.PreparationDate = $("#SpotterSummary_PreparationDate").val();
    $("#tblContracDetails tbody tr").each(function () {
        var $isChecked = $(this).find('input[type="checkbox"]');
        if ($isChecked.is(":checked")) {
            var spotterDetails = {
                ReferralName: $(this).find('.referralName').text(),
                SpotterAmt: $(this).find('.contractAmount').text(),
                SpotterDetailId: $(this).find('.spotterDetailId').text(),
                ContractNumber: $(this).find('.contractNumber').text(),
                RolloverNumber: $(this).find('.rolloverNumber').text(),
                ItemNumber: $(this).find('.itemNumber').text()
            };
            model.OutstandingFee.push(spotterDetails);
        }
    });
    $.post(varSitePath + '/Approval/ApproveRejectSpotter',
        { json: JSON.stringify(model) }, function (response) {
            if (response.Status === 1) {
                swal({
                    title: swalGlobal.SwalTitle_Success,
                    text: response.Message,
                    type: swalGlobal.SwalType_Success
                }, function () {
                    $("#modalDialog").dialog(optDtlDialog).dialog("close");
                    $("#RejectApprovalReasonPopUp").dialog(optDtlDialog).dialog('close');
                    location.reload(true);
                });
            }
            else {
                swal({
                    title: swalGlobal.SwalTitle_Error,
                    text: response.Message,
                    type: swalGlobal.SwalType_Error
                });
            }
        });
}

function onReassignApproval() {

    var reaasignReason = $("#txtReassignReason").val();
    var assignTo = $("#reAssignTo").find('option:selected').val();
    var approvalProcessDetailID = $("#ApprovalProcessDetailID").val();

    $("#divReassignApprovalDetails .required").each(function () {
        if ($(this).val().trim() === "" || $(this).val() === "0.0000" || $(this).val() === null) {
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
    if ($("#divReassignApprovalDetails .alert-danger").length > 0) {
        swal(swalGlobal.SwalTitle_Error, "Please enter required fields", swalGlobal.SwalType_Error);
        return;
    }
    else {
        $('#myModal').modal({
            backdrop: 'static',
            keyboard: false
        });
        model = {
            ApprovalProcessDetailID: '',
            AssignedTo: '',
            ReassignReason: '',
            SpotterSummary:
                { SpotterRefNumber: '' }
            ,
            OutstandingFee: []
        };
        model.ApprovalProcessDetailID = approvalProcessDetailID;
        model.AssignedTo = assignTo;
        model.ReassignReason = reaasignReason;
        model.SpotterSummary.SpotterRefNumber = $("#SpotterSummary_SpotterRefNumber").val();
        model.SpotterSummary.PreparationDate = $("#SpotterSummary_PreparationDate").val();
        $("#tblSummary tbody tr").each(function () {
            model.SpotterSummary.Amount = $(this).find('.txtAmount').text();
        });
        $("#tblContracDetails tbody tr").each(function () {
            var spotterDetails = {
                ReferralName: $(this).find('.referralName').text(),
                SpotterAmt: $(this).find('.contractAmount').text(),
                SpotterDetailId: $(this).find('.spotterDetailId').text(),
                ContractNumber: $(this).find('.contractNumber').text(),
                RolloverNumber: $(this).find('.rolloverNumber').text(),
                ItemNumber: $(this).find('.itemNumber').text()
            };
            model.OutstandingFee.push(spotterDetails);
        });
        FnReassignApproval(model);
    }
}

function FnReassignApproval(model) {

    $.post(varSitePath + '/Approval/ReassignApproval', { json: JSON.stringify(model) }, function (data) {
        if (data.Status === 2) {
            swal(swalGlobal.SwalTitle_Error, data.Message, swalGlobal.SwalType_Error);
            return;
        }
        else if (data.Status === 1) {
            swal({
                title: swalGlobal.SwalTitle_Success,
                text: data.Message,
                type: swalGlobal.SwalType_Success
            }, function () {
                $("#ReassignApprovalPopUp").dialog(optDtlDialog).dialog("close");
                $("#Reset").click();
                $("#sysApprovalName").empty();
                $.post(varSitePath + '/Approval/GetApprovingOfficerList', function (data) {
                    $.each(data, function () {
                        $("#sysApprovalName").append($("<option></option>").val(this['Value']).html(this['Text']));
                    });
                });
            });
            $('#myModal').modal("hide");
            return;
        }
        else {
            swal(swalGlobal.SwalTitle_Error, data.Message, swalGlobal.SwalType_Error);
            $('#myModal').modal("hide");
            return;
        }
    });
}
