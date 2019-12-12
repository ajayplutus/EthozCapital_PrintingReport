var InterestCode = (function () {

    var settings = {
    };

    $(document).ready(function () {
        setDefaultValue();
        var d = new Date();
        var yrRange = (d.getFullYear() - 20) + ":" + (d.getFullYear() + 20);

        $('#interstCodeForm').formValidation({
            framework: 'bootstrap',
            icon: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            fields: {
                dtpEffectiveDate: {
                    validators: {
                        notEmpty: {
                            message: 'Date is required!'
                        },
                        date: {
                            format: 'DD/MM/YY',
                            message: 'Date format is not valid!'
                        }
                    }
                }
            }
        })
            .find('[name = "EffectiveDate"],[name = "CreatedDate"],[name = "UpdatedDate"]')
            .datepicker({
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
                onSelect: function (selected) {
                    if (selected) {
                        $(this).removeClass('alert-danger');
                        $(this).addClass('readonly-date-bg');
                    }
                    if ($(this).attr('id') === "dtpEffectiveDate") {
                        $('#accountForm').formValidation('revalidateField', 'EffectiveDate');
                    }
                    else
                        alert("No Date");
                }
            });

        $(document).on('blur', '.rate_value', function () {
            let val = $(this).val();
            if (val == '' || val == '0' || val == '00' || val == '.00' || val == '.000') val = '0.0000';
            while (val.length > 0) {
                if (val.charAt(0) == '0' && val.charAt(1) != '.') {
                    val = val.substr(1);
                }
                else
                    break;
            }

            if (val.indexOf(',') == 0) {
                val = val.substr(1);
                if (val.charAt(0) == '0' && val.charAt(1) != '.') {
                    val = '0.0000';
                }
            }
            if (val.indexOf(".") == -1 && val != '') {
                $(this).val(val + '.0000');
            }
            else
                $(this).val(val);
        });

        $("#SaveInterestCode").click(function () {
            if (Validation()) {
                CheckInterestCode();
            }
        });

        $("#UpdateInterestCode").click(function () {
           
            if (Validation()) {
                swal({
                    title: swalGlobal.SwalTitle_Warning,
                    text: "Do you want to update the LEFS Interest Code?",
                    type: swalGlobal.SwalType_Warning,
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes",
                    cancelButtonText: "No"
                }, function (isConfirm) {
                    if (isConfirm) {
                        setTimeout(function () {
                            CheckInterestCode();
                        }, 1000);
                    }
                });
            }
        });

        $("#Reset").click(function () {
            setDefaultValue();
            $("#InterestCode").val("");
            $("#InterestType").prop('selectedIndex', 0);
            $("#SubContractType").prop('selectedIndex', 0);
            $("#dtpEffectiveDate").val("");
            $("#Description").val("");
            $("#RepaymentPeriodTo").prop('selectedIndex', 0);
            $("#Remarks").val("");
            $("#interstCodeForm").data('formValidation').resetForm();
            $("#interstCodeForm input,textarea,select").removeClass('alert-danger');
        });

        $("#interstCodeForm input,textarea,select").keydown(function () {
            if ($(this).val().length > 0 && $(this).val() !== "0.0000") {
                $(this).removeClass('alert-danger');
            }
        });

        inittblInterestCodeList();

    });

    function Validation() {
        $(".required").each(function () {
            if ($(this).val().trim() === "" || $(this).val() === "" || $(this).val() === "0.0000" || $(this).val() === null) {
                $(this).addClass('alert-danger');
                if ($(this).is("select")) {
                    $(this).parent().find("span.select2-selection__rendered").addClass('alert-danger');
                }
                if ($(this).attr('id') === "dtpEffectiveDate") {
                    $(this).removeClass('readonly-date-bg');
                    $(this).addClass('alert-danger');
                }
            }
            else {
                $(this).removeClass('alert-danger');
                if ($(this).is("select")) {
                    $(this).parent().find("span.select2-selection__rendered").removeClass('alert-danger');
                }
                if ($(this).attr('id') === "dtpEffectiveDate") {
                    $(this).addClass('readonly-date-bg');
                    $(this).removeClass('alert-danger');
                }
            }

        });
        if ($(".alert-danger").length > 0) {
            swal(swalGlobal.SwalTitle_Error, "Please enter required fields.", swalGlobal.SwalType_Error);
            return false;
        }

        if (!sumofRiskSharing()) {
            swal(swalGlobal.SwalTitle_Error, "Sum of Risk Sharing % By Spring and Risk Sharing % By ETHOZ must be equivalent to 100%", swalGlobal.SwalType_Error);
            return false;
        }

        if (!repaymentPeriodValidation()) {
            swal(swalGlobal.SwalTitle_Error, "Repayment Period To cannot be less than Repayment Payment From", swalGlobal.SwalType_Error);
            return false;
        }

        return true;
    }

	//added by Jason 01/07/2019 
    function repaymentPeriodValidation() {
        var RepaymentPeriodFromVal = $("#RepaymentPeriodFrom").find('option:selected').val();
        var RepaymentPeriodToVal = $("#RepaymentPeriodTo").find('option:selected').val();
        
        if (RepaymentPeriodToVal !== "" && RepaymentPeriodToVal !== "null") {
            if (parseInt(RepaymentPeriodToVal) >= parseInt(RepaymentPeriodFromVal))
                return true;
            else
                return false;
        }
        else
            return true;        
    }

    function sumofRiskSharing() {
        var riskSpring = $("#RiskSpring").val();
        var riskEthoz = $("#RiskEthoz").val();
        var totalRiskSharing = parseFloat(riskSpring) + parseFloat(riskEthoz);
        if (totalRiskSharing === 100)
            return true;
        else
            return false;
    }

    function setDefaultValue() {
        if ($("#InterestCodeId").val() == "0") {
            $("#RepaymentPeriodFrom").prop('selectedIndex', 1);
            $("#BankRate").val("0.0000");
            $("#CoyRate").val("0.0000");
            $("#RiskSpring").val("0.0000");
            $("#RiskEthoz").val("0.0000");
        }
    }

    function CheckInterestCode() {
        if ($("#InterestCode").val() !== "" && $("#InterestCode").val() !== "null") {
            var code = $("#InterestCode").val();
            if (code.charAt(0).toUpperCase() === "S" && $('#InterestType option:selected').text() !== "Fixed Rate") {
                swal({
                    title: swalGlobal.SwalTitle_Warning,
                    text: "Interest Code and selected Interest Type not match, do you want to continue",
                    type: swalGlobal.SwalType_Warning,
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes",
                    cancelButtonText: "No"
                }, function (isConfirm) {
                    if (isConfirm) {
                        onSaveNewInterestCode();
                    }
                });

            }
            else if (code.charAt(0).toUpperCase() !== "S" && $('#InterestType option:selected').text() === "Fixed Rate") {
                swal({
                    title: swalGlobal.SwalTitle_Warning,
                    text: "Interest Code and selected Interest Type not match, do you want to continue",
                    type: swalGlobal.SwalType_Warning,
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes",
                    cancelButtonText: "No"
                }, function (isConfirm) {
                    if (isConfirm) {
                        onSaveNewInterestCode();
                    }
                });
            }
            else {
                onSaveNewInterestCode();
            }
        }
        else
            isvalid = true;
    }

    function onClickSave() {

    }

    function onSaveNewInterestCode() {
        var model = {
            InterestCode: $("#InterestCode").val(),
            InterestType: $("#InterestType").find('option:selected').val(),
            SubContractType: $("#SubContractType").find('option:selected').val(),
            EffectiveDate: $("#dtpEffectiveDate").val(),
            Description: $("#Description").val(),
            BankRate: $("#BankRate").val(),
            CoyRate: $("#CoyRate").val(),
            RiskSpring: $("#RiskSpring").val(),
            RiskEthoz: $("#RiskEthoz").val(),
            RepaymentPeriodFrom: $("#RepaymentPeriodFrom").find('option:selected').val(),
            RepaymentPeriodTo: $("#RepaymentPeriodTo").find('option:selected').val(),
            Remarks: $("#Remarks").val(),
            Id: $("#InterestCodeId").val(),
            CreatedDate: $("#dtpCreatedDate").val()
        };
        $.post(varSitePath + "/LEFSInterestCode/InsertLEFSInterestCode", { json: JSON.stringify(model) }).done(function (response) {
            if (response.Status === 1) {
                if ($("#UpdateInterestCode").is(":visible")) {
                    window.location.href = varSitePath + "/LEFSInterestCode/LEFSInterestCodeEdit?CTGroupCode=&SubConGroupCode=&SubMenuId=" + SubMenuID;
                }
                else {
                    swal(swalGlobal.SwalTitle_Success, response.Message, swalGlobal.SwalType_Success);
                    $("#Reset").click();
                }
            }
            else {
                if (response.Status === 2) $("#Property_Address").addClass('alert-danger');
                swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
            }
        });
    }

    var manage =
        {
            onClickSave: onClickSave,
            onValidateDecimal: onValidateDecimal
        };

    return {
        Settings: settings,
        Manage: manage
    };
}());

