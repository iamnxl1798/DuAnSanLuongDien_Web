
$(document).ready(function () {
    var url = "/RoleAccount/TableDataRole";
    $.ajax({
        url: url,
        type: 'POST',
        success: function (data) {
            $('#datatable_role_ajax').html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 401) {
                showMessage('Bạn không có quyền này', false);
            } else {
                showMessage('Đã xảy ra lỗi trong quá trình tải bảng vai trò: ' + jqXHR.responseText, false);
            }
        }

    });
});

$('#newrole').on('click', function () {
    var url = "/RoleAccount/CreateRoleForm";
    var id = $(this).attr("data-id");
    $.ajax({
        url: url,
        type: 'POST',
        data: {
            roleID: id,
        },
        success: function (data) {
            $('#formInsertEditRole').html(data);
            $('#formInsertEditRole').modal('show');
            $('#formInsertEditRole').modal({
                backdrop: false
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 401) {
                showMessage('Bạn không có quyền này', false);
            } else {
                showMessage('Đã xảy ra lỗi trong quá trình tải form thêm mới vai trò: ' + jqXHR.responseText, false);
            }
        }
    });
});

$('#reloadRoleTable').on('click', function (e) {
    e.preventDefault();
    $('#my_datatable_role').DataTable().destroy();
    loadDatatableRole();
});