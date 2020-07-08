
	$(document).ready(function () {
		$('#my_datatable_role').DataTable({
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
				"data": {
					Actions: "Actions",
					ID: "ID"
				},
				"name": "Actions",
				"orderable": false,
				"render": function (data, type, full) {
					return '\
	                        <div class="dropdown dropdown-inline">\
	                            <a href="javascript:;" style="background-color : #2E64FE; color : white !important" class="btn btn-sm btn-default btn-text-primary dropdown-toggle btn-hover-primary" data-toggle="dropdown">\
																Actions\
	                            </a>\
	                            <div class="dropdown-menu dropdown-menu-sm dropdown-menu-right">\
									<a class="dropdown-item bt-open-edit-role-form" style="color: #007AD9 !important" data-toggle="modal" data-id="'+ data.ID + '" href="#">Edit</a>\
									<a class="dropdown-item bt-delete-role" style="color: #007AD9 !important" data-id="'+ data.ID + '" href="#">Delete</a>\
	                        </div>\
	                    ';
				}
			},
				/*@* {
			data: "ID", name: "ID",
			orderable: true
		},*@
				@* {
			"data": {
				Username: "Username",
				Avatar: "Avatar"
			},
			"name": "Username",
			render: function (data, type, full) {
				return '<img class="" src="' + data.Avatar + '" style="float:left; width:20px; height:20px" /><span style="margin-left:10px">' + data.Username + '</span>';
			}
		},*@*/
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
	}
			]
		});
	});

$('#my_datatable_role').on('click', '.bt-open-edit-role-form', function () {
	alert("haha");
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
	$('#btnDelteYes').attr("data-id", id);
	$('#confirmDelete').modal('show');
	$('#confirmDelete').modal({
		backdrop: true
	});
});

$('#btnDelteYes').on('click', function (e) {
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