function getEquipmentModel(equipmentId) {
    $.post(varSitePath +'/Security/GetModelByBrand', { code: equipmentId }, function (result) {
        $("#Equipment_Model").html("");
        $("#txtEquipmentDescription").val('');
        $.each(result, function (i, v) {
            var model = v.label.replace(/\s/g, "&nbsp;");
            $("#Equipment_Model").append('<option value=' + v.value + '>' + model + '</option>');
        });
        var desc = "";
        if ($("#Equipment_Brand option:selected").text().trim() !== '--Select--') {
            desc = $("#Equipment_Brand option:selected").text().trim() + ' ' + $("#Equipment_Model option:selected").text().trim();
        }
        $("#txtEquipmentDescription").val(desc);
    })
        .fail(function (ex) {
            $('#myModal').modal("hide");
            swal(swalGlobal.SwalTitle_Error, "Some Error Occured while loading Vehicle Model!", swalGlobal.SwalType_Error);
        });
}

function getEquipmentDesc() {
    var desc = $("#Equipment_Brand option:selected").text().trim() + ' ' + $("#Equipment_Model option:selected").text().trim();
    $("#txtEquipmentDescription").val(desc);
}
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
    GetYears();
    var year = $("#YearofManufacture").val();
    $('.js-example-basic-single5').select2();

    $('#ManufactureYear').on('click', function () {
        if (year === undefined || year === "") {
            $('#select2-ManufactureYear-container').text($(this).text());
        }
        else {
            $('#select2-ManufactureYear-container').text(year);
        }
    });

    $("#Reset").on('click', function () {
        var yy = new Date();
        $("#txtEquipmentDescription").val("");
        $('#ManufactureYear [value=' + yy.getFullYear() + ']').attr('selected', 'selected');
        $('#ManufactureYear').val(yy.getFullYear());
        $('#select2-ManufactureYear-container').empty();
        $("#Equipment_Brand").parent().find("span.select2-selection__rendered").text("--Select--");
        $("#Equipment_Brand").prop("selectedIndex", 0);
        $("#Equipment_Model option").remove();
        $("#Equipment_Model").append($('<option>' + "--Select--" + '</option>'));
        $("#txtSerialNumber").val("");
        $("#txtEngineNumber").val("");
        $("#txtSecuredAmount").val("0.00");
        $("#txtChargeDate").val("");
        $("#txtChargeNumber").val("");
        $("#tblCustomerToAccessList tbody tr").each(function () {
            $(this).find('button.deleting').click();
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
    
    if (year === undefined || year === "") {
        $("#ManufactureYear").next("span").find("span.select2-selection__rendered").text('');
    }
    else {
        $("#ManufactureYear").next("span").find("span.select2-selection__rendered").text(year);
    }

    $("#ManufactureYear").next("span").find("span.select2-selection").click(function () {
        if (year === undefined || year === "") {
            var selectedRear = $("#ManufactureYear").children("option").filter(":selected").text();
            $("#ManufactureYear").next("span").find("span.select2-selection__rendered").text(selectedRear);
        }
        else {
            $("#ManufactureYear").next("span").find("span.select2-selection__rendered").text(year);
        }
    });
});

function GetYears() {
    var d = new Date();
    var y = parseInt(d.getFullYear() - 100);
    for (var i = 0; i <= 200; i++) {
        if (parseInt(y + i) <= d.getFullYear()) {
            $("#ManufactureYear").append($('<option value=' + parseInt(y + i) + '>' + parseInt(y + i) + '</option>'));
        }
        else {
            $("#ManufactureYear").append($('<option value=' + parseInt(y + i) + '>' + parseInt(y + i) + '</option>').attr("disabled", "disabled"));
        }
    }
    var year = $("#YearofManufacture").val();
    if (year === undefined || year === "") {
        $('#ManufactureYear [value=' + d.getFullYear() + ']').attr('selected', 'selected');
    }
    else {
        $('#ManufactureYear [value=' + year + ']').attr('selected', 'selected');
    }
}

var model = {
    Id: "",
    Brand: "",
    Model: "",
    EquipmentDescription: "",
    Status: "",
    CreatedBy: "",
    CreatedDate: "",
    EquipmentDetails: {
        SerialNumber: "",
        SecuredAmount: "",
        ManufactureYear: "",
        EngineNumber: "",
        ChargeNumber: "",
        ChargeDate: ""
    },
    CustomerToSccess: [{
        IndividualCorporate: "",
        Customer: ""
    }]
};
function SaveNewConstructionEqu(url) {
    if (Validation()) {
        $('#myModal').modal({
            backdrop: 'static',
            keyboard: false
        });
        var model = {
            Id: "",
            Brand: "",
            Model: "",
            EquipmentDescription: "",
            Status: "",
            CreatedBy: "",
            CreatedDate: "",
            EquipmentDetails: {
                SerialNumber: "",
                SecuredAmount: "",
                ManufactureYear: "",
                EngineNumber: "",
                ChargeNumber: "",
                ChargeDate: ""
            },
            CustomerToAccess: []
        };
        model.Id = $("#txtEquipmentID").val();
        model.Brand = $("#Equipment_Brand").val();
        model.Model = $("#Equipment_Model").val();
        model.EquipmentDescription = $("#txtEquipmentDescription").val();
        model.CreatedBy = $("#CreatedBy").val();
        model.CreatedDate = $("#dtpCreatedDate").val();
        model.Status = $("#Status").val();
        var Equipment1 = {
            SerialNumber: $('.SerialNumber').val(),
            SecuredAmount: $('.SecuredAmount').val(),
            ManufactureYear: $("#ManufactureYear").next("span").find("span.select2-selection__rendered").text(),
            EngineNumber: $('.Engine_Number').val(),
            ChargeNumber: $('.Charge_Number').val(),
            ChargeDate: $('.chargedate').val()
        };
        model.EquipmentDetails = Equipment1;
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

        $.post(varSitePath +"/Security/InsertConstructionEquipDetails", { json: JSON.stringify(model) }, function (response) {
            $('#myModal').modal("hide");
            if (response.Status === 1) {
                swal({
                    title: swalGlobal.SwalTitle_Success,
                    text: response.Message,
                    type: swalGlobal.SwalType_Success
                }, function () {
                    if (url === undefined) {
                        $("#Reset").click();
                    }
                    else {
                        window.location.href = url;
                    }
                });
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


function SaveNewIndustrialEqu(url) {
    if (Validation()) {
        $('#myModal').modal({
            backdrop: 'static',
            keyboard: false
        });
        var model = {
            Id: "",
            Brand: "",
            Model: "",
            EquipmentDescription: "",
            Status: "",
            CreatedBy: "",
            CreatedDate: "",
            EquipmentDetails: {
                SerialNumber: "",
                SecuredAmount: "",
                ManufactureYear: "",
                EngineNumber: "",
                ChargeNumber: "",
                ChargeDate: ""
            },
            CustomerToAccess: []
        };
        model.Id = $("#txtEquipmentID").val();
        model.Brand = $("#Equipment_Brand").val();
        model.Model = $("#Equipment_Model").val();
        model.EquipmentDescription = $("#txtEquipmentDescription").val();
        model.CreatedBy = $("#CreatedBy").val();
        model.CreatedDate = $("#dtpCreatedDate").val();
        model.Status = $("#Status").val();
        var Equipment1 = {
            SerialNumber: $('.SerialNumber').val(),
            SecuredAmount: $('.SecuredAmount').val(),
            ManufactureYear: $('.YearofManufacture').val(),
            EngineNumber: $('.Engine_Number').val(),
            ChargeNumber: $('.Charge_Number').val(),
            ChargeDate: $('.chargedate').val()
        };
        model.EquipmentDetails = Equipment1;
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
        $.post(varSitePath +"/Security/InsertIndustrialEquipDetails", { json: JSON.stringify(model) }, function (response) {
            $('#myModal').modal("hide");
            if (response.Status === 1) {
                swal({
                    title: swalGlobal.SwalTitle_Success,
                    text: response.Message,
                    type: swalGlobal.SwalType_Success
                }, function () {
                    if (url === undefined) {
                        $("#Reset").click();
                    }
                    else {
                        window.location.href = url;
                    }
                });
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

function UpdateIndustrialEquStatus(url) {
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
                UpdateIndustrialStatus(url);
            }, 1000);
        }
    });
}

function UpdateConstructionEquStatus(url) {
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
                UpdateConstructionStatus(url);
            }, 1000);
        }
    });
}

function UpdateConstructionStatus(url) {
    var Status = "";
    var Id = $("#txtEquipmentID").val();
    if ($("#txtChargeDate").val() === "") {
        Status = "C";
    }
    else {
        Status = "D";
    }
    $.post(varSitePath +'/Security/UpdateConstructionEquStatus', { Id: Id, Status: Status }).done(function (response) {
        if (response.Status === 1) {
            swal({
                title: swalGlobal.SwalTitle_Success,
                text: response.Message,
                type: swalGlobal.SwalType_Success
            }, function () {
                window.location.href = url;
            });
        }
        else {
            if (response.Status === 2)
                swal(swalGlobal.SwalTitle_Error, response.Message, swalGlobal.SwalType_Error);
        }
    });
}

function UpdateIndustrialStatus(url) {
    var Status = "";
    var Id = $("#txtEquipmentID").val();
    if ($("#txtChargeDate").val() === "") {
        Status = "C";
    }
    else {
        Status = "D";
    }
    $.post(varSitePath +'/Security/UpdateIndustrialEquStatus', { Id: Id, Status: Status }).done(function (response) {
        if (response.Status === 1) {
            swal({
                title: swalGlobal.SwalTitle_Success,
                text: response.Message,
                type: swalGlobal.SwalType_Success
            }, function () {
                window.location.href = url;
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
        if ($(this).val() === "" || $(this).val() == 0.00 || $(this).val() === null) {
            $(this).addClass('alert-danger');
            if ($(this).is("select")) {
                $(this).parent().find("span.select2-selection__rendered").text("--Select--");
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

function ResetDebentureView(url) {
    $('#myModal').modal({
        backdrop: 'static',
        keyboard: false
    });
    startTimer();
    resetTimer();
    window.location.href = url;
}
