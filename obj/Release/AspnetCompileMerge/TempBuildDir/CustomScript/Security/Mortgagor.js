var Individual_Corporate = [];
var Main_Secondary = [];
var Mortgagor = [];
var NricFinPassport;
var RocUen;
var Department = [];
var Contact_Person = [];
var Address = [];
var isMainDuplicate = false;
var IsCustomer = false;
var i = 0;
var j = 0;
var newRow = {
    "Action": "<button type='button' tabindex='1' class='btn deleting btnAction' >Delete</button>", "Individual_Corporate": "", "Mortgagor": "", "Main_Secondary": "", "NRIC_FIN_PASSPORT": "", "ROC_UEN": "", "Address": "", "Department": "", "Contact_Person": ""
};
var tblMortgagorList;
var IsEditView = false;
function getAddress(selected, IndividualCorporate, el) {
    if (selected) {
        $.ajax({
            type: 'GET',
            url: 'getAddress',
            dataType: 'json',
            data: { selected: selected, IndividualCorporate: IndividualCorporate },
            success: function (response) {
                Address = response.data;
                if (el) {
                    var ddlAddress = $(el).closest("tr").find(".address");
                    $(ddlAddress).empty();
                    $.each(Address, function (i, v) {
                        $(ddlAddress).append("<option value=" + v.AddressId + ">" + v.Address + "</option>");
                    });
                    getDepartmentList($(ddlAddress).val(), el);
                }
                else {
                    getDepartmentList(Address[0].AddressId, el);
                }
            },
            error: function (ex) {
                swal(swalGlobal.SwalTitle_Error, "Error Occured whiling loading Address", swalGlobal.SwalType_Error);
                return [];
            }
        });
    }
    else {
        $(el).closest("tr").find(".address").empty();
        $(el).closest("tr").find(".department").empty();
        $(el).closest("tr").find(".contact_Person").empty();
    }
};

function getRocUenType(selected, el) {

    if (selected) {
        $.ajax({
            type: 'GET',
            url: 'getRocUenType',
            dataType: 'json',
            data: { selected: selected },
            success: function (response) {
                RocUen = response.data;
                if (el) {
                    var txtRocUen = $(el).closest("tr").find(".RocUen");
                    $(txtRocUen.val()).empty();
                    $(txtRocUen).val(RocUen);

                }
            },
            error: function (ex) {
                swal(swalGlobal.SwalTitle_Error, "Error Occured whiling loading Roc/ UEN", swalGlobal.SwalType_Error);
                return [];
            }
        });
    }
    else {
        $(el).closest("tr").find(".RocUen").empty();
    }
};

function GetDefaultValue(selected, el) {
    getNricFinPassportType(selected, el);
    getRocUenType(selected, el);
    if (el) {
        var IndividualCorporate = $(el).closest("tr").find(".Individual_Corporate").val();
        getAddress(selected, IndividualCorporate, el);
    }
    else {
        getAddress(selected, "Individual");
    }
};

function bindDropdown(selectedAddress, el) {
    getDepartmentList(selectedAddress, el);

};

function setContactPerson(selectedDepartment, el) {
    getContactPerson(selectedDepartment, el);
};

function getNricFinPassportType(selected, el) {
    if (selected) {
        $.ajax({
            type: 'GET',
            url: 'getNricFinPassportType',
            dataType: 'json',
            data: { selected: selected },
            success: function (response) {
                NricFinPassport = response.data;
                if (el) {
                    var txtNricFinPassport = $(el).closest("tr").find(".NricFinPassport");
                    $(txtNricFinPassport.val()).empty();
                    $(txtNricFinPassport).val(NricFinPassport);
                }
            },
            error: function (ex) {
                swal(swalGlobal.SwalTitle_Error, "Error Occured whiling loading NRIC/ FIN/ PASSPORT", swalGlobal.SwalType_Error);
                return [];
            }
        });
    }
    else {
        $(el).closest("tr").find(".NricFinPassport").empty();
    }
};

