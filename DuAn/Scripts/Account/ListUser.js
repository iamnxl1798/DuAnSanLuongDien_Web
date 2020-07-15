﻿
//ListUser
jQuery('#newaccount').on('click', function () {
	var url = "/Account/EditAccountForm";
	var id = $(this).attr("data-id");
	$.ajax({
		url: url,
		type: 'POST',
		data: {
			accID: id
		},
		success: function (data) {
			$('#formEditAccount').html(data);
			$('#formEditAccount').modal('show');
			$('#formEditAccount').modal({
				backdrop: false
			});
		},
		error: function (data) {
			showMessage('Error load ajax edit/insert role', false);
		}
	});
});


jQuery(document).ready(function () {
	var url = "/Account/TableDataUser";
	$.ajax({
		url: url,
		type: 'POST',
		success: function (data) {
			$('#datatable_account_ajax').html(data);
			/*@* $('#test_ajax').modal('show');*@*/
		},
		error: function (data) {
			showMessage('Error load dataTable ajax', false);
		}
	});
});

$('#reloadAccountTable').on('click', function (e) {
	e.preventDefault();
	$('#my_datatable_account').DataTable().destroy();
	loadDataTableAccount();
});