function onValidateDecimal(key) {
    //getting key code of pressed key
    var keycode = (key.which) ? key.which : key.keyCode;
    //comparing pressed keycodes
    if (!(keycode === 8 || keycode === 46) && (keycode < 48 || keycode > 57)) {
        return false;
    }
    else {
        var parts = key.srcElement.value.split('.');
        var index = key.srcElement.value.indexOf('.');
        if (parts.length > 1 && keycode === 46) {
            return false;
        }
        if (index > 0) {
            var CharAfterdot = (parts[1].length + 1);
            if (CharAfterdot > 3) {
                return false;
            }
        }
        return true;
    }

}

$("#InterestCode").focusout(function () {
    if ($("#InterestCode").val() !== "" && $("#InterestCode").val() !== "null") {
        var code = $("#InterestCode").val()
        if (code.charAt(0).toUpperCase() === "S") {
            $("#InterestType").prop('selectedIndex', 1);
        }
        else {
            $("#InterestType").prop('selectedIndex', 2);
        }
    }
});

function onInterestCodeSearchClick(action) {

    var param = {
        InterestCode: $("#InterestCode").val(),
        InterestType: $("#InterestType").find('option:selected').val(),
        ContractType: $("#ContractType").find('option:selected').val(),
        Status: $("#Status").find('option:selected').val(),
        viewType: action
    };
   
    $.post(varSitePath +'/LEFSInterestCode/LEFSInterestCodeListView', param, function (data) {

        $("#divSearchResult").html(data);
        inittblInterestCodeList();
    });

    // You could also use an ajax property on the data table initialization

}
function inittblInterestCodeList() {
    $('#tblInterestCodeList').DataTable({
        dom: 'Blfrtip',
        pageLength: 10,
        buttons: ['copy', 'print',
            {
                extend: 'csvHtml5',
                title: 'Export-Data-Csv-' + new Date().getTime().toString()
            },
            {
                extend: 'excelHtml5',
                title: 'Export-Data-Excel-' + new Date().getTime().toString()
            },
            {
                extend: 'pdfHtml5',
                title: 'Export-Data-PDF-' + new Date().getTime().toString(),
                orientation: 'landscape',
                pageSize: 'TABLOID'
            }
        ] ///,
    });

    $('#tblInterestCodeList tbody').on('click', 'tr', function () {
        $('#tblInterestCodeList tr.selected').removeClass('selected');
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            $(this).addClass('selected');
        }
    });
}

