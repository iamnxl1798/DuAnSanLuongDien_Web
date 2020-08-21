
function showMessage(text, status) {
    if (status == true) {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-center",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "linear",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };

        toastr.success(text);
    }
    else {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-center",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "linear",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };

        toastr.error(text);
    }
}

function hideLoading() {
    $(".modal-backdrop").remove();  
    $("#loadingModal").hide();
}

function showLoading() {
    $('<div class="modal-backdrop" style="background-color: #CCCCCC;opacity: 0.7;"><div class="modal-dialog modal-dialog-centered d-flex justify-content-center" role="document"><div class="spinner-border" role="status"></div></div></div>').appendTo(document.body);
    $('#loadingModal').modal('show');
}