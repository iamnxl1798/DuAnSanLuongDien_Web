
var loadDatatableRole = function () {
    $('#my_datatable_role').DataTable({
        "responsive": true,
        "serverSide": true,
        "processing": true,
        "language": {
            "processing": "Loading Database ....."
        },
        "searching": true,
        /*@* "ordering": false,*@*/
        "order": [0, "asc"],
        "ajax": {
            url: '/RoleAccount/GetAllRole',
            type: "POST",
            dataType: 'json'/*@*,
							success: function () {
								alert("Success get all role");
							},
				error: function () {
					alert("Fail get all role");
				}*@*/
        },
        "columns": [
            {
                "data": "ID",
                "name": "ID",
                orderable: true
            },
            {
                "data": "Role", "name": "Role",
                /* @* "orderable": false,*@*/
                render: function (Role) {
                    var status = {
                        'Lãnh Đạo': 'label-light-danger',
                        'Quản Trị': 'label-light-info',
                        'Chuyên Viên': 'label-light-primary'
                        //label-light-danger //label-light-warning
                    };
                    if (Role in status) {
                        return '<span class="label label-lg font-weight-bold ' + status[Role] + ' label-inline">' + Role + '</span>';
                    } else {
                        return '<span class="label label-lg font-weight-bold label-light-success label-inline">' + Role + '</span>';
                    }
                }
            },
            {
                "data": {
                    Actions: "Actions",
                    ID: "ID",
                    Role: "Role"
                },
                "name": "Actions",
                "orderable": false,
                "render": function (data, type, full) {
                    return '\
	                        <a href="javascript:;" class="btn btn-sm btn-default btn-text-primary btn-hover-primary btn-icon mr-2 bt-filter-account" data-role="'+ data.Role + '" title = "Show Account">\
								<span class="svg-icon svg-icon-md">\
									<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="24px" height="24px" viewBox="0 0 24 24" version="1.1" class="svg-icon">\
										<g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">\
											<rect x="0" y="0" width="24" height="24"/>\
											<path d="M7,3 L17,3 C19.209139,3 21,4.790861 21,7 C21,9.209139 19.209139,11 17,11 L7,11 C4.790861,11 3,9.209139 3,7 C3,4.790861 4.790861,3 7,3 Z M7,9 C8.1045695,9 9,8.1045695 9,7 C9,5.8954305 8.1045695,5 7,5 C5.8954305,5 5,5.8954305 5,7 C5,8.1045695 5.8954305,9 7,9 Z" fill="#000000"/>\
											<path d="M7,13 L17,13 C19.209139,13 21,14.790861 21,17 C21,19.209139 19.209139,21 17,21 L7,21 C4.790861,21 3,19.209139 3,17 C3,14.790861 4.790861,13 7,13 Z M17,19 C18.1045695,19 19,18.1045695 19,17 C19,15.8954305 18.1045695,15 17,15 C15.8954305,15 15,15.8954305 15,17 C15,18.1045695 15.8954305,19 17,19 Z" fill="#000000" opacity="0.3"/>\
										</g>\
									</svg>\
								</span>\
	                        </a>\
	                        <a href="javascript:;" class="btn btn-sm btn-default btn-text-primary btn-hover-primary btn-icon mr-2 bt-open-edit-role-form" data-id="'+ data.ID + '" title="Edit details">\
	                            <span class="svg-icon svg-icon-md">\
									<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="24px" height="24px" viewBox="0 0 24 24" version="1.1">\
										<g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">\
											<rect x="0" y="0" width="24" height="24"/>\
											<path d="M12.2674799,18.2323597 L12.0084872,5.45852451 C12.0004303,5.06114792 12.1504154,4.6768183 12.4255037,4.38993949 L15.0030167,1.70195304 L17.5910752,4.40093695 C17.8599071,4.6812911 18.0095067,5.05499603 18.0083938,5.44341307 L17.9718262,18.2062508 C17.9694575,19.0329966 17.2985816,19.701953 16.4718324,19.701953 L13.7671717,19.701953 C12.9505952,19.701953 12.2840328,19.0487684 12.2674799,18.2323597 Z" fill="#000000" fill-rule="nonzero" transform="translate(14.701953, 10.701953) rotate(-135.000000) translate(-14.701953, -10.701953) "/>\
											<path d="M12.9,2 C13.4522847,2 13.9,2.44771525 13.9,3 C13.9,3.55228475 13.4522847,4 12.9,4 L6,4 C4.8954305,4 4,4.8954305 4,6 L4,18 C4,19.1045695 4.8954305,20 6,20 L18,20 C19.1045695,20 20,19.1045695 20,18 L20,13 C20,12.4477153 20.4477153,12 21,12 C21.5522847,12 22,12.4477153 22,13 L22,18 C22,20.209139 20.209139,22 18,22 L6,22 C3.790861,22 2,20.209139 2,18 L2,6 C2,3.790861 3.790861,2 6,2 L12.9,2 Z" fill="#000000" fill-rule="nonzero" opacity="0.3"/>\
										</g>\
									</svg>\
	                            </span>\
	                        </a>\
							\
	                        <a href="javascript:;" class="btn btn-sm btn-default btn-text-primary btn-hover-primary btn-icon bt-delete-role" data-id="'+ data.ID + '"  title="Delete">\
								<span class="svg-icon svg-icon-md">\
									<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="24px" height="24px" viewBox="0 0 24 24" version="1.1">\
										<g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">\
											<rect x="0" y="0" width="24" height="24"/>\
											<path d="M6,8 L6,20.5 C6,21.3284271 6.67157288,22 7.5,22 L16.5,22 C17.3284271,22 18,21.3284271 18,20.5 L18,8 L6,8 Z" fill="#000000" fill-rule="nonzero"/>\
											<path d="M14,4.5 L14,4 C14,3.44771525 13.5522847,3 13,3 L11,3 C10.4477153,3 10,3.44771525 10,4 L10,4.5 L5.5,4.5 C5.22385763,4.5 5,4.72385763 5,5 L5,5.5 C5,5.77614237 5.22385763,6 5.5,6 L18.5,6 C18.7761424,6 19,5.77614237 19,5.5 L19,5 C19,4.72385763 18.7761424,4.5 18.5,4.5 L14,4.5 Z" fill="#000000" opacity="0.3"/>\
										</g>\
									</svg>\
								</span>\
	                        </a>\
	                    ';
                }
            }
        ]
    });
}
$(document).ready(loadDatatableRole);