function getMortgagor(type, el) {
    var txtCustomer = $(el).closest("tr").find(".mortgagor");
    var activeRow = $("#tblMortgagorList tr.activeRow");

    $(txtCustomer).val('');
    $(txtCustomer).data('id', '');
    activeRow.find(".NricFinPassport").val('');
    activeRow.find('.NricFinPassport').empty();
    activeRow.find(".RocUen").val('');
    activeRow.find('.RocUen').empty();
    activeRow.find('.address').empty();
    activeRow.find('.department').empty();
    activeRow.find('.contact_Person').empty();
};

function getDepartmentList(selectedAddress, el) {
    $.ajax({
        type: 'GET',
        url: 'getDepartmentList',
        dataType: 'json',
        data: { selectedAddress: selectedAddress },
        success: function (response) {
            if (el) {
                var ddlDepartment = $(el).closest("tr").find(".department");
                $(ddlDepartment).empty();
                $.each(response.data, function (i, v) {
                    $(ddlDepartment).append("<option value=" + v.cd_ref_num + ">" + v.cd_dept_desc + "</option>");
                });
                if (response.data.length > 0)
                    getContactPerson(response.data[0].cd_ref_num, el);
                else
                    $(el).closest("tr").find(".contact_Person").empty();
            }
            else {
                Department = response.data;
                getContactPerson(Department[0].cd_ref_num);
            }
        },
        error: function (ex) {
            swal(swalGlobal.SwalTitle_Error, "Error Occured whiling loading Departments", swalGlobal.SwalType_Error);
            return [];
        }
    });
};

function getContactPerson(selectedDepartment, el) {
    $.ajax({
        type: 'GET',
        url: 'getContactPerson',
        dataType: 'json',
        data: { selectedDepartment: selectedDepartment },
        success: function (response) {
            Contact_Person = response.data;
            if (el) {
                var ddlContact = $(el).closest("tr").find(".contact_Person");
                $(ddlContact).empty();
                $.each(Contact_Person, function (i, v) {
                    $(ddlContact).append("<option value=" + v.Value + ">" + v.Contact + "</option>");
                });
            }
        },
        error: function (ex) {
            swal(swalGlobal.SwalTitle_Error, "Error Occured whiling loading Contact Person", swalGlobal.SwalType_Error);
            return [];
        }
    });
};

function targetRowEvent(event) {
    if (!$(event.target).closest('#tblMortgagorList tbody tr.activeRow').length &&
        !$(event.target).closest('#tblMortgagorList thead').length &&
        !$(event.target).closest('.sweet-alert').length &&
        !$(event.target).closest('#SaveInventory').length &&
        !$(event.target).closest('.ui-autocomplete').length &&
        !$(event.target).closest('.select2-container').length &&
        !$(event.target).closest('#btnAddMortgagorList').length &&
        !$(event.target).closest('#btnAddInsurance').length &&
        !$(event.target).closest('#btnCustomerToAccessList').length &&
        !$(event.target).closest('button.deleting').length) {
        if (!isRowChanged()) {
            $('#tblMortgagorList tr.activeRow').find('button.deleting').click();
        }
        else if (isValidRow() && !IsMortgagor && !isMainDuplicate) {
            setActiveRow(null);
        }
    }
};

