var Individual_Corporate_Customer = [];
var Customer = [];
var i = 0;
var j = 0;
var customerNewRow = {
    "Action": "<button type='button' tabindex='1' class='btnAction btn deleting' >Delete</button>", "Individual_Corporate_Customer": "", "Customer": "", "NRICFINPASSPORTCust": "", "UENNOCust": ""
};
var tblCustomerToAccessList;
var NricFinPassportCust;
var RocUenCust;

function getRocUenTypeCust(selected, el) {

    if (selected) {
        $.ajax({
            type: 'GET',
            url: 'getRocUenType',
            dataType: 'json',
            data: { selected: selected },
            success: function (response) {
                RocUenCust = response.data;
                if (el) {
                    var txtRocUenCust = $(el).closest("tr").find(".UENNOCust");
                    $(txtRocUenCust.val()).empty();
                    $(txtRocUenCust).val(RocUenCust);
                }
            },
            error: function (ex) {
                swal(swalGlobal.SwalTitle_Error, "Error Occured whiling loading Roc/ UEN", swalGlobal.SwalType_Error);
                return [];
            }
        });
    }
    else {
        $(el).closest("tr").find(".UENNOCust").empty();
    }
};

function GetDefaultValueCust(selected, el) {
    getNricFinPassportTypeCust(selected, el);
    getRocUenTypeCust(selected, el);
};

function getNricFinPassportTypeCust(selected, el) {
    if (selected) {
        $.ajax({
            type: 'GET',
            url: 'getNricFinPassportType',
            dataType: 'json',
            data: { selected: selected },
            success: function (response) {
                NricFinPassportCust = response.data;
                if (el) {
                    var txtNricFinPassportCust = $(el).closest("tr").find(".NRICFINPASSPORTCust");
                    $(txtNricFinPassportCust.val()).empty();
                    $(txtNricFinPassportCust).val(NricFinPassportCust);
                }
            },
            error: function (ex) {
                swal(swalGlobal.SwalTitle_Error, "Error Occured whiling loading NRIC/ FIN/ PASSPORT", swalGlobal.SwalType_Error);
                return [];
            }
        });
    }
    else {
        $(el).closest("tr").find(".NRICFINPASSPORTCust").empty();
    }
};
function targetCustomerToAccessRowEvent(event) {
    if (!$(event.target).closest('#tblCustomerToAccessList tbody tr.activeRow').length &&
        !$(event.target).closest('#tblCustomerToAccessList thead').length &&
        !$(event.target).closest('.sweet-alert').length &&
        !$(event.target).closest('#SaveInventory').length &&
        !$(event.target).closest('.ui-autocomplete').length &&
        !$(event.target).closest('.select2-container').length &&
        !$(event.target).closest('#btnAddMortgagorList').length &&
        !$(event.target).closest('#btnAddInsurance').length &&
        !$(event.target).closest('#btnCustomerToAccessList').length &&
        !$(event.target).closest('button.deleting').length) {
        if (!isCustomerRowChanged()) {
            $('#tblCustomerToAccessList tr.activeRow').find('button.deleting').click();
        }
        else if (!IsDuplicateCustomer() && isValidCustomerRow()) {
            setCustomerActiveRow(null);
        }
    }
}

