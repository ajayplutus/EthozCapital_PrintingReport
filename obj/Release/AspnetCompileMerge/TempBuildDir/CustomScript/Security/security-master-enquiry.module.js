
var opt = {
    modal: true,
    autoOpen: false,
    title: "Security Contracts",
    open: function () {
        var closeBtn = $('.ui-dialog-titlebar-close');
        closeBtn.html('<span class="ui-button-icon-primary ui-icon fm-button-icon-closethick"></span><span class="ui-button-text"></span>');
    },
    clickOutside: false
};

var SecurityInquiry = (function () {
    $(document).ready(function () {
        $('.js-example-basic-single3').select2();
        var d = new Date();
        var yrRange = (d.getFullYear() - 20) + ":" + (d.getFullYear() + 20);

        $('#securityMasterInqSearch').formValidation({
            framework: 'bootstrap',
            icon: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            fields: {
                dtpCreatedDateFrom: {
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
            .find('[name = "CreatedDateFrom"],[name = "CreatedDateTo"],[name = "ChargeDateFrom"],[name = "ChargeDateTo"]')
            .datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: yrRange,
                showButtonPanel: true,
                //minDate: "06/06/2019",
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
                    }
                    if ($(this).attr('id') === "dtpCreatedDateFrom") {
                        $('#securityMasterInqSearch').formValidation('revalidateField', 'CreatedDateFrom');

                        var dateTo = $('#dtpCreatedDateFrom').datepicker('getDate');
                        dateTo.setDate(dateTo.getDate());
                        $('#dtpCreatedDateTo').datepicker('option', 'minDate', dateTo);
                    }
                    else if ($(this).attr('id') === "dtpCreatedDateTo") {
                        $('#securityMasterInqSearch').formValidation('revalidateField', 'CreatedDateTo');
                    }
                    else if ($(this).attr('id') === "dtpChargeDateFrom") {
                        $('#securityMasterInqSearch').formValidation('revalidateField', 'ChargeDateFrom');

                        var toDate = $('#dtpChargeDateFrom').datepicker('getDate');
                        toDate.setDate(toDate.getDate());
                        $('#dtpChargeDateTo').datepicker('option', 'minDate', toDate);
                    }
                    else if ($(this).attr('id') === "dtpChargeDateTo") {
                        $('#securityMasterInqSearch').formValidation('revalidateField', 'ChargeDateTo');
                    }
                    else
                        alert("No Date");
                }
            });

        $("#SecurityTypeLevel1").change(function () {
            if ($("#SecurityTypeLevel1").val() !== "") {
                $("#SecurityTypeLevel2").empty();
                $.post(varSitePath + '/Security/GetSecurityTypeLevel2ByParentId',
                    { parentId: $("#SecurityTypeLevel1").val() }, function (data) {
                        $.each(data, function () {
                            $("#SecurityTypeLevel2").append($("<option></option>").val(this['Value']).html(this['Text']));
                        });
                    });
            }
        });

        $("#txtSecurityItemsCustomer").autocomplete({
            source: function (request, response) {
                var indVal = $('#SecurityItemsIndividual').val();
                $.ajax({
                    url: "GetCustomerAutoComplete",
                    data: { textFilter: request.term, IndividualCorporate: indVal },
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
                $('#txtSecurityItemsCustomer').val(ui.item.label);
                $('#securityItemsCustomerId').text(ui.item.value);
                return false;
            },
            change: function (event, ui) {
                if (!ui.item) {
                    $('#txtSecurityItemsCustomer').val('');
                    $('#securityItemsCustomerId').text('');
                }
                return false;

            }
        });

        $("#txtContractsCustomer").autocomplete({
            source: function (request, response) {
                var indVal = $('#ContractsIndividual').val();
                $.ajax({
                    url: "GetCustomerAutoComplete",
                    data: { textFilter: request.term, IndividualCorporate: indVal },
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
                $('#txtContractsCustomer').val(ui.item.label);
                $('#contractsCustomerId').text(ui.item.value);
                return false;
            },
            change: function (event, ui) {
                if (!ui.item) {
                    $('#txtContractsCustomer').val('');
                    $('#contractsCustomerId').text('');
                }
                return false;
            }
        });

        $("#SecurityItemsIndividual").change(function () {
            $("#txtSecurityItemsCustomer").val("");
        });

        $("#ContractsIndividual").change(function () {
            $("#txtContractsCustomer").val("");
        });

        $("#dialog").dialog(opt).dialog();

        initSecurityInquiryList();
        initSecurityContractList();

        $('.redirectUrl').click(function () {
            $('#myModal').modal({
                backdrop: 'static',
                keyboard: false
            });
            startTimer();
        });

        $(document).on('blur', '.security-customer', function () {
            if ($(this).context.value === '') {
                $(this).parent().find('.securityCustomerId').text('');
            }
        }); 
    });

    function onClickSave() {

    }
    var manage =
    {
        onClickSave: onClickSave
    };

    return {
        Manage: manage
    };
}());
function onSecurityInqResetClick(action) {
    $("#SecurityTypeLevel1").prop('selectedIndex', 0);
    $("#SecurityTypeLevel2").prop('selectedIndex', 0);
    $("#SecuritySystemId").val("");
    $("#SecurityItemStatus").select2("val", 0);
    $("#dtpCreatedDateFrom").val("");
    $("#dtpCreatedDateTo").val("");
    $("#dtpChargeDateFrom").val("");
    $("#dtpChargeDateTo").val("");
    $("#SecurityItemsIndividual").prop('selectedIndex', 0);
    $("#txtSecurityItemsCustomer").val("");
    $("#ContractsIndividual").prop('selectedIndex', 0);
    $("#txtContractsCustomer").val("");
    $("#ContractNumber").val("");
    $("#ContractRolloverNo").val("");
    $("#ContractsStatus").select2("val", 0);
    $('#contractsCustomerId').text("");
    $('#securityItemsCustomerId').text("");
    var param = {
        SecurityTypeLevel1: $("#SecurityTypeLevel1").find('option:selected').val(),
        SecurityTypeLevel2: $("#SecurityTypeLevel2").find('option:selected').val(),
        SecuritySystemId: $("#SecuritySystemId").val(),
        SecurityItemStatus: $('#SecurityItemStatus').val() === null ? "" : $('#SecurityItemStatus').val().join("','"),
        CreatedDateFrom: $("#dtpCreatedDateFrom").val(),
        CreatedDateTo: $("#dtpCreatedDateTo").val(),
        ChargeDateFrom: $("#dtpChargeDateFrom").val(),
        ChargeDateTo: $("#dtpChargeDateTo").val(),
        SecurityItemsIndividual: $("#SecurityItemsIndividual").find('option:selected').val(),
        SecurityItemsCustomer: $("#securityItemsCustomerId").text(),
        ContractsIndividual: $("#ContractsIndividual").find('option:selected').val(),
        ContractsCustomer: $('#contractsCustomerId').text(),
        ContractNumber: $("#ContractNumber").val(),
        ContractRolloverNo: $("#ContractRolloverNo").val(),
        ContractsStatus: $('#ContractsStatus').val() === null ? "" : $('#ContractsStatus').val().join("','"),
        viewType: action
    };

    $.post(varSitePath + '/Security/SecurityMasterEnquiryListView', param, function (data) {
        $("#divSearchResult").html(data);
        initSecurityInquiryList();
    });
}
function onSecurityInqSearchClick(action) {

    var param = {
        SecurityTypeLevel1: $("#SecurityTypeLevel1").find('option:selected').val(),
        SecurityTypeLevel2: $("#SecurityTypeLevel2").find('option:selected').val(),
        SecuritySystemId: $("#SecuritySystemId").val(),
        SecurityItemStatus: $('#SecurityItemStatus').val() === null ? "" : $('#SecurityItemStatus').val().join("','"),
        CreatedDateFrom: $("#dtpCreatedDateFrom").val(),
        CreatedDateTo: $("#dtpCreatedDateTo").val(),
        ChargeDateFrom: $("#dtpChargeDateFrom").val(),
        ChargeDateTo: $("#dtpChargeDateTo").val(),
        SecurityItemsIndividual: $("#SecurityItemsIndividual").find('option:selected').val(),
        SecurityItemsCustomer: $("#securityItemsCustomerId").text(),
        ContractsIndividual: $("#ContractsIndividual").find('option:selected').val(),
        ContractsCustomer: $('#contractsCustomerId').text(),
        ContractNumber: $("#ContractNumber").val(),
        ContractRolloverNo: $("#ContractRolloverNo").val(),
        ContractsStatus: $('#ContractsStatus').val() === null ? "" : $('#ContractsStatus').val().join("','"),
        viewType: action
    };
    if ($("#dtpCreatedDateFrom").val() !== '' && $("#dtpCreatedDateTo").val() !== '') {
        if ($("#dtpCreatedDateFrom").val() > $("#dtpCreatedDateTo").val()) {
            swal(swalGlobal.SwalTitle_Error, "Creation Date From must be earlier or equal to Creation Date To!", swalGlobal.SwalType_Error);
            return false;
        }
    }
    if ($("#dtpChargeDateFrom").val() !== '' && $("#dtpChargeDateTo").val() !== '') {
        if ($("#dtpChargeDateFrom").val() > $("#dtpChargeDateTo").val()) {
            swal(swalGlobal.SwalTitle_Error, "Charge Date From must be earlier or equal to Charge Date To", swalGlobal.SwalType_Error);
            return false;
        }
    }
    $.post(varSitePath + '/Security/SecurityMasterEnquiryListView', param, function (data) {

        $("#divSearchResult").html(data);
        initSecurityInquiryList();
    });
}

function initSecurityInquiryList() {
    $('#tblSecurityInquiryList').DataTable({
        dom: 'Blfrtip',
        pageLength: 10,
        buttons: ['copy', 'print',
            {
                extend: 'csvHtml5',
                title: 'Security-Inq-Export-Csv-' + new Date().getTime().toString()
            },
            {
                extend: 'excelHtml5',
                title: 'Security-Inq-Export-Excel-' + new Date().getTime().toString()
            },
            {
                extend: 'pdfHtml5',
                title: 'Security-Inq-Export-PDF-' + new Date().getTime().toString(),
                orientation: 'landscape',
                pageSize: 'TABLOID'
            }
        ] ///,
    });

    $('#tblSecurityInquiryList tbody').on('click', 'tr', function () {
        $('#tblSecurityInquiryList tr.selected').removeClass('selected');
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            $(this).addClass('selected');
        }
    });
}

function initSecurityContractList() {
    $('#tblSecurityContractList').DataTable({
        dom: 'Blfrtip',
        pageLength: 10,
        buttons: ['copy', 'print',
            {
                extend: 'csvHtml5',
                title: 'SecurityContract-Export-Csv-' + new Date().getTime().toString()
            },
            {
                extend: 'excelHtml5',
                title: 'SecurityContract-Export-Excel-' + new Date().getTime().toString()
            },
            {
                extend: 'pdfHtml5',
                title: 'SecurityContract-Export-PDF-' + new Date().getTime().toString(),
                orientation: 'portrait',
                pageSize: 'TABLOID'
            }
        ]
    });

    $('#tblSecurityContractList tbody').on('click', 'tr', function () {
        $('#tblSecurityInquiryList tr.selected').removeClass('selected');
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

function showSecurityContractsPopup(securityID, securityListLevel2, event) {
    $('#tblSecurityInquiryList tr.selected').removeClass('selected');
    $(event).parent().parent().addClass('selected');
    $.post(varSitePath + '/Security/SecurityContractsListView',
        { securityID: securityID, securityListLevel2: securityListLevel2 }, function (data) {
            $('#dialog').html(data);
            initSecurityContractList();
            $("#dialog").dialog(opt).dialog("open");
        });
}

function redirectToPage(Level2Code, SecurityID, ViewOrUpdate) {
    $.post(varSitePath + '/Security/GetPageUrlByLevel2Code', { level2Code: Level2Code, ViewOrUpdate: ViewOrUpdate }, function (response) {
        url = "/" + response.ControllerName + "/" + response.ActionName + "?SecurityId=" + SecurityID;
        window.location.href = url;
    });

}