function setActiveRow(row) {
    $("#tblMortgagorList tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                $(this).next('span').html('');

                var DropText = '';
                if ($(this).hasClass('js-example-basic-single4')) {
                    DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                    $(this).next().remove();

                }
                if ($(this).val() == "" && !($(this).hasClass("unrequired")) && !($(this).hasClass("deleting"))) {
                    isValid = false;
                    $(this).addClass("alert-danger");
                    $(this).next('span').empty();
                }
                else {
                    if ($(this).hasClass('js-example-basic-single4')) {

                        $(this).next('span').html($(this).find("option:selected").text());
                        $(this).next('span').data("value", $(this).val())
                    }
                    else {
                        $(this).next('span').html($(this).val());
                        if ($(this).hasClass('mortgagor')) {
                            $(this).next('span').data("value", $(this).data('id'));
                        }
                    }
                }
            });
            $(this).removeClass("activeRow");
        }
    });


    if (row != null)
        $(row).addClass('activeRow');

    $('.activeRow .mortgagor').autocomplete({
        source: function (request, response) {
            var indVal = $('#tblMortgagorList tr.activeRow').find('.Individual_Corporate').val();
            $.ajax({
                url: 'GetMortgagorAutoComplete',
                type: 'GET',
                cache: false,
                dataType: 'json',
                data: { textFilter: request.term, IndividualCorporate: indVal },
                global: false,
                success: function (json) {
                    $(json.data).each(function (i, v) {
                        v.label = v.label.replace("&#39", '\'').replace("&#34", "\"").replace("&#12296", "<").replace("&#12297", ">");
                    });
                    response(json.data);
                }
            });
        },
        minLength: 1,
        scroll: true,
        autoFocus: false,
        autoFill: false,
        focus: function (event, ui) {
            return false;
        },
        select: function (event, ui) {

            var el = $('.activeRow .mortgagor');
            el.val(ui.item.label);
            el.data('id', ui.item.value);
            GetDefaultValue($(el).data("id"), el);
            return false;
        },
        change: function (event, ui) {
            if (!ui.item) {
                var el = $('.activeRow .mortgagor');
                $('.activeRow .mortgagor').val('');
                $('.activeRow .mortgagor').data('id', '');
                GetDefaultValue($(el).data("id"), el);
            }
            return false;
        }
    });

    $('.activeRow .js-example-basic-single4').select2();

};

function initializeNewRow() {

    if (Individual_Corporate) {
        var strIndividual_Corporate = "<select class='editor select Individual_Corporate js-example-basic-single4' tabindex='2' style='width:130px;' onchange='getMortgagor($(this).val(),this)'>";
        strIndividual_Corporate += "<option selected='true'>" + "Individual" + "</option>";
        strIndividual_Corporate += "<option>" + "Corporate" + "</option>";
        strIndividual_Corporate += "</select><span class='edited Individual_Corporate_edited'></span>";
        newRow.Individual_Corporate = strIndividual_Corporate;
    }
    else {
        newRow.Individual_Corporate = "<select class='editor select Individual_Corporate js-example-basic-single4' tabindex='2'></select><span class='edited Individual_Corporate_edited'></span>";
    }
    newRow.Mortgagor = "<input style='width:200px !important' class='editor mortgagor required' type='text' tabindex='3' value=''><span data-value='' class='edited mortgagorName' style='width:200px;'></span><span class='ItemNumber' style='display:none'>0</span>";
    if (Main_Secondary) {
        strMain_Secondary = "<select class='editor select Main_Secondary js-example-basic-single4' tabindex='4'></span>";
        strMain_Secondary += "<option>" + "Main" + "</option>";
        strMain_Secondary += "<option>" + "Secondary" + "</option>";
        strMain_Secondary += "</select><span class='edited checkMain'></span>";
        newRow.Main_Secondary = strMain_Secondary;
    }
    else {
        newRow.Main_Secondary = "<select class='editor' tabindex='4'></select><span class='edited checkMain'></span>";
    }
    newRow.NRIC_FIN_PASSPORT = "<input class='editor NricFinPassport unrequired' type='text' disabled tabindex='5' value=''><span class='edited NricType_edited'></span>";

    newRow.ROC_UEN = "<input class='editor RocUen unrequired tabindex='5' style='width:80px;' type='text' disabled value=''><span class='edited RocUen_edited' style='width:80px;'></span>";
    Address = [];
    if (Address) {
        var strAddress = "<select tabindex='7' class='editor select address required js-example-basic-single4 ddlAddress' onchange='bindDropdown($(this).val(),this)'>";
        $.each(Address, function (i, v) {
            strAddress += "<option value='" + v.AddressId + "'>" + v.Address + "</option>";
        })
        strAddress += "</select><span data-value='' class='edited Address_edited' style='width:300px;'></span>";
        newRow.Address = strAddress;
    }
    else {
        newRow.Address = "<select class='editor Address_edited required' tabindex='7'></select><span class='edited'></span>";
    }
    if (Department) {
        var strDepartment = "<select tabindex='8' class='editor department js-example-basic-single4 required ddlDepartment' onchange='setContactPerson($(this).val(),this)'>";
        $.each(Department, function (i, v) {
            strDepartment += "<option value='" + v.cd_ref_num + "'>" + v.cd_dept_desc + "</option>";
        })
        strDepartment += "</select><span data-value='' class='edited Department_edited'></span>";
        newRow.Department = strDepartment;

    }
    else {
        newRow.Department = "<select class='editor Department_edited required' tabindex='8'></select><span class='edited'></span>";
    }
    Contact_Person = [];
    if (Contact_Person) {
        var strContact_Person = "<select tabindex='9' class='editor contact_Person js-example-basic-single4 required'>";
        $.each(Contact_Person, function (i, v) {
            strContact_Person += "<option value= '" + v.Value + "'>" + v.Contact + "</option>";
        })
        strContact_Person += "</select><span data-value='' class='edited ContactPerson_edited' style='width:200px;'></span>";
        newRow.Contact_Person = strContact_Person;
    }
    else {
        newRow.Contact_Person = "<select class='editor' tabindex='9'></select><span class='edited ContactPerson_edited required'></span>";
    }
};