function setCustomerActiveRow(row) {    
    $("#tblCustomerToAccessList tr").each(function () {
        if ($(this).hasClass('activeRow')) {
            $(this).find(":input").each(function () {
                $(this).next('span').html('');

                var DropText = '';
                if ($(this).hasClass('js-example-basic-single2')) {
                    DropText = $(this).next('span.select2-container').find('.select2-selection__rendered').text();
                    $(this).next().remove();
                }
          if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && $(this).val() == "") {
                    $(this).addClass("alert-danger");
                    $(this).next('span').empty();
                }
                else {
                    if ($(this).hasClass('js-example-basic-single2')) {

                        $(this).next('span').html($(this).find("option:selected").text());
                        $(this).next('span').data("value", $(this).val());
                    }
                    else {
                        $(this).next('span').html($(this).val());
                        if ($(this).hasClass('customer')) {
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

    $('.activeRow .customer').autocomplete({
        source: function (request, response) {
            var indVal = $('#tblCustomerToAccessList tr.activeRow').find('.Individual_Corporate_Customer').val();
            $.ajax({
                url: 'GetCustomerAutoComplete',
                data: { textFilter: request.term, IndividualCorporate: indVal },
                type: 'GET',
                cache: false,
                dataType: 'json',
                global: false,
                success: function (json) {
              var res = JSON.parse(json.data);
              $(res).each(function (i, v) {
                  v.label = v.label.replace("&#39", '\'').replace("&#34", "\"").replace("&#12296", "<").replace("&#12297", ">");
              });
              response(res);
                }
            });
        },
        minLength: 1,
        scroll: true,
        autoFocus: false,
        autoFill: false,
        focus: function (event, ui) {
          //Amended By Jason 04/07/2019
        //$('.activeRow .customer').val(ui.item.label);
        //$('.activeRow .customer').data('id', ui.item.value);
            return false;
        },
        select: function (event, ui) {
          //Amended By Jason 04/07/2019
          var el = $('.activeRow .customer');
          el.val(ui.item.label);
          el.data('id', ui.item.value);
          GetDefaultValueCust($(el).data("id"), el);
        //$('.activeRow .customer').val(ui.item.label);
        //$('.activeRow .customer').data('id', ui.item.value);        
            return false;
        },
        change: function (event, ui) {
          //Amended By Jason 04/07/2019
          if (!ui.item) {
              //Amended By Jason 04/07/2019
              var el = $('.activeRow .customer');
                $('.activeRow .customer').val('');
              $('.activeRow .customer').data('id','');
              GetDefaultValueCust($(el).data("id"), el);
          }

        //$('.activeRow .customer').val('');
        //GetDefaultValue($(el).data("id"), el);
            return false;
        }
    });

    $('.activeRow .js-example-basic-single2').select2();
}

function initializeCustomerToAccessListNewRow() {
    var strIndividual_Corporate_Customer = "<select id=Individual_Corporate_Customer class='editor select Individual_Corporate_Customer js-example-basic-single2' tabindex='2'>";
    strIndividual_Corporate_Customer += "<option selected='true'>" + "Individual" + "</option>";
    strIndividual_Corporate_Customer += "<option>" + "Corporate" + "</option>";
    strIndividual_Corporate_Customer += "</select><span class='edited Individual_Corporate_Customer_edited'></span>";
    customerNewRow.Individual_Corporate_Customer = strIndividual_Corporate_Customer;

    customerNewRow.Customer = "<input style='width:100% !important' class='editor customer required customer' type='text' tabindex='3' value=''><span class='edited customer_edited'></span><span class='ItemNumber' style='display:none'>0</span>";
	//Amended By Jason 04/07/2019
    customerNewRow.NRICFINPASSPORTCust = "<input class='editor NRICFINPASSPORTCust unrequired' type='text' disabled tabindex='4' value=''><span class='edited NricType_edited'></span>";
    customerNewRow.UENNOCust = "<input class='editor UENNOCust unrequired' tabindex='5' style='width:80px;' type='text' disabled value=''><span class='edited RocUen_edited' style='width:80px;'></span>";
}

function isCustomerRowChanged() {
    let activeRow = $("#tblCustomerToAccessList tr.activeRow");
    let isValid = false;
    if (activeRow && activeRow.length > 0) {
        $(activeRow[0]).find(":input").each(function () {
            if ($(this).hasClass('unrequired')) {
                if ($(this).val() != '')
                    isValid = true;
            }
        });

        if ($(activeRow[0]).find('.Individual_Corporate_Customer').val() === ''
            && $(activeRow[0]).find('.Customer').val() === ''
        )
            isValid = false;
        else
            isValid = true;
    }
    else { return true; }
    return isValid;
}

function initializeCustomerToAccessListUpdate(customer) {    
    if (customer.length > 0) {
        $.each(customer, function (key, value) {
            var strIndividual_Corporate_Customer = "<select id=Individual_Corporate_Customer class='editor select Individual_Corporate_Customer js-example-basic-single2' tabindex='2'>";
            if (value.IndividualCorporate === 'Individual') {
                strIndividual_Corporate_Customer += "<option selected='true'>" + "Individual" + "</option>";
                strIndividual_Corporate_Customer += "<option>" + "Corporate" + "</option>";
            }
            else {
                strIndividual_Corporate_Customer += "<option>" + "Individual" + "</option>";
                strIndividual_Corporate_Customer += "<option  selected='true'>" + "Corporate" + "</option>";
            }
            strIndividual_Corporate_Customer += "</select><span class='edited Individual_Corporate_Customer_edited'>" + value.IndividualCorporate +"</span>";
            customerNewRow.Individual_Corporate_Customer = strIndividual_Corporate_Customer;

            customerNewRow.Customer = "<input style='width:100% !important' class='editor customer required customer' type='text' tabindex='3' value='" + value.CustomerName + "'><span data-value='" + value.Customer + "' class='edited customer_edited'>" + value.CustomerName + "</span><span class='ItemNumber' style='display:none'>" + value.ItemNumber +"</span>";

            var _row1 = tblCustomerToAccessList.rows.add([[customerNewRow.Action, customerNewRow.Individual_Corporate_Customer, customerNewRow.Customer,customerNewRow.NRICFINPASSPORTCust,customerNewRow.UENNOCust]]);

            _row1.draw();
            $("#tblCustomerToAccessList_wrapper").removeClass("alert-danger-table");
        });
    };
}

$(document).ready(function () {
    $('.js-example-basic-single3').select2();

    $(document).on('change', '.Individual_Corporate_Customer', function () {
        var txtCustomer = $(this).closest("tr").find(".customer");
        var type = $(this).val();
        $(txtCustomer).val('');
        $(txtCustomer).data('id', '');
    });

    targetCustomerToAccessRowEvent(event);

    $(document).on('click', function (event) {
        targetCustomerToAccessRowEvent(event);
    });

    $("#tblCustomerToAccessList tbody").on('click', 'tr', function (event) {
        if (!($(this).hasClass("activeRow")) && isValidCustomerRow()) {
            if (!IsDuplicateCustomer()) {
                setCustomerActiveRow(this);
            }
        }
    });

    setCustomerActiveRow();

    initializeCustomerToAccessListNewRow();

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

    $('#tblModificationHistory').DataTable({
        dom: 'Blfrtip',
        pageLength: 10,
        buttons: ['copy', 'print',
            {
                extend: 'csvHtml5',
                title: 'ModificationHistory-Export-Csv-' + new Date().getTime().toString()
            },
            {
                extend: 'excelHtml5',
                title: 'ModificationHistory-Export-Excel-' + new Date().getTime().toString()
            },
            {
                extend: 'pdfHtml5',
                title: 'ModificationHistory-Export-PDF-' + new Date().getTime().toString(),
                orientation: 'portrait',
                pageSize: 'TABLOID'
            }
        ]
    });
    $("#btnCustomerToAccessList ").on("click", function () {
        if (!IsDuplicateCustomer() && ($.isFunction(isValidRow) ? isValidRow() : true) && ($.isFunction(isValidInsuranceRow) ? isValidInsuranceRow() : true)) {
            j++;
            if (tblCustomerToAccessList.rows().count() === 0) {
                initializeCustomerToAccessListNewRow();
            }
            else {
                initializeCustomerToAccessListNewRow();
            }
            if (tblCustomerToAccessList.rows().count() == 0 || (!IsCustomer && isValidCustomerRow())) {
          var _row1 = tblCustomerToAccessList.rows.add([[customerNewRow.Action, customerNewRow.Individual_Corporate_Customer, customerNewRow.Customer,customerNewRow.NRICFINPASSPORTCust,customerNewRow.UENNOCust]]);

                _row1.draw();
                setCustomerActiveRow($("#tblCustomerToAccessList tr:last"));
                $("#tblCustomerToAccessList_wrapper").removeClass("alert-danger-table");
            }
        }
    });

    $('#tblCustomerToAccessList tbody').on('click', 'button.deleting', function () {
        IsCustomer = false;
        tblCustomerToAccessList.row($(this).parents("tr")).remove().draw();
    });

    isCustomerRowChanged();

    $('body').on('click', '.ui-state-default', function () {
        event.stopPropagation();
    });

});

function IsDuplicateCustomer() {
    IsCustomer = false;
    var customerName = $("#tblCustomerToAccessList tbody tr.activeRow").find(".customer").val();
    $(".customer_edited").each(function () {
        if (customerName == $(this).text() && !$(this).closest('tr.activeRow').length) {
            swal(swalGlobal.SwalTitle_Error, "This customer already selected!", swalGlobal.SwalType_Error);
            IsCustomer = true;
        }
    });
    return IsCustomer;
}