function onViewClick(path) {
    window.location.href = path;
}


function onResetViewClick() {
    document.getElementById('interstCodeForm').reset();
    $("#interstCodeForm").data('formValidation').resetForm();
}

function showInterestCodeDeactivatePopup(id, IsPreContract, event) {
    $('#tblInterestCodeList tr.selected').removeClass('selected');
    $(event).parent().parent().addClass('selected');

    if (IsPreContract == 1) {
        swal(swalGlobal.SwalTitle_Warning, "Interest code is selected in Pre-Contract, you are not allowed to deactivate it.", swalGlobal.SwalType_Warning);
    }
    else {
        swal({
            title: swalGlobal.SwalTitle_Warning,
            text: "Do you want to deactivate the LEFS Interest Code?",
            type: swalGlobal.SwalType_Warning,
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Yes",
            cancelButtonText: "No"
        }, function (isConfirm) {
            if (isConfirm) {
                $("#InterestCodeDeactivatePopup").modal('show');
                $("#hdnInterestCodeId").val(id);
            }
        });

    }

}

function showInterestCodeActivatePopup(id, IsPreContract, event) {
    $('#tblInterestCodeList tr.selected').removeClass('selected');
    $(event).parent().parent().addClass('selected');

    if (IsPreContract == 1) {
        swal(swalGlobal.SwalTitle_Warning, "Interest code is selected in Pre-Contract, you are not allowed to Activate it.", swalGlobal.SwalType_Warning);
    }
    else {
        swal({
            title: swalGlobal.SwalTitle_Warning,
            text: "Do you want to activate the LEFS Interest Code?",
            type: swalGlobal.SwalType_Warning,
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Yes",
            cancelButtonText: "No"
        }, function (isConfirm) {
            if (isConfirm) {
                $("#InterestCodeActivatePopup").modal('show');
                $("#hdnInterestCodeIds").val(id);
            }
        });

    }

}