function initializeNewRowUpdate(mortgagor) {

    if (mortgagor.length > 0) {

        $.each(mortgagor, function (key, value) {

            if (Individual_Corporate) {
                var strIndividual_Corporate = "<select class='editor select Individual_Corporate js-example-basic-single4' tabindex='2' style='width:130px;' onchange='getMortgagor($(this).val(),this)'>";
                if (value.IndividualCorporate === 'Individual') {
                    strIndividual_Corporate += "<option selected='true'>" + "Individual" + "</option>";
                    strIndividual_Corporate += "<option>" + "Corporate" + "</option>";
                }
                else {
                    strIndividual_Corporate += "<option>" + "Individual" + "</option>";
                    strIndividual_Corporate += "<option selected='true'>" + "Corporate" + "</option>";
                }
                strIndividual_Corporate += "</select><span class='edited Individual_Corporate_edited'>" + value.IndividualCorporate + "</span>";
                newRow.Individual_Corporate = strIndividual_Corporate;
            }
            else {
                newRow.Individual_Corporate = "<select class='editor select Individual_Corporate js-example-basic-single4' tabindex='2'></select><span class='edited Individual_Corporate_edited'>" + value.IndividualCorporate + "</span>";
            }
            newRow.Mortgagor = "<input style='width:200px !important' class='editor mortgagor required' type='text' tabindex='3' value='" + value.MortgagorDisplay + "'><span data-value='" + value.Mortgagor + "' class='edited mortgagorName' style='width:200px;'>" + value.MortgagorDisplay + "</span><span class='ItemNumber' style='display:none'>" + value.ItemNumber +"</span>";
            if (Main_Secondary) {
                strMain_Secondary = "<select class='editor select Main_Secondary js-example-basic-single4' tabindex='4'></span>";
                if (value.MainDisplay === 'Main') {
                    strMain_Secondary += "<option selected='true'>" + "Main" + "</option>";
                    strMain_Secondary += "<option>" + "Secondary" + "</option>";
                }
                else {
                    strMain_Secondary += "<option>" + "Main" + "</option>";
                    strMain_Secondary += "<option selected='true'>" + "Secondary" + "</option>";
                }
                strMain_Secondary += "</select><span class='edited checkMain'>" + value.MainDisplay + "</span>";
                newRow.Main_Secondary = strMain_Secondary;
            }
            else {
                newRow.Main_Secondary = "<select class='editor' tabindex='4'></select><span class='edited checkMain'>" + value.MainDisplay + "</span>";
            }
            newRow.NRIC_FIN_PASSPORT = "<input class='editor NricFinPassport unrequired NricFinPassport' type='text' disabled tabindex='5' value='" + value.NRICType + "'><span class='edited NricType_edited'>" + value.NRICType + "</span>";

            newRow.ROC_UEN = "<input class='editor RocUen unrequired tabindex='5' style='width:80px;' type='text' disabled value='" + value.ROCType + "'><span class='edited RocUen_edited' style='width:80px;'>" + value.ROCType + "</span>";

            if (Address) {
                var strAddress = "<select tabindex='7' class='editor select address required js-example-basic-single4 ddlAddress' onchange='bindDropdown('" + value.Mortgagor + "',this)'>";
                $.each(Address, function (i, v) {
                    strAddress += "<option value='" + v.AddressId + "'>" + v.Address + "</option>";
                })
                strAddress += "</select><span data-value='" + value.Address + "' class='edited Address_edited' style='width:300px;'>" + value.AddressDisplay + "</span>";
                newRow.Address = strAddress;
            }
            else {
                newRow.Address = "<select class='editor Address_edited required' tabindex='7'></select><span class='edited'>" + value.AddressDisplay + "</span>";
            }
            if (Department) {
                var strDepartment = "<select tabindex='8' class='editor department js-example-basic-single4 required ddlDepartment' onchange='setContactPerson('" + value.Department + "',this)'>";
                $.each(Department, function (i, v) {
                    strDepartment += "<option value='" + v.cd_ref_num + "'>" + v.cd_dept_desc + "</option>";
                })
                strDepartment += "</select><span data-value='" + value.Department + "' class='edited Department_edited'>" + value.DepartmentDisplay + "</span>";
                newRow.Department = strDepartment;

            }
            else {
                newRow.Department = "<select class='editor Department_edited required' tabindex='8'></select><span class='edited'>" + value.DepartmentDisplay + "</span>";
            }

            if (Contact_Person) {
                var strContact_Person = "<select tabindex='9' class='editor contact_Person js-example-basic-single4 required'>";
                $.each(Contact_Person, function (i, v) {
                    strContact_Person += "<option value= '" + v.Value + "'>" + v.Contact + "</option>";
                })
                strContact_Person += "</select><span data-value='" + value.ContactPerson + "' class='edited ContactPerson_edited' style='width:200px;'>" + value.ContactPersonDisplay + "</span>";
                newRow.Contact_Person = strContact_Person;
            }
            else {
                newRow.Contact_Person = "<select class='editor' tabindex='9'></select><span class='edited ContactPerson_edited required'>" + value.ContactPersonDisplay + "</span>";
            }

            var _row = tblMortgagorList.rows.add([[newRow.Action, newRow.Individual_Corporate, newRow.Mortgagor, newRow.Main_Secondary, newRow.NRIC_FIN_PASSPORT, newRow.ROC_UEN, newRow.Address, newRow.Department, newRow.Contact_Person]]);
            _row.draw();

            var el = $("#tblMortgagorList tr:last");
            el.val(value.MortgagorDisplay);
            el.data('id', value.Mortgagor);
            GetDefaultValue(value.Mortgagor, el);
        });
        $("#mortgatorDetails").removeClass("alert-danger-table");
    };
};


