//$('.money').mask("#,##0.00", { reverse: true });
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
    HideRocUenorFinPassport();
    
    $(document).click(function () {
        if ($("#txtCustomer").val() == '') {
            ResetDropdownList();
            $("#txtROCUEN").val('');
            $("#txtNRICFINPASSPORT").val('');
        }
    });



    $("#txtCustomer").autocomplete({
        source: function (request, response) {
            var indVal = $('#ddlIndividualCorporate').val();
            $.get(varSitePath +"/Security/GetCustomerAutoComplete", { textFilter: request.term, IndividualCorporate: indVal }, function (json) {
                response(JSON.parse(json.data));
            });
        },
        minLength: 1,
        scroll: true,
        autoFocus: false,
        autoFill: false,
        select: function (event, ui) {
            $('#txtCustomer').val(ui.item.label);
            $('#customerId').val(ui.item.value);
            GetNricFinPassport(ui.item.value);
            GetRocUen(ui.item.value);
            GetAddress(ui.item.value);
            return false;
        },
        change: function (event, ui) {
            if (!ui.item)
                $('#txtCustomer').val('');
            return false;
        }
    });

    $("#ddlIndividualCorporate").on('change', function (event) {
        $("#txtCustomer").val('');
        ResetDropdownList();
        HideRocUenorFinPassport();
    });
});

function GetNricFinPassport(selectedCustomer) {
    $.get(varSitePath +"/Security/getNricFinPassportType", { selected: selectedCustomer }, function (json) {
        $("#txtNRICFINPASSPORT").val(json.data);
    });
}

function GetRocUen(selected) {
    $.get(varSitePath +"/Security/getRocUenType", { selected: selected }, function (json) {
        $("#txtROCUEN").val(json.data);
    });
}

function HideRocUenorFinPassport() {
    var IndividualCorporate;
    IndividualCorporate = $("#ddlIndividualCorporate").val();
    if (IndividualCorporate === 'Individual') {
        $("#RocUendiv").hide();
        $("#NRICFINPASSPORTdiv").show();
    }
    else {
        $("#RocUendiv").show();
        $("#NRICFINPASSPORTdiv").hide();
    }
}

function GetAddress(selected) {
    var IndividualCorporate = $("#ddlIndividualCorporate").val();
    $.get(varSitePath +"/Security/getAddress", { selected: selected, IndividualCorporate: IndividualCorporate }, function (json) {
        if (json.data.length > 0) {
            $("#Address").html("");
            $.each(json.data, function (i, v) {
                $("#Address").append($('<option value=' + v.AddressId + '>' + v.Address + '</option>'));
            });
            getDepartment($("#Address").val());
        }
        else {
            $("#Address").html("");
            $("#Address").append($('<option>' + "--Select--" + '</option>'));
            $("#Department").html("");
            $("#ContactPerson").html("");
        }
    });
}

function getDepartment(selectedAddress) {
    $.get(varSitePath +"/Security/getDepartmentList", { selectedAddress: selectedAddress }, function (json) {
        if (json.data.length > 0) {
            $("#Department").html("");
            $.each(json.data, function (i, v) {
                $("#Department").append($('<option value=' + v.cd_ref_num + '>' + v.cd_dept_desc + '</option>'));
            });
            getContactPerson($("#Department").val());
        }
        else {
            $("#Department").html("");
            $("#ContactPerson").html("");
        }
    });
}

function getContactPerson(selectedDepartment) {
    $.get(varSitePath +"/Security/getContactPerson", { selectedDepartment: selectedDepartment }, function (json) {
        $("#ContactPerson").html("");
        $.each(json.data, function (i, v) {
            $("#ContactPerson").append($('<option value=' + v.Value + '>' + v.Contact + '</option>'));
        });
    });
}

function ResetDropdownList() {
    $("#Address").html('');
    $("#Address").append($('<option>' + "--Select--" + '</option>'));
    $("#Department").html('');
    $("#Department").append($('<option>' + "--Select--" + '</option>'));
    $("#ContactPerson").html('');
    $("#ContactPerson").append($('<option>' + "--Select--" + '</option>'));
}

