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

function getAddress(selectedCustomer) {
    $.get(varSitePath +"/Security/getAddress", { selected: selectedCustomer, IndividualCorporate: "Individual" }, function (json) {
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
        if (json.data.length > 0) {
            $("#ContactPerson").html("");
            $.each(json.data, function (i, v) {
                $("#ContactPerson").append($('<option value=' + v.Value + '>' + v.Contact + '</option>'));
            });
        }
        else {
            $("#ContactPerson").html("");
        }
    });
}

$(document).ready(function () {

    $(document).click(function () {
        if ($("#txtCustomer").val() === '') {
            ResetDropdownList();
            $("#txtNRICFINPASSPORT").val('');
        }
    });

    $('#txtCustomer').autocomplete({
        source: function (request, response) {
            $.get(varSitePath +"/Security/GetCustomerAutoComplete", { textFilter: request.term, IndividualCorporate: "Individual" }, function (json) {
                response(JSON.parse(json.data));
            });
        },
        minLength: 1,
        scroll: true,
        autoFocus: false,
        autoFill: false,
        select: function (event, ui) {
            $("#Department").html("");
            $("#Address").html("");
            $('#txtCustomer').val(ui.item.label);
            $('#txtCustomer').data('id', ui.item.value);
            $('#customerId').val(ui.item.value);
            $.get(varSitePath +"/Security/getNricFinPassportType", { selected: ui.item.value }, function (json) {
                $('#txtNRICFINPASSPORT').val(json.data);
            });
            getAddress(ui.item.value);
            return false;
        },
        change: function (event, ui) {
            if (!ui.item)
                $('#txtCustomer').val('');
            return false;
        }
    });
});

var model = {
    Id: "",
    Refundable: "",
    GuaranteeBondsType: "",
    Amount: "",
    CreatedBy: "",
    CreatedDate: "",
    Status: "",
    BillToModel: {
        Customer: "",
        NricFinPassport: "",
        Address: "",
        Department: "",
        ContactPerson: ""
    },
    CustomerToAccess: [{
        IndividualCorporate: "",
        Customer: ""
    }]
};

function SaveNewCashandEquivalent(url) {
    event.stopPropagation();
    if (Validation()) {
        $('#myModal').modal({
            backdrop: 'static',
            keyboard: false
        });
        model = {
            Id: "",
            Refundable: "",
            GuaranteeBondsType: "",
            Amount: "",
            CreatedBy: "",
            CreatedDate: "",
            Status: "",
            BillToModel: {
                Customer: "",
                NricFinPassport: "",
                Address: "",
                Department: "",
                ContactPerson: ""
            },
            CustomerToAccess: []
        };
        model.Id = $("#Id").val();
        model.Refundable = $("#Refundable").val();
        model.GuaranteeBondsType = $("#GuaranteeBondsType").val();
        model.Amount = $("#txtAmount").val();
        model.CreatedBy = $("#CreatedBy").val();
        model.CreatedDate = $("#dtpCreatedDate").val();
        model.Status = $("#Status").val();
        var BillTo1 = {
            Customer: $('#customerId').val(),
            NricFinPassport: $('#txtNRICFINPASSPORT').val(),
            Address: $('#Address').val(),
            Department: $('#Department').val(),
            ContactPerson: $('#ContactPerson').val()
        };
        model.BillToModel = BillTo1;
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
        $.post(varSitePath +"/Security/InsertCashEquivalentIndividualDetail", { json: JSON.stringify(model) }, function (response) {
            if (response.Status === 1) {
                swal(swalGlobal.SwalTitle_Success, response.Message, swalGlobal.SwalType_Success);
                $("#MainView").load(url);
            }
            else {
                if (response.Status === 0)
                    swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
            }
        })
            .fail(function (ex) {
                $('#myModal').modal("hide");
                swal(swalGlobal.SwalTitle_Error, "Error", swalGlobal.SwalType_Error);
            });
    }
}

function Validation() {
    $(".required").each(function () {
        if ($(this).val() === "" || $(this).val() === "0.00" || $(this).val() === "--Select--") {
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

function ResetDropdownList() {
    $("#Address").html('');
    $("#Address").append($('<option>' + "--Select--" + '</option>'));
    $("#Department").html('');
    $("#Department").append($('<option>' + "--Select--" + '</option>'));
    $("#ContactPerson").html('');
    $("#ContactPerson").append($('<option>' + "--Select--" + '</option>'));
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
} function UpdateStatus(url) {
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
    $.post(varSitePath + '/Security/UpdateCashAndEquivalentIndStatus', { Id: Id, Status: Status }).done(function (response) {
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