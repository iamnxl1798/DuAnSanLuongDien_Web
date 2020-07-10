
$(document).ready(function () {
    var url = "/RoleAccount/TableDataRole";
    $.ajax({
        url: url,
        type: 'POST',
        success: function (data) {
            $('#datatable_role_ajax').html(data);
        },
        error: function (data) {
            alert("Error load role ajax");
        }
    });
});

$('#newrole').on('click', function () {
    var url = "/RoleAccount/PermissionTree";
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
        error: function (data) {
            alert("Error load ajax edit role");
        }
    });
});

$('#reloadRoleTable').on('click', function (e) {
    e.preventDefault();
    $('#my_datatable_role').DataTable().destroy();
    loadDatatableRole();
});