var model = {
    Refundable: '',
    Amount: '',
    BillToDetailModel: {
        IndividualCorporate: '',
        Customer: '',
        NRICFINPASSPORT: '',
        ROCUEN: '',
        Address: '',
        Department: '',
        ContactPerson: ''
    },
    CustomerToAccess: [{
        IndividualCorporate: "",
        Customer: ""
    }]
}

function SaveSecurityDeposit(url) {
    event.stopPropagation();
    if (Validation()) {
        $('#myModal').modal({
            backdrop: 'static',
            keyboard: false
        });

        model = {
            Id: '',
            Refundable: '',
            Amount: '',
            CreatedBy: '',
            CreatedDate: '',
            Status : '',
            BillToDetailModel: {
                IndividualCorporate: '',
                Customer: '',
                NRICFINPASSPORT: '',
                ROCUEN: '',
                Address: '',
                Department: '',
                ContactPerson: ''
            },
            CustomerToAccess: []
        };
        model.Id = $("#Id").val();
        model.Refundable = $("#Refundable").val();
        model.Amount = $("#txtAmount").val();
        var billto1 = {
            IndividualCorporate: $('#ddlIndividualCorporate').val(),
            Customer: $('#customerId').val(),
            NRICFINPASSPORT: $('#txtNRICFINPASSPORT').val(),
            ROCUEN: $('#txtROCUEN').val(),
            Address: $('#Address').val(),
            Department: $('#Department').val(),
            ContactPerson: $('#ContactPerson').val()
        }
        model.CreatedBy = $("#Status").val();
        model.CreatedDate = $("#dtpCreatedDate").val();
        model.BillToDetailModel = billto1;
        model.Status = $("#Status").val();
        $("#tblCustomerToAccessList tbody tr").each(function () {
            if (!$(this).hasClass('activeRow')) {
                var customerToAccess1 = {
                    IndividualCorporate: $(this).find('.Individual_Corporate_Customer_edited').text(),
                    Customer: $(this).find('.customer_edited').data("value")
                };
                model.CustomerToAccess.push(customerToAccess1);
            }
            else {
                var customerToAccess1 = {
                    IndividualCorporate: $(this).find(".Individual_Corporate_Customer").find("option:selected").val(),
                    Customer: $(this).find('.customer').data('id')
                };
                model.CustomerToAccess.push(customerToAccess1);
            }
        });

        $.post(varSitePath +"/Security/InsertSecurityDepositDetail", { json: JSON.stringify(model) }, function (response) {
            $('#myModal').modal("hide");
            if (response.Status === 1) {
                swal(swalGlobal.SwalTitle_Success, response.Message, swalGlobal.SwalType_Success);
                $("#MainView").load(url);
            }
            else {
                if (response.Status === 0) {
                    swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
                }
            }
        })
            .fail(function () {
                $('#myModal').modal("hide");
                swal(swalGlobal.SwalTitle_Error, "Error", swalGlobal.SwalType_Error);
            });
    }
}

function Validation() {
    $(".required").each(function () {
        if ($(this).val() == "" || $(this).val() == "0.00" || $(this).val() == "--Select--") {
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

    if ($('#tblCustomerToAccessList tbody').find('td.dataTables_empty').length > 0)
        $("#tblCustomerToAccessList_wrapper").addClass("alert-danger-table");
    else
        $("#tblCustomerToAccessList_wrapper").removeClass("alert-danger-table");

    if ($(".alert-danger").length > 0 || $('.alert-danger-table').length > 0) {
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
    startTimer();
    resetTimer();
    $("#MainView").load(url);
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
    var Status = "C";
    var Id = $("#Id").val();
    $.post(varSitePath + '/Security/UpdateSecurityDepositStatus', { Id: Id, Status: Status }).done(function (response) {
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