function formInterestCodeDeactivate() {
    $('#frmInterestCodeDeactivate').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
    });
}

function formInterestCodeActivate() {
    $('#frmInterestCodeActivate').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
    });
}

function interestCodeDeactivateClick() {
    $("#btnInterestCodeDeactivate").click(function () {

        $("#InterestCodeDeactivatePopup .required").each(function () {
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
        if ($("#InterestCodeDeactivatePopup .alert-danger").length > 0) {
            //swal(swalGlobal.SwalTitle_Error, "Please enter required fields.", swalGlobal.SwalType_Error);
            return false;
        }
        if ($("#frmInterestCodeDeactivate")[0].checkValidity()) {
            var model = {
                id: $("#hdnInterestCodeId").val(),
                reason: $("#DeactivateReson").val(),
                confirm: true,
            };

            $.post(varSitePath +'/LEFSInterestCode/DeactivateInterestCode', model).done(function (response) {
                console.info("response");
                $("#InterestCodeDeactivatePopup").modal('hide');
                if (response.Status === 1) {
                    swal(swalGlobal.SwalTitle_Success, response.Message, swalGlobal.SwalType_Success);
                    setTimeout(function () { onInterestCodeSearchClick('ViewAndUpdate'); }, 1000);
                }
                else {
                    if (response.Status === 2)
                        swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
                }
            });
        }
    });
}

function interestCodeActivateClick() {
    $("#btnInterestCodeActivate").click(function () {

        $("#InterestCodeActivatePopup .required").each(function () {
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
        if ($("#InterestCodeActivatePopup .alert-danger").length > 0) {
            //swal(swalGlobal.SwalTitle_Error, "Please enter required fields.", swalGlobal.SwalType_Error);
            return false;
        }
        if ($("#frmInterestCodeActivate")[0].checkValidity()) {
            var model = {
                id: $("#hdnInterestCodeIds").val(),
                reason: $("#ActivateReson").val(),
                confirm: true,
            };

            $.post(varSitePath +'/LEFSInterestCode/ActivateInterestCode', model).done(function (response) {
                console.info("response");
                $("#InterestCodeActivatePopup").modal('hide');
                if (response.Status === 1) {
                    swal(swalGlobal.SwalTitle_Success, response.Message, swalGlobal.SwalType_Success);
                    setTimeout(function () { onInterestCodeSearchClick('ViewAndUpdate'); }, 1000);
                }
                else {
                    if (response.Status === 2)
                        swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
                }
            });
        }
    });
}

$('select').on('change', function () {
    if ($(this).hasClass('alert-danger')) {
        $(this).removeClass('alert-danger');
    }
});