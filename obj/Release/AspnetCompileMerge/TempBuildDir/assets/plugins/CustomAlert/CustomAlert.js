function showCustomPopUp(icon, message, type) {

    
    if (type == 'Success') {
        $('#alertmsg').html('<div class="alert alert-success">' +
                                 '<strong>Success !</strong><p>' + message + '</p>' +
                             '</div>');
    }
    else if (type == 'warning') {
        $('#alertmsg').html('<div class="alert alert-warning">' +
                           '<strong>Warning !</strong><p>' + message + '</p>' +
                       '</div>');
    }
    else if (type == 'error') {
        $('#alertmsg').html('<div class="alert alert-danger">' +
                           '<strong>Error !</strong><p>' + message + '</p>' +
                       '</div>');
    }
   // $('#alertBox1').modal('show');
}