function isRowChanged() {
    let activeRow = $("#tblMortgagorList tr.activeRow");
    let isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass('unrequired')) {
                if ($(this).val() != '')
                    isValid = true;
            }
        });
        if (!isValid) {
            if ($(activeRow[0]).find(':input.NricFinPassport').val() == ''
                && $(activeRow[0]).find('.Main_Secondary').val() == ''
                && $(activeRow[0]).find('.address').val() == ''
                && $(activeRow[0]).find(':input.RocUen').val() == ''
                && $(activeRow[0]).find('.department').val() == ''
                && $(activeRow[0]).find('.contact_Person').val() == ''
                && $(activeRow[0]).find('.Individual_Corporate').val() == ''
                && $(activeRow[0]).find('.mortgagor').val() == ''
            )
                isValid = false;
            else
                isValid = true;
        }
    }
    else { return true; }
    return isValid;
};

$(document).ready(function () {

    $('.js-example-basic-single3').select2();

    //    targetRowEvent(event);

    //tblMortgagorList = $("#tblMortgagorList").DataTable({
    //    "paging": false,
    //    "info": false,
    //    "searching": false,
    //    "language": {
    //        "zeroRecords": "No Data Available.",
    //        "infoEmpty": "No Data Available."
    //    },
    //    "dom": '<"top"i>rt<"bottom"flp><"clear">'
    //});

    $(document).on('click', function (event) {
        targetRowEvent(event);
    });

    if (!IsEditView) {
        $("#tblMortgagorList tbody").on('click', 'tr', function (event) {
            if (!($(this).hasClass("activeRow")) && isValidRow()) {
                if (!isMainDuplicate && !IsMortgagor) {
                    setActiveRow(this);
                    //setActiveRow(this);
                }
            }
        });
    }

    // setActiveRow();

    //  initializeNewRow();

    $("#btnAddMortgagorList").click(function () {
        if (($.isFunction(isValidInsuranceRow) ? isValidInsuranceRow() : true) && ($.isFunction(isValidCustomerRow) ? isValidCustomerRow() : true)) {
            i++;
            if (tblMortgagorList.rows().count() === 0) {
                initializeNewRow();
            }

            if (tblMortgagorList.rows().count() == 0 || (isValidRow() && !isMainDuplicate && !IsMortgagor)) {

                initializeNewRow();
                var _row = tblMortgagorList.rows.add([[newRow.Action, newRow.Individual_Corporate, newRow.Mortgagor, newRow.Main_Secondary, newRow.NRIC_FIN_PASSPORT, newRow.ROC_UEN, newRow.Address, newRow.Department, newRow.Contact_Person]]);
                _row.draw();
                setActiveRow($("#tblMortgagorList tr:last"));
                $("#mortgatorDetails").removeClass("alert-danger-table");
            }
        }
    });


    $('#tblMortgagorList tbody').on('click', 'button.deleting', function () {
        IsMortgagor = false;
        isMainDuplicate = false;
        Individual_Corporate = [];
        Main_Secondary = [];
        Mortgagor = [];
        NricFinPassport;
        RocUen;
        Department = [];
        Contact_Person = [];
        Address = [];
        tblMortgagorList.row($(this).parents("tr")).remove().draw();
    });

    // isRowChanged();

    $('body').on('click', '.ui-state-default', function () {

        event.stopPropagation();
    });

});

function isValidInput(input) {
    if (!($(input).hasClass("deleting"))) {
        if ($(input).val() == "" && !$(input).hasClass('unrequired') && !$(input).parents('td').hasClass('hideColumn')) {
            $(input).addClass("alert-danger");
        }
        else {
            $(input).removeClass("alert-danger");
        }
    }
};

$(document).on('blur', '.money', function () {
    let val = $(this).val();
    if (val == '' || val == '0') val = '0.00';
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
            val = '0.00';
        }
    }
    if (val.indexOf(".") == -1 && val != '') {
        $(this).val(val + '.00');
    }
    else
        $(this).val(val);
});

function CheckMainMortgagor() {
    var ismain = false;
    $("#tblMortgagorList tbody tr").each(function () {
        var ddlmain = $(this).find('select.Main_Secondary').val();
        if (ddlmain == 'Main')
            ismain = true;
    })
    if (!ismain)
        swal(swalGlobal.SwalTitle_Error, "Must have one record with Main/Secondary = Main", swalGlobal.SwalType_Error);
    return ismain;
}