$('#my_datatable_role').on('click', '.bt-open-edit-role-form', function () {
    var url = "/RoleAccount/PermissionTree";
    var id = $(this).attr("data-id");
    $.ajax({
        url: url,
        type: 'POST',
        data: {
            RoleID: id
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
var reloadRoleDatatable = function () {
    $('#my_datatable_role').DataTable().ajax.reload(null, false);
};
/*@* $('.modal-edit-role').on('hidden.bs.hidden.bs.modal', reloadRoleDatatable);*@*/
$('#my_datatable_role').on('click', '.bt-delete-role', function () {
    //add xac thuc trc khi xoa
    var id = $(this).attr("data-id");
    $('#btnDelteYesRole').attr("data-id", id);
    $('#confirmDeleteRole').modal('show');
    $('#confirmDeleteRole').modal({
        backdrop: true
    });
});

$('#btnDelteYesRole').on('click', function (e) {
    e.preventDefault();
    var url = "/RoleAccount/DeleteRole";
    var id = $(this).attr("data-id");
    $.ajax({
        url: url,
        type: 'POST',
        data: {
            roleID: id
        },
        success: function (data) {
            if (!data) {
                alert("Error delete role ");
            } else {
                reloadRoleDatatable();
            }
        },
        error: function (data) {
            alert("Error load ajax delete role");
        }
    });
});

var searchAccountFollowRole = function (role) {
    $('#my_datatable_account').DataTable().column('Role:name').search(role).draw();
}

$('#my_datatable_role').on('click', '.bt-filter-account', function () {
    var search_str = $(this).attr("data-role");
    searchAccountFollowRole(search_str);
});