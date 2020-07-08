
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
			alert("Error load ajax edit/insert role");
		}
	});
});


jQuery(document).ready(function () {
	var url = "/Account/TableDataUser";
	$.ajax({
		url: url,
		type: 'POST',
		success: function (data) {
			$('#datatable_ajax').html(data);
			/*@* $('#test_ajax').modal('show');*@*/
		},
		error: function (data) {
			alert("Error load dataTable ajax");
		}
	});
});