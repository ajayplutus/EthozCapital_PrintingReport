
function isValidRow() {
  var activeRow = $("#tblMortgagorList tr.activeRow");
  var isValid = false;
  if (activeRow && activeRow.length > 0) {
    $(activeRow[0]).find(":input").each(function () {
      if ($(this).hasClass("mortgagor")) {
        IsDuplicateMortgagor(this);
      }
      if ($(this).hasClass("Main_Secondary")) {
        checkMainSecondary(this);
      }
      if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
        if ($(this).val() == "" || $(this).val() == null) {
          $(this).html('');
          $(this).append($('<option value=>' + "--Select--" + '</option>'));
          $(this).parent().find("span.select2-selection__rendered").addClass("alert-danger");
          $(this).addClass("alert-danger");
          if ($(this).hasClass("js-example-basic-single")) {
            $(this).next().children().children().addClass("selection-danger");
          }
        }
        else {
          $(this).removeClass("alert-danger");
          $(this).parent().find("span.select2-selection__rendered").removeClass("alert-danger");
          if ($(this).hasClass('js-example-basic-single')) {
            $(this).next().children('.select2-selection').removeClass("selection-danger");
          }
        }
      }
    });
  }
  else { return true; }
  isValid = (!IsMortgagor) && (!isMainDuplicate) && (($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true);
  if (!isValid) {
    if (!isMainDuplicate && !IsMortgagor) swal(swalGlobal.SwalTitle_Error, "Please fill required field(s)", swalGlobal.SwalType_Error);
  }
  return isValid;
};

var IsMortgagor = false;
function IsDuplicateMortgagor(el) {
  var i = 0;
  IsMortgagor = false;
  var morgagorName = $(el).val();
  $(".mortgagorName").each(function () {
    if (morgagorName == $(this).text() && !$(this).closest('tr.activeRow').length) {
      swal(swalGlobal.SwalTitle_Error, "This mortgagor already selected!", swalGlobal.SwalType_Error);
      IsMortgagor = true;
    }
  });
  return IsMortgagor;
}

var isMainDuplicate = false;
function checkMainSecondary(el) {
  var i = 0;
  isMainDuplicate = false;
  $(".checkMain").each(function () {
    if ($(this).text() == "Main" && !$(this).closest('tr.activeRow').length)
      i++;
  });
  if ($(el).find("option:selected").text() == "Main" && i > 0) {
    var mortagagorName = $(el).closest('tr').parent().find("span.mortgagorName").text();
    swal(swalGlobal.SwalTitle_Error, "Main mortgagor is already selected!", swalGlobal.SwalType_Error);
    isMainDuplicate = true;
  }
  else {
    isMainDuplicate = false;
  }
  return isMainDuplicate;
}

function isValidCustomerRow(row) {

  var activeRow = $("#tblCustomerToAccessList tr.activeRow");
  var isValid = false;
  if (activeRow && activeRow.length > 0) {
    $(activeRow[0]).find(":input").each(function () {

      if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
        if ($(this).val() == "") {
          $(this).addClass("alert-danger");
        }
        else {
          $(this).removeClass("alert-danger");
        }
      }
    });
  }
  else { return true; }
  isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;
  if (!isValid) {
    if (!IsCustomer) swal(swalGlobal.SwalTitle_Error, "Please fill required field(s)", swalGlobal.SwalType_Error);
  }
  return isValid;

}

function isValidInsuranceRow() {
  var activeRow = $("#tblInsurance tr.activeRow");
  var isValid = false;
  if (activeRow && activeRow.length > 0) {
    $(activeRow[0]).find(":input").each(function () {
      if ($(this).hasClass("select2-selection__rendered"))
        IsDuplicateInsuranceType(this);
      if (!($(this).hasClass("deleting")) && !($(this).hasClass("unrequired")) && !($(this).parents('td').hasClass('hideColumn'))) {
        if ($(this).val() == "" || $(this).val() == null) {
          $(this).addClass("alert-danger");
        }
        else {
          $(this).removeClass("alert-danger");
        }
      }
    });
  }
  else { return true; }
  isValid = ($(activeRow[0]).find(".alert-danger") && $(activeRow[0]).find(".alert-danger").length > 0) ? false : true;
  if (!isValid) {
    if (!IsInsuranceType) swal(swalGlobal.SwalTitle_Error, "Please fill required field(s)", swalGlobal.SwalType_Error);
  }
  return isValid;
}
